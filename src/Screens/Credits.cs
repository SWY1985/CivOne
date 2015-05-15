// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Windows.Forms;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Credits : BaseScreen
	{
		private readonly Picture _canvas;
		
		public override Picture Canvas
		{
			get
			{
				return _canvas;
			}
		}
		
		public override MouseCursor Cursor
		{
			get
			{
				return MouseCursor.Pointer;
			}
		}
		
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
	}
}