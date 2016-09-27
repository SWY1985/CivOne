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
using CivOne.IO;
using CivOne.Tasks;
using CivOne.Templates;
using CivOne.Units;

namespace CivOne.Screens
{
	internal class GameMap : BaseScreen
	{
		private struct RenderTile
		{
			public bool Visible;
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
		private bool _centerChanged = false;
		private int _x, _y;
		private IUnit _lastUnit;

		internal int X
		{
			get
			{
				return _x;
			}
		}

		internal int Y
		{
			get
			{
				return _y;
			}
		}
		
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
		
		public bool MustUpdate(uint gameTick)
		{
			// Check if the active unit is on the screen and the blink status has changed.
			IUnit activeUnit = Game.ActiveUnit;
			if (activeUnit == null)
			{
				_update = true;
				return false;
			}

			if (RenderTiles.Any(t => t.Tile.X == activeUnit.X && t.Tile.Y == activeUnit.Y) && (gameTick % 2) == 0)
			{
				_lastUnit = activeUnit;
				_update = true;
			}
			else if (activeUnit.Moving)
			{
				_update = true;
			}
			else if (activeUnit != _lastUnit && ShouldCenter())
			{
				if (activeUnit.Owner != Game.PlayerNumber(Human))
				{
					if (!Settings.RevealWorld && !Human.Visible(activeUnit.X, activeUnit.Y))
					{
						return (_update = false);
					}
				} 
				CenterOnUnit();
				_update = true;
			}
			return _update;
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				RenderTile[] renderTiles = RenderTiles.ToArray();
				if (Game.MovingUnit != null && !_centerChanged)
				{
					IUnit unit = Game.MovingUnit;
					ITile[] tiles = Map.QueryMapPart(unit.X - 1, unit.Y - 1, 3, 3).ToArray();
					renderTiles = renderTiles.Where(t => tiles.Any(x => x != null && x.X == t.Tile.X && x.Y == t.Tile.Y)).ToArray();
				}
				else
				{
					_centerChanged = false;
					_canvas = new Picture(240, 192, _palette);
				}

				foreach (RenderTile t in renderTiles)
				{
					if (!Settings.RevealWorld && !t.Visible)
					{
						_canvas.FillRectangle(5, t.X * 16, t.Y * 16, 16, 16);
						continue;
					}
					AddLayer(t.Image, t.Position);
					if (Settings.RevealWorld) continue;
					
					if (!Human.Visible(t.Tile, Direction.West)) AddLayer(Resources.Instance.GetFog(Direction.West), t.Position);
					if (!Human.Visible(t.Tile, Direction.North)) AddLayer(Resources.Instance.GetFog(Direction.North), t.Position);
					if (!Human.Visible(t.Tile, Direction.East)) AddLayer(Resources.Instance.GetFog(Direction.East), t.Position);
					if (!Human.Visible(t.Tile, Direction.South)) AddLayer(Resources.Instance.GetFog(Direction.South), t.Position);
				}
				
				foreach (RenderTile t in renderTiles)
				{
					if (!Settings.RevealWorld && !t.Visible) continue;

					if (t.Tile.City != null) continue;
					
					IUnit[] units = t.Tile.Units.Where(u => !u.Moving).ToArray();
					if (t.Tile.Type == Terrain.Ocean)
					{
						// Always show naval units first at sea
						units = units.OrderBy(u => (u.Class == UnitClass.Water) ? 1 : 0).ToArray();
					}
					if (units.Length == 0) continue;
					
					IUnit drawUnit = units.FirstOrDefault(u => u == Game.ActiveUnit);
					
					if (drawUnit == null)
					{
						// No active unit on this tile, show top unit
						drawUnit = units[0];
					}
					else if (!Common.HasScreenType(typeof(Input)) && ((gameTick % 4) >= 2 || drawUnit.Moving))
					{
						// Active unit on this tile or unit is currently moving. Drawing happens later.
						continue;
					}

					if (t.Tile.IsOcean && drawUnit.Class != UnitClass.Water && drawUnit.Sentry)
					{
						// Do not draw sentried land units at sea
						continue;
					}

					AddLayer(drawUnit.GetUnit(units[0].Owner), t.Position);
					if (units.Length == 1) continue;
					AddLayer(drawUnit.GetUnit(units[0].Owner), t.Position.X - 1, t.Position.Y - 1);
				}
				
				foreach (RenderTile t in renderTiles.Reverse())
				{
					if (!Settings.RevealWorld && !t.Visible) continue;

					City city = t.Tile.City;
					if (city == null) continue;
					
					AddLayer(Icons.City(city), t.Position);
					
					if (t.Y == 11) continue;
					int labelX = (t.X == 0) ? t.Position.X : t.Position.X - 8;
					int labelY = t.Position.Y + 16;
					_canvas.DrawText(city.Name, 0, 5, labelX, labelY + 1, TextAlign.Left);
					_canvas.DrawText(city.Name, 0, 11, labelX, labelY, TextAlign.Left);
				}
				
				foreach (RenderTile t in renderTiles)
				{
					if (!Settings.RevealWorld && !t.Visible) continue;

					IUnit[] units = t.Tile.Units.Where(u => !u.Moving).ToArray();
					if (units.Length == 0) continue;
					
					IUnit drawUnit = units.FirstOrDefault(u => u == Game.ActiveUnit);
					
					if (drawUnit == null)
					{
						continue;
					}

					// Active unit on this tile
					
					if (drawUnit.Moving)
					{
						// Unit is currently moving, do not draw the unit here.
						continue;
					}

					if (drawUnit.Owner == Game.PlayerNumber(Human) && (gameTick % 4) >= 2 && !GameTask.Any())
					{
						// Unit is owned by human player, blink status is off and no tasks are running. Do not draw unit.
						continue;
					}

					if (t.Tile.City != null && units.Length == 1 && !GameTask.Any())
					{
						AddLayer(drawUnit.GetUnit(units[0].Owner), t.Position.X - 1, t.Position.Y - 1);
						continue;
					}

					AddLayer(drawUnit.GetUnit(units[0].Owner), t.Position);
					if (units.Length == 1) continue;
					AddLayer(drawUnit.GetUnit(units[0].Owner), t.Position.X - 1, t.Position.Y - 1);
				}
				
				if (Game.MovingUnit != null)
				{
					IUnit unit = Game.MovingUnit;
					if (renderTiles.Any(t => (t.Tile.X == unit.X && t.Tile.Y == unit.Y)))
					{
						RenderTile tile = renderTiles.First(t => (t.Tile.X == unit.X && t.Tile.Y == unit.Y));
						AddLayer(unit.GetUnit(unit.Owner), tile.Position.X + unit.Movement.X, tile.Position.Y + unit.Movement.Y);
						if (unit is IBoardable && tile.Tile.Units.Any(u => u.Class == UnitClass.Land && (tile.Tile.City == null || (tile.Tile.City != null && unit.Sentry))))
						{
							// If there are units on the ship, draw a stack
							AddLayer(unit.GetUnit(unit.Owner), tile.Position.X + unit.Movement.X - 1, tile.Position.Y + unit.Movement.Y - 1);
						}
					}
					return true;
				}
				
				_update = false;
				return true;
			}
			
			return false;
		}
		
		internal void CenterOnPoint(int x, int y)
		{
			_x = x - 8;
			_y = y - 6;
			while (_y < 0) _y++;
			while (_y + 11 >= Map.HEIGHT) _y--;
			_centerChanged = true;
		}
		
		private void CenterOnUnit()
		{
			if (Game.ActiveUnit == null) return;
			_x = Game.ActiveUnit.X - 8;
			_y = Game.ActiveUnit.Y - 6;
			while (_y < 0) _y++;
			while (_y + 11 >= Map.HEIGHT) _y--;
			_centerChanged = true;
		}

		private bool ShouldCenter(int relX = 0, int relY = 0)
		{
			if (Game.ActiveUnit == null)
				return false;
			return (!Map.QueryMapPart(_x + 1, _y + 1, 13, 10).Any(t => t.X == Game.ActiveUnit.X + relX && t.Y == Game.ActiveUnit.Y + relY));
		}

		private bool MoveTo(int relX, int relY)
		{
			if (Game.ActiveUnit == null)
				return false;
			
			if (ShouldCenter(relX, relY))
			{
				// The unit is moving near the edge of the on screen map, center on the unit before moving.
				CenterOnUnit();
			}
			
			return Game.ActiveUnit.MoveTo(relX, relY);
		}

		private bool KeyDownActiveUnit(KeyboardEventArgs args)
		{
			if (Game.ActiveUnit == null || Game.ActiveUnit.Moving)
				return false;
			
			switch (args.Key)
			{
				case Key.Space:
					Game.ActiveUnit.SkipTurn();
					return true;
				case Key.NumPad1:
					return MoveTo(-1, 1);
				case Key.NumPad2:
				case Key.Down:
					return MoveTo(0, 1);
				case Key.NumPad3:
					return MoveTo(1, 1);
				case Key.NumPad4:
				case Key.Left:
					return MoveTo(-1, 0);
				case Key.NumPad5:
					GameTask.Enqueue(Show.Empty);
					return true;
				case Key.NumPad6:
				case Key.Right:
					return MoveTo(1, 0);
				case Key.NumPad7:
					return MoveTo(-1, -1);
				case Key.NumPad8:
				case Key.Up:
					return MoveTo(0, -1);
				case Key.NumPad9:
					return MoveTo(1, -1);
			}
			
			switch (args.KeyChar)
			{
				case 'B':
					GameTask.Enqueue(Orders.FoundCity(Game.ActiveUnit));
					return true;
				case 'C':
					if (Game.ActiveUnit == null) break;
					CenterOnUnit();
					return true;
				case 'D':
					if (!args.Shift) break;
					Game.DisbandUnit(Game.ActiveUnit);
					return true;
				case 'I':
					GameTask.Enqueue(Orders.BuildIrrigation(Game.ActiveUnit));
					return true;
				case 'M':
					GameTask.Enqueue(Orders.BuildMines(Game.ActiveUnit));
					break;
				case 'R':
					GameTask.Enqueue(Orders.BuildRoad(Game.ActiveUnit));
					break;
				case 'S':
					Game.ActiveUnit.Sentry = true;
					break;
				case 'F':
					if (Game.ActiveUnit is Settlers)
					{
						GameTask.Enqueue(Orders.BuildFortress(Game.ActiveUnit));
						break;
					}
					Game.ActiveUnit.Fortify = true;
					break;
				case 'U':
					if (Game.ActiveUnit is IBoardable)
					{
						return (Game.ActiveUnit as BaseUnitSea).Unload();;
					}
					break;
				case 'W':
					GameTask.Enqueue(Orders.Wait(Game.ActiveUnit));
					break;
			}

			return false;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (Game.CurrentPlayer != Human)
			{
				// Ignore all keypresses if the current player is not human
				return false;
			}
			
			switch (args.KeyChar)
			{
				case 'G':
					GameTask.Enqueue(Show.Goto);
					return true;
				case 'T':
					GameTask.Enqueue(Show.Terrain);
					return true;
			}

			if (Game.ActiveUnit != null)
			{
				return KeyDownActiveUnit(args);
			}
			
			switch (args.Key)
			{
				case Key.Space:
				case Key.Enter:
					GameTask.Enqueue(Turn.End());
					return true;
			}
			return false;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			int x = (int)Math.Floor((float)args.X / 16);
			int y = (int)Math.Floor((float)args.Y / 16);
			
			int xx = _x + x;
			int yy = _y + y;
			while (xx  < 0) xx += Map.WIDTH;
			while (xx  >= Map.WIDTH) xx -= Map.WIDTH;
			
			City city = Map[_x + x, _y + y].City;
			
			if ((args.Buttons & MouseButton.Right) > 0)
			{
				if (Game.ActiveUnit != null && (Game.ActiveUnit as BaseUnit).MoveTargets.Any(t => t.X == xx && t.Y == yy))
				{
					int relX = xx - Game.ActiveUnit.X;
					int relY = yy - Game.ActiveUnit.Y;
					if (relX < -1) relX = 1;
					if (relY > 1) relY = -1; 

					MoveTo(relX, relY);
					_update = true;
					return true;
				}

				Common.AddScreen(new Civilopedia(Map[_x + x, _y + y]));
				return _update;
			}
			if ((args.Buttons & MouseButton.Left) > 0)
			{
				_x += x - 8;
				_y += y - 6;
				while (_x < 0) _x += Map.WIDTH;
				while (_x >= Map.WIDTH) _x -= Map.WIDTH;
				while (_y < 0) _y++;
				while (_y + 12 > Map.HEIGHT) _y--;
				_update = true;
				
				if (city != null)
				{
					if (city.Owner == Game.PlayerNumber(Human) || Settings.RevealWorld)
					{
						Common.AddScreen(new CityManager(city));
					}
				}
				else if (Map[xx, yy].Units.Any(u => u.Owner == Game.PlayerNumber(Human)))
				{
					GameTask.Enqueue(Show.UnitStack(xx, yy));
				}
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