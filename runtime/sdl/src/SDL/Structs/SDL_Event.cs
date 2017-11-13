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
		private struct SDL_Event
		{
			public uint SDL_CommonEvent;
			public SDL_WindowEvent SDL_WindowEvent;
			uint SDL_KeyboardEvent;
			uint SDL_TextEditingEvent;
			uint SDL_TextInputEvent;
			uint SDL_MouseMotionEvent;
			uint SDL_MouseButtonEvent;
			uint SDL_MouseWheelEvent;
			uint SDL_JoyAxisEvent;
			uint SDL_JoyBallEvent;
			uint SDL_JoyHatEvent;
			uint SDL_JoyButtonEvent;
			uint SDL_JoyDeviceEvent;
			uint SDL_ControllerAxisEvent;
			uint SDL_ControllerButtonEvent;
			uint SDL_ControllerDeviceEvent;
			uint SDL_AudioDeviceEvent;
			uint SDL_QuitEvent;
			uint SDL_UserEvent;
			uint SDL_SysWMEvent;
			uint SDL_TouchFingerEvent;
			uint SDL_MultiGestureEvent;
			uint SDL_DollarGestureEvent;
			uint SDL_DropEvent;
		}
	}
}