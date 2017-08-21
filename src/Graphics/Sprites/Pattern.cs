// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.Graphics.Sprites
{
	public static class Pattern
	{
		private static Resources Resources => Resources.Instance;
		private static Free Free => Free.Instance;

		public static ISprite PanelGrey = new CachedSprite(() =>
		{
			if (!Resources.Exists("SP299"))
				return Free.PanelGrey.Bitmap;
			return Resources["SP299"][288, 120, 32, 16].Bitmap;
		});

		public static ISprite PanelBlue = new CachedSprite(() =>
		{
			if (!Resources.Exists("SP299"))
				return Free.PanelBlue.Bitmap;
			return Resources["SP299"][288, 120, 32, 16]
				.ColourReplace((7, 57), (22, 9)).Bitmap;
		});
	}
}