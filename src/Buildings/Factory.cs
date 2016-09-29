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
	internal class Factory : BaseBuilding
	{
		public Factory() : base(20, 4, 200)
		{
			Name = "Factory";
			RequiredTech = new Industrialization();
			SetIcon(3, 1, true);
			SetSmallIcon(2, 4);
			Type = Building.Factory;
		}
	}
}