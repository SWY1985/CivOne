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

namespace CivOne.Screens.Dialogs
{
	internal abstract class BaseDialog : BaseScreen
	{
		private readonly int _left, _top;

		private bool _update = true;
		
		protected Picture DialogBox { get; private set; }
		
		protected Picture[] TextLines { get; private set; }

		protected int TextWidth
		{
			get
			{
				return TextLines.Max(x => x.Width);
			}
		}

		protected int TextHeight
		{
			get
			{
				return TextLines.Sum(x => x.Height);
			}
		}
		
		protected IBitmap Selection(int left, int top, int width, int height)
		{
			return DialogBox[left, top, width, height].ColourReplace((7, 11), (22, 3));
		}

		protected virtual void Cancel(object sender = null, EventArgs args = null)
		{
			Destroy();
		}

		protected virtual void FirstUpdate()
		{
			// Override this function to add menus and/or expand the dialog
		} 

		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;
				this.AddLayer(DialogBox, _left, _top);

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

		private void Initialize(int left, int top, int width, int height)
		{
			Palette = Common.DefaultPalette;

			// We expand the size to add space for the black border
			left -= 1;
			top -= 1;
			width += 2;
			height += 2;
			
			DialogBox = new Picture(width, height)
				.Tile(Patterns.PanelGrey, 1, 1)
				.DrawRectangle3D(1, 1, width - 2, height - 2)
				.DrawRectangle()
				.As<Picture>();
		}

		public BaseDialog(int left, int top, int marginWidth, int marginHeight, string[] message) : base(MouseCursor.Pointer)
		{
			_left = left;
			_top = top;
			TextLines = new Picture[message.Length];
			for (int i = 0; i < message.Length; i++)
				TextLines[i] = Resources.GetText(message[i], 0, 15);

			Initialize(left, top, TextWidth + marginWidth, TextHeight + marginHeight);
		}
		
		public BaseDialog(int left, int top, int width, int height) : base(MouseCursor.Pointer)
		{
			_left = left;
			_top = top;

			Initialize(left, top, width, height);
		}
	}
}