// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Enums;

namespace CivOne
{
	internal class Program
	{
		private static string HelpText => @"CivOne - An open source implementation of Sid Meier's Civilization
Usage: civone-sdl [argument|runtime-options]

arguments:
  -h|--help             Show this documentation.
  -D|--desktop-icon     Create an icon on the desktop (windows only)

runtime-options:
  --demo                Show the Demo screen before launching the game
  --setup               Show the Setup screen before launching the game
  --free                Launch the game with free assets. Does not load assets,
                        disables sound, skips data check, intro and credits
  --no-data-check       Disables checking for game data files
  --no-sound            Disable ingame sounds
  --skip-credits        Skips the game credits sequence
  --skip-intro          Skips the game intro sequence
  --software-render     Force the use of SDL software rendererer
";

		private static string ErrorText => @"civone-sdl: Invalid options: '{0}'
Try 'civone-sdl --help' for more information.
";

		private static void Main(string[] args)
		{
			if (Resources.WriteSdlStub()) Console.WriteLine("Written SDL2 library stub...");
			if (Resources.WriteWin32Icon()) Console.WriteLine("Written Win32 icon file...");

			RuntimeSettings settings = new RuntimeSettings();
			settings["software-render"] = false;
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
						case "desktop-icon":
						case "D":
							switch (Native.Platform)
							{
								case Platform.Windows:
									Console.Write("Creating desktop icon... ");
									Console.WriteLine(Native.CreateDesktopIcon("CivOne", "An open source implementation of Sid Meier's Civilization") ? "done" : "failed");
									break;
								default:
									Console.WriteLine($"Creating a desktop icon is not implemented on {Native.Platform.Name()}.");
									break;
							}
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
					case "software-render": settings["software-render"] = true; continue;
					default: Console.WriteLine(ErrorText); return;
				}
			}

			if (settings.Free)
			{
				settings["no-sound"] = true;
			}

			using (Runtime runtime = new Runtime(settings))
			using (GameWindow window = new GameWindow(runtime, (bool)settings["software-render"]))
			{
				runtime.Log("Game started");
				window.Run();
				runtime.Log("Game stopped");
			}
		}
	}
}