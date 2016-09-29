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
	internal class RecyclingCenter : BaseBuilding
	{
		public RecyclingCenter() : base(20, 2, 200, 800)
		{
			Name = "Recycling Cntr.";
			RequiredTech = new Recycling();
			SetIcon(4, 0, true);
			SetSmallIcon(3, 2);
			Type = Building.RecyclingCenter;
		}
	}
}