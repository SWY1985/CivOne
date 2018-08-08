// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Linq;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.Units;

namespace CivOne
{
	internal partial class AI
	{
		private void BarbarianMove(IUnit unit)
		{
			switch (unit.Class)
			{
				case UnitClass.Water:
					BarbarianMoveWater(unit);
					return;
				case UnitClass.Land:
					BarbarianMoveLand(unit);
					return;
				default:
					Game.DisbandUnit(unit);
					return;
			}
		}

		private void BarbarianMoveWater(IUnit unit)
		{
			if (!unit.Tile.Units.Any(x => x.Class == UnitClass.Land))
			{
				Game.DisbandUnit(unit);
				return;
			}

			for (int i = 0; i < 1000; i++)
			{
				if (unit.Tile.GetBorderTiles().Any(x => !x.IsOcean))
				{
					if (Game.GetCities().Any(x => x.Owner != 0) && unit.Tile.GetBorderTiles().Any(x => !x.IsOcean && !x.Units.Any(u => u.Owner != 0)))
					{
						City nearestCity = Game.GetCities().Where(x => x.Owner != 0).OrderBy(x => Common.DistanceToTile(x.X, x.Y, unit.X, unit.Y)).ThenBy(x => x.Player == Human ? 0 : 1).First();
						if (nearestCity.Player == Human && Human.Visible(unit.Tile))
						{
							GameTask.Insert(Message.Advisor(Advisor.Defense, false, "Barbarian raiding party", $"lands near {nearestCity.Name}!", "Citizens are alarmed."));
						}
					}

					foreach (IUnit landUnit in unit.Tile.Units.Where(x => x.Class == UnitClass.Land && x.Sentry))
					{
						landUnit.Sentry = false;
					}
					if (unit.Tile.Units.Any(x => x.Class == UnitClass.Land && x.MovesLeft > 0))
						Game.UnitWait();
					else
						unit.SkipTurn();
					return;
				}

				if (unit.Goto.IsEmpty)
				{
					if (!Game.GetCities().Any(x => x.Owner != 0 && x.HasBuilding<Palace>())) Game.DisbandUnit(unit);
					
					City nearestCity = Game.GetCities().Where(x => x.Owner != 0 && x.HasBuilding<Palace>()).OrderBy(x => Common.DistanceToTile(x.X, x.Y, unit.X, unit.Y)).First();
					if (Common.DistanceToTile(unit.X, unit.Y, nearestCity.X, nearestCity.Y) > 10) Game.DisbandUnit(unit);
					unit.Goto = new Point(nearestCity.X, nearestCity.Y);
					continue;
				}

				if (!unit.Goto.IsEmpty)
				{
					int distance = unit.Tile.DistanceTo(unit.Goto);
					ITile[] tiles = unit.MoveTargets.OrderBy(x => x.DistanceTo(unit.Goto)).ThenBy(x => x.Movement).ToArray();
					if (tiles.Length == 0 || tiles[0].DistanceTo(unit.Goto) > distance)
					{
						// No valid tile to move to, cancel goto
						unit.Goto = Point.Empty;
						continue;
					}
					else if (tiles[0].DistanceTo(unit.Goto) == distance)
					{
						// Distance is unchanged, 50% chance to cancel goto
						if (Common.Random.Next(0, 100) < 50)
						{
							unit.Goto = Point.Empty;
							continue;
						}
					}

					if (!unit.MoveTo(tiles[0].X - unit.X, tiles[0].Y - unit.Y))
					{
						unit.Goto = Point.Empty;
						unit.SkipTurn();
					}
					return;
				}

				unit.SkipTurn();
				return;
			}
		}

		private void BarbarianMoveLand(IUnit unit)
		{
			if (unit.Tile.IsOcean && unit.Tile.GetBorderTiles().Where(x => !x.IsOcean).All(x => x.Units.Any(u => u.Owner != 0)))
			{
				IUnit ship = unit.Tile.Units.FirstOrDefault(u => u.Class == UnitClass.Water && u.MovesLeft > 0);
				if (ship != null)
				{
					ITile[] landTiles = unit.Tile.GetBorderTiles().Where(x => !x.IsOcean && x.Units.Any(u => u.Owner != 0)).ToArray();
					if (landTiles.Length > 0)
					{
						ITile tile = landTiles[Common.Random.Next(landTiles.Length)];
						ship.MoveTo(tile.X - unit.X, tile.Y - unit.Y);
						return;
					}
				}
				unit.SkipTurn();
				return;
			}

			if (unit is Diplomat)
			{
				ITile[] friendlyTiles = unit.Tile.GetBorderTiles().Where(x => !x.IsOcean && x.Units.Any() && x.Units.First().Owner == 0).ToArray(); //Game.GetUnits().Where(x => x.Owner == 0 && x.Class == UnitClass.Land && x.Tile.DistanceTo(unit.Tile) == 1).FirstOrDefault();
				if (friendlyTiles.Length > 0)
				{
					ITile moveTo = friendlyTiles[Common.Random.Next(friendlyTiles.Length)];
					int relX = moveTo.X - unit.X;
					int relY = moveTo.Y - unit.Y;
					unit.MoveTo(relX, relY);
					return;
				}

				if (unit.Tile.Units.Any(x => !(x is Diplomat) && x.MovesLeft > 0))
				{
					Game.UnitWait();
					return;
				}

				if (unit.Tile.Units.Any(x => !(x is Diplomat)))
				{
					unit.SkipTurn();
					return;
				}
			}

			ITile[] tiles = unit.Tile.GetBorderTiles().Where(t => !((unit.Tile.IsOcean || unit is Diplomat) && t.City != null) && !t.IsOcean && t.Units.Any(u => u.Owner != 0)).ToArray();
			if (tiles.Length == 0)
			{
				// No adjecent units found
				if (Common.Random.Next(100) < 95)
				{
					for (int i = 0; i < 1000; i++)
					{
						int relX = Common.Random.Next(-1, 2);
						int relY = Common.Random.Next(-1, 2);
						if (relX == 0 && relY == 0) continue;
						if (unit.Tile[relX, relY] is Ocean) continue;
						if (unit is Diplomat && unit.Tile[relX, relY].City != null) continue;
						if (unit.Tile.IsOcean && unit.Tile[relX, relY].City != null) continue;
						unit.MoveTo(relX, relY);
						return;
					}
				}
				Game.DisbandUnit(unit);
				return;
			}
			else
			{
				ITile moveTo = tiles[Common.Random.Next(tiles.Length)];
				int relX = moveTo.X - unit.X;
				int relY = moveTo.Y - unit.Y;
				while (relX < -1) relX += 80;
				while (relX > 1) relX -= 80;
				if (unit is Diplomat && unit.Tile.City != null) return;

				unit.MoveTo(relX, relY);
			}
		}
	}
}