// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.IO;
using CivOne.Enums;
using CivOne.Events;

using TkKey = OpenTK.Input.Key;
using TkKeyArgs = OpenTK.Input.KeyboardKeyEventArgs;
using TkKeyPressArgs = OpenTK.KeyPressEventArgs;
using TkWinState = OpenTK.WindowState;

namespace CivOne
{
	internal partial class Window
	{
		private KeyModifier _keyModifier = KeyModifier.None;

		private KeyboardEventArgs ConvertKeyboardEvents(TkKeyArgs args)
		{
			switch (args.Key)
			{
				case TkKey.F1: return new KeyboardEventArgs(Key.F1, _keyModifier);
				case TkKey.F2: return new KeyboardEventArgs(Key.F2, _keyModifier);
				case TkKey.F3: return new KeyboardEventArgs(Key.F3, _keyModifier);
				case TkKey.F4: return new KeyboardEventArgs(Key.F4, _keyModifier);
				case TkKey.F5: return new KeyboardEventArgs(Key.F5, _keyModifier);
				case TkKey.F6: return new KeyboardEventArgs(Key.F6, _keyModifier);
				case TkKey.F7: return new KeyboardEventArgs(Key.F7, _keyModifier);
				case TkKey.F8: return new KeyboardEventArgs(Key.F8, _keyModifier);
				case TkKey.F9: return new KeyboardEventArgs(Key.F9, _keyModifier);
				case TkKey.F10: return new KeyboardEventArgs(Key.F10, _keyModifier);
				case TkKey.F11: return new KeyboardEventArgs(Key.F11, _keyModifier);
				case TkKey.F12: return new KeyboardEventArgs(Key.F12, _keyModifier);
				case TkKey.Keypad0: return new KeyboardEventArgs(Key.NumPad0, _keyModifier);
				case TkKey.Keypad1: return new KeyboardEventArgs(Key.NumPad1, _keyModifier);
				case TkKey.Keypad2: return new KeyboardEventArgs(Key.NumPad2, _keyModifier);
				case TkKey.Keypad3: return new KeyboardEventArgs(Key.NumPad3, _keyModifier);
				case TkKey.Keypad4: return new KeyboardEventArgs(Key.NumPad4, _keyModifier);
				case TkKey.Keypad5: return new KeyboardEventArgs(Key.NumPad5, _keyModifier);
				case TkKey.Keypad6: return new KeyboardEventArgs(Key.NumPad6, _keyModifier);
				case TkKey.Keypad7: return new KeyboardEventArgs(Key.NumPad7, _keyModifier);
				case TkKey.Keypad8: return new KeyboardEventArgs(Key.NumPad8, _keyModifier);
				case TkKey.Keypad9: return new KeyboardEventArgs(Key.NumPad9, _keyModifier);
				case TkKey.Up: return new KeyboardEventArgs(Key.Up, _keyModifier);
				case TkKey.Left: return new KeyboardEventArgs(Key.Left, _keyModifier);
				case TkKey.Right: return new KeyboardEventArgs(Key.Right, _keyModifier);
				case TkKey.Down: return new KeyboardEventArgs(Key.Down, _keyModifier);
				case TkKey.KeypadEnter:
				case TkKey.Enter: return new KeyboardEventArgs(Key.Enter, _keyModifier);
				case TkKey.Space: return new KeyboardEventArgs(Key.Space, _keyModifier);
				case TkKey.Escape: return new KeyboardEventArgs(Key.Escape, _keyModifier);
				case TkKey.Delete: return new KeyboardEventArgs(Key.Delete, _keyModifier);
				case TkKey.Back: return new KeyboardEventArgs(Key.Backspace, _keyModifier);
				case TkKey.Period: return new KeyboardEventArgs('.', _keyModifier);
				case TkKey.Comma: return new KeyboardEventArgs(',', _keyModifier);
				case TkKey.KeypadPlus:
				case TkKey.Plus: return new KeyboardEventArgs(Key.Plus, _keyModifier);
				case TkKey.KeypadMinus:
				case TkKey.Minus: return new KeyboardEventArgs(Key.Minus, _keyModifier);
				case TkKey.Number0: return new KeyboardEventArgs('0', _keyModifier);
				case TkKey.Number1: return new KeyboardEventArgs('1', _keyModifier);
				case TkKey.Number2: return new KeyboardEventArgs('2', _keyModifier);
				case TkKey.Number3: return new KeyboardEventArgs('3', _keyModifier);
				case TkKey.Number4: return new KeyboardEventArgs('4', _keyModifier);
				case TkKey.Number5: return new KeyboardEventArgs('5', _keyModifier);
				case TkKey.Number6: return new KeyboardEventArgs('6', _keyModifier);
				case TkKey.Number7: return new KeyboardEventArgs('7', _keyModifier);
				case TkKey.Number8: return new KeyboardEventArgs('8', _keyModifier);
				case TkKey.Number9: return new KeyboardEventArgs('9', _keyModifier);
				case TkKey.KeypadDivide:
				case TkKey.Slash: return new KeyboardEventArgs(Key.Slash, _keyModifier);
			}

			return null;
		}

		private void OnKeyDown(object sender, TkKeyArgs args)
		{
			_keyModifier = KeyModifier.None;
			if (args.Control) _keyModifier |= KeyModifier.Control;
			if (args.Shift) _keyModifier |= KeyModifier.Shift;
			if (args.Alt) _keyModifier |= KeyModifier.Alt;

			if (_keyModifier == KeyModifier.Alt && args.Key == TkKey.Enter)
			{
				if (WindowState == TkWinState.Fullscreen)
				{
					Console.WriteLine("Windowed mode");
					WindowState = _previousState;
					return;
				}
				Console.WriteLine("Fullscreen mode");
				_previousState = WindowState;
				WindowState = TkWinState.Fullscreen;
				return;
			}

			if (_keyModifier == KeyModifier.Control && args.Key == TkKey.F5)
			{
				string filename = Common.CaptureFilename;
				using (CivOne.GFX.ImageFormats.GifFile file = new CivOne.GFX.ImageFormats.GifFile(_canvas))
				using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
				{
					byte[] output = file.GetBytes();
					fs.Write(output, 0, output.Length);
					Console.WriteLine($"Screenshot saved: {filename}");
				}
				return;
			}

			if (TopScreen == null) return;
			KeyboardEventArgs KeyboardEventArgs = ConvertKeyboardEvents(args);

			if (KeyboardEventArgs == null) return;
			TopScreen.KeyDown(KeyboardEventArgs);
		}

		private void OnKeyUp(object sender, TkKeyArgs args)
		{
			_keyModifier = KeyModifier.None;
			if (args.Control) _keyModifier |= KeyModifier.Control;
			if (args.Shift) _keyModifier |= KeyModifier.Shift;
			if (args.Alt) _keyModifier |= KeyModifier.Alt;
		}

		protected override void OnKeyPress(TkKeyPressArgs args)
		{
			if (TopScreen == null) return;
			char keyChar = (char)args.KeyChar;
			if (char.IsLetter(keyChar))
			{
				TopScreen.KeyDown(new KeyboardEventArgs(char.ToUpper((char)args.KeyChar), _keyModifier));
			}
		}
	}
}