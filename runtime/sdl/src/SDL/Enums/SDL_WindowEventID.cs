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
		private enum SDL_WindowEventID : byte
		{
			SDL_WINDOWEVENT_NONE,
			SDL_WINDOWEVENT_SHOWN,
			SDL_WINDOWEVENT_HIDDEN,
			SDL_WINDOWEVENT_EXPOSED,
			SDL_WINDOWEVENT_MOVED,
			SDL_WINDOWEVENT_RESIZED,
			SDL_WINDOWEVENT_SIZE_CHANGED,
			SDL_WINDOWEVENT_MINIMIZED,
			SDL_WINDOWEVENT_MAXIMIZED,
			SDL_WINDOWEVENT_RESTORED,
			SDL_WINDOWEVENT_ENTER,
			SDL_WINDOWEVENT_LEAVE,
			SDL_WINDOWEVENT_FOCUS_GAINED,
			SDL_WINDOWEVENT_FOCUS_LOST,
			SDL_WINDOWEVENT_CLOSE,
			SDL_WINDOWEVENT_TAKE_FOCUS,
			SDL_WINDOWEVENT_HIT_TEST
		}
	}
}