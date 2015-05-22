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
		public override bool HasUpdate(uint gameTick)
		{
			return false;
		}
		
		public SideBar(Color[] palette)
		{
			_canvas = new Picture(80, 192, palette);
			_canvas.FillRectangle(9, 0, 0, 80, 192);
		}
	}
}