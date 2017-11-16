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
		[Flags]
		private enum SDL_INIT : uint
		{
			TIMER = 0x001,
			AUDIO = 0x002,
			VIDEO = 0x004,
			JOYSTICK = 0x008,
			HAPTIC = 0x010,
			GAMECONTROLLER = 0x020,
			EVENTS = 0x040,
			EVERYTHING = 0x080,
			NOPARACHUTE = 0x100
		}
	}
}