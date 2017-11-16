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
		private enum SDL_BlendMode : int
		{
			SDL_BLENDMODE_NONE =	0x00000000,
			SDL_BLENDMODE_BLEND =	0x00000001,
			SDL_BLENDMODE_ADD =	0x00000002,
			SDL_BLENDMODE_MOD =	0x00000004,
			SDL_BLENDMODE_INVALID = 0x7FFFFFFF
		}
	}
}