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
		private struct SDL_Keysym
		{
			public SDL_Scancode Scancode;
			public SDL_Keycode Keycode;
			public SDL_KMOD Modifier;
			public uint Unicode;
		}
	}
}