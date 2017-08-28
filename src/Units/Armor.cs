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
	internal class Armor : BaseUnitLand
	{
		public Armor() : base(8, 10, 5, 3)
		{
			Type = UnitType.Armor;
			Name = "Armor";
			RequiredTech = new Automobile();
			ObsoleteTech = null;
			SetIcon('D', 0, 1);
		}
	}
}