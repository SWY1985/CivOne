// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using static CivOne.SDL.SDL_KeyState;

namespace CivOne
{
	internal static partial class SDL
	{
		internal abstract partial class Window
		{
			protected class SdlKeyEventArgs : EventArgs
			{
				internal SDL_Keycode KeyCode { get; private set; }
				internal SDL_Scancode ScanCode { get; private set; }
				internal char Character { get; private set; }
				internal bool Repeat { get; private set; }

				internal SdlKeyEventArgs(SDL_KeyboardEvent keyboardEvent)
				{
					KeyCode = keyboardEvent.KeySym.Keycode;
					ScanCode = keyboardEvent.KeySym.Scancode;
					Character = (char)keyboardEvent.KeySym.Keycode;
					Repeat = (keyboardEvent.Repeat > 0);
				}
			}

			protected event EventHandler<SdlKeyEventArgs> OnKeyDown, OnKeyUp;

			private void HandleEventKeyboard(SDL_KeyboardEvent keyboardEvent)
			{
				switch (keyboardEvent.State)
				{
					case SDL_PRESSED:
						OnKeyDown?.Invoke(this, new SdlKeyEventArgs(keyboardEvent));
						break;
					case SDL_RELEASED:
						OnKeyUp?.Invoke(this, new SdlKeyEventArgs(keyboardEvent));
						break;
				}
			}
		}
	}
}