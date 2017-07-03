// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Advances;
using CivOne.Attributes;
using CivOne.Enums;
using CivOne.Templates;

namespace CivOne.Units
{
	[Default]
	internal class Militia : BaseUnitLand
	{
		public Militia() : base(1, 1, 1, 1)
		{
			Type = Unit.Militia;
			Name = "Militia";
			RequiredTech = null;
			ObsoleteTech = new Gunpowder();
			SetIcon('C', 0, 2);
		}
	}
}