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
using CivOne.Interfaces;

namespace CivOne.Units
{
	internal class Sail : BaseUnitSea, IBoardable
	{
		public int Cargo
		{
			get
			{
				return 3;
			}
		}

		public Sail() : base(4, 1, 1, 3)
		{
			Type = Unit.Sail;
			Name = "Sail";
			RequiredTech = new Navigation();
			ObsoleteTech = new Magnetism();
			SetIcon('B', 1, 1);
		}
	}
}