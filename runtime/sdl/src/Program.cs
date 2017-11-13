using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CivOne
{
	internal class Program
	{
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
			if (WriteSdlStub())
			{
				Console.WriteLine("Written SDL2 library stub...");
			}

			using (Runtime runtime = new Runtime())
			using (GameWindow window = new GameWindow(runtime))
			{
				window.Run();
			}
		}
	}
}