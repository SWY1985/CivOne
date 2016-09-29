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

namespace CivOne.Buildings
{
	internal class HydroPlant : BaseBuilding
	{
		public HydroPlant() : base(24, 4, 240)
		{
			Name = "Hydro Plant";
			RequiredTech = new Electronics();
			SetIcon(4, 2, false);
			SetSmallIcon(3, 4);
			// TODO: Fix icon in patch, should be: SetSmallIcon(3, 3);
			Type = Building.HydroPlant;
		}
	}
}