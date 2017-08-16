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
	internal class PopupMessage : BaseScreen
	{
		private bool _update = true;
		
		protected override bool HasUpdate(uint gameTick)
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

		public PopupMessage(byte colour, string title, string[] message) : base(MouseCursor.Pointer)
		{
			Palette = Common.DefaultPalette;
			
			byte colourLight = (byte)(colour + 8);
			int lineHeight = Resources.Instance.GetFontHeight(1);
			int width = 209;
			int height = ((message.Length + (title != null ? 1 : 0)) * lineHeight) + 5;

			this.FillRectangle(56, 16, width, 1, colourLight)
				.FillRectangle(56, 17, 1, height - 2, colourLight)
				.FillRectangle(56 + width - 1, 17, 1, height - 2, colourLight)
				.FillRectangle(56, 16 + height - 1, width, 1, colourLight)
				.FillRectangle(57, 17, width - 2, height - 2, colour);

			int yy = 19 - lineHeight;
			if (title != null)
				this.DrawText(title, 1, 5, 160, (yy += lineHeight), TextAlign.Center);
			for (int i = 0; i < message.Length; i++)
				this.DrawText(message[i], 1, 15, 64, (yy += lineHeight));
		}
	}
}