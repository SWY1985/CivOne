// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Events;

using TkKey = OpenTK.Input.Key;
using TkKeyArgs = OpenTK.Input.KeyboardKeyEventArgs;
using TkKeyModifier = OpenTK.Input.KeyModifiers;
using TkKeyPressArgs = OpenTK.KeyPressEventArgs;
using TkWinState = OpenTK.WindowState;

namespace CivOne
{
	internal static class Extensions
	{
		internal static KeyboardEventArgs ConvertKeyboardEvents(this TkKeyArgs args)
		{
			KeyModifier modifier = KeyModifier.None;
			if ((args.Modifiers & TkKeyModifier.Control) > 0) modifier |= KeyModifier.Control;
			if ((args.Modifiers & TkKeyModifier.Shift) > 0) modifier |= KeyModifier.Shift;
			if ((args.Modifiers & TkKeyModifier.Alt) > 0) modifier |= KeyModifier.Alt;

			switch (args.Key)
			{
				case TkKey.F1: return new KeyboardEventArgs(Key.F1, modifier);
				case TkKey.F2: return new KeyboardEventArgs(Key.F2, modifier);
				case TkKey.F3: return new KeyboardEventArgs(Key.F3, modifier);
				case TkKey.F4: return new KeyboardEventArgs(Key.F4, modifier);
				case TkKey.F5: return new KeyboardEventArgs(Key.F5, modifier);
				case TkKey.F6: return new KeyboardEventArgs(Key.F6, modifier);
				case TkKey.F7: return new KeyboardEventArgs(Key.F7, modifier);
				case TkKey.F8: return new KeyboardEventArgs(Key.F8, modifier);
				case TkKey.F9: return new KeyboardEventArgs(Key.F9, modifier);
				case TkKey.F10: return new KeyboardEventArgs(Key.F10, modifier);
				case TkKey.F11: return new KeyboardEventArgs(Key.F11, modifier);
				case TkKey.F12: return new KeyboardEventArgs(Key.F12, modifier);
				case TkKey.Keypad0: return new KeyboardEventArgs(Key.NumPad0, modifier);
				case TkKey.Keypad1: return new KeyboardEventArgs(Key.NumPad1, modifier);
				case TkKey.Keypad2: return new KeyboardEventArgs(Key.NumPad2, modifier);
				case TkKey.Keypad3: return new KeyboardEventArgs(Key.NumPad3, modifier);
				case TkKey.Keypad4: return new KeyboardEventArgs(Key.NumPad4, modifier);
				case TkKey.Keypad5: return new KeyboardEventArgs(Key.NumPad5, modifier);
				case TkKey.Keypad6: return new KeyboardEventArgs(Key.NumPad6, modifier);
				case TkKey.Keypad7: return new KeyboardEventArgs(Key.NumPad7, modifier);
				case TkKey.Keypad8: return new KeyboardEventArgs(Key.NumPad8, modifier);
				case TkKey.Keypad9: return new KeyboardEventArgs(Key.NumPad9, modifier);
				case TkKey.Up: return new KeyboardEventArgs(Key.Up, modifier);
				case TkKey.Left: return new KeyboardEventArgs(Key.Left, modifier);
				case TkKey.Right: return new KeyboardEventArgs(Key.Right, modifier);
				case TkKey.Down: return new KeyboardEventArgs(Key.Down, modifier);
				case TkKey.Unknown: // Somehow, the keypad enter key registers as Unknown on some systems
				case TkKey.KeypadEnter:
				case TkKey.Enter: return new KeyboardEventArgs(Key.Enter, modifier);
				case TkKey.Space: return new KeyboardEventArgs(Key.Space, modifier);
				case TkKey.Escape: return new KeyboardEventArgs(Key.Escape, modifier);
				case TkKey.Delete: return new KeyboardEventArgs(Key.Delete, modifier);
				case TkKey.Home: return new KeyboardEventArgs(Key.Home, modifier);
				case TkKey.End: return new KeyboardEventArgs(Key.End, modifier);
				case TkKey.Back: return new KeyboardEventArgs(Key.Backspace, modifier);
				case TkKey.KeypadPlus:
				case TkKey.Plus: return new KeyboardEventArgs(Key.Plus, modifier);
				case TkKey.KeypadMinus:
				case TkKey.Minus: return new KeyboardEventArgs(Key.Minus, modifier);
				case TkKey.Number0: return new KeyboardEventArgs('0', modifier);
				case TkKey.Number1: return new KeyboardEventArgs('1', modifier);
				case TkKey.Number2: return new KeyboardEventArgs('2', modifier);
				case TkKey.Number3: return new KeyboardEventArgs('3', modifier);
				case TkKey.Number4: return new KeyboardEventArgs('4', modifier);
				case TkKey.Number5: return new KeyboardEventArgs('5', modifier);
				case TkKey.Number6: return new KeyboardEventArgs('6', modifier);
				case TkKey.Number7: return new KeyboardEventArgs('7', modifier);
				case TkKey.Number8: return new KeyboardEventArgs('8', modifier);
				case TkKey.Number9: return new KeyboardEventArgs('9', modifier);
				case TkKey.KeypadDivide:
				case TkKey.Slash: return new KeyboardEventArgs(Key.Slash, modifier);
			}

			return new KeyboardEventArgs(Key.None, modifier);;
		}
	}
}