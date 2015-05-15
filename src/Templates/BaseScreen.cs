// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.GFX;

namespace CivOne.Templates
{
	internal abstract class BaseScreen : IScreen
	{		
		private Picture _canvas;
		public abstract Picture Canvas { get; }
		public abstract MouseCursor Cursor { get; }
		public abstract bool HasUpdate(uint gameTick);
		public abstract bool KeyDown(KeyEventArgs args);
		public abstract bool MouseDown(MouseEventArgs args);
	}
}