// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.UserInterface;

namespace CivOne.Screens.Debug
{
	internal class SpawnUnit : BaseScreen
	{
		private readonly IUnit[] _units = Reflect.GetUnits().OrderBy(x => (int)x.Type).ToArray();

		private readonly Menu _civSelect;

		private Menu _unitSelect;

		private int _index = 0;

		private Player _selectedPlayer = null;

		private IUnit _selectedUnit = null;

		public string Value { get; private set; }

		private MouseCursor _cursor = MouseCursor.Pointer;
		public override MouseCursor Cursor => _cursor;

		public event EventHandler Cancel;

		private bool _hasUpdate = false;

		private int _unitX, _unitY;

		private int UnitX
		{
			get
			{
				int output = Common.GamePlay.X + _unitX;
				while (output < 0) output += Map.WIDTH;
				while (output >= Map.WIDTH) output -= Map.WIDTH;
				return output;
			}
		}

		private int UnitY
		{
			get
			{
				return Common.GamePlay.Y + _unitY;
			}
		}

		private void UnitsMenu()
		{
			_canvas = new Picture(320, 200, Common.Screens.Last().OriginalColours);

			IUnit[] units = _units.Skip(_index).Take(15).ToArray();

			int fontHeight = Resources.Instance.GetFontHeight(0);
			int hh = (fontHeight * (units.Length + 2)) + 5;
			int ww = 136;

			int xx = (320 - ww) / 2;
			int yy = (200 - hh) / 2;

			Picture menuGfx = new Picture(ww, hh);
			menuGfx.Tile(Patterns.PanelGrey);
			menuGfx.AddBorder(15, 8, 0, 0, ww, hh);
			Picture menuBackground = menuGfx.GetPart(2, 11, ww - 4, hh - 11);
			Picture.ReplaceColours(menuBackground, new byte[] { 7, 22 }, new byte[] { 11, 3 });

			this.FillRectangle(5, xx - 1, yy - 1, ww + 2, hh + 2)
				.AddLayer(menuGfx, xx, yy)
				.DrawText("Spawn Unit...", 0, 15, xx + 8, yy + 3);

			_unitSelect = new Menu(Palette, menuBackground)
			{
				X = xx + 2,
				Y = yy + 11,
				MenuWidth = ww - 4,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 3,
				FontId = 0,
				Indent = 8
			};

			foreach (IUnit unit in units)
			{
				_unitSelect.Items.Add(unit.Name).OnSelect(SpawnUnit_Accept);
			}

			_unitSelect.Items.Add($" ---MORE---").OnSelect(SpawnUnit_More);

			_unitSelect.Cancel += SpawnUnit_Cancel;
			_unitSelect.MissClick += SpawnUnit_Cancel;
			_unitSelect.ActiveItem = (_unitSelect.Items.Count - 1);
		}

		private void CivSelect_Accept(object sender, EventArgs args)
		{
			_selectedPlayer = Game.GetPlayer((byte)_civSelect.ActiveItem);
			_canvas = new Picture(320, 200, Common.Screens.Last().OriginalColours);
			CloseMenus();
		}

		private void SpawnUnit_More(object sender, EventArgs args)
		{
			_index += 15;
			if (_index > _units.Count()) _index = 0;
			CloseMenus();
		}

		private void SpawnUnit_Accept(object sender, EventArgs args)
		{
			_selectedUnit = _units[_unitSelect.ActiveItem + _index];
			CloseMenus();
		}

		private void SpawnUnit_Cancel(object sender, EventArgs args)
		{
			if (Cancel != null)
				Cancel(this, null);
			Destroy();
		}

		private bool ValidTile
		{
			get
			{
				if (_unitX < 0 || _unitY < 0) return false;
				ITile tile = Map[UnitX, UnitY];
				if (tile.Units.Any(x => _selectedPlayer != x.Owner)) return false;
				if (_selectedUnit.Class == UnitClass.Land && tile.City != null)
				{
					return (_selectedPlayer == tile.City.Owner);
				}
				if (_selectedUnit.Class == UnitClass.Land && tile.Type == Terrain.Ocean)
				{
					if (!tile.Units.Any(x => x.Class == UnitClass.Water && x is IBoardable)) return false;
					
					int capacity = tile.Units.Where(x => x.Class == UnitClass.Water && x is IBoardable).Sum(x => (x as IBoardable).Cargo);
					int unitCount = tile.Units.Count(x => x.Class == UnitClass.Land);
					return (unitCount < capacity);
				}
				if (_selectedUnit.Class == UnitClass.Water && tile.Type != Terrain.Ocean)
				{
					return (tile.City != null && _selectedPlayer == tile.City.Owner);
				}
				return true;
			}
		}

		private void SidebarHint()
		{
			int xx = (Settings.RightSideBar ? 240 : 0);
			this.FillRectangle(15, xx, 153, 79, 1)
				.FillRectangle(9, xx, 154, 80, 46)
				.FillRectangle(1, xx + 1, 155, 78, 44)
				.DrawText("Left click:", 1, 15, xx + 3, 157)
				.DrawText("One unit", 1, 15, xx + 8, 164)
				.DrawText("Right click:", 1, 15, xx + 3, 171)
				.DrawText("Multiple units", 1, 15, xx + 8, 178)
				.DrawText("Escape key:", 1, 15, xx + 3, 185)
				.DrawText("Cancel", 1, 15, xx + 8, 192);
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			switch (args.Key)
			{
				case Key.Escape:
					Destroy();
					return true;
			}
			return false;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (_selectedUnit == null) return false;

			if (ValidTile)
			{
				IUnit unit = Game.CreateUnit(_selectedUnit.Type, UnitX, UnitY, Game.PlayerNumber(_selectedPlayer), false);
				if (unit.Class == UnitClass.Land && Map[UnitX, UnitY].Type == Terrain.Ocean) unit.Sentry = true;

				if (Game.PlayerNumber(_selectedPlayer) < Game.PlayerNumber(Game.CurrentPlayer))
				{
					unit.MovesLeft = 0;
				}

				if (unit.Class == UnitClass.Land && Map[UnitX, UnitY].Hut)
				{
					Map[UnitX, UnitY].Hut = false;
				}

				if (unit.Class == UnitClass.Air)
				{
					(unit as BaseUnitAir).FuelLeft = (unit as BaseUnitAir).TotalFuel;
				}
				
				unit.Explore();
				_hasUpdate = true;
				Common.GamePlay.RefreshMap();
			}
			if ((args.Buttons & MouseButton.Left) > 0 || !ValidTile)
			{
				Destroy();
			}
			return true;
		}

		public override bool MouseMove(ScreenEventArgs args)
		{
			if (_selectedUnit == null) return false;

			if (args.Y < 8 || (Settings.RightSideBar && args.X > 240) || (!Settings.RightSideBar && args.X < 80))
			{
				_unitX = -1;
				_unitY = -1;
				_hasUpdate = true;
				return true;
			}

			_unitX = (int)Math.Floor(((double)args.X - (Settings.RightSideBar ? 0 : 80)) / 16);
			_unitY = (int)Math.Floor(((double)args.Y - 8) / 16);
			_hasUpdate = true;
			return true;
		}

		protected override bool HasUpdate(uint gameTick)
		{
			if (_selectedPlayer == null && Common.TopScreen.GetType() != typeof(Menu))
			{
				AddMenu(_civSelect);
				return false;
			}
			else if (_selectedPlayer != null && _selectedUnit == null && Common.TopScreen.GetType() != typeof(Menu))
			{
				UnitsMenu();
				AddMenu(_unitSelect);
			}
			else if (_selectedUnit != null && _hasUpdate)
			{
				int xx = (_unitX * 16) + (Settings.RightSideBar ? 0 : 80);
				int yy = (_unitY * 16) + 8;

				if (xx > 320 || yy > 200) return false;

				Bitmap.Clear();
				SidebarHint();
				_cursor = ValidTile ? MouseCursor.Goto : MouseCursor.Pointer;
				if (!ValidTile) return _hasUpdate;
				this.AddLayer(_selectedUnit.GetUnit(Game.PlayerNumber(_selectedPlayer), false), xx, yy);
				
				return _hasUpdate;
			}
			return false;
		}

		public SpawnUnit()
		{
			_canvas = new Picture(320, 200, Common.DefaultPalette);

			int fontHeight = Resources.Instance.GetFontHeight(0);
			int hh = (fontHeight * (Game.Players.Count() + 1)) + 5;
			int ww = 136;

			int xx = (320 - ww) / 2;
			int yy = (200 - hh) / 2;

			Picture menuGfx = new Picture(ww, hh);
			menuGfx.Tile(Patterns.PanelGrey);
			menuGfx.AddBorder(15, 8, 0, 0, ww, hh);
			Picture menuBackground = menuGfx.GetPart(2, 11, ww - 4, hh - 11);
			Picture.ReplaceColours(menuBackground, new byte[] { 7, 22 }, new byte[] { 11, 3 });

			this.FillRectangle(5, xx - 1, yy - 1, ww + 2, hh + 2)
				.AddLayer(menuGfx, xx, yy)
				.DrawText("Spawn Unit...", 0, 15, xx + 8, yy + 3);

			_civSelect = new Menu(Palette, menuBackground)
			{
				X = xx + 2,
				Y = yy + 11,
				MenuWidth = ww - 4,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 3,
				FontId = 0,
				Indent = 8
			};

			foreach (Player player in Game.Players)
			{
				_civSelect.Items.Add(player.TribeNamePlural).OnSelect(CivSelect_Accept);
			}

			_civSelect.Cancel += SpawnUnit_Cancel;
			_civSelect.MissClick += SpawnUnit_Cancel;
			_civSelect.ActiveItem = Game.PlayerNumber(Human);
		}
	}
}