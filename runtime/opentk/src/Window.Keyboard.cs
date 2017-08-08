// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Events;
using OpenTK;
using OpenTK.Input;

using CKey = CivOne.Enums.Key;
using CMod = CivOne.Enums.KeyModifier;

namespace CivOne
{
	internal partial class Window
	{
		private CMod _keyModifier = CMod.None;

		private void InitializeKeyboard()
		{
			// Bind events
			KeyUp += KeyboardKeyUp;
			KeyDown += KeyboardKeyDown;
			KeyPress += KeyboardKeyPress;
		}

		private void KeyboardKeyUp(object sender, KeyboardKeyEventArgs args)
		{
			KeyboardEventArgs kbArgs = args.ConvertKeyboardEvents();
			_keyModifier = kbArgs.Modifier;
			if (kbArgs.None) return;
			_runtime.InvokeKeyboardUp(kbArgs);
		}

		private void KeyboardKeyDown(object sender, KeyboardKeyEventArgs args)
		{
			KeyboardEventArgs kbArgs = args.ConvertKeyboardEvents();
			_keyModifier = kbArgs.Modifier;
			if (kbArgs.None) return;

			if (kbArgs[CMod.Alt, CKey.Enter])
			{
				if (!args.IsRepeat)
					ToggleFullscreen();
				return;
			}

			_runtime.InvokeKeyboardDown(kbArgs);
		}

		private void KeyboardKeyPress(object sender, KeyPressEventArgs args)
		{
			char keyChar = (char)args.KeyChar;
			if (keyChar != '.' && keyChar != ',' && !char.IsLetter(keyChar)) return;
			_runtime.InvokeKeyboardDown(new KeyboardEventArgs(char.ToUpper(keyChar), _keyModifier));
		}
	}
}