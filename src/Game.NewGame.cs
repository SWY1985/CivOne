// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Linq;
using CivOne.Advances;
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Tiles;
using CivOne.Units;

namespace CivOne
{
	public partial class Game : BaseInstance
	{
		private void AddStartingUnits(byte player)
		{
			// Translated from this post by darkpanda, might contain errors:
			// http://forums.civfanatics.com/showthread.php?p=12895306&highlight=starting+position#post12895306
			int loopCounter = 0;
			while (loopCounter++ < 2000)
			{
				// Choose a map square randomly
				int x = Common.Random.Next(0, Map.WIDTH);
				int y = Common.Random.Next(2, Map.HEIGHT - 2);
				if (Map.FixedStartPositions && GameTurn == 0)
				{
					// Map position is fixed, don't check anything
					x = _players[player].Civilization.StartX;
					y = _players[player].Civilization.StartY;
					if (Map[x, y].Hut) Map[x, y].Hut = false;
				}
				else
				{
					ITile tile = Map[x, y];
					
					if (tile.IsOcean) continue; // Is it an ocean tile?
					if (tile.Hut) continue; // Is there a hut on this tile?
					if (_units.Any(u => u.X == x || u.Y == y)) continue; // Is there already a unit on this tile?
					if (tile.LandValue < (12 - (loopCounter / 32))) continue; // Is the land value high enough?
					if (_cities.Any(c => Common.DistanceToTile(x, y, c.X, c.Y) < (10 - (loopCounter / 64)))) continue; // Distance to other cities
					if (_units.Any(u => (u is Settlers) && Common.DistanceToTile(x, y, u.X, u.Y) < (10 - (loopCounter / 64)))) continue; // Distance to other settlers
					if (Map.ContinentTiles(tile.ContinentId).Count(t => Map.TileIsType(t, Terrain.Plains, Terrain.Grassland1, Terrain.Grassland2, Terrain.River)) < (32 - (GameTurn / 16))) continue; // Check buildable tiles on continent
					
					// After 0 AD, don't spawn a Civilization on a continent that already contains cities.
					if (Common.TurnToYear(GameTurn) >= 0 && Map.ContinentTiles(tile.ContinentId).Any(t => t.City != null)) continue;
					
					Log(loopCounter.ToString());
				}
				
				// Starting position found, add Settlers
				IUnit unit = CreateUnit(UnitType.Settlers, x, y);
				unit.Owner = player;
				_units.Add(unit);

				_players[player].StartX = (short)x;
				return;
			}
		}

		private void CalculateHandicap(byte player)
		{
			// Translated drom this post by Gowron:
			// http://forums.civfanatics.com/showthread.php?t=494994
			
			// All Handicap values start from 0.
			byte handicap = 0;
			IUnit startUnit = _units.Where(u => u.Owner == player).FirstOrDefault();
			if (startUnit == null) return;
			int x = startUnit.X, y = startUnit.Y;

			ITile[] continent = Map.ContinentTiles(Map[x, y].ContinentId).ToArray();
			IUnit[] unitsOnContinent = _units.Where(u => continent.Any(c => (c.X == u.X && c.Y == u.Y))).ToArray();
			
			if (unitsOnContinent.Count() == 0)
			{
				// Add +4 if the civ does not share its land mass with any other civs.
				handicap += 4;
			}
			else if (unitsOnContinent.Select(u => Common.DistanceToTile(x, y, u.X, u.Y)).Min() >= 20)
			{
				// If that is not the case, then add +2 if the nearest civ on the same continent is 20 or more squares away.
				handicap += 2;
			}
			else if (unitsOnContinent.Select(u => Common.DistanceToTile(x, y, u.X, u.Y)).Min() >= 10)
			{
				// Add +1 instead if the nearest civ on the same continent is 10-19 squares away.
				handicap += 1;
			}

			// Check the terrain of the starting position and the 8 adjacent map squares.
			if (Map[x, y].GetBorderTiles().Count(t => (t is River)) >= 1)
			{
				// Add +2 if there's at least one river square among them.
				handicap += 2;
			}
			else if (Map[x, y].GetBorderTiles().Count(t => (t is Grassland)) >= 3)
			{
				// If that is not the case, then add +1 if there are 3 or more grassland squares among them.
				handicap += 1;
			}

			if (continent.Count() >= 200)
			{
				// Add +2 if the civ starts on a continent that covers at least 200 map squares.
				handicap += 2;
			}
			else if (continent.Count() >= 100)
			{
				// If that is not the case, then add +1 if the civ starts on a continent that covers at least 100 map squares.
				handicap += 1;
			}

			_players[player].Handicap = handicap;
		}

		private void ApplyBonus(byte player)
		{
			byte bonus = (byte)(_players.Max(p => p.Handicap) - _players[player].Handicap);
			IUnit startUnit = _units.Where(u => u.Owner == player).FirstOrDefault();
			if (startUnit == null) return;
			int x = startUnit.X, y = startUnit.Y;

			if (bonus >= 4)
			{
				// If the Bonus value of the civ is 4 or higher, then the civ is granted an extra Settlers unit, for a total of two Settlers units.
				// In this case, the Bonus value is reduced by 3 afterwards.
				IUnit unit = CreateUnit(UnitType.Settlers, x, y);
				unit.Owner = player;
				_units.Add(unit);

				bonus -= 3;
			}

			// If the Bonus value is (still) greater than zero, then the civ gains a number of technologies equal to the Bonus value.
			while (bonus > 0)
			{
				IAdvance[] available = _players[player].AvailableResearch.ToArray();
				int advanceId = Common.Random.Next(0, 72);
				for (int i = 0; i < 1000; i++)
				{
					if (!available.Any(a => a.Id == (advanceId + i) % 72)) continue;
					IAdvance advance = available.First(a => a.Id == (advanceId + i) % 72);
					SetAdvanceOrigin(advance, null);
					_players[player].AddAdvance(advance, false);
					break;
				}
				bonus--;
			}
		}
		
		public static void CreateGame(int difficulty, int competition, ICivilization tribe, string leaderName = null, string tribeName = null, string tribeNamePlural = null)
		{
			if (_instance != null)
			{
				Log("ERROR: Game instance already exists");
				return;
			}
			_instance = new Game(difficulty, competition, tribe, leaderName, tribeName, tribeNamePlural);
			
			foreach (IUnit unit in _instance._units)
			{
				unit.Explore();
			}
		}

		private Game(int difficulty, int competition, ICivilization tribe, string leaderName, string tribeName, string tribeNamePlural)
		{
			_difficulty = difficulty;
			_competition = competition;
			Log("Game instance created (difficulty: {0}, competition: {1})", _difficulty, _competition);

			Settings.Animations = true;
			Settings.CivilopediaText = true;
			Settings.Sound = true;
			Settings.EnemyMoves = true;
			
			_cities = new List<City>();
			_units = new List<IUnit>();
			_players = new Player[competition + 1];
			for (int i = 0; i <= competition; i++)
			{
				if (i == tribe.PreferredPlayerNumber)
				{
					_players[i] = new Player(tribe, leaderName, tribeName, tribeNamePlural);
					_players[i].Destroyed += PlayerDestroyed;
					HumanPlayer = _players[i];
					if (difficulty == 0)
					{
						// Chieftain starts with 50 Gold
						HumanPlayer.Gold = 50;
						Settings.InstantAdvice = true;
					}
					Log("- Player {0} is {1} of the {2} (human)", i, _players[i].LeaderName, _players[i].TribeNamePlural);
					continue;
				}
				
				ICivilization[] civs = Common.Civilizations.Where(civ => civ.PreferredPlayerNumber == i).ToArray();
				int r = Common.Random.Next(civs.Length);
				
				_players[i] = new Player(civs[r]);
				_players[i].Destroyed += PlayerDestroyed;
				
				Log("- Player {0} is {1} of the {2}", i, _players[i].LeaderName, _players[i].TribeNamePlural);
			}
			
			Log("Adding starting units...");
			for (byte i = 1; i <= competition; i++)
			{
				AddStartingUnits(i);
			}

			Log("Calculate players handicap...");
			for (byte i = 1; i <= competition; i++)
			{
				CalculateHandicap(i);
			}

			Log("Apply players bonus...");
			for (byte i = 1; i <= competition; i++)
			{
				ApplyBonus(i);
			}

			GameTurn = 0;

			// Number of turns to next antholoy needs to be checked
			_anthologyTurn = (ushort)Common.Random.Next(1, 128);
		}
	}
}