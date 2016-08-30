// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class GameMap : BaseScreen
	{
		private struct RenderTile
		{
			public int X, Y;
			public ITile Tile;
			public Bitmap Image
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
		
		private readonly Color[] _palette;
		private bool _update = true;
		private int _x, _y;
		private IUnit _lastUnit;
		
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
					
					yield return new RenderTile
					{
						X = x,
						Y = y,
						Tile = Map.Instance.GetTile(tx, ty)
					};
				}
			}
		}
		
		public bool MustUpdate(uint gameTick)
		{
			// Check if the active unit is on the screen and the blink status has changed.
			IUnit activeUnit = Game.Instance.ActiveUnit;
			if (activeUnit != null && RenderTiles.Any(t => t.Tile.X == activeUnit.X && t.Tile.Y == activeUnit.Y) && (gameTick % 2) == 0)
			{
				_lastUnit = activeUnit;
				_update = true;
			}
			else if (activeUnit != null && activeUnit.Moving)
			{
				_update = true;
			}
			else if (activeUnit != _lastUnit)
			{
				CenterOnUnit();
				_update = true;
			}
			return _update;
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas = new Picture(240, 192, _palette);
				foreach (RenderTile t in RenderTiles)
				{
					AddLayer(t.Image, t.Position);
				}
				
				foreach (RenderTile t in RenderTiles)
				{
					City city = Game.Instance.GetCity(t.Tile.X, t.Tile.Y);
					if (city != null) continue;
					
					IUnit[] units = Game.Instance.GetUnits(t.Tile.X, t.Tile.Y).Where(u => !u.Moving).ToArray();
					if (units.Length == 0) continue;
					
					IUnit drawUnit = units.FirstOrDefault(u => u == Game.Instance.ActiveUnit);
					
					if (drawUnit == null)
					{
						// No active unit on this tile, show top unit
						drawUnit = units[0];
					}
					else if ((gameTick % 4) >= 2 || drawUnit.Moving)
					{
						// Active unit on this tile, and blink status is off.
						continue;
					}

					AddLayer(drawUnit.GetUnit(units[0].Owner), t.Position);
					if (units.Length == 1) continue;
					AddLayer(drawUnit.GetUnit(units[0].Owner), t.Position.X - 1, t.Position.Y - 1);
				}
				
				foreach (RenderTile t in RenderTiles.Reverse())
				{
					City city = Game.Instance.GetCity(t.Tile.X, t.Tile.Y);
					if (city == null) continue;
					
					if (Game.Instance.GetUnits(t.Tile.X, t.Tile.Y).Length > 0)
						_canvas.FillRectangle(5, t.Position.X, t.Position.Y, 16, 16);
					_canvas.FillRectangle(15, t.Position.X + 1, t.Position.Y + 1, 14, 14);
					_canvas.FillRectangle(Common.ColourDark[city.Owner], t.Position.X + 2, t.Position.Y + 1, 13, 13);
					_canvas.FillRectangle(Common.ColourLight[city.Owner], t.Position.X + 2, t.Position.Y + 2, 12, 12);
					
					Bitmap resource = (Bitmap)Resources.Instance.GetPart("SP257", 192, 112, 16, 16).Clone();
					Picture.ReplaceColours(resource, 3, 0);
					Picture.ReplaceColours(resource, 5, Common.ColourDark[city.Owner]);
					AddLayer(resource, t.Position);
					_canvas.DrawText(city.Size.ToString(), 0, 5, 5, t.Position.X + 9, t.Position.Y + 5, TextAlign.Center);
					
					if (t.Y == 11) continue;
					int labelX = (t.X == 0) ? t.Position.X : t.Position.X - 8;
					int labelY = t.Position.Y + 16;
					_canvas.DrawText(city.Name, 0, 5, labelX, labelY + 1, TextAlign.Left);
					_canvas.DrawText(city.Name, 0, 11, labelX, labelY, TextAlign.Left);
				}
				
				if (Game.Instance.MovingUnit != null)
				{
					IUnit movingUnit = Game.Instance.MovingUnit;
					int relX = movingUnit.X - movingUnit.FromX;
					int relY = movingUnit.Y - movingUnit.FromY;
					while (relX < -1) relX += Map.WIDTH;
					while (relX > 1) relX -= Map.WIDTH;

					relX *= (movingUnit.MoveFrame * 2);
					relY *= (movingUnit.MoveFrame * 2);
					
					movingUnit.MoveUpdate();
					RenderTile tile = RenderTiles.First(t => t.Tile.X == movingUnit.FromX && t.Tile.Y == movingUnit.FromY);
					AddLayer(movingUnit.GetUnit(movingUnit.Owner), tile.Position.X + relX, tile.Position.Y + relY);
					return true;
				}
				
				_update = false;
				return true;
			}
			
			return false;
		}
		
		private void CenterOnUnit()
		{
			if (Game.Instance.ActiveUnit == null) return;
			_x = Game.Instance.ActiveUnit.X - 8;
			_y = Game.Instance.ActiveUnit.Y - 6;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			switch (args.Key)
			{
				case Key.Space:
				case Key.Enter:
					if (Game.Instance.ActiveUnit != null)
						Game.Instance.ActiveUnit.SkipTurn();
					else
						Game.Instance.NextTurn();
					return true;
				case Key.NumPad1:
					return Game.Instance.ActiveUnit.MoveTo(-1, 1);
				case Key.NumPad2:
				case Key.Down:
					return Game.Instance.ActiveUnit.MoveTo(0, 1);
				case Key.NumPad3:
					return Game.Instance.ActiveUnit.MoveTo(1, 1);
				case Key.NumPad4:
				case Key.Left:
					return Game.Instance.ActiveUnit.MoveTo(-1, 0);
				case Key.NumPad6:
				case Key.Right:
					return Game.Instance.ActiveUnit.MoveTo(1, 0);
				case Key.NumPad7:
					return Game.Instance.ActiveUnit.MoveTo(-1, -1);
				case Key.NumPad8:
				case Key.Up:
					return Game.Instance.ActiveUnit.MoveTo(0, -1);
				case Key.NumPad9:
					return Game.Instance.ActiveUnit.MoveTo(1, -1);
			}
			
			switch (args.KeyChar)
			{
				case 'C':
					if (Game.Instance.ActiveUnit == null) break;
					CenterOnUnit();
					return true;
				case 'D':
					if (!args.Shift) break;
					Game.Instance.DisbandUnit(Game.Instance.ActiveUnit);
					return true;
			}
			
			return false;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			int x = (int)Math.Floor((float)args.X / 16);
			int y = (int)Math.Floor((float)args.Y / 16);
			
			if ((args.Buttons & MouseButton.Right) > 0)
			{
				Console.WriteLine(_x.ToString() + "-" + _y.ToString());
				if (Game.Instance.GetCity(_x + x, _y + y) == null)
				{
					Common.AddScreen(new Civilopedia(Map.Instance.GetTile(_x + x, _y + y)));
				}
			}
			if ((args.Buttons & MouseButton.Left | MouseButton.Right) > 0)
			{
				_x += x - 8;
				_y += y - 6;
				while (_x < 0) _x += Map.WIDTH;
				while (_x >= Map.WIDTH) _x -= Map.WIDTH;
				while (_y < 0) _y++;
				while (_y + 12 > Map.HEIGHT) _y--;
				_update = true;
			}
			return _update;
		}
		
		public GameMap()
		{
			_x = 0;
			_y = 0;
			
			_palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
		}
	}
}