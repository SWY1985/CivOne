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
	internal class NuclearPlant : BaseBuilding
	{
		public NuclearPlant() : base(16, 2, 160)
		{
			Name = "Nuclear Plant";
			RequiredTech = new NuclearPower();
			SetIcon(4, 3, true);
			SetSmallIcon(4, 0);
			Type = Building.NuclearPlant;
		}
	}
}