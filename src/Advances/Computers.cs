// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;

namespace CivOne.Advances
{
	internal class Computers : BaseAdvance
	{
		public Computers() : base(Advance.Mathematics, Advance.Electronics)
		{
			Name = "Computers";
			Type = Advance.Computers;
			SetIcon(3, 0, 2);
		}
	}
}