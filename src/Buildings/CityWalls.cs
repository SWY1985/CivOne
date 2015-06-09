// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Templates;

namespace CivOne.Buildings
{
	internal class CityWalls : BaseBuilding
	{
		public CityWalls() : base(12, 2)
		{
			Name = "City Walls";
			RequiredTech = null;
			SetIcon(1, 2, false);
		}
	}
}