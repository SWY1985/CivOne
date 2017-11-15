// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CivOne.Advances;
using CivOne.Buildings;
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Screens;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.Wonders;

namespace CivOne
{
	public class Game : BaseInstance
	{
	    internal readonly string[] _cityNames = Common.AllCityNames.ToArray();
	    internal readonly bool[] _cityNameUsed = new bool[Common.AllCityNames.Count()];
		private readonly int _difficulty, _competition;
	    internal readonly Player[] _players;
	    internal readonly List<City> _cities;
	    internal readonly List<IUnit> _units;

	    internal int _currentPlayer = 0;
	    internal int _activeUnit;

	    internal ushort _anthologyTurn = 0;

	    internal Dictionary<byte, byte> _advanceOrigin;
		public void SetAdvanceOrigin(IAdvance advance, Player player)
		{
			if (_advanceOrigin == null)
				_advanceOrigin = new Dictionary<byte, byte>();
			if (_advanceOrigin.ContainsKey(advance.Id))
				return;
			byte playerNumber = 0;
			if (player != null)
				playerNumber = PlayerNumber(player);
			_advanceOrigin.Add(advance.Id, playerNumber);
		}
		public bool GetAdvanceOrigin(IAdvance advance, Player player)
		{
			if (_advanceOrigin == null)
				_advanceOrigin = new Dictionary<byte, byte>();
			if (_advanceOrigin.ContainsKey(advance.Id))
				return (_advanceOrigin[advance.Id] == PlayerNumber(player));
			return false;
		}

		public int Difficulty => _difficulty;

		public bool HasUpdate => false;
		
		private ushort _gameTurn;
		internal ushort GameTurn
		{
			get
			{
				return _gameTurn;
			}
			set
			{
				_gameTurn = value;
				Log($"Turn {_gameTurn}: {GameYear}");
				if (_anthologyTurn >= _gameTurn)
				{
					//TODO: Show anthology
					_anthologyTurn = (ushort)(_gameTurn + 20 + Common.Random.Next(40));
				}
			}
		}
		
		internal string GameYear
		{
			get
			{
				return Common.YearString(GameTurn);
			}
		}
		
		internal Player HumanPlayer { get; set; }
		
		internal Player CurrentPlayer
		{
			get
			{
				return _players[_currentPlayer];
			}
		}
		
		internal byte PlayerNumber(Player player)
		{
			byte i = 0;
			foreach (Player p in _players)
			{
				if (p == player)
					return i;
				i++;
			}
			return 0;
		}

		internal Player GetPlayer(byte number)
		{
			if (_players.Length < number)
				return null;
			return _players[number];
		}

		internal IEnumerable<Player> Players
		{
			get
			{
				foreach (Player player in _players)
					yield return player;
			}
		}

		public void EndTurn()
		{
			if (++_currentPlayer >= _players.Length)
			{
				_currentPlayer = 0;
				GameTurn++;
				if (GameTurn % 50 == 0 && Settings.AutoSave)
				{
					GameTask.Enqueue(Show.AutoSave);
				}
			}

			if (CurrentPlayer.DestroyTurn == -1 && CurrentPlayer.IsDestroyed)
			{
				GameTask.Enqueue(Message.Advisor(Advisor.Defense, false, CurrentPlayer.Civilization.Name, "civilization", "destroyed", $"by {Game.GetPlayer(0).Civilization.NamePlural}!"));
			}

			if (!_players.Any(x => Game.PlayerNumber(x) != 0 && x != Human && !x.IsDestroyed))
			{
				GameTask conquest;
				GameTask.Enqueue(Message.Newspaper(null, "Your civilization", "has conquered", "the entire planet!"));
				GameTask.Enqueue(conquest = Show.Screen<Conquest>());
				conquest.Done += (s, a) => Runtime.Quit();
			}

			foreach (IUnit unit in _units.Where(u => u.Owner == _currentPlayer))
			{
				GameTask.Enqueue(Turn.New(unit));
			}
			foreach (City city in _cities.Where(c => c.Owner == _currentPlayer).ToArray())
			{
				GameTask.Enqueue(Turn.New(city));
			}
			GameTask.Enqueue(Turn.New(CurrentPlayer));

			if (CurrentPlayer == HumanPlayer) return;
		}
		
		public void Update()
		{
			IUnit unit = ActiveUnit;
			if (CurrentPlayer == HumanPlayer)
			{
				if (unit != null && !unit.Goto.IsEmpty)
				{
					int distance = unit.Tile.DistanceTo(unit.Goto);
					ITile[] tiles = (unit as BaseUnit).MoveTargets.OrderBy(x => x.DistanceTo(unit.Goto)).ThenBy(x => x.Movement).ToArray();
					if (tiles.Length == 0 || tiles[0].DistanceTo(unit.Goto) > distance)
					{
						// No valid tile to move to, cancel goto
						unit.Goto = Point.Empty;
						return;
					}
					else if (tiles[0].DistanceTo(unit.Goto) == distance)
					{
						// Distance is unchanged, 50% chance to cancel goto
						if (Common.Random.Next(0, 100) < 50)
						{
							unit.Goto = Point.Empty;
							return;
						}
					}

					unit.MoveTo(tiles[0].X - unit.X, tiles[0].Y - unit.Y);
					return;
				}
				return;
			}
			if (unit != null && (unit.MovesLeft > 0 || unit.PartMoves > 0))
			{
				GameTask.Enqueue(Turn.Move(unit));
				return;
			}
			GameTask.Enqueue(Turn.End());
		}

		private int GetCityIndex(ICivilization civilization)
		{
			// TODO: This should be a lot easier... let me think about it...

			int indexFrom = Array.IndexOf(_cityNames, civilization.CityNames[0]); //_cityNames.IndexOf(civilization.CityNames[0]);
			int indexTo = civilization.CityNames.Length + indexFrom;
			for (int i = indexFrom; i < indexTo; i++)
			{
				if (_cityNameUsed[i]) continue;
				return i;
			}
			
			civilization = _players[0].Civilization;
			indexFrom = Array.IndexOf(_cityNames, civilization.CityNames[0]);
			indexTo = civilization.CityNames.Length + indexFrom;
			for (int i = indexFrom; i < indexTo; i++)
			{
				if (_cityNameUsed[i]) continue;
				return i;
			}

			for (int i = 0; i < _cityNames.Length; i++)
			{
				if (_cityNameUsed[i]) continue;
				return i;
			}

			return 0;
		}

		internal string CityName(Player player)
		{
			ICivilization civilization = player.Civilization;
			int index = GetCityIndex(civilization);
			_cityNameUsed[index] = true;
			return _cityNames[index];
		}

		internal City AddCity(Player player, string name, int x, int y)
		{
			if (_cities.Any(c => c.X == x && c.Y == y))
				return null;

			City city = new City(PlayerNumber(player))
			{
				X = (byte)x,
				Y = (byte)y,
				Name = name,
				Size = 1
			};
			if (!_cities.Any(c => c.Size > 0 && c.Owner == city.Owner))
			{
				Palace palace = new Palace();
				palace.SetFree();
				city.AddBuilding(palace);
			}
			if ((Map[x, y] is Desert) || (Map[x, y] is Grassland) || (Map[x, y] is Hills) || (Map[x, y] is Plains) || (Map[x, y] is River))
			{
				Map[x, y].Irrigation = true;
			}
			if (!Map[x, y].RailRoad)
			{
				Map[x, y].Road = true;
			}
			_cities.Add(city);
			Game.UpdateResources(city.Tile);
			return city;
		}

		public void DestroyCity(City city)
		{
			foreach (IUnit unit in _units.Where(u => u.Home == city).ToArray())
				_units.Remove(unit);
			city.X = 255;
			city.Y = 255;
			city.Owner = 0;
		}
		
		internal City GetCity(int x, int y)
		{
			while (x < 0) x += Map.WIDTH;
			while (x >= Map.WIDTH) x-= Map.WIDTH;
			if (y < 0) return null;
			if (y >= Map.HEIGHT) return null;
			return _cities.Where(c => c.X == x && c.Y == y && c.Size > 0).FirstOrDefault();
		}

		public IUnit CreateUnit(UnitType type, int x, int y, byte owner, bool endTurn = false)
		{
			IUnit unit = UnitsFactory.CreateUnit((UnitType)type, x, y);
			if (unit == null) return null;

			unit.Owner = owner;
			if (unit.Class == UnitClass.Water)
			{
				Player player = GetPlayer(owner);
				if ((player.HasWonder<Lighthouse>() && !WonderObsolete<Lighthouse>()) ||
					(player.HasWonder<MagellansExpedition>() && !WonderObsolete<MagellansExpedition>()))
				{
					unit.MovesLeft++;
				}
			}
			if (endTurn)
				unit.SkipTurn();
			_instance._units.Add(unit);
			return unit;
		}
		
		internal IUnit[] GetUnits(int x, int y)
		{
			while (x < 0) x += Map.WIDTH;
			while (x >= Map.WIDTH) x-= Map.WIDTH;
			if (y < 0) return null;
			if (y >= Map.HEIGHT) return null; 
			return _units.Where(u => u.X == x && u.Y == y).OrderBy(u => (u == ActiveUnit) ? 0 : (u.Fortify || u.FortifyActive ? 1 : 2)).ToArray();
		}

		internal IUnit[] GetUnits()
		{
			return _units.ToArray();
		}

		internal void UpdateResources(ITile tile, bool ownerCities = true)
		{
			for (int relY = -3; relY <= 3; relY++)
			for (int relX = -3; relX <= 3; relX++)
			{
				if (tile[relX, relY] == null) continue;
				City city = tile[relX, relY].City;
				if (city == null) continue;
				if (!ownerCities && CurrentPlayer == city.Owner) continue;
				city.UpdateResources();
			}
		}

		public City[] GetCities()
		{
			return _cities.ToArray();
		}

		public IWonder[] BuiltWonders
		{
			get
			{
				return _cities.SelectMany(c => c.Wonders).ToArray();
			}
		}

		public bool WonderBuilt<T>() where T : IWonder
		{
			return BuiltWonders.Any(w => w is T);
		}

		public bool WonderBuilt(IWonder wonder)
		{
			return BuiltWonders.Any(w => w.Id == wonder.Id);
		}

		public bool WonderObsolete<T>() where T : IWonder, new()
		{
			return WonderObsolete(new T());
		}

		public bool WonderObsolete(IWonder wonder)
		{
			return (wonder.ObsoleteTech != null && _players.Any(x => x.HasAdvance(wonder.ObsoleteTech)));
		}
		
		public void DisbandUnit(IUnit unit)
		{
			IUnit activeUnit = ActiveUnit;

			if (unit == null) return;
			if (!_units.Contains(unit)) return;
			if (unit.Tile is Ocean && unit is IBoardable)
			{
				int totalCargo = unit.Tile.Units.Where(u => u is IBoardable).Sum(u => (u as IBoardable).Cargo) - (unit as IBoardable).Cargo;
				while (unit.Tile.Units.Count(u => u.Class != UnitClass.Water) > totalCargo)
				{
					IUnit subUnit = unit.Tile.Units.First(u => u.Class != UnitClass.Water);
					subUnit.X = 255;
					subUnit.Y = 255;
					_units.Remove(subUnit);
				} 
			}
			unit.X = 255;
			unit.Y = 255;
			_units.Remove(unit);

			if (_units.Contains(activeUnit))
			{
				_activeUnit = _units.IndexOf(activeUnit);
			}
		}

		public void UnitWait()
		{
			_activeUnit++;
		}
		
		public IUnit ActiveUnit
		{
			get
			{
				if (_units.Count(u => u.Owner == _currentPlayer && !u.Busy) == 0)
					return null;
				
				// If the unit counter is too high, return to 0
				if (_activeUnit >= _units.Count)
					_activeUnit = 0;
					
				// Does the current unit still have moves left?
				if (_units[_activeUnit].Owner == _currentPlayer && (_units[_activeUnit].MovesLeft > 0 || _units[_activeUnit].PartMoves > 0) && !_units[_activeUnit].Sentry && !_units[_activeUnit].Fortify)
					return _units[_activeUnit];

				// Task busy, don't change the active unit
				if (GameTask.Any())
					return _units[_activeUnit];
				
				// Check if any units are still available for this player
				if (!_units.Any(u => u.Owner == _currentPlayer && (u.MovesLeft > 0 || u.PartMoves > 0) && !u.Busy))
				{
					if (CurrentPlayer == HumanPlayer && !Settings.Instance.EndOfTurn && !GameTask.Any() && (Common.TopScreen is GamePlay))
					{
						GameTask.Enqueue(Turn.End());
					}
					return null;
				}
				
				// Loop through units
				while (_units[_activeUnit].Owner != _currentPlayer || (_units[_activeUnit].MovesLeft == 0 && _units[_activeUnit].PartMoves == 0) || (_units[_activeUnit].Sentry || _units[_activeUnit].Fortify))
				{
					_activeUnit++;
					if (_activeUnit >= _units.Count)
						_activeUnit = 0;
				}
				return _units[_activeUnit];
			}
			internal set
			{
				if (value == null || value.MovesLeft == 0 && value.PartMoves == 0)
					return;
				value.Sentry = false;
				value.Fortify = false;
				_activeUnit = _units.IndexOf(value);
			}
		}

		public IUnit MovingUnit
		{
			get
			{
				return _units.FirstOrDefault(u => u.Moving);
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
				IUnit unit = UnitsFactory.CreateUnit(UnitType.Settlers, x, y);
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
				IUnit unit = UnitsFactory.CreateUnit(UnitType.Settlers, x, y);
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

		public static bool Started
		{
			get
			{
				return (_instance != null);
			}
		}

	    internal static Game _instance;
		public static Game Instance
		{
			get
			{
				if (_instance == null)
				{
					Log("ERROR: Game instance does not exist");
				}
				return _instance;
			}
		}
		
		internal Game(int difficulty, int competition)
		{
			_difficulty = difficulty;
			_competition = competition;
			_players = new Player[competition + 1];
			_cities = new List<City>();
			_units = new List<IUnit>();
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
