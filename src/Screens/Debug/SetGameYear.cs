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

namespace CivOne.Screens.Debug
{
	internal class SetGameYear : BaseScreen
	{
		private readonly Input _input;

		public string Value { get; private set; }

		public event EventHandler Accept, Cancel;

		private void GameYear_Accept(object sender, EventArgs args)
		{
			Value = (sender as Input).Text;
			
			int gameYear;
			if (!int.TryParse(Value, out gameYear) || gameYear < -4000 || gameYear > 6000)
			{
				GameTask.Enqueue(Message.Error("-- DEBUG: Set Game Year --", $"The value {Value} is invalid or out of range.", "Please enter a value between -4000 and", "6000."));
			}
			else
			{
				Game.GameState._gameTurn = Common.YearToTurn(gameYear);
				GameTask.Enqueue(Message.General($"Game year set to {Game.GameState.GameYear}."));
			}

			if (Accept != null)
				Accept(this, null);
			if (sender is Input)
				((Input)sender)?.Close();
			Destroy();
		}

		private void GameYear_Cancel(object sender, EventArgs args)
		{
			if (Cancel != null)
				Cancel(this, null);
			if (sender is Input)
				((Input)sender)?.Close();
			Destroy();
		}

		protected override bool HasUpdate(uint gameTick)
		{
			if (!Common.HasScreenType<Input>())
			{
				Common.AddScreen(_input);
			}
			return false;
		}

		public SetGameYear()
		{
			Palette = Common.Screens.Last().OriginalColours;

			this.FillRectangle(80, 80, 161, 33, 11)
				.FillRectangle(81, 81, 159, 31, 15)
				.DrawText("Set Game Year...", 0, 5, 88, 82)
				.FillRectangle(88, 95, 105, 14, 5)
				.FillRectangle(89, 96, 103, 12, 15);

			_input = new Input(Palette, Common.TurnToYear(Game.GameState._gameTurn).ToString(), 0, 5, 11, 90, 97, 101, 10, 5);
			_input.Accept += GameYear_Accept;
			_input.Cancel += GameYear_Cancel;
		}
	}
}