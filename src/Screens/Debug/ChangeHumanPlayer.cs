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
using CivOne.UserInterface;

namespace CivOne.Screens.Debug
{
	internal class ChangeHumanPlayer : BaseScreen
	{
		private readonly Menu<Player> _civSelect;

		private Player _selectedPlayer = null;

		public string Value { get; private set; }

		public event EventHandler Accept, Cancel;

		private void ChangePlayer_Accept(object sender, MenuItemEventArgs<Player> args)
		{
			_selectedPlayer = args.Value;

			if (_selectedPlayer != Game.HumanPlayer)
			{
				Game.HumanPlayer = _selectedPlayer;
				Game.EndTurn();
			}

			if (Accept != null)
				Accept(this, null);
			Destroy();
		}

		private void ChangePlayer_Cancel(object sender, EventArgs args)
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
			return false;
		}

		public ChangeHumanPlayer() : base(MouseCursor.Pointer)
		{
			Palette = Common.Screens.Last().OriginalColours;

			int fontHeight = Resources.Instance.GetFontHeight(0);
			int hh = (fontHeight * (Game.Players.Count() + 1)) + 5;
			int ww = 128;

			int xx = (320 - ww) / 2;
			int yy = (200 - hh) / 2;

			Picture menuGfx = new Picture(ww, hh)
				.Tile(Patterns.PanelGrey)
				.DrawRectangle3D()
				.As<Picture>();
			Picture menuBackground = menuGfx.GetPart(2, 11, ww - 4, hh - 11)
				.ColourReplace(new byte[] { 7, 22 }, new byte[] { 11, 3 })
				.As<Picture>();

			this.FillRectangle(xx - 1, yy - 1, ww + 2, hh + 2, 5)
				.AddLayer(menuGfx, xx, yy)
				.DrawText("Change Human Player...", 0, 15, xx + 8, yy + 3);

			_civSelect = new Menu<Player>("ChangeHumanPlayer", Palette, menuBackground)
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
				_civSelect.Items.Add(player.TribeNamePlural, player).OnSelect(ChangePlayer_Accept);
			}

			_civSelect.Cancel += ChangePlayer_Cancel;
			_civSelect.MissClick += ChangePlayer_Cancel;
			_civSelect.ActiveItem = Game.PlayerNumber(Human);
		}
	}
}