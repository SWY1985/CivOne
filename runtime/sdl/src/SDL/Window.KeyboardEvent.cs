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

			private KeyModifier ConvertModifier(SDL_KMOD kmod)
			{
				KeyModifier output = KeyModifier.None;
				if ((kmod & SDL_KMOD.KMOD_CTRL) > 0) output |= KeyModifier.Control;
				if ((kmod & SDL_KMOD.KMOD_ALT) > 0) output |= KeyModifier.Alt;
				if ((kmod & SDL_KMOD.KMOD_SHIFT) > 0) output |= KeyModifier.Shift;
				return output;
			}

			private Key ConvertKey(SDL_Scancode scanCode)
			{
				switch (scanCode)
				{
					case SDL_Scancode.SDL_SCANCODE_F1:
						return Key.F1;
					case SDL_Scancode.SDL_SCANCODE_F2:
						return Key.F2;
					case SDL_Scancode.SDL_SCANCODE_F3:
						return Key.F3;
					case SDL_Scancode.SDL_SCANCODE_F4:
						return Key.F4;
					case SDL_Scancode.SDL_SCANCODE_F5:
						return Key.F5;
					case SDL_Scancode.SDL_SCANCODE_F6:
						return Key.F6;
					case SDL_Scancode.SDL_SCANCODE_F7:
						return Key.F7;
					case SDL_Scancode.SDL_SCANCODE_F8:
						return Key.F8;
					case SDL_Scancode.SDL_SCANCODE_F9:
						return Key.F9;
					case SDL_Scancode.SDL_SCANCODE_F10:
						return Key.F10;
					case SDL_Scancode.SDL_SCANCODE_F11:
						return Key.F11;
					case SDL_Scancode.SDL_SCANCODE_F12:
						return Key.F12;
					case SDL_Scancode.SDL_SCANCODE_RETURN:
					case SDL_Scancode.SDL_SCANCODE_KP_ENTER:
						return Key.Enter;
					case SDL_Scancode.SDL_SCANCODE_ESCAPE:
						return Key.Escape;
					case SDL_Scancode.SDL_SCANCODE_BACKSPACE:
						return Key.Backspace;
					case SDL_Scancode.SDL_SCANCODE_DELETE:
						return Key.Delete;
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
					case SDL_Scancode.SDL_SCANCODE_KP_MINUS:
						return Key.Minus;
					case SDL_Scancode.SDL_SCANCODE_KP_PLUS:
						return Key.Plus;
					case SDL_Scancode.SDL_SCANCODE_KP_0:
						return Key.NumPad0;
					case SDL_Scancode.SDL_SCANCODE_KP_1:
						return Key.NumPad1;
					case SDL_Scancode.SDL_SCANCODE_KP_2:
						return Key.NumPad2;
					case SDL_Scancode.SDL_SCANCODE_KP_3:
						return Key.NumPad3;
					case SDL_Scancode.SDL_SCANCODE_KP_4:
						return Key.NumPad4;
					case SDL_Scancode.SDL_SCANCODE_KP_5:
						return Key.NumPad5;
					case SDL_Scancode.SDL_SCANCODE_KP_6:
						return Key.NumPad6;
					case SDL_Scancode.SDL_SCANCODE_KP_7:
						return Key.NumPad7;
					case SDL_Scancode.SDL_SCANCODE_KP_8:
						return Key.NumPad8;
					case SDL_Scancode.SDL_SCANCODE_KP_9:
						return Key.NumPad9;
				}
				return Key.None;
			}

			private KeyboardEventArgs ConvertKeyEvent(SDL_KeyboardEvent keyboardEvent)
			{
				KeyModifier modifier = ConvertModifier(keyboardEvent.KeySym.Modifier);
				Key key = ConvertKey(keyboardEvent.KeySym.Scancode);
				if (key != Key.None)
				{
					return new KeyboardEventArgs(key, modifier);
				}

				char keyChar = (char)keyboardEvent.KeySym.Keycode;
				if (keyChar != '.' && keyChar != ',' && (char.ToLower(keyChar) < 'a' || (int)char.ToLower(keyChar) > 'z') && (keyChar < '0' || keyChar > '9')) return null;
				return new KeyboardEventArgs(char.ToUpper(keyChar), modifier);
			}

			private void HandleEventKeyboard(SDL_KeyboardEvent keyboardEvent)
			{
				KeyboardEventArgs args = ConvertKeyEvent(keyboardEvent);
				if (args == null) return;

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