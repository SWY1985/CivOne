// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;

namespace CivOne
{
	internal class Program
	{
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
			
			Console.WriteLine("Loaded plugins:");
			foreach (CivOne.Interfaces.IPlugin plugin in Reflect.Plugins())
			{
				Console.WriteLine($@" - ""{plugin.Name}"" by {plugin.Author} [version {plugin.Version}]");
			}
			
			Console.WriteLine("Game Start");
			
			using (Window window = new Window(screen))
			{
				window.Run(60.0);
			}
			
			Console.WriteLine("Game End");
		}
	}
}