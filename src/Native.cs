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
using CivOne.Enums;

namespace CivOne
{
	internal partial class Native
	{
		private static Platform Platform
		{
			get
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return Platform.Windows;
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return Platform.Linux;
				if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return Platform.macOS;
				return Platform.Unknown;
			}
		}

		public static string FolderBrowser(string caption = "")
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
				default:
					return null;
			}
		}
	}
}