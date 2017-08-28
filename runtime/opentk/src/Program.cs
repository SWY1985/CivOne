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
		private static string HelpText => @"CivOne - An open source implementation of Sid Meier's Civilization
Usage: civone-opentk [argument|runtime-options]

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
Try 'civone-opentk --help' for more information.
";

		private static void Main(string[] args)
		{
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
			using (Window window = new Window(runtime))
			{
				runtime.Log("Game started");
				window.Run();
				runtime.Log("Game stopped");
			}
		}
	}
}