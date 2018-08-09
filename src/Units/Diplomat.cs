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
using System.Collections.Generic;
using CivOne.Players;

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
			City capital = cityToIncite.Player.GetCapital();

			int distance = capital == null ? 16 : cityToIncite.Tile.DistanceTo(capital);
			
			int cost = (cityToIncite.Player.Gold + 1000) / (distance + 3);

			// todo: if city is in disorder need to halve the cost
			return cost;
		}

		public IAdvance GetAdvanceToSteal(IPlayer victim)
		{
			IList<IAdvance> possible = victim.Advances.Where(p => !Player.Advances.Any(p2 => p2.Id == p.Id)).ToList();

			if (!possible.Any())
				return null;

			return possible[Common.Random.Next(0, possible.Count - 1)];
		}

		public string Sabotage(City city)
		{
			Game.DisbandUnit(this);

			IList<IBuilding> buildings = city.Buildings.Where(b => (b.GetType() != typeof(Buildings.Palace))).ToList();

			int random = Common.Random.Next(0, buildings.Count);

			if (random == buildings.Count)
			{
				city.Shields = (ushort)0;
				string production = (city.CurrentProduction as ICivilopedia).Name;
				return $"{production} production sabotaged";
			}
			else
			{
				// sabotage a building
				city.RemoveBuilding(buildings[random]);
				return $"{buildings[random].Name} sabotaged";
			}
		}

		protected override bool Confront(int relX, int relY)
		{
			ITile moveTarget = Map[X, Y][relX, relY];

			if (moveTarget.City != null)
			{
				if (Human.Is(Owner))
				{
					GameTask.Enqueue(Show.DiplomatCity(moveTarget.City, this));
					return true;
				}
				else
				{
					if (moveTarget.City.Player == Human)
						GameTask.Enqueue(Tasks.Show.DiplomatSabotage(moveTarget.City, this));
					else
						Sabotage(moveTarget.City);
						
					return true;
				}
			}

			IUnit[] units = moveTarget.Units;

			if (units.Length == 1)
			{
				IUnit unit = units[0];

				if (Human.Is(unit.Owner) && unit.Owner != Owner && unit is BaseUnitLand)
				{
					GameTask.Enqueue(Show.DiplomatBribe(unit as BaseUnitLand, this));
					return true;
				}
			}

			MovementTo(relX, relY);
			return true;
		}

		internal void KeepMoving(IUnit unit) => MovementTo(unit.X - X, unit.Y - Y);
		
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