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
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Graphics.Sprites;
using CivOne.UserInterface;

namespace CivOne.Screens.Debug
{
	internal class SetPlayerAdvances : BaseScreen
	{
		private readonly IAdvance[] _advances = Reflect.GetAdvances().OrderBy(x => x.Name).ToArray();

		private readonly Menu _civSelect;

		private Menu _advanceSelect;

		private int _index = 0;

		private int _selected = -1;

		private Player _selectedPlayer = null;

		public string Value { get; private set; }

		public event EventHandler Cancel;

		private void AdvancesMenu()
		{
			Palette = Common.Screens.Last().OriginalColours;

			IAdvance[] advances = _advances.Skip(_index).Take(15).ToArray();

			int fontHeight = Resources.GetFontHeight(0);
			int hh = (fontHeight * (advances.Length + 2)) + 5;
			int ww = 136;

			int xx = (320 - ww) / 2;
			int yy = (200 - hh) / 2;

			Picture menuGfx = new Picture(ww, hh)
				.Tile(Pattern.PanelGrey)
				.DrawRectangle3D()
				.As<Picture>();
			IBitmap menuBackground = menuGfx[2, 11, ww - 4, hh - 11].ColourReplace((7, 11), (22, 3));

			this.FillRectangle(xx - 1, yy - 1, ww + 2, hh + 2, 5)
				.AddLayer(menuGfx, xx, yy)
				.DrawText("Set Player Advances...", 0, 15, xx + 8, yy + 3);

			_advanceSelect = new Menu(Palette, menuBackground)
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

			foreach (IAdvance advance in advances)
			{
				bool hasAdvance = _selectedPlayer.HasAdvance(advance);
				_advanceSelect.Items.Add($"{(hasAdvance ? '^' : ' ')}{advance.Name}").OnSelect(PlayerAdvances_Accept);
			}

			_advanceSelect.Items.Add($" ---MORE---").OnSelect(PlayerAdvances_More);

			_advanceSelect.Cancel += PlayerAdvances_Cancel;
			_advanceSelect.MissClick += PlayerAdvances_Cancel;
			if (_selected == -1)
				_advanceSelect.ActiveItem = (_advanceSelect.Items.Count - 1);
			else
				_advanceSelect.ActiveItem = (_selected + 1);
		}

		private void CivSelect_Accept(object sender, EventArgs args)
		{
			_selectedPlayer = Game.GameState.GetPlayer((byte)_civSelect.ActiveItem);

			CloseMenus();
		}

		private void PlayerAdvances_More(object sender, EventArgs args)
		{
			_index += 15;
			if (_index > _advances.Count()) _index = 0;
			CloseMenus();
		}

		private void PlayerAdvances_Accept(object sender, EventArgs args)
		{
			IAdvance advance = _advances[_advanceSelect.ActiveItem + _index];
			_selected = _advanceSelect.ActiveItem;
			if (_selectedPlayer.HasAdvance(advance))
				_selectedPlayer.DeleteAdvance(advance);
			else
				_selectedPlayer.AddAdvance(advance);
			CloseMenus();
		}

		private void PlayerAdvances_Cancel(object sender, EventArgs args)
		{
			if (Cancel != null)
				Cancel(this, null);
			Destroy();
		}

		protected override bool HasUpdate(uint gameTick)
		{
			if (_selectedPlayer == null && Common.TopScreen.GetType() != typeof(Menu))
			{
				AddMenu(_civSelect);
				return false;
			}
			else if (_selectedPlayer != null && Common.TopScreen.GetType() != typeof(Menu))
			{
				AdvancesMenu();
				AddMenu(_advanceSelect);
			}
			return false;
		}

		public SetPlayerAdvances() : base(MouseCursor.Pointer)
		{
			Palette = Common.Screens.Last().OriginalColours;

			int fontHeight = Resources.GetFontHeight(0);
			int hh = (fontHeight * (Game.Players.Count() + 1)) + 5;
			int ww = 136;

			int xx = (320 - ww) / 2;
			int yy = (200 - hh) / 2;

			Picture menuGfx = new Picture(ww, hh)
				.Tile(Pattern.PanelGrey)
				.DrawRectangle3D()
				.As<Picture>();
			IBitmap menuBackground = menuGfx[2, 11, ww - 4, hh - 11].ColourReplace((7, 11), (22, 3));

			this.FillRectangle(xx - 1, yy - 1, ww + 2, hh + 2, 5)
				.AddLayer(menuGfx, xx, yy)
				.DrawText("Set Player Advances...", 0, 15, xx + 8, yy + 3);

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

			_civSelect.Cancel += PlayerAdvances_Cancel;
			_civSelect.MissClick += PlayerAdvances_Cancel;
			_civSelect.ActiveItem = Game.GameState.PlayerNumber(Human);
		}
	}
}