// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Setup : BaseScreen
	{
		public override bool HasUpdate(uint gameTick)
		{
			return false;
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			return false;
		}
		
		public override bool MouseDown(MouseEventArgs args)
		{
			return false;
		}
		
		public Setup()
		{
			Cursor = MouseCursor.Pointer;
			
            Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			
			_canvas = new Picture(320, 200, palette);
			_canvas.FillRectangle(1, 0, 0, 320, 200);
			_canvas.DrawText("CivOne Setup", 3, 14, 4, 4, TextAlign.Left);
		}
	}
}