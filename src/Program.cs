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
			
			if (!Window.CheckFiles())
			{
				Console.WriteLine("- Fatal error: Data directory is missing file(s).");
				Environment.Exit(1);
			}
			
			Console.WriteLine("Game Start");
			
			Window.CreateWindow(screen);
			
			Console.WriteLine("Game End");
		}
	}
}