// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.GFX
{
	internal class Patterns
	{
		private static Picture _panelGrey;
		public static Picture PanelGrey
		{
			get
			{
				if (_panelGrey == null)
				{
					_panelGrey = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
				}
				return _panelGrey;
			}
		}

		private static Picture _panelBlue;
		public static Picture PanelBlue
		{
			get
			{
				if (_panelBlue == null)
				{
					_panelBlue = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
					Picture.ReplaceColours(_panelBlue, new byte[] { 7, 22 }, new byte[] { 57, 9 });
				}
				return _panelBlue;
			}
		}
	}
}