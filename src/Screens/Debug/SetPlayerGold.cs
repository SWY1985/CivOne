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
using CivOne.Tasks;
using CivOne.UserInterface;

namespace CivOne.Screens.Debug
{
	internal class SetPlayerGold : BaseScreen
	{
		private readonly Menu _civSelect;

		private Input _input;

		private Player _selectedPlayer = null;

		public string Value { get; private set; }

		public event EventHandler Accept, Cancel;

		private void CivSelect_Accept(object sender, EventArgs args)
		{
			Bitmap.Clear();

			this.FillRectangle(80, 80, 161, 33, 11)
				.FillRectangle(81, 81, 159, 31, 15)
				.DrawText("Set Player Gold...", 0, 5, 88, 82)
				.FillRectangle(88, 95, 105, 14, 5)
				.FillRectangle(89, 96, 103, 12, 15);

			_selectedPlayer = Game.GetPlayer((byte)_civSelect.ActiveItem);

			_input = new Input(Palette, _selectedPlayer.Gold.ToString(), 0, 5, 11, 90, 97, 101, 10, 5);
			_input.Accept += PlayerGold_Accept;
			_input.Cancel += PlayerGold_Cancel;

			CloseMenus();
		}

		private void PlayerGold_Accept(object sender, EventArgs args)
		{
			Value = (sender as Input).Text;
			
			short playerGold;
			if (!short.TryParse(Value, out playerGold) || playerGold < 0 || playerGold > 30000)
			{
				GameTask.Enqueue(Message.Error("-- DEBUG: Set Player Gold --", $"The value {Value} is invalid or out of range.", "Please enter a value between 0 and", "30000."));
			}
			else
			{
				_selectedPlayer.Gold = playerGold;
				GameTask.Enqueue(Message.General($"{_selectedPlayer.TribeName} gold set to {playerGold}$."));
			}

			if (Accept != null)
				Accept(this, null);
			if (sender is Input)
				((Input)sender)?.Close();
			Destroy();
		}

		private void PlayerGold_Cancel(object sender, EventArgs args)
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
			else if (_selectedPlayer != null && !Common.HasScreenType<Input>())
			{
				Common.AddScreen(_input);
			}
			return false;
		}

		public SetPlayerGold() : base(MouseCursor.Pointer)
		{
			Palette = Common.Screens.Last().OriginalColours;

			int fontHeight = Resources.Instance.GetFontHeight(0);
			int hh = (fontHeight * (Game.Players.Count() + 1)) + 5;
			int ww = 108;

			int xx = (320 - ww) / 2;
			int yy = (200 - hh) / 2;

			Picture menuGfx = new Picture(ww, hh)
				.Tile(Patterns.PanelGrey)
				.DrawRectangle3D()
				.As<Picture>();
			Picture menuBackground = menuGfx.GetPart(2, 11, ww - 4, hh - 11)
				.ColourReplace((7, 11), (22, 3))
				.As<Picture>();

			this.FillRectangle(xx - 1, yy - 1, ww + 2, hh + 2, 5)
				.AddLayer(menuGfx, xx, yy)
				.DrawText("Set Player Gold...", 0, 15, xx + 8, yy + 3);

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

			_civSelect.Cancel += PlayerGold_Cancel;
			_civSelect.MissClick += PlayerGold_Cancel;
			_civSelect.ActiveItem = Game.PlayerNumber(Human);
		}
	}
}