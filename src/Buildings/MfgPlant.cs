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

namespace CivOne.Buildings
{
	internal class MfgPlant : BaseBuilding
	{
		public MfgPlant() : base(32, 6)
		{
			Name = "Mfg. Plant";
			RequiredTech = new Robotics();
			SetIcon(3, 2, true);
			SetSmallIcon(3, 0);
			Type = Building.MfgPlant;
		}
	}
}