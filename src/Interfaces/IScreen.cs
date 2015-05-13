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

namespace CivOne.Interfaces
{
	internal interface IScreen
	{
		MouseCursor Cursor { get; }
        bool HasUpdate(uint gameTick);
        void Draw(Graphics gfx);
        bool KeyDown(KeyEventArgs args);
		bool MouseDown(MouseEventArgs args);
	}
}