// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class MenuBar : BaseScreen
	{
		public override bool HasUpdate(uint gameTick)
		{
			return false;
		}
		
		public GameMenu(Color[] palette)
		{
			_canvas = new Picture(320, 8, palette);
			_canvas.FillRectangle(5, 0, 0, 320, 8);
			_canvas.DrawText("GAME", 0, 15, 7, 8, 1, TextAlign.Left);
			_canvas.DrawText("ORDERS", 0, 15, 7, 64, 1, TextAlign.Left);
			_canvas.DrawText("ADVISORS", 0, 15, 7, 128, 1, TextAlign.Left);
			_canvas.DrawText("WORLD", 0, 15, 7, 192, 1, TextAlign.Left);
			_canvas.DrawText("CIVILOPEDIA", 0, 15, 7, 240, 1, TextAlign.Left);
		}
	}
}