// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CivOne
{
	internal partial class Native
	{
		private const string LIBGTK3 = "libgtk-3.so.0";
		private const string GLIB2 = "libglib-2.0.so.0";

		[DllImport(LIBGTK3, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr gtk_file_chooser_dialog_new(IntPtr title, IntPtr parent, int action, IntPtr nil);

		[DllImport(LIBGTK3, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr gtk_file_chooser_get_filename(IntPtr raw);

		[DllImport(LIBGTK3, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr gtk_dialog_add_button(IntPtr raw, IntPtr button_text, int response_id);

		[DllImport (LIBGTK3, CallingConvention = CallingConvention.Cdecl)]
		private static extern void gtk_init (ref int argc, ref IntPtr argv);

		[DllImport (LIBGTK3, CallingConvention = CallingConvention.Cdecl)]
		private static extern void gtk_main_iteration();

		[DllImport (LIBGTK3, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool gtk_events_pending();

		[DllImport(LIBGTK3, CallingConvention = CallingConvention.Cdecl)]
		private static extern int gtk_dialog_run(IntPtr handle);

		[DllImport (LIBGTK3, CallingConvention = CallingConvention.Cdecl)]
		private static extern void gtk_widget_destroy (IntPtr handle);

		[DllImport (GLIB2, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr g_filename_to_utf8 (IntPtr mem, int len, IntPtr read, out IntPtr written, out IntPtr error);

		[DllImport (GLIB2, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr g_malloc(UIntPtr size);
		
		[DllImport (GLIB2, CallingConvention = CallingConvention.Cdecl)]
		private static extern void g_free (IntPtr mem);

		public static IntPtr StringToIntPtr(string input)
		{
			if (input == null) return IntPtr.Zero;

			byte[] bytes = Encoding.UTF8.GetBytes(input);
			IntPtr output = g_malloc(new UIntPtr ((ulong)bytes.Length + 1));
			Marshal.Copy (bytes, 0, output, bytes.Length);
			Marshal.WriteByte(output, bytes.Length, 0);

			return output;
		}

		public static string GetFileName(IntPtr input) 
		{
			if (input == IntPtr.Zero) return null;

			IntPtr written, error;
			IntPtr filename = g_filename_to_utf8(input, -1, IntPtr.Zero, out written, out error);

			int i = 0;
			byte[] bytes;
			while(true)
			{
				bytes = new byte[++i];
				Marshal.Copy(filename, bytes, 0, i);
				if (bytes[bytes.GetUpperBound(0)] == 0) break;
			}
			
			return Encoding.UTF8.GetString(bytes.Take(bytes.Length - 1).ToArray());
		}

		private static IntPtr AddButton(IntPtr handle, string text, int responseId)
		{
			IntPtr native_button_text = StringToIntPtr(text);
			IntPtr raw_ret = gtk_dialog_add_button(handle, native_button_text, responseId);
			g_free(native_button_text);
			return raw_ret;
		}

		private static string GtkFolderBrowser(string caption)
		{
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
		}
	}
}