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
	public static class Pattern
	{
		private static Free Free => Free.Instance;
		private static Resources Resources => Resources.Instance;

		private static Bytemap PatternPanelGrey()
		{
			if (!Resources.Exists("SP299"))
				return Free.PanelGrey;
			return Resources["SP299"].Bitmap[288, 120, 32, 16];
		}

		private static Bytemap PatternPanelBlue()
		{
			if (!Resources.Exists("SP299"))
				return Free.PanelBlue;
			return Resources["SP299"].Bitmap[288, 120, 32, 16]
				.ColourReplace((7, 57), (22, 9));
		}

		public static readonly ISprite PanelGrey = new CachedSprite(PatternPanelGrey);
		public static readonly ISprite PanelBlue = new CachedSprite(PatternPanelBlue);
	}
}