// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Advances;
using CivOne.Enums;

namespace CivOne.Units
{
	internal class Frigate : BaseUnitSea, IBoardable
	{
		public int Cargo
		{
			get
			{
				return 4;
			}
		}

		public Frigate() : base(4, 2, 2, 3)
		{
			Type = Unit.Frigate;
			Name = "Frigate";
			RequiredTech = new Magnetism();
			ObsoleteTech = null;
			SetIcon('B', 1, 0);
		}
	}
}