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
		private enum SDL_WINDOW : uint
		{
			FULLSCREEN = 0x001,
			FULLSCREEN_DESKTOP = 0x002,
			OPENGL = 0x004,
			HIDDEN = 0x008,
			BORDERLESS = 0x010,
			RESIZABLE = 0x020,
			MINIMIZED = 0x040,
			MAXIMIZED = 0x080,
			INPUT_GRABBED = 0x100,
			ALLOW_HIGHDPI = 0x200
		}
	}
}