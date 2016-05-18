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

namespace CivOne.Events
{
	public class KeyboardEventArgs : EventArgs
	{
		public Key Key { get; private set; }
		public char KeyChar { get; private set; }
		public KeyModifier Modifier { get; private set; }
		
		public bool Control
		{
			get
			{
				return (Modifier & KeyModifier.Control) > 0;
			}
		}
		
		public bool Alt
		{
			get
			{
				return (Modifier & KeyModifier.Alt) > 0;
			}
		}
		
		public bool Shift
		{
			get
			{
				return (Modifier & KeyModifier.Shift) > 0;
			}
		}
		
		public KeyboardEventArgs(Key key, KeyModifier modifier = KeyModifier.None)
		{
			Key = key;
			KeyChar = (char)0x00;
			Modifier = modifier;
		}
		
		public KeyboardEventArgs(char keyChar, KeyModifier modifier = KeyModifier.None)
		{
			Key = Key.Character;
			KeyChar = keyChar;
			Modifier = modifier;
		}
	}
}