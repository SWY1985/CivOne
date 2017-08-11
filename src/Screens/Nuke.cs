// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Interfaces;
using CivOne.Graphics;
using CivOne.Tiles;
using CivOne.Units;

namespace CivOne.Screens
{
	internal class Nuke : BaseScreen
	{
		private struct RenderTile
		{
			public bool Visible;
			public int X, Y;
			public ITile Tile;
			public IBitmap Image => Resources[Tile];
			public Point Position => new Point(X * 16, Y * 16);
		}

		private const int FRAME_COUNT = 28;

		private int _x, _y, _dx, _dy;
		
		private int _frameCounter = FRAME_COUNT + 2;

		private Picture _gameMap;

		private Picture[] _sprites = null;
		
		private IEnumerable<RenderTile> RenderTiles
		{
			get
			{
				for (int x = 0; x < 15; x++)
				for (int y = 0; y < 12; y++)
				{
					int tx = _x + x;
					int ty = _y + y;
					while (tx >= Map.WIDTH) tx -= Map.WIDTH;
					
					if (ty < 0 || ty >= Map.HEIGHT) continue;

					yield return new RenderTile
					{
						Visible = Human.Visible(tx, ty),
						X = x,
						Y = y,
						Tile = Map[tx, ty]
					};
				}
			}
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_frameCounter < 0)
			{
				Destroy();
				return true;
			}

			int cx = Settings.RightSideBar ? 0 : 80;
			int cy = 8;

			int xx = _dx + cx;
			int yy = _dy + cy;
			
			int step = 28 - _frameCounter--;

			AddLayer(_gameMap, cx, cy);
			if (step >= 0 && step < 28)
			{
				AddLayer(_sprites[step], xx, yy);
			}

			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			return false;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			return false;
		}

		private Picture GameMap
		{
			get
			{
				Picture output = new Picture(240, 192);

				RenderTile[] renderTiles = RenderTiles.ToArray();
				foreach (RenderTile t in renderTiles)
				{
					if (!Settings.RevealWorld && !t.Visible)
					{
						output.FillRectangle(5, t.X * 16, t.Y * 16, 16, 16);
						continue;
					}
					output.AddLayer(t.Image, t.Position);
					if (Settings.RevealWorld) continue;
					
					if (!Human.Visible(t.Tile, Direction.West)) output.AddLayer(Resources.Instance.GetFog(Direction.West), t.Position);
					if (!Human.Visible(t.Tile, Direction.North)) output.AddLayer(Resources.Instance.GetFog(Direction.North), t.Position);
					if (!Human.Visible(t.Tile, Direction.East)) output.AddLayer(Resources.Instance.GetFog(Direction.East), t.Position);
					if (!Human.Visible(t.Tile, Direction.South)) output.AddLayer(Resources.Instance.GetFog(Direction.South), t.Position);
				}

				foreach (RenderTile t in renderTiles)
				{
					if (!Settings.RevealWorld && !t.Visible) continue;

					if (t.Tile.City != null) continue;

					IUnit[] units = t.Tile.Units.ToArray();
					if (units.Length == 0) continue;
					
					IUnit drawUnit = units[0];

					if (t.Tile.IsOcean && drawUnit.Class != UnitClass.Water && drawUnit.Sentry)
					{
						// Do not draw sentried land units at sea
						continue;
					}
					
					output.AddLayer(drawUnit.GetUnit(units[0].Owner), t.Position);
					if (units.Length == 1) continue;
					output.AddLayer(drawUnit.GetUnit(units[0].Owner), t.Position.X - 1, t.Position.Y - 1);
				}

				foreach (RenderTile t in renderTiles.Reverse())
				{
					if (!Settings.RevealWorld && !t.Visible) continue;

					City city = t.Tile.City;
					if (city == null) continue;
					
					output.AddLayer(Icons.City(city), t.Position);
					
					if (t.Y == 11) continue;
					int labelX = (t.X == 0) ? t.Position.X : t.Position.X - 8;
					int labelY = t.Position.Y + 16;
					output.DrawText(city.Name, 0, 5, labelX, labelY + 1, TextAlign.Left);
					output.DrawText(city.Name, 0, 11, labelX, labelY, TextAlign.Left);
				}

				return output;
			}
		}

		internal Nuke(int x, int y)
		{
			_x = Common.GamePlay.X;
			_y = Common.GamePlay.Y;

			_dx = x - 14;
			_dy = y - 14;
			
			Picture nukePic = Resources.Instance.LoadPIC("NUKE1");
			Color[] palette = Common.DefaultPalette;
			for (int i = 192; i < 256; i++)
			{
				palette[i] = nukePic.Palette[i];
			}
			_canvas = new Picture(320, 200, palette);
			_gameMap = GameMap;

			_sprites = new Picture[28];
			for (int yy = 0; yy < 4; yy++)
			for (int xx = 0; xx < 7; xx++)
			{
				_sprites[(yy * 7) + xx] = Resources["NUKE1"].GetPart(1 + (45 * xx), 1 + (45 * yy), 44, 44);
			}
		}
	}
}