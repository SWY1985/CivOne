// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

#if GTK
using Gtk;
using System;

namespace CivOne
{
	internal class GtkWindow : IDisposable
	{
		private readonly Window window;
		
		internal GtkWindow()
		{
			window = new Window("CivOne");
			window.Resize(640, 400);
			
			window.Add(test);
			
			window.ShowAll();
		}
		
		public void Dispose()
		{
			
		}
	}
}
#endif