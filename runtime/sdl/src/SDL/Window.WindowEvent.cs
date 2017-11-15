// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;

namespace CivOne
{
	internal static partial class SDL
	{
		internal abstract partial class Window
		{
			protected event EventHandler OnLoad, OnDraw, OnUpdate;

			public event EventHandler OnClose;

			private void Close()
			{
				if (OnClose != null) OnClose(this, EventArgs.Empty);
				Dispose();
				_running = false;
			}

			private void HandleEventWindow(SDL_WindowEvent windowEvent)
			{
				switch(windowEvent.Event)
				{
					case SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
						Close();
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_TAKE_FOCUS:
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_HIT_TEST:
						break;
				}
			}
		}
	}
}