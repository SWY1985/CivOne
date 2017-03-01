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
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

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

		public event EventHandler Cancel;

		private bool _hasUpdate = false;

		private int _unitX, _unitY;

		private void UnitsMenu()
		{
			_canvas = new Picture(320, 200, Common.Screens.Last().Canvas.OriginalColours);

			IUnit[] units = _units.Skip(_index).Take(15).ToArray();

			int fontHeight = Resources.Instance.GetFontHeight(0);
			int hh = (fontHeight * (units.Length + 2)) + 5;
			int ww = 136;

			int xx = (320 - ww) / 2;
			int yy = (200 - hh) / 2;

			Picture background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
			Picture menuGfx = new Picture(ww, hh);
			menuGfx.FillLayerTile(background);
			menuGfx.AddBorder(15, 8, 0, 0, ww, hh);
			Picture menuBackground = menuGfx.GetPart(2, 11, ww - 4, hh - 11);
			Picture.ReplaceColours(menuBackground, new byte[] { 7, 22 }, new byte[] { 11, 3 });

			_canvas.FillRectangle(5, xx - 1, yy - 1, ww + 2, hh + 2);
			_canvas.AddLayer(menuGfx, xx, yy);
			_canvas.DrawText("Spawn Unit...", 0, 15, xx + 8, yy + 3);

			_unitSelect = new Menu(Canvas.Palette, menuBackground)
			{
				X = xx + 2,
				Y = yy + 11,
				Width = ww - 4,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 3,
				FontId = 0,
				Indent = 8
			};

			foreach (IUnit unit in units)
			{
				_unitSelect.Items.Add(new Menu.Item(unit.Name));
				_unitSelect.Items[_unitSelect.Items.Count() - 1].Selected += SpawnUnit_Accept;
			}

			_unitSelect.Items.Add(new Menu.Item($" ---MORE---"));
			_unitSelect.Items[_unitSelect.Items.Count() - 1].Selected += SpawnUnit_More;

			_unitSelect.Cancel += SpawnUnit_Cancel;
			_unitSelect.MissClick += SpawnUnit_Cancel;
			_unitSelect.ActiveItem = (_unitSelect.Items.Count() - 1);
		}

		private void CivSelect_Accept(object sender, EventArgs args)
		{
			_selectedPlayer = Game.GetPlayer((byte)_civSelect.ActiveItem);
			_canvas = new Picture(320, 200, Common.Screens.Last().Canvas.OriginalColours);
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
			Cursor = MouseCursor.Goto;
			CloseMenus();
		}

		private void SpawnUnit_Cancel(object sender, EventArgs args)
		{
			if (Cancel != null)
				Cancel(this, null);
			Destroy();
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (_selectedUnit == null) return false;

			if (_unitX > -1 && _unitY > -1)
			{
				GamePlay gamePlay = (GamePlay)Common.Screens.First(s => (s is GamePlay));
				int ux = gamePlay.X + _unitX;
				int uy = gamePlay.Y + _unitY;
				while (ux < 0) ux += 80;
				while (ux >= 80) ux -= 80;
				Game.CreateUnit(_selectedUnit.Type, ux, uy, Game.PlayerNumber(_selectedPlayer), true);
			}
			Destroy();
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

		public override bool HasUpdate(uint gameTick)
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

				_canvas = new Picture(320, 200, Common.Screens.Last().Canvas.OriginalColours);
				if (_unitX == -1 || _unitY == -1) return _hasUpdate;
				_canvas.AddLayer(_selectedUnit.GetUnit(Game.PlayerNumber(_selectedPlayer), false), xx, yy);
				
				return _hasUpdate;
			}
			return false;
		}

		public SpawnUnit()
		{
			Cursor = MouseCursor.Pointer;

			_canvas = new Picture(320, 200, Common.Screens.Last().Canvas.OriginalColours);

			int fontHeight = Resources.Instance.GetFontHeight(0);
			int hh = (fontHeight * (Game.Players.Count() + 1)) + 5;
			int ww = 136;

			int xx = (320 - ww) / 2;
			int yy = (200 - hh) / 2;

			Picture background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
			Picture menuGfx = new Picture(ww, hh);
			menuGfx.FillLayerTile(background);
			menuGfx.AddBorder(15, 8, 0, 0, ww, hh);
			Picture menuBackground = menuGfx.GetPart(2, 11, ww - 4, hh - 11);
			Picture.ReplaceColours(menuBackground, new byte[] { 7, 22 }, new byte[] { 11, 3 });

			_canvas.FillRectangle(5, xx - 1, yy - 1, ww + 2, hh + 2);
			_canvas.AddLayer(menuGfx, xx, yy);
			_canvas.DrawText("Spawn Unit...", 0, 15, xx + 8, yy + 3);

			_civSelect = new Menu(Canvas.Palette, menuBackground)
			{
				X = xx + 2,
				Y = yy + 11,
				Width = ww - 4,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 3,
				FontId = 0,
				Indent = 8
			};

			foreach (Player player in Game.Players)
			{
				_civSelect.Items.Add(new Menu.Item(player.TribeNamePlural));
				_civSelect.Items[_civSelect.Items.Count() - 1].Selected += CivSelect_Accept;
			}

			_civSelect.Cancel += SpawnUnit_Cancel;
			_civSelect.MissClick += SpawnUnit_Cancel;
			_civSelect.ActiveItem = Game.PlayerNumber(Human);
		}
	}
}