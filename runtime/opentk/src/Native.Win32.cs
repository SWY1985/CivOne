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
	internal partial class Native
	{
		[StructLayout(LayoutKind.Sequential)]
		private struct BROWSEINFO 
		{
			public IntPtr hwndOwner;
			public IntPtr pidlRoot;
			public IntPtr pszDisplayName;
			[MarshalAs(UnmanagedType.LPStr)]
			public string lpszTitle;
			public uint ulFlags;
			public BrowseCallbackProc lpfn;
			public IntPtr lParam;
			public int iImage;
		}

		private delegate int BrowseCallbackProc(IntPtr hwnd, int uMsg, IntPtr lParam, IntPtr lpData);

		[DllImport("shell32.dll")]
		private static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

		[DllImport("shell32.dll", CharSet=CharSet.Unicode)]
		private static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

		[DllImport("user32.dll")]
		public static extern int ShowCursor(bool bShow);

		private static string Win32FolderBrowser(string caption)
		{
			ShowCursor();
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

			HideCursor();
			
			return Marshal.PtrToStringUni(bufferAddress);
		}
	}
}