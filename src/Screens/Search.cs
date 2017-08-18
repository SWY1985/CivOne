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

namespace CivOne.Screens
{
	internal class Search : BaseScreen
	{
		private readonly Input _input;

		private bool _done = false;

		public void Close()
		{
			_done = true;
			_input.Close();
			Destroy();
		}

		public City City { get; private set; }

		public event EventHandler Accept, Cancel;

		public override bool MouseDown(ScreenEventArgs args)
		{
			Close();
			return true;
		}

		public override bool KeyDown(KeyboardEventArgs args)
		{
			Close();
			return true;
		}

		private void Search_Accept(object sender, EventArgs args)
		{
			City = Game.GetCities().FirstOrDefault(x => x.Name.ToLower().StartsWith(_input.Text.ToLower()) && Human.Visible(x.X, x.Y));
			_done = true;
			if (City == null)
			{
				this.FillRectangle(64, 78, 224, 10, 15)
					.DrawText("Unknown city.", 0, 5, 82, 80)
					.FillRectangle(67, 89, 135, 12, 15)
					.DrawText(_input.Text, 0, 5, 68, 91);
				((Input)sender).Close();
				return;
			}
			if (Accept != null)
				Accept(this, null);
			((Input)sender).Close();
			Close();
		}

		private void Search_Cancel(object sender, EventArgs args)
		{
			_done = true;
			if (Cancel != null)
				Cancel(this, null);
			((Input)sender).Close();
			Close();
		}

		protected override bool HasUpdate(uint gameTick)
		{
			if (!_done && !Common.HasScreenType<Input>())
			{
				Common.AddScreen(_input);
			}
			return false;
		}

		public Search()
		{
			Palette = Common.Screens.Last().OriginalColours;

			this.FillRectangle(64, 78, 225, 25, 5)
				.FillRectangle(65, 79, 223, 23, 15)
				.DrawText("Where in the heck is ... (city name)", 0, 5, 66, 80)
				.FillRectangle(66, 88, 137, 14, 5)
				.FillRectangle(67, 89, 135, 12, 15);

			_input = new Input(Palette, string.Empty, 0, 5, 11, 68, 90, 133, 10, 16);
			_input.Accept += Search_Accept;
			_input.Cancel += Search_Cancel;
		}
	}
}