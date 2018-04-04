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
	internal class WindowTitle : BaseScreen
	{
		private readonly Input _input;

		private bool _done = false;

		public void Close()
		{
			_done = true;
			_input.Close();
			Destroy();
		}

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

		private void Input_Accept(object sender, EventArgs args)
		{
			_done = true;
			Settings.WindowTitle = _input.Text;
			Accept?.Invoke(this, null);
			((Input)sender).Close();
			Close();
		}

		private void Input_Cancel(object sender, EventArgs args)
		{
			_done = true;
			Cancel?.Invoke(this, null);
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

		public WindowTitle()
		{
			Palette = Common.Screens.Last().OriginalColours;

			this.FillRectangle(64, 78, 225, 25, 5)
				.FillRectangle(65, 79, 223, 23, 15)
				.DrawText("Set window title...", 0, 5, 66, 80)
				.FillRectangle(66, 88, 221, 14, 5)
				.FillRectangle(67, 89, 219, 12, 15);

			_input = new Input(Palette, Settings.WindowTitle, 0, 5, 11, 68, 90, 133, 10, 32);
			_input.Accept += Input_Accept;
			_input.Cancel += Input_Cancel;
		}
	}
}