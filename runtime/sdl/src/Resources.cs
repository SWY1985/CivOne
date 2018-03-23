using System;
using System.IO;
using System.Reflection;
using CivOne.Enums;

namespace CivOne
{
	internal class Resources
	{
		private static Stream GetInternalResource(string name) => Assembly.GetExecutingAssembly().GetManifestResourceStream($"CivOne.Resources.{name}");
		
		private static Stream SdlSo => GetInternalResource("SDL2.so");

		private static Stream CivOneIco => GetInternalResource("CivOne.ico");

		private static Stream HelpTextTxt => GetInternalResource("HelpText.txt");
		
		private static bool WriteResourceToFile(Stream resource, string filePath, Func<bool> condition)
		{
			if (!condition()) return false;
			if (File.Exists(filePath)) return false;

			using (Stream resourceStream = resource)
			{
				if (resourceStream == null)
				{
					Console.WriteLine($"Could not write {Path.GetFileName(filePath)}, embedded resource missing");
					return false;
				}
				
				using (FileStream sw = new FileStream(Path.Combine(filePath), FileMode.CreateNew, FileAccess.Write))
				{
					resourceStream.CopyTo(sw);
				}
			}
			return File.Exists(filePath);
		}

		private static string GetResourceString(Stream resource)
		{
			using (Stream resourceStream = resource)
			{
				if (resourceStream == null) return null;

				using (StreamReader sr = new StreamReader(resourceStream))
				{
					return sr.ReadToEnd();
				}
			}
		}

		public static string BinPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public static string WorkingPath => Environment.CurrentDirectory;
		
		public static bool WriteSdlStub() => WriteResourceToFile(
			Resources.SdlSo,
			Path.Combine(BinPath, "SDL2.so"),
			() => Native.Platform == Platform.Linux);

		public static bool WriteWin32Icon() => WriteResourceToFile(
			Resources.CivOneIco,
			Path.Combine(BinPath, "CivOne.ico"),
			() => Native.Platform == Platform.Windows);
		
		public static string HelpText => GetResourceString(HelpTextTxt);
	}
}