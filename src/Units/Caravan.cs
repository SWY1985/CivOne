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

namespace CivOne.Units
{
	internal class Caravan : BaseUnitLand
	{
		public Caravan() : base(5, 0, 1, 1)
		{
			Type = Unit.Caravan;
			Name = "Caravan";
			RequiredTech = new Trade();
			ObsoleteTech = null;
			SetIcon('E', 0, 1);
		}
	}
}