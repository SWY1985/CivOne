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
using System.Runtime.InteropServices;
using CivOne.Enums;

namespace CivOne
{
	internal partial class Native
	{
		private static IntPtr _handle = IntPtr.Zero;

		internal static Platform Platform
		{
			get
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return Platform.Windows;
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return Platform.Linux;
				if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return Platform.macOS;
				return Platform.Unknown;
			}
		}

		internal static string StorageFolder
		{
			get
			{
				return Directory.GetCurrentDirectory();
			}
		}

		internal static string FolderBrowser(string caption = "")
		{
			switch (Platform)
			{
				case Platform.Windows:
					IntPtr bufferAddress = Marshal.AllocHGlobal(256);
					IntPtr pidl;
					BROWSEINFO browseInfo = new BROWSEINFO()
					{
						hwndOwner = IntPtr.Zero,
						pidlRoot = IntPtr.Zero,
						lpszTitle = caption,
						ulFlags = 0x310,
						lParam = IntPtr.Zero,
						iImage = 0
					};
					pidl = SHBrowseForFolder(ref browseInfo);
					if (!SHGetPathFromIDList(pidl, bufferAddress))
					{
						// User pressed cancel
						return null;
					}
					return Marshal.PtrToStringUni(bufferAddress);
				case Platform.Linux:
					IntPtr title = StringToIntPtr(caption);
					IntPtr test = gtk_file_chooser_dialog_new(title, IntPtr.Zero, 2, IntPtr.Zero);
					g_free(title);

					AddButton(test, "Cancel", -6);
					AddButton(test, "OK", -5);

					string output = null;
					if (gtk_dialog_run(test) == -5)
					{
						IntPtr response = gtk_file_chooser_get_filename(test);
						string test2 = GetFileName(response);
						g_free(response);
						output = test2;
					}
					gtk_widget_destroy(test);
					while (gtk_events_pending())
						gtk_main_iteration();
					return output;
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