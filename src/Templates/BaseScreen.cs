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
using CivOne.Interfaces;

namespace CivOne.Templates
{
	public abstract partial class BaseScreen : BaseInstance, IScreen
	{
		
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
		public virtual MouseCursor Cursor { get; protected set; }
		public abstract bool HasUpdate(uint gameTick);
		public virtual bool KeyDown(KeyboardEventArgs args)
		{
			return false;
		}
		public virtual bool MouseDown(ScreenEventArgs args)
		{
			return false;
		}
		public virtual bool MouseUp(ScreenEventArgs args)
		{
			return false;
		}
		public virtual bool MouseDrag(ScreenEventArgs args)
		{
			return false;
		}
		public virtual bool MouseMove(ScreenEventArgs args)
		{
			return false;
		}

		protected void Destroy()
		{
			CloseMenus();
			HandleClose();
			Common.DestroyScreen(this);
		}
	}
}