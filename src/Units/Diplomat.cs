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
using System.Linq;
using CivOne.Buildings;

namespace CivOne.Units
{
	internal class Diplomat : BaseUnitLand
	{
		public static bool CanIncite(City cityToIncice, short gold)
		{
			return gold >= InciteCost(cityToIncice) && !cityToIncice.HasBuilding<Palace>();
		}

		public static int InciteCost(City cityToIncite)
		{
			City capital = cityToIncite.Player.Cities.Where(c => c.HasBuilding(new Palace())).FirstOrDefault();

			int distance = capital == null ? 16 : cityToIncite.Tile.DistanceTo(capital);
			
			int cost = (cityToIncite.Player.Gold + 1000) / (distance + 3);

			// todo: if city is in disorder need to halve the cost
			return cost;
		}
		protected override bool Confront(int relX, int relY)
		{
			ITile moveTarget = Map[X, Y][relX, relY];

			if (moveTarget.City != null)
			{
					GameTask.Enqueue(Show.DiplomatCity(moveTarget.City, this));
					return true;
			}

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

		internal void KeepMoving(IUnit unit)
		{
			Movement = new MoveUnit(unit.X - X, unit.Y - Y);
			Movement.Done += MoveEnd;
			GameTask.Enqueue(Movement);
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