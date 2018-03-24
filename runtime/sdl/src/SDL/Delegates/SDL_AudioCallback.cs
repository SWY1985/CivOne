// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Runtime.InteropServices;

namespace CivOne
{
	internal static partial class SDL
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SDL_AudioCallback(IntPtr userData, IntPtr stream, int length);
	}
}