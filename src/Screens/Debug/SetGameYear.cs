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
				Game.GameTurn = Common.YearToTurn(gameYear);
				GameTask.Enqueue(Message.General($"Game year set to {Game.GameYear}."));
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
			_canvas = new Picture(320, 200, Common.Screens.Last().OriginalColours);

			_canvas.FillRectangle(11, 80, 80, 161, 33);
			_canvas.FillRectangle(15, 81, 81, 159, 31);
			_canvas.DrawText("Set Game Year...", 0, 5, 88, 82);
			_canvas.FillRectangle(5, 88, 95, 105, 14);
			_canvas.FillRectangle(15, 89, 96, 103, 12);

			_input = new Input(_canvas.Palette, Common.TurnToYear(Game.GameTurn).ToString(), 0, 5, 11, 90, 97, 101, 10, 5);
			_input.Accept += GameYear_Accept;
			_input.Cancel += GameYear_Cancel;
		}
	}
}