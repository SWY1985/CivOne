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
using CivOne.Advances;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Governments;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.Units;

using Democratic = CivOne.Governments.Democracy;

namespace CivOne
{
	internal partial class AI : BaseInstance
	{
		internal static void Move(IUnit unit)
		{
			Player player = Game.GetPlayer(unit.Owner);

			if (unit.Owner == 0)
			{
				BarbarianMove(unit);
				return;
			}
			
			if (unit is Settlers)
			{
				ITile tile = unit.Tile;

				bool hasCity = (tile.City != null);
				bool validCity = (tile is Grassland || tile is River || tile is Plains) && (tile.City == null);
				bool validIrrigaton = (tile is Grassland || tile is River || tile is Plains || tile is Desert) && (tile.City == null) && (!tile.Mine) && (!tile.Irrigation) && tile.CrossTiles().Any(x => x.IsOcean || x is River || x.Irrigation);
				bool validMine = (tile is Mountains || tile is Hills) && (tile.City == null) && (!tile.Mine) && (!tile.Irrigation);
				bool validRoad = (tile.City == null) && tile.Road;
				int nearestCity = 255;
				int nearestOwnCity = 255;
				
				if (Game.GetCities().Any()) nearestCity = Game.GetCities().Min(x => Common.DistanceToTile(x.X, x.Y, tile.X, tile.Y));
				if (Game.GetCities().Any(x => x.Owner == unit.Owner)) nearestOwnCity = Game.GetCities().Where(x => x.Owner == unit.Owner).Min(x => Common.DistanceToTile(x.X, x.Y, tile.X, tile.Y));
				
				if (validCity && nearestCity > 3)
				{
					GameTask.Enqueue(Orders.FoundCity(unit as Settlers));
					return;
				}
				else if (nearestOwnCity < 3)
				{
					switch (Common.Random.Next(5 * nearestOwnCity))
					{
						case 0:
							if (validRoad)
							{
								GameTask.Enqueue(Orders.BuildRoad(unit));
								return;
							}
							break;
						case 1:
							if (validIrrigaton)
							{
								GameTask.Enqueue(Orders.BuildIrrigation(unit));
								return;
							}
							break;
						case 2:
							if (validMine)
							{
								GameTask.Enqueue(Orders.BuildMines(unit));
								return;
							}
							break;
					}
				}

				for (int i = 0; i < 1000; i++)
				{
					int relX = Common.Random.Next(-1, 2);
					int relY = Common.Random.Next(-1, 2);
					if (relX == 0 && relY == 0) continue;
					if (unit.Tile[relX, relY] is Ocean) continue;
					if (unit.Tile[relX, relY].Units.Any(x => x.Owner != unit.Owner)) continue;
					if (!unit.MoveTo(relX, relY)) continue;
					return;
				}
				unit.SkipTurn();
				return;
			}
			else if (unit is Militia || unit is Phalanx || unit is Musketeers || unit is Riflemen || unit is MechInf)
			{
				unit.Fortify = true;
				while (unit.Tile.City != null && unit.Tile.Units.Count(x => x is Militia || x is Phalanx || x is Musketeers || x is Riflemen || x is MechInf) > 2)
				{
					IUnit disband = null;
					IUnit[] units = unit.Tile.Units.Where(x => x != unit).ToArray();
					if ((disband = unit.Tile.Units.FirstOrDefault(x => x is Militia)) != null) { Game.DisbandUnit(disband); continue; }
					if ((disband = unit.Tile.Units.FirstOrDefault(x => x is Phalanx)) != null) { Game.DisbandUnit(disband); continue; }
					if ((disband = unit.Tile.Units.FirstOrDefault(x => x is Musketeers)) != null) { Game.DisbandUnit(disband); continue; }
					if ((disband = unit.Tile.Units.FirstOrDefault(x => x is Riflemen)) != null) { Game.DisbandUnit(disband); continue; }
					if ((disband = unit.Tile.Units.FirstOrDefault(x => x is MechInf)) != null) { Game.DisbandUnit(disband); continue; }
				}
			}
			else
			{
				if (unit.Class != UnitClass.Land) Game.DisbandUnit(unit);

				for (int i = 0; i < 1000; i++)
				{
					if (unit.Goto.IsEmpty)
					{
						int gotoX = Common.Random.Next(-5, 6);
						int gotoY = Common.Random.Next(-5, 6);
						if (gotoX == 0 && gotoY == 0) continue;
						if (!player.Visible(unit.X + gotoX, unit.Y + gotoY)) continue;

						unit.Goto = new Point(unit.X + gotoX, unit.Y + gotoY);
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

						if (tiles[0].Units.Any(x => x.Owner != unit.Owner))
						{
							if (unit.Role == UnitRole.Civilian || unit.Role == UnitRole.Settler)
							{
								// do not attack with civilian or settler units
								unit.Goto = Point.Empty;
								continue;
							}

							if (unit.Role == UnitRole.Transport && Common.Random.Next(0, 100) < 67)
							{
								// 67% chance of cancelling attack with transport unit
								unit.Goto = Point.Empty;
								continue;
							}

							if (unit.Attack < tiles[0].Units.Select(x => x.Defense).Max() && Common.Random.Next(0, 100) < 50)
							{
								// 50% of attacking cancelling attack of stronger unit
								unit.Goto = Point.Empty;
								continue;
							}
						}

						if (!unit.MoveTo(tiles[0].X - unit.X, tiles[0].Y - unit.Y))
						{
							// The code below is to prevent the game from becoming stuck...
							if (Common.Random.Next(0, 100) < 67)
							{
								unit.Goto = Point.Empty;
								continue;
							}
							else if (Common.Random.Next(0, 100) < 67)
							{
								unit.SkipTurn();
								return;
							}
							else
							{
								Game.DisbandUnit(unit);
								return;
							}
						}
						return;
					}
				}
				unit.SkipTurn();
				return;
			}
		}

		internal static void ChooseResearch(Player player)
		{
			if (player.CurrentResearch != null) return;
			
			IAdvance[] advances = player.AvailableResearch.ToArray();
			
			// No further research possible
			if (advances.Length == 0) return;

			player.CurrentResearch = advances[Common.Random.Next(0, advances.Length)];

			Log($"AI: {player.LeaderName} of the {player.TribeNamePlural} starts researching {player.CurrentResearch.Name}.");
		}

		internal static void CityProduction(City city)
		{
			if (city == null || city.Size == 0 || city.Tile == null) return;

			Player player = Game.GetPlayer(city.Owner);
			IProduction production = null;

			// Create 2 defensive units per city
			if (player.HasAdvance<LaborUnion>())
			{
				if (city.Tile.Units.Count(x => x is MechInf) < 2) production = new MechInf();
			}
			else if (player.HasAdvance<Conscription>())
			{
				if (city.Tile.Units.Count(x => x is Riflemen) < 2) production = new Riflemen();
			}
			else if (player.HasAdvance<Gunpowder>())
			{
				if (city.Tile.Units.Count(x => x is Musketeers) < 2) production = new Musketeers();
			}
			else if (player.HasAdvance<BronzeWorking>())
			{
				if (city.Tile.Units.Count(x => x is Phalanx) < 2) production = new Phalanx();
			}
			else
			{
				if (city.Tile.Units.Count(x => x is Militia) < 2) production = new Militia();
			}
			
			// Create city improvements
			if (production == null)
			{
				if (!city.HasBuilding<Barracks>()) production = new Barracks();
				else if (player.HasAdvance<Pottery>() && !city.HasBuilding<Granary>()) production = new Granary();
				else if (player.HasAdvance<CeremonialBurial>() && !city.HasBuilding<Temple>()) production = new Temple();
				else if (player.HasAdvance<Masonry>() && !city.HasBuilding<CityWalls>()) production = new CityWalls();
			}

			// Create Settlers
			if (production == null)
			{
				if (city.Size > 3 && !city.Units.Any(x => x is Settlers) && player.Cities.Length < 10) production = new Settlers();
			}

			// Create some other unit
			if (production == null)
			{
				if (city.Units.Length < 4)
				{
					if (player.Government is Republic || player.Government is Democratic)
					{
						if (player.HasAdvance<Writing>()) production = new Diplomat();
					}
					else 
					{
						if (player.HasAdvance<Automobile>()) production = new Armor();
						else if (player.HasAdvance<Metallurgy>()) production = new Cannon();
						else if (player.HasAdvance<Chivalry>()) production = new Knights();
						else if (player.HasAdvance<TheWheel>()) production = new Chariot();
						else if (player.HasAdvance<HorsebackRiding>()) production = new Cavalry();
						else if (player.HasAdvance<IronWorking>()) production = new Legion();
					}
				}
				else
				{
					if (player.HasAdvance<Trade>()) production = new Caravan();
				}
			}

			// Set random production
			if (production == null)
			{
				IProduction[] items = city.AvailableProduction.ToArray();
				production = items[Common.Random.Next(items.Length)];
			}

			city.SetProduction(production);
		}
	}
}