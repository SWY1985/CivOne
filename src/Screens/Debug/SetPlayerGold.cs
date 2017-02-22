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
using CivOne.GFX;
using CivOne.Tasks;
using CivOne.Templates;

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
			Cursor = MouseCursor.None;

			_canvas = new Picture(320, 200, Common.Screens.Last().Canvas.OriginalColours);

			_canvas.FillRectangle(11, 80, 80, 161, 33);
			_canvas.FillRectangle(15, 81, 81, 159, 31);
			_canvas.DrawText("Set Player Gold...", 0, 5, 88, 82);
			_canvas.FillRectangle(5, 88, 95, 105, 14);
			_canvas.FillRectangle(15, 89, 96, 103, 12);

			_selectedPlayer = Game.GetPlayer((byte)_civSelect.ActiveItem);

			_input = new Input(_canvas.Palette, _selectedPlayer.Gold.ToString(), 0, 5, 11, 90, 97, 101, 10, 5);
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
			((Input)sender).Close();
			Destroy();
		}

		private void PlayerGold_Cancel(object sender, EventArgs args)
		{
			if (Cancel != null)
				Cancel(this, null);
			((Input)sender).Close();
			CloseMenus();
			Destroy();
		}

		public override bool HasUpdate(uint gameTick)
		{
			if (_selectedPlayer == null && Common.TopScreen.GetType() != typeof(Menu))
			{
				AddMenu(_civSelect);
				return false;
			}
			else if (_selectedPlayer != null && !Common.HasScreenType(typeof(Input)))
			{
				Common.AddScreen(_input);
			}
			return false;
		}

		public SetPlayerGold()
		{
			Cursor = MouseCursor.Pointer;

			_canvas = new Picture(320, 200, Common.Screens.Last().Canvas.OriginalColours);

			int fontHeight = Resources.Instance.GetFontHeight(0);
			int hh = (fontHeight * (Game.Players.Count() + 1)) + 5;
			int ww = 108;

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
			_canvas.DrawText("Set Player Gold...", 0, 15, xx + 8, yy + 3);

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

			_civSelect.Cancel += PlayerGold_Cancel;
			_civSelect.MissClick += PlayerGold_Cancel;
			_civSelect.ActiveItem = Game.PlayerNumber(Human);
		}
	}
}