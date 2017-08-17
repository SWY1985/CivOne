// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.IO;

namespace CivOne.Screens
{
	public abstract partial class BaseScreen : BaseInstance, IScreen
	{
		private readonly MouseCursor _cursor;
		
		private int CanvasWidth => RuntimeHandler.Instance.CanvasWidth;
		private int CanvasHeight => RuntimeHandler.Instance.CanvasHeight;
		private bool CanExpand => Common.HasAttribute<Expand>(this);
		private bool SizeChanged => (this.Width() != CanvasWidth || this.Height() != CanvasHeight);

		protected event ResizeEventHandler OnResize;

		protected void MouseArgsOffset(ref ScreenEventArgs args, int offsetX, int offsetY)
		{
			args = new ScreenEventArgs(args.X - offsetX, args.Y - offsetY, args.Buttons);
		}

		public event EventHandler Closed;

		protected void HandleClose()
		{
			if (Closed == null)
				return;
			Closed(this, null);
		}

		protected abstract bool HasUpdate(uint gameTick);

		private void Resize(int width, int height)
		{
			Bitmap = new Bytemap(width, height);
			OnResize?.Invoke(this, new ResizeEventArgs(width, height));
		}

		public virtual MouseCursor Cursor => _cursor;

		public bool Update(uint gameTick)
		{
			if (CanExpand && SizeChanged)
			{
				Resize(Runtime.CanvasWidth, Runtime.CanvasHeight);
				HasUpdate(gameTick);
				return true;
			}
			return HasUpdate(gameTick);
		}
		public virtual bool KeyDown(KeyboardEventArgs args) => false;
		public virtual bool MouseDown(ScreenEventArgs args) => false;
		public virtual bool MouseUp(ScreenEventArgs args) => false;
		public virtual bool MouseDrag(ScreenEventArgs args) => false;
		public virtual bool MouseMove(ScreenEventArgs args) => false;

		protected void Destroy()
		{
			CloseMenus();
			HandleClose();
			Common.DestroyScreen(this);
		}

		protected BaseScreen(MouseCursor cursor = MouseCursor.None)
		{
			_cursor = cursor;
			if (CanExpand)
			{
				Bitmap = new Bytemap(CanvasWidth, CanvasHeight);
			}
			else
			{
				Bitmap = new Bytemap(320, 200);
			}
		}

		protected BaseScreen(int width, int height, MouseCursor cursor = MouseCursor.None)
		{
			_cursor = cursor;
			Bitmap = new Bytemap(width, height);
		}
	}
}