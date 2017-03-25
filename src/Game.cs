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
using CivOne.Wonders;

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

		private ushort _anthologyTurn = 0;

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
			set
			{
				_gameTurn = value;
				Console.WriteLine($"Turn {_gameTurn}: {GameYear}");
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
				conquest.Done += (s, a) => Common.Quit();
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
				if (unit != null && unit.GotoX != -1 && unit.GotoY != -1)
				{
					int distance = unit.Tile.DistanceTo(unit.GotoX, unit.GotoY);
					ITile[] tiles = (unit as BaseUnit).MoveTargets.OrderBy(x => x.DistanceTo(unit.GotoX, unit.GotoY)).ThenBy(x => x.Movement).ToArray();
					if (tiles.Length == 0 || tiles[0].DistanceTo(unit.GotoX, unit.GotoY) > distance)
					{
						// No valid tile to move to, cancel goto
						unit.GotoX = -1;
						unit.GotoY = -1;
						return;
					}
					else if (tiles[0].DistanceTo(unit.GotoX, unit.GotoY) == distance)
					{
						// Distance is unchanged, 50% chance to cancel goto
						if (Common.Random.Next(0, 100) < 50)
						{
							unit.GotoX = -1;
							unit.GotoY = -1;
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
			return _units.Where(u => u.X == x && u.Y == y).ToArray();
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
				
				Map.Instance.LoadMap(mapFile, randomSeed);

				string[] leaderNames = Common.BinaryReadStrings(br, 16, 112, 14);
				string[] tribeNamesPlural = Common.BinaryReadStrings(br, 128, 96, 12);
				string[] tribeNames = Common.BinaryReadStrings(br, 224, 88, 11);
				string[] cityNames = Common.BinaryReadStrings(br, 26992, 3328, 13);
				ushort[] unitCount = new ushort[8];
				for (int i = 0; i < 8; i++)
					unitCount[i] = Common.BinaryReadUShort(br, 1752 + (i * 2));
				ushort[] wonderList = new ushort[21];
				for (int i = 1; i <= 21; i++)
					wonderList[i - 1] = Common.BinaryReadUShort(br, 34418 + (i * 2));
				List<City> cities = new List<City>();

				Dictionary<byte, City> cityList = new Dictionary<byte, City>();
				byte cityId = 255;
				for (int i = 5384; i < 8968; i+= 28)
				{
					cityId++;
					byte[] buildings = Common.BinaryReadBytes(br, i, 4);
					byte x = Common.BinaryReadByte(br, i + 4);
					byte y = Common.BinaryReadByte(br, i + 5);
					byte actualSize = Common.BinaryReadByte(br, i + 7);
					byte currentProduction = Common.BinaryReadByte(br, i + 9);
					byte owner = Common.BinaryReadByte(br, i + 11);
					ushort food = Common.BinaryReadUShort(br, i + 12);
					ushort shields = Common.BinaryReadUShort(br, i + 14);
					byte[] resourceTiles = Common.BinaryReadBytes(br, i + 16, 6);
					byte nameId = Common.BinaryReadByte(br, i + 22);
					string name = cityNames[nameId];
					
					if (x == 0 && y == 0 && actualSize == 0 && owner == 0 && nameId == 0) continue;
					
					City city = new City(owner)
					{
						X = x,
						Y = y,
						Name = name,
						Size = actualSize,
						Food = food,
						Shields = shields
					};
					city.SetProduction(currentProduction);
					city.SetResourceTiles(resourceTiles);

					// Set city buildings
					for (int j = 0; j < 32; j++)
					{
						if (!Common.Buildings.Any(b => b.Id == j)) continue;
						int bit = (j % 8);
						int index = (j - bit) / 8;
						if (((buildings[index] >> bit) & 1) == 0) continue;
						city.AddBuilding(Common.Buildings.First(b => b.Id == j));
					}

					// Set city wonders
					foreach (IWonder wonder in Common.Wonders)
					{
						if (wonderList[wonder.Id - 1] != cityId) continue;
						city.AddWonder(wonder);
					}
					
					cities.Add(city);
					cityList.Add(cityId, city);
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
					byte moves = Common.BinaryReadByte(br, i + 4);
					byte homeCity = Common.BinaryReadByte(br, i + 11);
					
					IUnit unit = CreateUnit((Unit)type, x, y);
					if (unit == null) continue;

					unit.Status = status;
					unit.Owner = (byte)civ;
					unit.PartMoves = (byte)(moves % 3);
					unit.MovesLeft = (byte)((moves - unit.PartMoves) / 3);
					if (cityList.ContainsKey(homeCity))
					{
						unit.SetHome(cityList[homeCity]);
					}
					units.Add(unit);
				}

				// Game Settings
				ushort settings = Common.BinaryReadUShort(br, 35492);
				Settings.InstantAdvice = (settings & (0x01 << 0)) > 0;
				Settings.AutoSave = (settings & (0x01 << 1)) > 0;
				Settings.EndOfTurn = (settings & (0x01 << 2)) > 0;
				Settings.Animations = (settings & (0x01 << 3)) > 0;
				Settings.Sound = (settings & (0x01 << 4)) > 0;
				// Settings.EnemyMoves = (settings & (0x01 << 5)) > 0;
				Settings.CivilopediaText = (settings & (0x01 << 6)) > 0;
				// Settings.Palace = (settings & (0x01 << 7)) > 0;
				
				ushort anthologyTurn = Common.BinaryReadUShort(br, 35778);

				ushort competition = (ushort)(Common.BinaryReadUShort(br, 37820) + 1);
				ushort civIdentity = Common.BinaryReadUShort(br, 37854);
				
				_instance = new Game(difficulty, competition);
				Console.WriteLine("Game instance loaded (difficulty: {0}, competition: {1})", difficulty, competition);
				
				// Load map visibility
				byte[] visibility = Common.BinaryReadBytes(br, 22208, 4000);

				for (int i = 0; i <= competition; i++)
				{
					int identity = ((civIdentity >> i) & 0x1);
					ICivilization[] civs = Common.Civilizations.Where(c => c.PreferredPlayerNumber == i).ToArray();
					ICivilization civ = civs[identity];
					Player player = (_instance._players[i] = new Player(civ, leaderNames[i], tribeNames[i], tribeNamesPlural[i]));
					player.Gold = (short)Common.BinaryReadUShort(br, 312 + (i * 2));
					player.Science = (short)Common.BinaryReadUShort(br, 328 + (i * 2));
					player.Government = Reflect.GetGovernments().FirstOrDefault(x => x.Id == Common.BinaryReadUShort(br, 1336 + (i * 2)));

					player.TaxesRate = (short)Common.BinaryReadUShort(br, 1848 + (i * 2));
					player.LuxuriesRate = 10 - (short)Common.BinaryReadUShort(br, 35760 + (i * 2)) - player.TaxesRate;
					
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
							player.AddAdvance(advance, false);
							
							int originId = Common.BinaryReadUShort(br, 26720 + (advance.Id * 2));
							if (originId == player.Civilization.Id)
								_instance.SetAdvanceOrigin(advance, player);
						}
					}
					
					Console.WriteLine("- Player {0} is {1} of the {2}{3}", i, player.LeaderName, _instance._players[i].TribeNamePlural, (i == humanPlayer) ? " (human)" : "");
				}
				_instance.GameTurn = Common.BinaryReadUShort(br, 0);
				_instance.HumanPlayer = _instance._players[humanPlayer];
				_instance.HumanPlayer.CurrentResearch = Common.Advances.FirstOrDefault(a => a.Id == Common.BinaryReadUShort(br, 14));
				
				_instance._anthologyTurn = anthologyTurn;
				
				foreach (City city in cities)
				{
					_instance._cities.Add(city);
				}
				foreach (IUnit unit in units)
				{
					_instance._units.Add(unit);
				}

				for (int i = 0; i < _instance._cityNames.Length; i++)
				{
					if (!cities.Any(x => x.Name == _instance._cityNames[i])) continue;
					_instance._cityNameUsed[i] = true;
				}

				_instance._currentPlayer = humanPlayer;
				for (int i = 0; i < _instance._units.Count(); i++)
				{
					if (_instance._units[i].Owner != humanPlayer) continue;
					_instance._activeUnit = i;
					if (_instance._units[i].MovesLeft > 0) break;
				}
			}
		}

		public void Save(string sveFile, string mapFile)
		{
			// TODO: Implement full save file configuration
			// - http://forums.civfanatics.com/showthread.php?p=12422448
			// - http://forums.civfanatics.com/showthread.php?t=493581
			using (FileStream fs = new FileStream(sveFile, FileMode.Create, FileAccess.Write))
			using (BinaryWriter bw = new BinaryWriter(fs))
			{
				ushort randomSeed = Map.Instance.SaveMap(mapFile);
				ushort activeCivilizations = 1;
				for (int i = 1; i < _players.Length; i++)
					if (_players[i].Cities.Any() || GetUnits().Any(x => x.Owner == i))
						activeCivilizations |= (ushort)(0x01 << i);

				bw.Write(GameTurn);
				bw.Write((ushort)PlayerNumber(HumanPlayer));
				bw.Write((ushort)(0x01 << PlayerNumber(HumanPlayer)));
				bw.Write(randomSeed);
				bw.Write((short)Common.TurnToYear(GameTurn));
				bw.Write((ushort)Difficulty);
				bw.Write(activeCivilizations);
				if (HumanPlayer.CurrentResearch == null)
					bw.Write((ushort)0x00);
				else
					bw.Write((ushort)HumanPlayer.CurrentResearch.Id);

				// Leader names
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						for (int x = 0; x < 14; x++)
						{
							bw.Write((byte)0x00);
						}
						continue;
					}
					bw.Write(_players[i].LeaderName.PadRight(14, (char)0x00).Select(x => (byte)x).ToArray());
				}
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						for (int x = 0; x < 12; x++)
						{
							bw.Write((byte)0x00);
						}
						continue;
					}
					bw.Write(_players[i].Civilization.NamePlural.PadRight(12, (char)0x00).Select(x => (byte)x).ToArray());
				}
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						for (int x = 0; x < 11; x++)
						{
							bw.Write((byte)0x00);
						}
						continue;
					}
					bw.Write(_players[i].Civilization.Name.PadRight(11, (char)0x00).Select(x => (byte)x).ToArray());
				}

				// Player gold
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write((short)0);
						continue;
					}
					bw.Write(_players[i].Gold);
				}

				// Research progress
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write((short)0);
						continue;
					}
					bw.Write(_players[i].Science);
				}

				// Units active
				for (int i = 0; i < 8; i++)
				for (byte unitId = 0; unitId < 28; unitId++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write((short)0);
						continue;
					}
					bw.Write((short)GetUnits().Where(x => x.Owner == i).Count(x =>
						x.ProductionId == unitId));
				}

				// Units currently in production
				for (int i = 0; i < 8; i++)
				for (byte unitId = 0; unitId < 28; unitId++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write((short)0);
						continue;
					}
					bw.Write((short)_players[i].Cities.Count(x =>
						x.CurrentProduction is IUnit &&
						(x.CurrentProduction as IUnit).ProductionId == unitId));
				}

				// Discovered Advances Count
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write((short)0);
						continue;
					}
					bw.Write((short)_players[i].Advances.Count());
				}

				// Set civilization advances
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
						continue;
					}
					for (int techGroup = 0; techGroup < 5; techGroup++)
					{
						ushort techFlag = 0;
						foreach (IAdvance advance in _players[i].Advances.Where(x => ((x.Id - (x.Id % 16)) / 16) == techGroup))
						{
							techFlag |= (ushort)(0x01 << (advance.Id % 16));
						}
						bw.Write(techFlag);
					}
				}

				// Civilization Governments
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write((short)0);
						continue;
					}
					bw.Write((short)_players[i].Government.Id);
				}

				// TODO: Civ AI strategy
				for (int i = 0; i < 256; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Diplomacy
				for (int i = 0; i < 128; i++)
				{
					bw.Write((byte)0);
				}

				// City counts
				for (int i = 0; i < 8; i++)
				{
					bw.Write((short)GetCities().Count(x => x.Owner == i));
				}

				// Unit counts
				for (int i = 0; i < 8; i++)
				{
					bw.Write((short)GetUnits().Count(x => x.Owner == i));
				}

				// TODO: Land counts
				for (int i = 0; i < 8; i++)
				{
					bw.Write((short)0);
				}

				// Settler counts
				for (int i = 0; i < 8; i++)
				{
					bw.Write((short)(GetUnits().Count(x => (x is Settlers) && x.Owner == i) + 1));
				}

				// Total Civ size
				for (int i = 0; i < 8; i++)
				{
					bw.Write((short)GetCities().Where(x => x.Owner == i).Sum(x => x.Size));
				}

				// Military power
				for (int i = 0; i < 8; i++)
				{
					bw.Write((short)GetUnits().Where(x => x.Owner == i).Sum(x => x.Attack + x.Defense));
				}

				// TODO: Civ Rankings
				for (int i = 0; i < 8; i++)
				{
					bw.Write((short)i);
				}

				// Tax rate
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write((short)0);
						continue;
					}
					bw.Write((short)_players[i].TaxesRate);
				}

				// TODO: Civ score
				for (int i = 0; i < 8; i++)
				{
					bw.Write((short)i);
				}

				// TODO: Human contact turn counter
				for (int i = 0; i < 8; i++)
				{
					bw.Write((short)0);
				}

				// Starting position X coordinate
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write((short)0xFF);
					}
					bw.Write((short)_players[i].StartX);
				}

				// Leader graphics
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write((short)0);
						continue;
					}
					bw.Write((short)_players[i].Civilization.Id);
				}

				// TODO: Per-continent Civ defense
				for (int i = 0; i < 256; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Per-continent Civ attack
				for (int i = 0; i < 256; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Per-continent Civ city count
				for (int i = 0; i < 256; i++)
				{
					bw.Write((byte)0);
				}

				// Continent sizes
				for (int i = 0; i < 16; i++)
				{
					bw.Write((short)Map.AllTiles().Count(x => !x.IsOcean && x.ContinentId == i));
				}
				for (int i = 0; i < 96; i++)
				{
					// Fill remaining bytes
					bw.Write((byte)0);
				}

				// TODO: Oceans sizes
				for (int i = 0; i < 128; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Continent building site counts
				for (int i = 0; i < 32; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Score chart data
				for (int i = 0; i < 1200; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Peace chart data
				for (int i = 0; i < 1200; i++)
				{
					bw.Write((byte)0);
				}

				// Cities
				Dictionary<byte, City> cityList = new Dictionary<byte, City>();
				for (int i = 0; i < 128; i++)
				{
					if (_cities.Count - 1 < i)
					{
						for (int b = 0; b < 28; b++)
						{
							bw.Write((byte)0);
						}
						continue;
					}
					City city = _cities[i];
					cityList.Add((byte)cityList.Count, city);
					for (int buildingGroup = 0; buildingGroup < 4; buildingGroup++)
					{
						byte b = 0;
						foreach (IBuilding building in city.Buildings.Where(x => (x.Id - (x.Id % 8)) / 8 == buildingGroup))
						{
							b |= (byte)(0x01 << (building.Id % 8));
						}
						bw.Write(b);
					}
					bw.Write(city.X);
					bw.Write(city.Y);
					bw.Write(city.Status);
					bw.Write(city.Size);
					bw.Write(city.Size);
					bw.Write(city.CurrentProduction.ProductionId);
					bw.Write((byte)city.TradeTotal);
					bw.Write(city.Owner);
					bw.Write((ushort)city.FoodTotal);
					bw.Write((ushort)city.ShieldTotal);
					bw.Write(city.GetResourceTiles());
					// TODO: City specialists
					for (int b = 0; b < 3; b++)
					{
						bw.Write((byte)0);
					}
					//
					bw.Write((byte)i);
					// TODO: Trading cities
					for (int b = 0; b < 3; b++)
					{
						bw.Write((byte)0);
					}
					// Unknown:
					for (int b = 0; b < 2; b++)
					{
						bw.Write((byte)0);
					}
				}

				// Unit types
				for (int i = 0; i < 28; i++)
				{
					IUnit unit = CreateUnit(((Unit)i), -1, -1);

					short obsoleteTech = 0;
					if (unit.ObsoleteTech != null) obsoleteTech = unit.ObsoleteTech.Id;
					short requiredTech = 0;
					if (unit.RequiredTech != null) requiredTech = unit.RequiredTech.Id;
					short outdoors = 0;
					if (unit is Fighter || unit is Nuclear) outdoors = 1;
					else if (unit is Bomber) outdoors = 2;
					short range = 1;
					if (unit is BaseUnitAir) range = 2;
					else if (unit is BaseUnitSea) range = (short)((unit as BaseUnitSea).Range == 1 ? 1 : 3);
					short cargo = 0;
					if (unit is IBoardable) cargo = (short)(unit as IBoardable).Cargo;

					bw.Write(unit.Name.PadRight(12, (char)0x00).Select(x => (byte)x).ToArray());
					bw.Write(obsoleteTech);
					bw.Write((short)unit.Class);
					bw.Write((short)unit.Move);
					bw.Write(outdoors);
					bw.Write((short)unit.Attack);
					bw.Write((short)unit.Defense);
					bw.Write((short)unit.Price);
					bw.Write(range);
					bw.Write(cargo);
					bw.Write((short)unit.Role);
					bw.Write(requiredTech);
				}

				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						for (int x = 0; x < (12 * 128); x++)
						{
							bw.Write((byte)0);
						}
						continue;
					}
					Player player = _players[i];
					IUnit[] units = _units.Where(x => x.Owner == i).ToArray();
					for (int playerUnit = 0; playerUnit < 128; playerUnit++)
					{
						if (units.GetUpperBound(0) < playerUnit)
						{
							for (int x = 0; x < 12; x++)
							{
								switch (x)
								{
									case 3:
										bw.Write((byte)0xFF); // Unit type
										break;
									default:
										bw.Write((byte)0);
										break;
								}
							}
							continue;
						}
						IUnit unit = units[playerUnit];
						byte unitStatus = 0;
						if (unit.Sentry) unitStatus |= (byte)(0x01 << 0);
						if ((unit is Settlers) && (unit as Settlers).BuildingIrrigation > 0) unitStatus |= (byte)(0x01 << 1);
						if (unit.FortifyActive) unitStatus |= (byte)(0x01 << 2);
						if (unit.Fortify) unitStatus |= (byte)(0x01 << 3);
						//
						if (unit.Veteran) unitStatus |= (byte)(0x01 << 5);
						if ((unit is Settlers) && (unit as Settlers).BuildingFortress > 0) unitStatus |= (byte)(0x01 << 6);
						// TODO: Bit 8: Cleaning polution

						byte visibility = 0;
						for (int p = 0; p < 8; p++)
						{
							if (_players.GetUpperBound(0) < p) continue;
							if (!_players[p].Visible(unit.X, unit.Y)) continue;
							visibility |= (byte)(0x01 << p);
						}
						byte stack = (byte)i;
						if (Game.GetUnits(unit.X, unit.Y).Count() > 0)
						{
							for (int u = stack + 1; u < stack + 128; u++)
							{
								byte id = (byte)(u % 128);
								if (units.GetUpperBound(0) < id) continue;
								if (units[id].X != unit.X || units[id].Y != unit.Y) continue;
								stack = id;
								break;
							}
						}

						bw.Write(unitStatus);
						bw.Write((byte)unit.X);
						bw.Write((byte)unit.Y);
						bw.Write((byte)unit.Type);
						bw.Write((byte)((unit.MovesLeft * 3) + unit.PartMoves));
						bw.Write((byte)0);
						// TODO: Goto coordinates
						bw.Write(new byte[] { 0xFF, 0xFF });
						bw.Write((byte)0); // Unknown
						bw.Write(visibility); // Visibility per Civ
						bw.Write(stack); // Next unit in stack
						if (unit.Home == null)
							bw.Write((byte)0xFF);
						else
							bw.Write(cityList.First(x => x.Value == unit.Home).Key);
					}
				}

				// Map visibility
				for (int xx = 0; xx < 80; xx++)
				for (int yy = 0; yy < 50; yy++)
				{
					byte visibility = 0;
					for (int i = 0; i < 8; i++)
					{
						if (_players.GetUpperBound(0) < i) continue;
						if (!_players[i].Visible(xx, yy)) continue;
						visibility |= (byte)(0x01 << i);
					}
					bw.Write(visibility);
				}

				// TODO: Strategic locations status
				for (int i = 0; i < 128; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Strategic locations policy
				for (int i = 0; i < 128; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Strategic locations X
				for (int i = 0; i < 128; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Strategic locations Y
				for (int i = 0; i < 128; i++)
				{
					bw.Write((byte)0);
				}

				// Tech origins
				for (byte i = 0; i < 72; i++)
				{
					if (_advanceOrigin == null || !_advanceOrigin.ContainsKey(i))
					{
						bw.Write((ushort)0);
						continue;
					}
					bw.Write((ushort)_advanceOrigin[i]);
				}

				// TODO: Civ-to-Civ destroyed unit counts
				for (int i = 0; i < 128; i++)
				{
					bw.Write((byte)0);
				}

				// City names
				for (int i = 0; i < 256; i++)
				{
					if (_cities.Count() - 1 < i)
					{
						bw.Write(_cityNames[i].PadRight(13, (char)0x00).Select(x => (byte)x).ToArray());
						continue;
					}
					bw.Write(_cities[i].Name.PadRight(13, (char)0x00).Select(x => (byte)x).ToArray());
				}

				// TODO: Replay data
				for (int i = 0; i < 4098; i++)
				{
					bw.Write((byte)0);
				}

				// Wonders
				for (int i = 0; i < 22; i++)
				{
					if (!cityList.Any(x => x.Value.Wonders.Any(w => w.Id == i)))
					{
						bw.Write(new byte[] { 0xFF, 0xFF });
						continue;
					}
					bw.Write((ushort)cityList.First(x => x.Value.Wonders.Any(w => w.Id == i)).Key);
				}
				
				// TODO: Units lost
				for (int i = 0; i < 448; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Source Civs for techs
				for (int i = 0; i < 576; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Polluted square count
				for (int i = 0; i < 2; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Pollution effect level
				for (int i = 0; i < 2; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Global warming count
				for (int i = 0; i < 2; i++)
				{
					bw.Write((byte)0);
				}

				// Game Settings
				ushort settings = 0;
				if (Settings.InstantAdvice) settings &= (0x01 << 0);
				if (Settings.AutoSave) settings &= (0x01 << 1);
				if (Settings.EndOfTurn) settings &= (0x01 << 2);
				if (Settings.Animations) settings &= (0x01 << 3);
				if (Settings.Sound) settings &= (0x01 << 4);
				// if (Settings.EnemyMoves) settings &= (0x01 << 5);
				if (Settings.CivilopediaText) settings &= (0x01 << 6);
				// if (Settings.Palace) settings &= (0x01 << 7);
				bw.Write(settings);

				// TODO: Land pathfinding
				for (int i = 0; i < 260; i++)
				{
					bw.Write((byte)0);
				}

				// Max tech count
				for (int i = 0; i < 2; i++)
				{
					bw.Write((byte)_players.Max(x => x.Advances.Count()));
				}

				// TODO: Player future techs
				for (int i = 0; i < 2; i++)
				{
					bw.Write((byte)0);
				}
				
				// TODO: Debug switches
				for (int i = 0; i < 2; i++)
				{
					bw.Write((byte)0);
				}
				
				// Science rates
				for (int i = 0; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i)
					{
						bw.Write((short)0);
						continue;
					}
					bw.Write((short)_players[i].ScienceRate);
				}
				
				// Next anthology turn
				bw.Write(_anthologyTurn);
				
				// TODO: Cumulative Epic Rankings
				for (int i = 0; i < 16; i++)
				{
					bw.Write((byte)0);
				}
				
				// TODO: Space ships
				for (int i = 0; i < 1462; i++)
				{
					bw.Write((byte)0);
				}
				
				// TODO: Palace
				for (int i = 0; i < 48; i++)
				{
					bw.Write((byte)0);
				}

				// City X coordinates
				for (int i = 0; i < 256; i++)
				{
					if (_cities.Count - 1 < i)
					{
						bw.Write((byte)0xFF);
						continue;
					}
					bw.Write(_cities[i].X);
				}

				// City Y coordinates
				for (int i = 0; i < 256; i++)
				{
					if (_cities.Count - 1 < i)
					{
						bw.Write((byte)0xFF);
						continue;
					}
					bw.Write(_cities[i].Y);
				}
				
				// TODO: Palace level
				for (int i = 0; i < 2; i++)
				{
					bw.Write((byte)0);
				}
				
				// TODO: Peace turn count
				for (int i = 0; i < 2; i++)
				{
					bw.Write((byte)0);
				}
				
				// TODO: AI opponents
				bw.Write((ushort)(_players.Length - 2));

				// TODO: Spaceship population
				for (int i = 0; i < 16; i++)
				{
					bw.Write((byte)0);
				}

				// TODO: Spaceship launch year
				for (int i = 0; i < 16; i++)
				{
					bw.Write((byte)0);
				}

				// Civ identity
				ushort identity = 0;
				for (int i = 1; i < 8; i++)
				{
					if (_players.GetUpperBound(0) < i) continue;
					if (_players[i].Civilization.Id > 7) identity |= (ushort)(0x01 << i);
				}
				bw.Write(identity);
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
					
					Console.WriteLine(loopCounter.ToString());
				}
				
				// Starting position found, add Settlers
				IUnit unit = CreateUnit(Unit.Settlers, x, y);
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
			Settings.Sound = true;
			
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

			// Number of turns to next antholoy needs to be checked
			_anthologyTurn = (ushort)Common.Random.Next(1, 128);
		}
	}
}