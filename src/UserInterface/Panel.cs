// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.UserInterface
{
	public abstract class Panel : Element
	{
		protected Panel(int left, int top)
		{
			Left = left;
			Top = top;
		}
	}
}