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
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Units
{
	internal class Trireme : BaseUnit
	{
		public Trireme() : base(4, 1, 0, 3)
		{
			Class = UnitClass.Water;
			Type = Unit.Trireme;
			Name = "Trireme";
			RequiredTech = new MapMaking();
			ObsoleteTech = new Navigation();
			SetIcon('B', 0, 1);
		}
	}
}