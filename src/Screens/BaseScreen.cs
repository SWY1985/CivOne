// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.IO;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	public abstract partial class BaseScreen : BaseInstance, IScreen
	{
		private readonly MouseCursor _cursor;
		
		private int CanvasWidth => RuntimeHandler.Instance.CanvasWidth;
		private int CanvasHeight => RuntimeHandler.Instance.CanvasHeight;
		private bool CanExpand => Common.HasAttribute<Expand>(this);
		private bool SizeChanged => (this.Width() != CanvasWidth || this.Height() != CanvasHeight);
		
		private bool _forceUpdate = false;

		protected readonly List<Element> Elements = new List<Element>();

		protected event ResizeEventHandler OnResize;
		protected event KeyboardEventHandler OnKeyDown, OnKeyUp;
		protected event ScreenEventHandler OnMouseDown, OnMouseUp, OnMouseDrag, OnMouseMove;

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

		private bool UpdateDraw(uint gameTick)
		{
			bool result = HasUpdate(gameTick);
			if (_forceUpdate) result = true;
			foreach (Element element in Elements)
			{
				if (element.Bitmap == null) continue;
				this.AddLayer(element.Bitmap, element.Left, element.Top);
			}
			return result;
		}

		public bool Update(uint gameTick)
		{
			if (CanExpand && SizeChanged)
			{
				Resize(Runtime.CanvasWidth, Runtime.CanvasHeight);
				_forceUpdate = true;
			}
			return UpdateDraw(gameTick);
		}
		
		public bool KeyUp(KeyboardEventArgs args)
		{
			OnKeyUp?.Invoke(this, args);
			return args.Handled;
		}

		public bool KeyDown(KeyboardEventArgs args)
		{
			OnKeyDown?.Invoke(this, args);
			return args.Handled;
		}

		public bool MouseDown(ScreenEventArgs args)
		{
			foreach (Element element in Elements)
			{
				int x = args.X - element.Left, y = args.Y - element.Top;
				switch (element)
				{
					case Button button:
						if (button.Bounds.Contains(args.Location))
						{
							_forceUpdate = true;
							button.Click(x, y);
							return (args.Handled = true);
						}
						break;
				}
			}

			OnMouseDown?.Invoke(this, args);
			return args.Handled;
		}

		public bool MouseUp(ScreenEventArgs args)
		{
			OnMouseUp?.Invoke(this, args);
			return args.Handled;
		}

		public bool MouseDrag(ScreenEventArgs args)
		{
			OnMouseDrag?.Invoke(this, args);
			return args.Handled;
		}

		public bool MouseMove(ScreenEventArgs args)
		{
			OnMouseMove?.Invoke(this, args);
			return args.Handled;
		}

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