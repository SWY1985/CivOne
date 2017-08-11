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
				_canvas.FillRectangle(15, 64, 78, 224, 10);
				_canvas.DrawText("Unknown city.", 0, 5, 82, 80);
				_canvas.FillRectangle(15, 67, 89, 135, 12);
				_canvas.DrawText(_input.Text, 0, 5, 68, 91);
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
			Cursor = MouseCursor.None;

			_canvas = new Picture(320, 200, Common.Screens.Last().OriginalColours);

			_canvas.FillRectangle(5, 64, 78, 225, 25);
			_canvas.FillRectangle(15, 65, 79, 223, 23);
			_canvas.DrawText("Where in the heck is ... (city name)", 0, 5, 66, 80);
			_canvas.FillRectangle(5, 66, 88, 137, 14);
			_canvas.FillRectangle(15, 67, 89, 135, 12);

			_input = new Input(_canvas.Palette, string.Empty, 0, 5, 11, 68, 90, 133, 10, 16);
			_input.Accept += Search_Accept;
			_input.Cancel += Search_Cancel;
		}
	}
}