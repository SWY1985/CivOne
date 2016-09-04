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
	internal class Submarine : BaseUnit
	{
		public override void Explore()
		{
			Explore(2);
		}
		
		public Submarine() : base(50, 8, 2, 3)
		{
			Class = UnitClass.Water;
			Type = Unit.Submarine;
			Name = "Submarine";
			RequiredTech = new MassProduction();
			ObsoleteTech = null;
			SetIcon('C', 1, 2);
		}
	}
}