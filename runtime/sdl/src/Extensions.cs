using CivOne.Enums;

namespace CivOne
{
	internal static class Extensions
	{
		public static string Name(this Platform platform)
		{
			switch (platform)
			{
				case Platform.Windows: return "Windows";
				case Platform.Linux: return "Linux";
				case Platform.macOS: return "macOS";
				default: return "Unknown";
			}
		}
	}
}