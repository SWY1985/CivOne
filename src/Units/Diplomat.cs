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
using CivOne.Tasks;
using CivOne.Tiles;

namespace CivOne.Units
{
	internal class Diplomat : BaseUnitLand
	{
		protected override bool Confront(int relX, int relY)
		{
			ITile moveTarget = Map[X, Y][relX, relY];
			IUnit[] units = moveTarget.Units;

			if (units.Length == 1)
			{
				IUnit unit = units[0];

				if (unit.Owner != Owner && unit is BaseUnitLand)
				{
					GameTask.Enqueue(Show.DiplomatBribe(unit as BaseUnitLand, this));
					return true;
				}
			}

			Movement = new MoveUnit(relX, relY);
			Movement.Done += MoveEnd;
			GameTask.Insert(Movement);
			return true;
		}
		public Diplomat() : base(3, 0, 0, 2)
		{
			Type = UnitType.Diplomat;
			Name = "Diplomat";
			RequiredTech = new Writing();
			ObsoleteTech = null;
			SetIcon('C', 1, 0);
		}
	}
}