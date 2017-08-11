// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne.Screens
{
	[Break]
	internal class Demo : BaseScreen
	{
		private readonly byte[] _textColours;
		
		protected override bool HasUpdate(uint gameTick)
		{
			_canvas.Cycle(224, 254);
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public Demo()
		{
			Cursor = MouseCursor.Pointer;
			Picture background = Resources.Instance.LoadPIC("BIRTH1");
			Picture logo = Resources.Instance.LoadPIC("LOGO");
			switch (Settings.GraphicsMode)
			{
				case GraphicsMode.Graphics256:
					_textColours = new byte[] { 239, 236, 233, 5, 229 };
					break;
				case GraphicsMode.Graphics16:
					_textColours = new byte[] { 15, 15, 7, 5, 8 };
					break;
			}
			
			_canvas = new Picture(320, 200, logo.Palette);
			AddLayer(background, 0, 0);
			AddLayer(logo, 0, 0);
			
			_canvas.DrawText("One more turn...", 3, _textColours[0], 160, 160, TextAlign.Center);
			_canvas.DrawText("One more turn...", 3, _textColours[2], 160, 162, TextAlign.Center);
			_canvas.DrawText("One more turn...", 3, _textColours[1], 160, 161, TextAlign.Center);
		}
	}
}