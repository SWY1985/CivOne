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
		[StructLayout(LayoutKind.Sequential)]
		private unsafe struct SDL_Event
		{
			public SDL_EventType SDL_EventType;
			private fixed byte _nil[49 - sizeof(SDL_EventType)];
		}

		[StructLayout(LayoutKind.Sequential)]
		private unsafe struct SDL_WindowEvent
		{
			public SDL_EventType SDL_EventType;
			public uint Timestamp;
			public uint WindowId;
			public SDL_WindowEventID Event;
			private fixed byte _nil[3];
			public int Data1;
			public int Data2;
		}

		[StructLayout(LayoutKind.Sequential)]
		private unsafe struct SDL_KeyboardEvent
		{
			public SDL_EventType Type;
			public uint Timestamp;
			public uint WindowId;
			public SDL_KeyState State;
			public byte Repeat;
			private fixed byte _nil[2];
			internal SDL_Keysym KeySym;
		}
	}
}