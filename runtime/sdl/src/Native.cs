// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CivOne.Enums;

namespace CivOne
{
	internal partial class Native
	{
		private static IntPtr _handle = IntPtr.Zero;

		internal static Platform Platform =>
			RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Platform.Windows :
			RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Platform.Linux :
			RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? Platform.macOS :
			Platform.Unknown;

		internal static string FolderBrowser(string caption = "")
		{
			switch (Platform)
			{
				case Platform.Windows:
					return Win32FolderBrowser(caption);
				case Platform.Linux:
					return GtkFolderBrowser(caption);
				case Platform.macOS:
					return MacFolderBrowser(caption);
				default:
					return null;
			}
		}

		internal static void ShowCursor()
		{
			switch (Platform)
			{
				case Platform.Windows:
					ShowCursor(true);
					break;
			}
		}

		internal static void HideCursor()
		{
			switch (Platform)
			{
				case Platform.Windows:
					ShowCursor(false);
					break;
			}
		}

		internal static bool CreateDesktopIcon(string name, string description, params string[] arguments)
		{
			switch (Platform)
			{
				case Platform.Windows:
#if DEBUG
					string path = "dotnet";
					arguments = new [] { $@"""{Path.Combine(Resources.BinPath, "CivOne.SDL.dll")}""" }.Union(arguments).ToArray();
#else
					if (!File.Exists(Path.Combine(Resources.BinPath, "CivOne.SDL.exe"))) return false;
					string path = Path.Combine(Resources.BinPath, "CivOne.SDL.exe");
#endif

					return Win32CreateShortcut(name, description, path, arguments, Environment.CurrentDirectory, Path.Combine(Resources.BinPath, "CivOne.ico"));
				default:
					return false;
			}
		}

		internal static void Init(IntPtr handle)
		{
			_handle = handle;

			switch (Platform)
			{
				case Platform.Windows:
					break;
				case Platform.Linux:
					// Init GTK
					IntPtr argv = new IntPtr(0);
					int argc = 0;
					gtk_init(ref argc, ref argv);
					break;
			}
		}
	}
}