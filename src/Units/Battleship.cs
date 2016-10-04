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
using CivOne.Templates;

namespace CivOne.Units
{
	internal class Battleship : BaseUnitSea
	{
		public Battleship() : base(16, 18, 12, 4, 2)
		{
			Type = Unit.Battleship;
			Name = "Battleship";
			RequiredTech = new Steel();
			ObsoleteTech = null;
			SetIcon('A', 1, 0);
		}
	}
}