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
using CivOne.Graphics;
using CivOne.IO;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.Units;

namespace CivOne.Screens.GamePlayPanels
{
	internal class GameMap : BaseScreen
	{
		private IUnit ActiveUnit => Game.ActiveUnit;
		
		private readonly Palette _palette;
		private Point _helperDirection = new Point(0, 0);
		private bool _update = true;
		private bool _fullRedraw = false;
		private int _x, _y;
		private IUnit _lastUnit;
		private ushort _lastTurn;

		private int _tilesX = 15, _tilesY = 12;

		internal int X => _x;
		internal int Y => _y;

		private ITile[,] Tiles => Map[_x, _y, _tilesX, _tilesY];

		private int GetX(ITile tile)
		{
			ITile[,] tiles = Tiles;
			for (int xx = 0; xx < Tiles.GetLength(0); xx++)
			{
				if (tiles[xx, 0].X == tile.X) return xx;
			}
			return -1;
		}

		private int GetY(ITile tile)
		{
			ITile[,] tiles = Tiles;
			for (int yy = 0; yy < Tiles.GetLength(1); yy++)
			{
				if (tiles[0, yy].Y == tile.Y) return yy;
			}
			return -1;
		}

		private IEnumerable<ITile> TileList
		{
			get
			{
				ITile[,] tiles = Tiles;
				for (int yy = 0; yy < tiles.GetLength(1); yy++)
				for (int xx = 0; xx < tiles.GetLength(0); xx++)
				{
					ITile tile = tiles[xx, yy];
					if (!Settings.RevealWorld && !Human.Visible(tile)) continue;
					yield return tile;
				}
			}
		}

		private void DrawHelperArrows(int x, int y)
		{
			if (_helperDirection.X == 0 && _helperDirection.Y == 0) return;
			
			if (_helperDirection.X < 0)
			{
				this.AddLayer(Icons.HelperArrow(Direction.North), x - 16, y - 16)
					.AddLayer(Icons.HelperArrow(Direction.West), x - 16, y)
					.AddLayer(Icons.HelperArrow(Direction.South), x - 16, y + 16);
			}
			if (_helperDirection.X > 0)
			{
				this.AddLayer(Icons.HelperArrow(Direction.North), x + 16, y - 16)
					.AddLayer(Icons.HelperArrow(Direction.East), x + 16, y)
					.AddLayer(Icons.HelperArrow(Direction.South), x + 16, y + 16);
			}
			if (_helperDirection.Y < 0)
			{
				this.AddLayer(Icons.HelperArrow(Direction.West), x - 16, y - 16)
					.AddLayer(Icons.HelperArrow(Direction.North), x, y - 16)
					.AddLayer(Icons.HelperArrow(Direction.East), x + 16, y - 16);
			}
			if (_helperDirection.Y > 0)
			{
				this.AddLayer(Icons.HelperArrow(Direction.West), x - 16, y + 16)
					.AddLayer(Icons.HelperArrow(Direction.South), x, y + 16)
					.AddLayer(Icons.HelperArrow(Direction.East), x + 16, y + 16);
			}
		}
		
		public bool MustUpdate(uint gameTick)
		{
			IUnit unit = ActiveUnit;

			if ((gameTick % 2) == 0 && (_lastTurn != Game.GameTurn || _lastUnit != unit))
			{
				if (_lastUnit != unit && unit != null && Game.Human == unit.Owner && ShouldCenter())
				{
					CenterOnUnit();
				}
				_fullRedraw = true;
				_update = true;
				_lastUnit = unit;
				_lastTurn = Game.GameTurn;
			}

			// Check if the active unit is on the screen and the blink status has changed.
			if (unit == null)
			{
				_update = true;
				return false;
			}

			if (TileList.Any(t => t != null && t.X == unit.X && t.Y == unit.Y) && (gameTick % 2) == 0)
			{
				// _lastUnit = unit;
				_update = true;
			}
			else if (unit.Moving)
			{
				_update = true;
			}
			else if (unit != _lastUnit && ShouldCenter())
			{
				if (!Settings.RevealWorld && Human != unit.Owner && !Human.Visible(unit.Tile))
				{
					return (_update = false);
				}
				CenterOnUnit();
				_update = true;
				_fullRedraw = true;
			}
			else
			{
				_update = (unit != _lastUnit);
			}
			return _update;
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (!(_update || _fullRedraw)) return false;
			if (Game.MovingUnit == null && (gameTick % 2 == 1)) return false;

			Player renderPlayer = Settings.RevealWorld ? null : Human;

			IUnit activeUnit = ActiveUnit;
			if (Game.MovingUnit != null && !_fullRedraw)
			{
				IUnit movingUnit = Game.MovingUnit;
				ITile tile = movingUnit.Tile;
				int dx = GetX(tile);
				int dy = GetY(tile);
				if (dx < _tilesX && dy < _tilesY)
				{
					dx *= 16; dy *= 16;

					MoveUnit movement = movingUnit.Movement;
					this.AddLayer(Map[movingUnit.X - 1, movingUnit.Y - 1, 3, 3].ToBitmap(player: renderPlayer), dx - 16, dy - 16, dispose: true);
					using (IBitmap unitPicture = movingUnit.GetUnit(movingUnit.Owner))
					{
						this.AddLayer(unitPicture, dx + movement.X, dy + movement.Y);
						if (movingUnit is IBoardable && tile.Units.Any(u => u.Class == UnitClass.Land && (tile.City == null || (tile.City != null && u.Sentry))))
						{
							this.AddLayer(unitPicture, dx + movement.X - 1, dy + movement.Y - 1);
						}
					}
					return true;
				}
			}
			else if (_fullRedraw)
			{
				_fullRedraw = false;
				this.Clear(5)
					.AddLayer(Tiles.ToBitmap(player: renderPlayer), dispose: true);
			}

			if (activeUnit != null && Game.CurrentPlayer == Human && !GameTask.Any())
			{
				ITile tile = activeUnit.Tile;
				int dx = GetX(tile);
				int dy = GetY(tile);
				if (dx < _tilesX && dy < _tilesY)
				{
					dx *= 16; dy *= 16;
					
					// blink status
					TileSettings setting = ((gameTick % 4) < 2) ? TileSettings.BlinkOn : TileSettings.BlinkOff;
					this.AddLayer(tile.ToBitmap(setting), dx, dy, dispose: true);

					DrawHelperArrows(dx, dy);
				}
				return true;
			}
			
			_update = false;
			return true;
		}

		internal void ForceRefresh()
		{
			_fullRedraw = true;
		}
		
		internal void CenterOnPoint(int x, int y)
		{
			_x = x - 7;
			_y = y - 6;
			while (_y < 0) _y++;
			while (_y + 11 >= Map.HEIGHT) _y--;
			_update = true;
			_fullRedraw = true;
		}
		
		private void CenterOnUnit()
		{
			if (Game.ActiveUnit == null) return;
			CenterOnPoint(Game.ActiveUnit.X, Game.ActiveUnit.Y);
		}

		private bool ShouldCenter(int relX = 0, int relY = 0)
		{
			if (Game.ActiveUnit == null)
				return false;
			int viewRange = 1;
			if (Game.ActiveUnit.Class == UnitClass.Water)
			{
				viewRange = (Game.ActiveUnit as BaseUnitSea).Range;
			}
			if (Game.ActiveUnit.Class == UnitClass.Air)
			{
				viewRange = 2;
			}
			return (!Map.QueryMapPart(_x + viewRange, _y + viewRange, (_tilesX - (viewRange * 2)), (_tilesY - (viewRange * 2))).Any(t => t.X == Game.ActiveUnit.X + relX && t.Y == Game.ActiveUnit.Y + relY));
		}

		private bool MoveTo(int relX, int relY)
		{
			_helperDirection = new Point(0, 0);
			
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
			
			if (args.Key == Key.Space)
			{
				Game.ActiveUnit.SkipTurn();
				return true;
			}
			else if (Settings.ArrowHelper)
			{
				switch (args.Key)
				{
					case Key.NumPad1:
						return MoveTo(-1, 1);
					case Key.NumPad2:
						return MoveTo(0, 1);
					case Key.NumPad3:
						return MoveTo(1, 1);
					case Key.NumPad4:
						return MoveTo(-1, 0);
					case Key.NumPad5:
						GameTask.Enqueue(Show.Empty);
						return true;
					case Key.NumPad6:
						return MoveTo(1, 0);
					case Key.NumPad7:
						return MoveTo(-1, -1);
					case Key.NumPad8:
						return MoveTo(0, -1);
					case Key.NumPad9:
						return MoveTo(1, -1);
					case Key.Escape:
						_helperDirection = new Point(0, 0);
						return true;
					case Key.Down:
						_helperDirection.Y++;
						break;
					case Key.Up:
						_helperDirection.Y--;
						break;
					case Key.Left:
						_helperDirection.X--;
						break;
					case Key.Right:
						_helperDirection.X++;
						break;
					default:
						_helperDirection = new Point(0, 0);
						break;
				}

				if (Math.Abs(_helperDirection.X) + Math.Abs(_helperDirection.Y) >= 2)
				{
					int x = 0, y = 0;
					if (_helperDirection.X < 0)
						x = -1;
					else if (_helperDirection.X > 0)
						x = 1;
					
					if (_helperDirection.Y < 0)
						y = -1;
					else if (_helperDirection.Y > 0)
						y = 1;
					
					_helperDirection = new Point(0, 0);
					return MoveTo(x, y);
				}
			}
			else
			{
				switch (args.Key)
				{
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
				case 'H':
					Game.ActiveUnit.SetHome();
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
				if (city != null && (Human == city.Owner || Settings.RevealWorld))
				{
					Common.AddScreen(new CityManager(city));
				}
				else if (Map[xx, yy].Units.Any(u => Human == u.Owner))
				{
					GameTask.Enqueue(Show.UnitStack(xx, yy));
				}
				else
				{
					_x += x - 8;
					_y += y - 6;
					while (_x < 0) _x += Map.WIDTH;
					while (_x >= Map.WIDTH) _x -= Map.WIDTH;
					while (_y < 0) _y++;
					while (_y + _tilesY > Map.HEIGHT) _y--;
					_update = true;
					_fullRedraw = true;
				}
			}
			return _update;
		}

		public void Resize(int width, int height)
		{
			_tilesX = (int)Math.Ceiling((double)width / 16);
			_tilesY = (int)Math.Ceiling((double)height / 16);

			Bitmap = new Bytemap(width, height);
			
			while (_y + _tilesY > Map.HEIGHT) _y--;
			_update = true;
			_fullRedraw = true;
		}
		
		public GameMap()
		{
			_x = 0;
			_y = 0;
			
			_palette = Resources.Instance.LoadPIC("SP257").Palette.Copy();
		}
	}
}