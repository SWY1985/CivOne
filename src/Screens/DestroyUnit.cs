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
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class DestroyUnit : BaseScreen
	{
		private struct RenderTile
		{
			public bool Visible;
			public int X, Y;
			public ITile Tile;
			public Picture Image
			{
				get
				{
					return Resources.Instance.GetTile(Tile);
				}
			}
			public Point Position
			{
				get
				{
					return new Point(X * 16, Y * 16);
				}
			}
		}

		private const int NOISE_COUNT = 16;

		private IUnit _unit;
		private int _x, _y;
		
		private int _noiseCounter = NOISE_COUNT + 2;
		private readonly byte[,] _noiseMap;

		private Picture _gameMap, _overlay = null;
		
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
		
		public override bool HasUpdate(uint gameTick)
		{
			int cx = Settings.RightSideBar ? 0 : 80;
			int cy = 8;

			if (_overlay == null)
			{
				_overlay = new Picture(_canvas);

				int xx = _unit.X - _x;
				int yy = _unit.Y - _y;
				while (xx < 0) xx += 80;
				while (xx >= 80) xx -= 80;

				_overlay.AddLayer(_unit.GetUnit(_unit.Owner), cx + (xx * 16), cy + (yy * 16));
			}
			_overlay.ApplyNoise(_noiseMap, --_noiseCounter);

			AddLayer(_gameMap, cx, cy);
			AddLayer(_overlay, 0, 0);

			if (_noiseCounter == 0)
			{
				Game.DisbandUnit(_unit);
				Destroy();
				HandleClose();
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
					
					IUnit[] units = t.Tile.Units.Where(u => u != Game.ActiveUnit).ToArray();
					if (t.Tile.Type == Terrain.Ocean)
					{
						// Always show naval units first at sea
						units = units.OrderBy(u => (u.Class == UnitClass.Water) ? 1 : 0).ToArray();
					}
					if (units.Length == 0) continue;
					
					IUnit drawUnit = units.FirstOrDefault(u => u == Game.ActiveUnit);
					drawUnit = units[0];

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

		internal DestroyUnit(IUnit unit)
		{
			Cursor = MouseCursor.None;
			_unit = unit;

			GamePlay gamePlay = (GamePlay)Common.Screens.First(s => (s is GamePlay));
			_x = gamePlay.X;
			_y = gamePlay.Y;

			_canvas = new Picture(320, 200, Common.TopScreen.Palette);
			_gameMap = GameMap;

			_noiseMap = new byte[320, 200];
			for (int x = 0; x < 320; x++)
			for (int y = 0; y < 200; y++)
			{
				_noiseMap[x, y] = (byte)Common.Random.Next(1, NOISE_COUNT);
			}
		}
	}
}