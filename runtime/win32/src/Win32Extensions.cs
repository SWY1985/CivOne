// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Windows.Forms;
using CivOne.Enums;
using CivOne.Events;

namespace CivOne
{
	internal static class Win32Extensions
	{
		internal static KeyboardEventArgs ConvertKeyboardEvents(this KeyEventArgs args)
		{
			KeyModifier modifier = KeyModifier.None;
			if (args.Control) modifier |= KeyModifier.Control;
			if (args.Shift) modifier |= KeyModifier.Shift;
			if (args.Alt) modifier |= KeyModifier.Alt;

			switch (args.KeyCode)
			{
				case Keys.F1: return new KeyboardEventArgs(Key.F1, modifier);
				case Keys.F2: return new KeyboardEventArgs(Key.F2, modifier);
				case Keys.F3: return new KeyboardEventArgs(Key.F3, modifier);
				case Keys.F4: return new KeyboardEventArgs(Key.F4, modifier);
				case Keys.F5: return new KeyboardEventArgs(Key.F5, modifier);
				case Keys.F6: return new KeyboardEventArgs(Key.F6, modifier);
				case Keys.F7: return new KeyboardEventArgs(Key.F7, modifier);
				case Keys.F8: return new KeyboardEventArgs(Key.F8, modifier);
				case Keys.F9: return new KeyboardEventArgs(Key.F9, modifier);
				case Keys.F10: return new KeyboardEventArgs(Key.F10, modifier);
				case Keys.F11: return new KeyboardEventArgs(Key.F11, modifier);
				case Keys.F12: return new KeyboardEventArgs(Key.F12, modifier);
				case Keys.NumPad0: return new KeyboardEventArgs(Key.NumPad0, modifier);
				case Keys.NumPad1: return new KeyboardEventArgs(Key.NumPad1, modifier);
				case Keys.NumPad2: return new KeyboardEventArgs(Key.NumPad2, modifier);
				case Keys.NumPad3: return new KeyboardEventArgs(Key.NumPad3, modifier);
				case Keys.NumPad4: return new KeyboardEventArgs(Key.NumPad4, modifier);
				case Keys.NumPad5: return new KeyboardEventArgs(Key.NumPad5, modifier);
				case Keys.NumPad6: return new KeyboardEventArgs(Key.NumPad6, modifier);
				case Keys.NumPad7: return new KeyboardEventArgs(Key.NumPad7, modifier);
				case Keys.NumPad8: return new KeyboardEventArgs(Key.NumPad8, modifier);
				case Keys.NumPad9: return new KeyboardEventArgs(Key.NumPad9, modifier);
				case Keys.Up: return new KeyboardEventArgs(Key.Up, modifier);
				case Keys.Left: return new KeyboardEventArgs(Key.Left, modifier);
				case Keys.Right: return new KeyboardEventArgs(Key.Right, modifier);
				case Keys.Down: return new KeyboardEventArgs(Key.Down, modifier);
				case Keys.Enter: return new KeyboardEventArgs(Key.Enter, modifier);
				case Keys.Space: return new KeyboardEventArgs(Key.Space, modifier);
				case Keys.Escape: return new KeyboardEventArgs(Key.Escape, modifier);
				case Keys.Delete: return new KeyboardEventArgs(Key.Delete, modifier);
				case Keys.Back: return new KeyboardEventArgs(Key.Backspace, modifier);
				case Keys.OemPeriod: return new KeyboardEventArgs('.', modifier);
				case Keys.Oemcomma: return new KeyboardEventArgs(',', modifier);
				case Keys.Add:
				case Keys.Oemplus: return new KeyboardEventArgs(Key.Plus, modifier);
				case Keys.Subtract:
				case Keys.OemMinus: return new KeyboardEventArgs(Key.Minus, modifier);
				default: return new KeyboardEventArgs(char.ToUpper((char)args.KeyCode), modifier);
			}
		}
	}
}