// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Diagnostics;
using System.IO;

namespace CivOne
{
	internal partial class Native
	{
		private static string EscapeAppleScriptString(string input)
		{
			return (input ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
		}

		private static string MacFolderBrowser(string caption)
		{
			Process process = new Process();
			process.StartInfo = new ProcessStartInfo()
			{
				FileName = "osascript",
				ArgumentList =
				{
					"-e",
					$@"POSIX path of (choose folder with prompt ""{EscapeAppleScriptString(caption)}"")"
				},
				RedirectStandardOutput = true,
				UseShellExecute = false
			};
			
			process.Start();
			string output = process.StandardOutput.ReadToEnd().Trim(new [] { '\n', '\r', '"' });
			process.WaitForExit();

			if (process.ExitCode != 0 || output.Length == 0 || !Directory.Exists(output)) return null;

			return output;
		}
	}
}
