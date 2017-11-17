// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Events;

namespace CivOne.UserInterface
{
	public interface IMouseElement
	{
		Rectangle Bounds { get; }
		bool MouseDown(int left, int top);
		bool MouseUp(int left, int top);
		bool MouseDrag(int left, int top);
	}
}