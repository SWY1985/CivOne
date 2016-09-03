// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
//using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class TribalHut : BaseScreen
	{
		private readonly Picture _messageBox;
		
		private bool _update = true;
		private bool _redraw = false;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas.AddLayer(_messageBox.Image, 100, 80);
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

		public TribalHut(params string[] message)
		{
			Cursor = MouseCursor.Pointer;

			Bitmap background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
			
			Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			
			_canvas = new Picture(320, 200, palette);
			
			Bitmap[] textLines = new Bitmap[message.Length];
			for (int i = 0; i < message.Length; i++)
				textLines[i] = Resources.Instance.GetText(message[i], 0, 15);
			int width = textLines.Max(x => x.Width) + 10;
			int height = textLines.Sum(x => x.Height) + 9;

			int fillWidth = 4 - (width % 4); 
			width += fillWidth;
			int actualWidth = width - fillWidth;

			_messageBox = new Picture(width, height);
			_messageBox.FillLayerTile(background, 1, 1);
			if (fillWidth > 0)
				_messageBox.FillRectangle(0, actualWidth, 0, fillWidth, height);
			_messageBox.AddBorder(15, 8, 1, 1, actualWidth - 2, height - 2);
			_messageBox.AddBorder(5, 5, 0, 0, actualWidth, height);
			for (int i = 0; i < textLines.Length; i++)
				_messageBox.AddLayer(textLines[i], 5, (textLines[i].Height * i) + 5);
		}
	}
}