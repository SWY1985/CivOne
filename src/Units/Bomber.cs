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
	internal class Bomber : BaseUnitAir
	{
		public override void Explore()
		{
			Explore(2);
		}
		
		public Bomber() : base(12, 12, 1, 8)
		{
			Type = UnitType.Bomber;
			Name = "Bomber";
			RequiredTech = new AdvancedFlight();
			ObsoleteTech = null;
			SetIcon('A', 1, 2);

			TotalFuel *= 2;
			FuelLeft = TotalFuel;
		}
	}
}