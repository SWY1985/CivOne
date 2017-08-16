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
using CivOne.Graphics;
using CivOne.UserInterface;

namespace CivOne.Screens.Debug
{
	internal class MeetWithKing : BaseScreen
	{
		private readonly Menu _civSelect;

		private readonly Player[] _players;

		private Player _selectedPlayer = null;

		public string Value { get; private set; }

		public event EventHandler Accept, Cancel;

		private void MeetKing_Accept(object sender, EventArgs args)
		{
			_selectedPlayer = _players[_civSelect.ActiveItem];

			if (_selectedPlayer != Game.HumanPlayer)
			{
				Common.AddScreen(new King(_selectedPlayer));
			}

			if (Accept != null)
				Accept(this, null);
			Destroy();
		}

		private void MeetKing_Cancel(object sender, EventArgs args)
		{
			if (Cancel != null)
				Cancel(this, null);
			if (sender is Input)
				((Input)sender)?.Close();
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

		public MeetWithKing() : base(MouseCursor.Pointer)
		{
			Palette = Common.Screens.Last().OriginalColours;
			_players = Game.Players.Where(p => p != 0 && p != Human).ToArray();

			int fontHeight = Resources.Instance.GetFontHeight(0);
			int hh = (fontHeight * (_players.Length + 1)) + 5;
			int ww = 144;

			int xx = (320 - ww) / 2;
			int yy = (200 - hh) / 2;

			Picture menuGfx = new Picture(ww, hh)
				.Tile(Patterns.PanelGrey)
				.DrawRectangle3D()
				.As<Picture>();
			Picture menuBackground = menuGfx.GetPart(2, 11, ww - 4, hh - 11);
			Picture.ReplaceColours(menuBackground, new byte[] { 7, 22 }, new byte[] { 11, 3 });

			this.FillRectangle(5, xx - 1, yy - 1, ww + 2, hh + 2)
				.AddLayer(menuGfx, xx, yy)
				.DrawText("Meet With King", 0, 15, xx + 8, yy + 3);

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

			foreach (Player player in _players)
			{
				_civSelect.Items.Add($"{player.LeaderName} ({player.TribeName})").OnSelect(MeetKing_Accept);
			}

			_civSelect.Cancel += MeetKing_Cancel;
			_civSelect.MissClick += MeetKing_Cancel;
			_civSelect.ActiveItem = Game.PlayerNumber(Human);
		}
	}
}