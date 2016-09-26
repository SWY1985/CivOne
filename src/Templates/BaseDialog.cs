// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseDialog : BaseScreen
	{
		private readonly int _left, _top;

		private bool _update = true;
		
		protected Picture DialogBox { get; private set; }
		
		protected Bitmap Selection(int left, int top, int width, int height)
		{
			Bitmap background = (Bitmap)DialogBox.GetPart(left, top, width, height).Clone();
			Picture.ReplaceColours(background, new byte[] { 7, 22 }, new byte[] { 11, 3 });
			return background;
		}

		protected virtual void Cancel(object sender = null, EventArgs args = null)
		{
			HandleClose();
			Destroy();
		}

		protected virtual void FirstUpdate()
		{
			// Override this function to add menus and/or expand the dialog
		} 

		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;
				_canvas.AddLayer(DialogBox.Image, _left, _top);

				FirstUpdate();

				return true;
			}
			return false;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			Cancel();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			Cancel();
			return true;
		}
		
		public BaseDialog(int left, int top, int width, int height)
		{
			_left = left;
			_top = top;

			Cursor = MouseCursor.Pointer;

			Bitmap background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
			
			_canvas = new Picture(320, 200, Common.TopScreen.Palette);

			// We expand the size to add space for the black border
			left -= 1;
			top -= 1;
			width += 2;
			height += 2;

			int actualWidth = width;
			if ((actualWidth % 4) > 0)
				actualWidth += (4 - (width % 4));
			
			DialogBox = new Picture(actualWidth, height);
			DialogBox.FillLayerTile(background, 1, 1);
			if (actualWidth > width)
				DialogBox.FillRectangle(0, width, 0, (actualWidth - width), height);
			DialogBox.AddBorder(15, 8, 1, 1, width - 2, height - 2);
			DialogBox.AddBorder(5, 5, 0, 0, width, height);
		}
	}
}