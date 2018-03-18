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
using System.Reflection;

namespace CivOne
{
	internal partial class Native
	{
		private static string MacFolderBrowser(string caption)
		{
			string scriptPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "OpenFolder.sh");
			using (StreamWriter sw = new StreamWriter(scriptPath, false))
			{
				sw.WriteLine("!#/bin/bash");
				sw.WriteLine($@"osascript -e 'set getPath to choose folder with prompt ""{caption}""' -e 'set output to POSIX path of getPath'");
				sw.Flush();
			}

			Process.Start("chmod", $@"+x ""{scriptPath}""");

			Process process = new Process();
			process.StartInfo = new ProcessStartInfo()
			{
				FileName = "/bin/sh",
				Arguments = scriptPath,
				RedirectStandardOutput = true,
				UseShellExecute = false
			};
			
			process.Start();
			string output = process.StandardOutput.ReadToEnd().Trim(new [] { '\n', '"' });
			process.WaitForExit();

			if (output.Length == 0 || !Directory.Exists(output)) return null;

			return output;
		}
	}
}