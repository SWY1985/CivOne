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

namespace CivOne
{
	internal static partial class SDL
	{
		internal abstract partial class Window
		{
			protected event KeyboardEventHandler OnKeyDown, OnKeyUp;

			private Key ConvertKey(SDL_Scancode scanCode)
			{
				switch (scanCode)
				{
					case SDL_Scancode.SDL_SCANCODE_RETURN:
					case SDL_Scancode.SDL_SCANCODE_KP_ENTER:
						return Key.Enter;
					case SDL_Scancode.SDL_SCANCODE_UP:
						return Key.Up;
					case SDL_Scancode.SDL_SCANCODE_DOWN:
						return Key.Down;
					case SDL_Scancode.SDL_SCANCODE_LEFT:
						return Key.Left;
					case SDL_Scancode.SDL_SCANCODE_RIGHT:
						return Key.Right;
					case SDL_Scancode.SDL_SCANCODE_SPACE:
						return Key.Space;
				}
				return Key.None;
			}

			private void HandleEventKeyboard(SDL_KeyboardEvent keyboardEvent)
			{
				if (keyboardEvent.Repeat > 0) return;

				Key key = ConvertKey(keyboardEvent.KeySym.Scancode);
				if (key == Key.None) return;

				KeyboardEventArgs args = new KeyboardEventArgs(key, KeyModifier.None);

				switch (keyboardEvent.State)
				{
					case SDL_KeyState.SDL_PRESSED:
						OnKeyDown?.Invoke(this, args);
						return;
					case SDL_KeyState.SDL_RELEASED:
						OnKeyUp?.Invoke(this, args);
						return;
				}
			}
		}
	}
}