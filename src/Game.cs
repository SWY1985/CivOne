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
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.Screens;
using CivOne.Units;

namespace CivOne
{
	internal class Game
	{
		private readonly string[] _cityNames = Common.AllCityNames.ToArray();
		private readonly bool[] _cityNameUsed = new bool[256];
		private readonly int _difficulty, _competition;
		private readonly Player[] _players;
		private readonly List<City> _cities;
		private readonly List<IUnit> _units;
		
		private int _currentPlayer = 1;
		private int _activeUnit;

		public bool HasUpdate
		{
			get
			{
				return false;
			}
		}
		
		internal ushort GameTurn { get; private set; }
		
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
		
		public void NextTurn()
		{
			_activeUnit = 0;
			//
			while (++_currentPlayer >= _players.Length || _players[_currentPlayer] != HumanPlayer)
			{
				// Skip all AI players for now
				if (_currentPlayer >= _players.Length)
				{
					_currentPlayer = 0;
					GameTurn++;
				}
				_players[_currentPlayer].NewTurn();
				foreach (City city in _cities.Where(c => c.Owner == _currentPlayer).ToArray())
				{
					city.NewTurn();
				}
				foreach (IUnit unit in _units.Where(c => c.Owner == _currentPlayer))
				{
					unit.NewTurn();
				}
				IUnit[] units = _units.Where(c => c.Owner == _currentPlayer).ToArray();
				foreach (IUnit unit in units)
				{
					AI.MoveUnit(unit);
				}
			}
			foreach (City city in _cities.Where(c => c.Owner == _currentPlayer).ToArray())
			{
				city.NewTurn();
			}
			foreach (IUnit unit in _units.Where(c => c.Owner == _currentPlayer))
			{
				unit.NewTurn();
			}
			_players[_currentPlayer].NewTurn();
			if (!_cities.Any(c => c.Owner == _currentPlayer) && !_units.Any(u => u.Owner == _currentPlayer))
				Common.AddScreen(new GameOver());
			
			// Temporary code until the science code is implemented
			if (_cities.Any(c => c.Owner == _currentPlayer))
			{
				_players[_currentPlayer].Science++;
			}
			//
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

		public void FoundCity(int x, int y, string cityName = null, bool discardSettlers = true)
		{
			IUnit unit = GetUnits(x, y).FirstOrDefault();
			if (discardSettlers && ActiveUnit != null && ActiveUnit.GetType() != typeof(Settlers))
			{
				unit = ActiveUnit;
			}
			else if (discardSettlers && (ActiveUnit == null || ActiveUnit.GetType() != typeof(Settlers)))
			{
				return;
			}

			if (cityName == null)
			{
				if (discardSettlers && GetCity(x, y) != null)
				{
					if (GetCity(x, y).Size > 9)
					{
						// TODO Show message?
						return;
					}
					GetCity(x, y).Size++;
					DisbandUnit(Game.Instance.ActiveUnit);
					return;
				}

				Player player = _players[unit.Owner];
				ICivilization civilization = player.Civilization;
				int index = GetCityIndex(civilization);
				_cityNameUsed[index] = true;

				cityName = _cityNames[index];
				if (unit.Owner == PlayerNumber(HumanPlayer))
				{
					Common.AddScreen(new CityName(x, y, cityName, discardSettlers));
				}
				else
				{
					FoundCity(x, y, cityName);
				}
				return;
			}

			City city = new City()
			{
				X = (byte)x,
				Y = (byte)y,
				Owner = unit.Owner,
				Name = cityName,
				Size = 1
			};
			if (!_cities.Any(c => c.Owner == unit.Owner))
				city.AddBuilding(new Palace());
			_cities.Add(city);
			if (unit.Owner == PlayerNumber(HumanPlayer))
			{
				Common.AddScreen(new CityView(city, founded: true));
			}
			DisbandUnit(Game.Instance.ActiveUnit);
			
			if (!discardSettlers) return;
			DisbandUnit(unit);
		}

		public void FoundCity()
		{
			if (ActiveUnit == null || !(ActiveUnit is Settlers)) return;
			FoundCity(ActiveUnit.X, ActiveUnit.Y);
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
			return _units.Where(u => (!u.Moving && u.X == x && u.Y == y) || (u.Moving && u.FromX == x && u.FromY == y)).ToArray();
		}

		internal IUnit[] GetUnits()
		{
			return _units.ToArray();
		}

		public City[] GetCities()
		{
			return _cities.ToArray();
		}
		
		public void DisbandUnit(IUnit unit)
		{
			if (!_units.Contains(unit)) return;
			_units.Remove(unit);
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
					if (!Settings.Instance.EndOfTurn)
					{
						NextTurn();
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
					byte owner = Common.BinaryReadByte(br, i + 11);
					byte nameId = Common.BinaryReadByte(br, i + 22);
					string name = cityNames[nameId];
					
					if (x == 0 && y == 0 && actualSize == 0 && owner == 0 && nameId == 0) continue;
					
					City city = new City()
					{
						X = x,
						Y = y,
						Owner = owner,
						Name = name,
						Size = actualSize
					};
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
				
				for (int i = 0; i <= competition; i++)
				{
					int identity = ((civIdentity >> i) & 0x1);
					ICivilization civ = Common.Civilizations.Where(c => c.PreferredPlayerNumber == i).ToArray()[identity];
					_instance._players[i] = new Player(civ, leaderNames[i], tribeNames[i], tribeNamesPlural[i]);
					_instance._players[i].Gold = (short)Common.BinaryReadUShort(br, 312 + (i * 2));
					
					Console.WriteLine("- Player {0} is {1} of the {2}{3}", i, _instance._players[i].LeaderName, _instance._players[i].TribeNamePlural, (i == humanPlayer) ? " (human)" : "");
				}
				_instance.GameTurn = Common.BinaryReadUShort(br, 0);
				_instance.HumanPlayer = _instance._players[humanPlayer];
				
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
				int y = Common.Random.Next(0, Map.HEIGHT);
				ITile tile = Map.Instance[x, y];
				
				if (tile.IsOcean) continue; // Is it an ocean tile?
				if (tile.Hut) continue; // Is there a hut on this tile?
				if (_units.Any(u => u.X == x || u.Y == y)) continue; // Is there already a unit on this tile?
				if (tile.LandValue < (12 - (loopCounter / 32))) continue; // Is the land value high enough?
				if (_cities.Any(c => Common.DistanceToTile(x, y, c.X, c.Y) < (10 - (loopCounter / 64)))) continue; // Distance to other cities
				if (_units.Any(u => (u is Settlers) && Common.DistanceToTile(x, y, u.X, u.Y) < (10 - (loopCounter / 64)))) continue; // Distance to other settlers
				if (Map.Instance.ContinentTiles(tile.ContinentId).Count(t => Map.TileIsType(t, Terrain.Plains, Terrain.Grassland1, Terrain.Grassland2, Terrain.River)) < (32 - (GameTurn / 16))) continue; // Check buildable tiles on continent
				
				// After 0 AD, don't spawn a Civilization on a continent that already contains cities.
				if (Common.TurnToYear(GameTurn) >= 0 && Map.Instance.ContinentTiles(tile.ContinentId).Any(t => t.City != null)) continue;
				
				Console.WriteLine(loopCounter.ToString());
				
				// Starting position found, add Settlers
				IUnit unit = CreateUnit(Unit.Settlers, x, y);
				unit.Owner = player;
				_units.Add(unit);
				return;
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
			
			_cities = new List<City>();
			_units = new List<IUnit>();
			_players = new Player[competition + 1];
			for (int i = 0; i <= competition; i++)
			{
				if (i == tribe.PreferredPlayerNumber)
				{
					_players[i] = new Player(tribe, leaderName, tribeName, tribeNamePlural);
					HumanPlayer = _players[i];
					_currentPlayer = i;
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
		}
	}
}