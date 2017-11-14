// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Runtime.InteropServices;

namespace CivOne
{
	internal static partial class SDL
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct SDL_Event
		{
			[FieldOffset(0)]
			public SDL_EventType SDL_EventType;
			[FieldOffset(0)]
			public SDL_WindowEvent SDL_WindowEvent;
			[FieldOffset(0)]
			public SDL_KeyboardEvent SDL_KeyboardEvent;
			// uint SDL_TextEditingEvent;
			// uint SDL_TextInputEvent;
			// uint SDL_MouseMotionEvent;
			// uint SDL_MouseButtonEvent;
			// uint SDL_MouseWheelEvent;
			// uint SDL_JoyAxisEvent;
			// uint SDL_JoyBallEvent;
			// uint SDL_JoyHatEvent;
			// uint SDL_JoyButtonEvent;
			// uint SDL_JoyDeviceEvent;
			// uint SDL_ControllerAxisEvent;
			// uint SDL_ControllerButtonEvent;
			// uint SDL_ControllerDeviceEvent;
			// uint SDL_AudioDeviceEvent;
			// uint SDL_QuitEvent;
			// uint SDL_UserEvent;
			// uint SDL_SysWMEvent;
			// uint SDL_TouchFingerEvent;
			// uint SDL_MultiGestureEvent;
			// uint SDL_DollarGestureEvent;
			// uint SDL_DropEvent;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SDL_WindowEvent
		{
			public SDL_EventType SDL_EventType;
			public uint Timestamp;
			public uint WindowId;
			public SDL_WindowEventID Event;
			byte nil1, nil2, nil3;
			public int Data1;
			public int Data2;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SDL_KeyboardEvent
		{
			public SDL_EventType Type;
			public uint Timestamp;
			public uint WindowId;
			//public SDL_KeyState State;
			public SDL_KeyState State;
			//public SDL_KeyState State => (SDL_KeyState)_state;
			public byte Repeat;
			private byte nil1, nil2;
			internal SDL_Keysym KeySym;
		}
	}
}