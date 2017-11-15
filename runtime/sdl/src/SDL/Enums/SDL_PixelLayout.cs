// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne
{
	internal static partial class SDL
	{
		private enum SDL_PixelLayout : byte
		{
			SDL_PACKEDLAYOUT_NONE,
			SDL_PACKEDLAYOUT_332,
			SDL_PACKEDLAYOUT_4444,
			SDL_PACKEDLAYOUT_1555,
			SDL_PACKEDLAYOUT_5551,
			SDL_PACKEDLAYOUT_565,
			SDL_PACKEDLAYOUT_8888,
			SDL_PACKEDLAYOUT_2101010,
			SDL_PACKEDLAYOUT_1010102
		}
	}
}