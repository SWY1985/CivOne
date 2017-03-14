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
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class PopupMessage : BaseScreen
	{
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;
				return true;
			}
			return false;
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

		public PopupMessage(byte colour, string title, string[] message)
		{
			Cursor = MouseCursor.Pointer;
			
			_canvas = new Picture(320, 200, Common.GamePlay.Palette);
			
			byte colourLight = (byte)(colour + 8);
			int lineHeight = Resources.Instance.GetFontHeight(1);
			int width = 209;
			int height = ((message.Length + (title != null ? 1 : 0)) * lineHeight) + 5;

			_canvas.FillRectangle(colourLight, 56, 16, width, 1);
			_canvas.FillRectangle(colourLight, 56, 17, 1, height - 2);
			_canvas.FillRectangle(colourLight, 56 + width - 1, 17, 1, height - 2);
			_canvas.FillRectangle(colourLight, 56, 16 + height - 1, width, 1);
			_canvas.FillRectangle(colour, 57, 17, width - 2, height - 2);

			int yy = 19 - lineHeight;
			if (title != null)
				_canvas.DrawText(title, 1, 5, 160, (yy += lineHeight), TextAlign.Center);
			for (int i = 0; i < message.Length; i++)
				_canvas.DrawText(message[i], 1, 15, 64, (yy += lineHeight));
		}
	}
}