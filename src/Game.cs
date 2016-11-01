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
using System.IO;
using System.Linq;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.Screens;
using CivOne.Tasks;
using CivOne.Templates;
using CivOne.Tiles;
using CivOne.Units;

namespace CivOne
{
	public class Game : BaseInstance
	{
		private readonly string[] _cityNames = Common.AllCityNames.ToArray();
		private readonly bool[] _cityNameUsed = new bool[Common.AllCityNames.Count()];
		private readonly int _difficulty, _competition;
		private readonly Player[] _players;
		private readonly List<City> _cities;
		private readonly List<IUnit> _units;
		
		private int _currentPlayer = 0;
		private int _activeUnit;

		private Dictionary<byte, byte> _advanceOrigin;
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

		public int Difficulty
		{
			get
			{
				return _difficulty;
			}
		}

		public bool HasUpdate
		{
			get
			{
				return false;
			}
		}
		
		private ushort _gameTurn;
		internal ushort GameTurn
		{
			get
			{
				return _gameTurn;
			}
			private set
			{
				_gameTurn = value;
				Console.WriteLine($"Turn {_gameTurn}: {GameYear}");
			}
		}
		
		internal string GameYear
		{
			get
			{
				return Common.YearString(GameTurn);
			}
		}
		
		internal Player HumanPlayer
		{
			get;
			private set;
		}
		
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

			// Enqueue AI moves
			foreach (IUnit unit in _units.Where(u => u.Owner == _currentPlayer))
			{
				GameTask.Enqueue(Turn.Move(unit));
			}
			GameTask.Enqueue(Turn.End());
		}
		
		public void Update()
		{

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
			if (!_cities.Any(c => c.Owner == city.Owner))
			{
				Palace palace = new Palace();
				palace.SetFree();
				city.AddBuilding(palace);
			}
			if ((Map[x, y] is Desert) || (Map[x, y] is Grassland) || (Map[x, y] is Hills) || (Map[x, y] is Plains) || (Map[x, y] is River))
			{
				Map[x, y].Irrigation = true;
			}
			_cities.Add(city);
			return city;
		}

		public void DestroyCity(City city)
		{
			foreach (IUnit unit in _units.Where(u => u.Home == city).ToArray())
				_units.Remove(unit);
			_cities.Remove(city);
		}
		
		internal City GetCity(int x, int y)
		{
			while (x < 0) x += Map.WIDTH;
			while (x >= Map.WIDTH) x-= Map.WIDTH;
			if (y < 0) return null;
			if (y >= Map.HEIGHT) return null;
			return _cities.Where(c => c.X == x && c.Y == y && c.Size > 0).FirstOrDefault();
		}
		
		private static IUnit CreateUnit(Unit type, int x, int y)
		{
			IUnit unit;
			switch (type)
			{
				case Unit.Settlers: unit = new Settlers(); break; 
				case Unit.Militia: unit = new Militia(); break;
				case Unit.Phalanx: unit = new Phalanx(); break;
				case Unit.Legion: unit = new Legion(); break;
				case Unit.Musketeers: unit = new Musketeers(); break;
				case Unit.Riflemen: unit = new Riflemen(); break;
				case Unit.Cavalry: unit = new Cavalry(); break;
				case Unit.Knights: unit = new Knights(); break;
				case Unit.Catapult: unit = new Catapult(); break;
				case Unit.Cannon: unit = new Cannon(); break;
				case Unit.Chariot: unit = new Chariot(); break;
				case Unit.Armor: unit = new Armor(); break;
				case Unit.MechInf: unit = new MechInf(); break;
				case Unit.Artillery: unit = new Artillery(); break;
				case Unit.Fighter: unit = new Fighter(); break;
				case Unit.Bomber: unit = new Bomber(); break;
				case Unit.Trireme: unit = new Trireme(); break;
				case Unit.Sail: unit = new Sail(); break;
				case Unit.Frigate: unit = new Frigate(); break;
				case Unit.Ironclad: unit = new Ironclad(); break;
				case Unit.Cruiser: unit = new Cruiser(); break;
				case Unit.Battleship: unit = new Battleship(); break;
				case Unit.Submarine: unit = new Submarine(); break;
				case Unit.Carrier: unit = new Carrier(); break;
				case Unit.Transport: unit = new Transport(); break;
				case Unit.Nuclear: unit = new Nuclear(); break;
				case Unit.Diplomat: unit = new Diplomat(); break;
				case Unit.Caravan: unit = new Caravan(); break;
				default: return null;
			}
			unit.X = x;
			unit.Y = y;
			return unit;
		}

		public IUnit CreateUnit(Unit type, int x, int y, byte owner, bool endTurn = false)
		{
			IUnit unit = CreateUnit((Unit)type, x, y);
			if (unit == null) return null;

			unit.Owner = owner;
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
			return _units.Where(u => u.X == x && u.Y == y).ToArray();
		}

		internal IUnit[] GetUnits()
		{
			return _units.ToArray();
		}

		internal void MovementDone()
		{
			foreach (City city in _cities)
			{
				foreach (ITile tile in city.ResourceTiles.Where(t => city.ValidTile(t)))
				{
					city.RelocateResourceTile(tile);
				} 
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
		
		public void DisbandUnit(IUnit unit)
		{
			if (!_units.Contains(unit)) return;
			if (unit.Tile is Ocean && unit is IBoardable)
			{
				int totalCargo = unit.Tile.Units.Where(u => u is IBoardable).Sum(u => (u as IBoardable).Cargo) - (unit as IBoardable).Cargo;
				while (unit.Tile.Units.Count(u => u.Class != UnitClass.Water) > totalCargo)
				{
					_units.Remove(unit.Tile.Units.First(u => u.Class != UnitClass.Water));
				} 
			}
			_units.Remove(unit);
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
				Console.WriteLine("ERROR: Game instance already exists");
				return;
			}
			_instance = new Game(difficulty, competition, tribe, leaderName, tribeName, tribeNamePlural);
			
			foreach (IUnit unit in _instance._units)
			{
				unit.Explore();
			}
		}
		
		public static void LoadGame(string sveFile, string mapFile)
		{
			if (_instance != null)
			{
				Console.WriteLine("ERROR: Game instance already exists");
				return;
			}
			
			// TODO: Implement full save file configuration
			// - http://forums.civfanatics.com/showthread.php?p=12422448
			// - http://forums.civfanatics.com/showthread.php?t=493581
			using (BinaryReader br = new BinaryReader(File.Open(sveFile, FileMode.Open)))
			{
				ushort humanPlayer = Common.BinaryReadUShort(br, 2);
				ushort randomSeed = Common.BinaryReadUShort(br, 6);
				ushort difficulty = Common.BinaryReadUShort(br, 10);
				string[] leaderNames = Common.BinaryReadStrings(br, 16, 112, 14);
				string[] tribeNamesPlural = Common.BinaryReadStrings(br, 128, 96, 12);
				string[] tribeNames = Common.BinaryReadStrings(br, 224, 88, 11);
				string[] cityNames = Common.BinaryReadStrings(br, 26992, 3328, 13);
				ushort[] unitCount = new ushort[8];
				for (int i = 0; i < 8; i++)
					unitCount[i] = Common.BinaryReadUShort(br, 1752 + (i * 2));
				List<City> cities = new List<City>();
				for (int i = 5384; i < 8968; i+= 28)
				{
					byte x = Common.BinaryReadByte(br, i + 4);
					byte y = Common.BinaryReadByte(br, i + 5);
					byte actualSize = Common.BinaryReadByte(br, i + 7);
					byte currentProduction = Common.BinaryReadByte(br, i + 9);
					byte owner = Common.BinaryReadByte(br, i + 11);
					byte nameId = Common.BinaryReadByte(br, i + 22);
					string name = cityNames[nameId];
					
					if (x == 0 && y == 0 && actualSize == 0 && owner == 0 && nameId == 0) continue;

					//TODO: For now, don't load destroyed cities
					if (actualSize == 0) continue;
					
					City city = new City(owner)
					{
						X = x,
						Y = y,
						Name = name,
						Size = actualSize
					};
					city.SetProduction(currentProduction);
					cities.Add(city);
				}
				List<IUnit> units = new List<IUnit>();
				for (int i = 9920; i < 22208; i+= 12)
				{
					int unitNo = ((i - 9920) / 12) % 128;
					int civ = (((i - 9920) / 12) - unitNo) / 128;
					//if ((unitNo + 1) > unitCount[civ]) continue;
					
					byte status = Common.BinaryReadByte(br, i);
					byte x = Common.BinaryReadByte(br, i + 1);
					byte y = Common.BinaryReadByte(br, i + 2);
					byte type = Common.BinaryReadByte(br, i + 3);
					
					IUnit unit = CreateUnit((Unit)type, x, y);
					if (unit == null) continue;

					unit.Status = status;
					unit.Owner = (byte)civ;
					units.Add(unit);
				}
				ushort competition = (ushort)(Common.BinaryReadUShort(br, 37820) + 1);
				ushort civIdentity = Common.BinaryReadUShort(br, 37854);
				
				Map.Instance.LoadMap(mapFile, randomSeed);
				_instance = new Game(difficulty, competition);
				Console.WriteLine("Game instance loaded (difficulty: {0}, competition: {1})", difficulty, competition);
				
				// Load map visibility
				byte[] visibility = Common.BinaryReadBytes(br, 22208, 4000);

				for (int i = 0; i <= competition; i++)
				{
					int identity = ((civIdentity >> i) & 0x1);
					ICivilization civ = Common.Civilizations.Where(c => c.PreferredPlayerNumber == i).ToArray()[identity];
					Player player = (_instance._players[i] = new Player(civ, leaderNames[i], tribeNames[i], tribeNamesPlural[i]));
					player.Gold = (short)Common.BinaryReadUShort(br, 312 + (i * 2));
					player.Science = (short)Common.BinaryReadUShort(br, 328 + (i * 2));
					
					// Set map visibility
					for (int xx = 0; xx < 80; xx++)
					for (int yy = 0; yy < 50; yy++)
					{
						byte tile = visibility[(50 * xx) + yy];
						if ((tile & (1 << i)) == 0) continue;
						player.Explore(xx, yy, 0);
					}

					// Set civilization advances
					for (int t = 0; t < 5; t++)
					{
						int offset = 1256 + (i * 10) + (t * 2);
						ushort techFlag = Common.BinaryReadUShort(br, offset);
						for (int b = 0; b < 16; b++)
						{
							if ((techFlag & (1 << b)) == 0) continue;
							IAdvance advance = Common.Advances.FirstOrDefault(a => a.Id == (16 * t) + b);
							if (advance == null) continue;
							player.AddAdvance(advance);
						}
					}
					
					Console.WriteLine("- Player {0} is {1} of the {2}{3}", i, player.LeaderName, _instance._players[i].TribeNamePlural, (i == humanPlayer) ? " (human)" : "");
				}
				_instance.GameTurn = Common.BinaryReadUShort(br, 0);
				_instance.HumanPlayer = _instance._players[humanPlayer];
				_instance.HumanPlayer.CurrentResearch = Common.Advances.FirstOrDefault(a => a.Id == Common.BinaryReadUShort(br, 14));
				
				foreach (City city in cities)
				{
					_instance._cities.Add(city);
				}
				foreach (IUnit unit in units)
				{
					_instance._units.Add(unit);
				}
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
					
					Console.WriteLine(loopCounter.ToString());
				}
				
				// Starting position found, add Settlers
				IUnit unit = CreateUnit(Unit.Settlers, x, y);
				unit.Owner = player;
				_units.Add(unit);
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
			else if (continent.Count() >= 200)
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
				IUnit unit = CreateUnit(Unit.Settlers, x, y);
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
		
		private static Game _instance;
		public static Game Instance
		{
			get
			{
				if (_instance == null)
				{
					Console.WriteLine("ERROR: Game instance does not exist");
				}
				return _instance;
			}
		}
		
		private Game(int difficulty, int competition)
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
			Console.WriteLine("Game instance created (difficulty: {0}, competition: {1})", _difficulty, _competition);

			Settings.Animations = true;
			Settings.CivilopediaText = true;
			
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
					Console.WriteLine("- Player {0} is {1} of the {2} (human)", i, _players[i].LeaderName, _players[i].TribeNamePlural);
					continue;
				}
				
				ICivilization[] civs = Common.Civilizations.Where(civ => civ.PreferredPlayerNumber == i).ToArray();
				int r = Common.Random.Next(civs.Length);
				
				_players[i] = new Player(civs[r]);
				
				Console.WriteLine("- Player {0} is {1} of the {2}", i, _players[i].LeaderName, _players[i].TribeNamePlural);
			}
			
			Console.WriteLine("Adding starting units...");
			for (byte i = 1; i <= competition; i++)
			{
				AddStartingUnits(i);
			}

			Console.WriteLine("Calculate players handicap...");
			for (byte i = 1; i <= competition; i++)
			{
				CalculateHandicap(i);
			}

			Console.WriteLine("Apply players bonus...");
			for (byte i = 1; i <= competition; i++)
			{
				ApplyBonus(i);
			}
			
			GameTurn = 0;
		}
	}
}