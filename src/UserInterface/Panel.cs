// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Events;
using CivOne.IO;

namespace CivOne.UserInterface
{
	public abstract class Panel : Element, IUpdateElement, IMouseElement
	{
		private bool _hasUpdate;

		protected event ScreenEventHandler OnMouseDown, OnMouseUp;
		protected event EventHandler OnDraw;

		private void Draw()
		{
			OnDraw?.Invoke(this, EventArgs.Empty);
			_hasUpdate = true;
		}

		protected virtual bool HasUpdate(uint gameTick)
		{
			if (_hasUpdate)
			{
				_hasUpdate = false;
				return true;
			}
			return false;
		}

		protected void Resize(int width, int height)
		{
			Bitmap?.Dispose();
			Bitmap = new Bytemap(width, height);
			Draw();
		}

		public bool MouseDown(int left, int top)
		{
			ScreenEventArgs args = new ScreenEventArgs(left - Left, top - Top);
			OnMouseDown?.Invoke(this, args);
			return args.Handled;
		}

		public bool MouseUp(int left, int top)
		{
			ScreenEventArgs args = new ScreenEventArgs(left - Left, top - Top);
			OnMouseUp?.Invoke(this, args);
			return args.Handled;
		}

		public bool Update(uint gameTick)
		{
			if (!HasUpdate(gameTick)) return false;
			Draw();
			return true;
		}

		protected Panel(int left, int top)
		{
			Left = left;
			Top = top;
		}
	}
}