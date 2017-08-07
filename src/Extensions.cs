// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.IO;
using System.Linq;

namespace CivOne
{
	public static class Extensions
	{
		private static Settings Settings => Settings.Instance;

		public static string GetSoundFile(this string input)
		{
			return Directory.GetFiles(Settings.SoundsDirectory).Where(x => Path.GetFileName(x).ToLower() == $"{input.ToLower()}.wav").FirstOrDefault();
		}
	}
}