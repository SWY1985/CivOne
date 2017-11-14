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
		private enum SDL_KMOD : ushort
		{
			KMOD_NONE = 0x0000,
			KMOD_LSHIFT = 0x0001,
			KMOD_RSHIFT = 0x0002,
			KMOD_LCTRL = 0x0040,
			KMOD_RCTRL = 0x0080,
			KMOD_LALT = 0x0100,
			KMOD_RALT = 0x0200,
			KMOD_LGUI = 0x0400,
			KMOD_RGUI = 0x0800,
			KMOD_NUM = 0x1000,
			KMOD_CAPS = 0x2000,
			KMOD_MODE = 0x4000,
			KMOD_RESERVED = 0x8000,
			
			KMOD_CTRL = (KMOD_LCTRL | KMOD_RCTRL),
			KMOD_SHIFT = (KMOD_LSHIFT | KMOD_RSHIFT),
			KMOD_ALT = (KMOD_LALT | KMOD_RALT),
			KMOD_GUI = (KMOD_LGUI | KMOD_RGUI)
		}
	}
}