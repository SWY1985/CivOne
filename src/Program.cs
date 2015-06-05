// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Windows.Forms;

namespace CivOne
{
	class Program
	{
		private static void RunGtk(string screen)
		{
			using (GtkWindow window = new GtkWindow(screen))
			{
				window.Run();
			}
			/*
			Gdk.Threads.Init();
			Gtk.Application.Init();
			using (GtkWindow window = new GtkWindow(screen))
			{
				Gdk.Threads.Enter();
				Gtk.Application.Run();
				Gdk.Threads.Leave();
			}*/
		}
		
		private static void RunForms(string screen)
		{
			Application.Run(new Window(screen));
		}
		
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
			
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Unix:
				case PlatformID.MacOSX:
					RunGtk(screen);
					break;
				default:
					RunGtk(screen);
					//RunForms(screen);
					break;
			}
			
			Console.WriteLine("Game End");
		}
	}
}