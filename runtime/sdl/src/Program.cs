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
using System.Reflection;
using System.Runtime.InteropServices;

namespace CivOne
{
	internal class Program
	{
		private static string HelpText => @"CivOne - An open source implementation of Sid Meier's Civilization
Usage: civone-sdl [argument|runtime-options]

arguments:
  -h|--help             Show this documentation.

runtime-options:
  --demo                Show the Demo screen before launching the game
  --setup               Show the Setup screen before launching the game
  --free                Launch the game with free assets. Does not load assets,
                        disables sound, skips data check, intro and credits
  --no-data-check       Disables checking for game data files
  --no-sound            Disable ingame sounds
  --skip-credits        Skips the game credits sequence
  --skip-intro          Skips the game intro sequence
";

		private static string ErrorText => @"civone-opentk: Invalid options: '{0}'
Try 'civone-sdl --help' for more information.
";

		private static bool WriteSdlStub()
		{
			string binPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SDL2.so");
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;
			if (File.Exists(binPath)) return false;
			foreach (string bla in Assembly.GetExecutingAssembly().GetManifestResourceNames())
			{
				Console.WriteLine(bla);
			}
			using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CivOne.SDL.Resources.SDL2.so"))
			{
				if (resourceStream == null)
				{
					Console.WriteLine("Could not read embedded resource for SDL2.so");
					return false;
				}
				
				using (FileStream sw = new FileStream(Path.Combine(binPath), FileMode.CreateNew, FileAccess.Write))
				{
					resourceStream.CopyTo(sw);
				}
			}
			return File.Exists(binPath);
		}

		static void Main(string[] args)
		{
			if (WriteSdlStub()) Console.WriteLine("Written SDL2 library stub...");

			RuntimeSettings settings = new RuntimeSettings();
			for (int i = 0; i < args.Length; i++)
			{
				string cmd = args[i].TrimStart('-');
				if (i == 0 && args.Length == 1)
				{
					switch(cmd)
					{
						case "help":
						case "h":
							Console.WriteLine(HelpText);
							return;
					}
				}

				switch(cmd)
				{
					case "demo": settings.Demo = true; continue;
					case "setup": settings.Setup = true; continue;
					case "free": settings.Free = true; continue;
					case "no-sound": settings["no-sound"] = true; continue;
					case "no-data-check": settings.DataCheck = false; continue;
					case "skip-credits": settings.ShowCredits = false; continue;
					case "skip-intro": settings.ShowIntro = false; continue;
					default: Console.WriteLine(ErrorText); return;
				}
			}

			if (settings.Free)
			{
				settings["no-sound"] = true;
			}

			using (Runtime runtime = new Runtime(settings))
			using (GameWindow window = new GameWindow(runtime))
			{
				runtime.Log("Game started");
				window.Run();
				runtime.Log("Game stopped");
			}
		}
	}
}