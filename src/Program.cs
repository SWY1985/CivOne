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
#endif
using System;
#if !GTK
using System.Windows.Forms;
#endif

namespace CivOne
{
	class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			string screen = null;
			if (args.Length > 0)
			{
				switch (args[0])
				{
					case "demo":
						screen = "demo";
						break;
					case "setup":
						screen = "setup";
						break;
				}
			}
			
			Console.WriteLine("Game Start");
#if GTK
			Gdk.Threads.Init();
			Application.Init();
			using (GtkWindow window = new GtkWindow(screen))
			{
				Gdk.Threads.Enter();
				Application.Run();
				Gdk.Threads.Leave();
			}
#else
			Application.Run(new Window(screen));
#endif
			Console.WriteLine("Game End");
		}
	}
}