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
	internal class MarketPlace : BaseBuilding
	{
		public MarketPlace() : base(8, 1, 80)
		{
			Name = "MarketPlace";
			RequiredTech = new Currency();
			SetIcon(0, 3, true);
			SetSmallIcon(0, 4);
			Type = Building.MarketPlace;
		}
	}
}