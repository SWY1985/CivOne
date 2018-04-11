using System;
using System.IO;
using System.Reflection;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Graphics.ImageFormats;

namespace CivOne
{
	internal class Resources
	{
		private static Stream GetInternalResource(string name) => Assembly.GetEntryAssembly().GetManifestResourceStream($"CivOne.Resources.{name}");

		private static Stream HelpTextTxt => GetInternalResource("HelpText.txt");

		private static Stream WindowIcon => GetInternalResource("WindowIcon.gif");
		
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

		public static string BinPath => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		
		public static IBitmap GetWindowIcon()
		{
			using (Stream resourceStream = WindowIcon)
			using (MemoryStream ms = new MemoryStream())
			{
				resourceStream.CopyTo(ms);
				using (GifFile gifFile = new GifFile(ms.ToArray()))
				{
					return gifFile.GetBitmap();
				}
			}
		}
		
		public static string HelpText => GetResourceString(HelpTextTxt);
	}
}