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
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class SideBar : BaseScreen
	{
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;
				return true;
			}
			return false;
		}
		
		public SideBar(Color[] palette)
		{
			Bitmap background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
			
			_canvas = new Picture(80, 192, palette);
			_canvas.FillLayerTile(background);
		}
	}
}