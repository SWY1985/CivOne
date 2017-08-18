// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.Graphics
{
	internal class Patterns
	{
		private static Resources Resources => Resources.Instance;
		private static Free Free => Free.Instance;

		private static IBitmap _panelGrey;
		public static IBitmap PanelGrey
		{
			get
			{
				if (_panelGrey == null)
				{
					if (!Resources.Exists("SP299"))
					{
						_panelGrey = Free.PanelGrey;
					}
					else
					{
						_panelGrey = Resources["SP299"].GetPart(288, 120, 32, 16);
					}
				}
				return _panelGrey;
			}
		}

		private static IBitmap _panelBlue;
		public static IBitmap PanelBlue
		{
			get
			{
				if (_panelBlue == null)
				{
					if (!Resources.Exists("SP299"))
					{
						_panelBlue = Free.Instance.PanelBlue;
					}
					else
					{
						_panelBlue = Resources["SP299"].GetPart(288, 120, 32, 16)
							.ColourReplace((7, 57), (22, 9));
					}
				}
				return _panelBlue;
			}
		}
	}
}