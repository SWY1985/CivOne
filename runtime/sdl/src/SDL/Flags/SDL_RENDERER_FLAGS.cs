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
		private enum SDL_RENDERER_FLAGS : uint
		{
			SDL_RENDERER_SOFTWARE = 0x1,
			SDL_RENDERER_ACCELERATED = 0x2,
			SDL_RENDERER_PRESENTVSYNC = 0x4,
			SDL_RENDERER_TARGETTEXTURE = 0x8
		}
	}
}