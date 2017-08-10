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

namespace CivOne.Units
{
	internal class Catapult : BaseUnitLand
	{
		public Catapult() : base(4, 6, 1, 1)
		{
			Type = Unit.Catapult;
			Name = "Catapult";
			RequiredTech = new Mathematics();
			ObsoleteTech = new Metallurgy();
			SetIcon('B', 0, 2);
		}
	}
}