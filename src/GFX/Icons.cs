// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.GFX;

namespace CivOne.GFX
{
	internal class Icons
	{
		private static Bitmap _food;
		public static Bitmap Food
		{
			get
			{
				if (_food == null)
				{
					_food = (Bitmap)Resources.Instance.GetPart("SP257", 128, 32, 8, 8);
					Picture.ReplaceColours(_food, 3, 0);
				}
				return _food;
			}
		}
		
		private static Bitmap _shield;
		public static Bitmap Shield
		{
			get
			{
				if (_shield == null)
				{
					_shield = (Bitmap)Resources.Instance.GetPart("SP257", 136, 32, 8, 8);
					Picture.ReplaceColours(_shield, 3, 0);
				}
				return _shield;
			}
		}
		
		private static Bitmap _trade;
		public static Bitmap Trade
		{
			get
			{
				if (_trade == null)
				{
					_trade = (Bitmap)Resources.Instance.GetPart("SP257", 144, 32, 8, 8);
					Picture.ReplaceColours(_trade, 3, 0);
				}
				return _trade;
			}
		}
	}
}