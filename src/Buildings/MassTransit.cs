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
	internal class MassTransit : BaseBuilding
	{
		public MassTransit() : base(16, 4)
		{
			Name = "Mass Transit";
			RequiredTech = new MassProduction();
			SetIcon(2, 3, false);
			SetSmallIcon(2, 2);
			Type = Building.MassTransit;
		}
	}
}