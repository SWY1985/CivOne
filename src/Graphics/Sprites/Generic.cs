// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.IO;

namespace CivOne.Graphics.Sprites
{
	public static class Generic
	{
		private static Free Free => Free.Instance;
		private static Resources Resources => Resources.Instance;

		private static Bytemap GetFortify()
		{
			if (!Resources.Exists("SP257"))
				return Free.Instance.Fortify.Bitmap;
			return Resources["SP257"][208, 112, 16, 16]
				.ColourReplace(3, 0)
				.Bitmap;
		}

		public static ISprite Fortify = new CachedSprite(GetFortify);
	}
}