// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Units;
using CivOne.Wonders;

namespace CivOne
{
	public partial class Game : BaseInstance
	{
		public static void LoadGame(string sveFile, string mapFile)
		{
			if (_instance != null)
			{
				Log("ERROR: Game instance already exists");
				return;
			}

			using (IGameData adapter = SaveDataAdapter.Load(File.ReadAllBytes(sveFile)))
			{
				if (!adapter.ValidData)
				{
					Log("SaveDataAdapter failed to load game");
					return;
				}

				Map.Instance.LoadMap(mapFile, adapter.RandomSeed);
				_instance = new Game(adapter);
				Log($"Game instance loaded (difficulty: {_instance._difficulty}, competition: {_instance._competition}");
			}
		}

		public void Save(string sveFile, string mapFile)
		{
			using (IGameData gameData = new SaveDataAdapter())
			{
				gameData.GameTurn = _gameTurn;
				gameData.HumanPlayer = (ushort)PlayerNumber(HumanPlayer);
				gameData.RandomSeed = Map.Instance.SaveMap(mapFile);
				gameData.Difficulty = (ushort)_difficulty;
				gameData.ActiveCivilizations = _players.Select(x => (x.Civilization is Barbarian) || (x.Cities.Any(c => c.Size > 0) || GetUnits().Any(u => x == u.Owner))).ToArray();
				gameData.CivilizationIdentity = _players.Select(x => (byte)(x.Civilization.Id > 7 ? 1 : 0)).ToArray();
				gameData.CurrentResearch = HumanPlayer.CurrentResearch?.Id ?? 0;
				byte[][] discoveredAdvanceIDs = new byte[_players.Length][];
				for (int p = 0; p < _players.Length; p++)
					discoveredAdvanceIDs[p] = _players[p].Advances.Select(x => x.Id).ToArray();
				gameData.DiscoveredAdvanceIDs = discoveredAdvanceIDs;
				gameData.LeaderNames = _players.Select(x => x.LeaderName).ToArray();
				gameData.CivilizationNames = _players.Select(x => x.TribeNamePlural).ToArray();
				gameData.CitizenNames = _players.Select(x => x.TribeName).ToArray();
				gameData.CityNames = CityNames;
				gameData.PlayerGold = _players.Select(x => x.Gold).ToArray();
				gameData.ResearchProgress = _players.Select(x => x.Science).ToArray();
				gameData.TaxRate = _players.Select(x => (ushort)x.TaxesRate).ToArray();
				gameData.ScienceRate = _players.Select(x => (ushort)(10 - x.ScienceRate - x.TaxesRate)).ToArray();
				gameData.StartingPositionX = _players.Select(x => (ushort)x.StartX).ToArray();
				gameData.Government = _players.Select(x => (ushort)x.Government.Id).ToArray();
				gameData.Cities = _cities.GetCityData().ToArray();
				gameData.Units = _players.Select(p => _units.Where(u => p == u.Owner).GetUnitData().ToArray()).ToArray();
				ushort[] wonders = Enumerable.Repeat(ushort.MaxValue, 22).ToArray();
				for (byte i = 0; i < _cities.Count(); i++)
				foreach (IWonder wonder in _cities[i].Wonders)
				{
					wonders[wonder.Id] = i;
				}
				gameData.Wonders = wonders;
				bool[][,] visibility = new bool[_players.Length][,];
				for (int p = 0; p < visibility.Length; p++)
				{
					visibility[p] = new bool[80, 50];
					for (int xx = 0; xx < 80; xx++)
					for (int yy = 0; yy < 50; yy++)
					{
						if (!_players[p].Visible(xx, yy)) continue;
						visibility[p][xx, yy] = true;
					}
				}
				gameData.TileVisibility = visibility;
				ushort[] firstDiscovery = new ushort[72];
				foreach (byte key in _advanceOrigin.Keys)
					firstDiscovery[key] = _advanceOrigin[key];
				gameData.AdvanceFirstDiscovery = firstDiscovery;
				gameData.GameOptions = new bool[]
				{
					Settings.InstantAdvice,
					Settings.AutoSave,
					Settings.EndOfTurn,
					Settings.Animations,
					Settings.Sound,
					Settings.EnemyMoves,
					Settings.CivilopediaText,
					// Settings.Palace
				};
				gameData.NextAnthologyTurn = _anthologyTurn;
				gameData.OpponentCount = (ushort)(_players.Length - 2);
				File.WriteAllBytes(sveFile, gameData.GetBytes());
			}
		}

		private Game(IGameData gameData)
		{
			_difficulty = gameData.Difficulty;
			_competition = (gameData.OpponentCount + 1);
			_players = new Player[_competition + 1];
			_cities = new List<City>();
			_units = new List<IUnit>();

			ushort[] advanceFirst = gameData.AdvanceFirstDiscovery;
			bool[][,] visibility = gameData.TileVisibility;
			for (int i = 0; i < _players.Length; i++)
			{
				ICivilization[] civs = Common.Civilizations.Where(c => c.PreferredPlayerNumber == i).ToArray();
				ICivilization civ = civs[gameData.CivilizationIdentity[i] % civs.Length];
				Player player = (_players[i] = new Player(civ, gameData.LeaderNames[i], gameData.CitizenNames[i], gameData.CivilizationNames[i]));
				player.Gold = gameData.PlayerGold[i];
				player.Science = gameData.ResearchProgress[i];
				player.Government = Reflect.GetGovernments().FirstOrDefault(x => x.Id == gameData.Government[i]);

				player.TaxesRate = gameData.TaxRate[i];
				player.LuxuriesRate = 10 - gameData.ScienceRate[i] - player.TaxesRate;
				player.StartX = (short)gameData.StartingPositionX[i];
				
				// Set map visibility
				for (int xx = 0; xx < 80; xx++)
				for (int yy = 0; yy < 50; yy++)
				{
					if (!visibility[i][xx, yy]) continue;
					if (i == 0 && Map[xx, yy].Hut) Map[xx, yy].Hut = false;
					player.Explore(xx, yy, 0);
				}

				byte[] advanceIds = gameData.DiscoveredAdvanceIDs[i];
				Common.Advances.Where(x => advanceIds.Any(id => x.Id == id)).ToList().ForEach(x =>
				{
					player.AddAdvance(x, false);
					if (advanceFirst[x.Id] != player.Civilization.Id) return;
					SetAdvanceOrigin(x, player);
				});
			}

			GameTurn = gameData.GameTurn;
			CityNames = gameData.CityNames;
			HumanPlayer = _players[gameData.HumanPlayer];
			HumanPlayer.CurrentResearch = Common.Advances.FirstOrDefault(a => a.Id == gameData.CurrentResearch);
		
			_anthologyTurn = gameData.NextAnthologyTurn;

			Dictionary<byte, City> cityList = new Dictionary<byte, City>();
			foreach (CityData cityData in gameData.Cities)
			{
				City city = new City(cityData.Owner)
				{
					X = cityData.X,
					Y = cityData.Y,
					NameId = cityData.NameId,
					Size = cityData.ActualSize,
					Food = cityData.Food,
					Shields = cityData.Shields
				};
				city.SetProduction(cityData.CurrentProduction);
				city.SetResourceTiles(cityData.ResourceTiles);
				
				// Set city buildings
				foreach (byte buildingId in cityData.Buildings)
				{
					city.AddBuilding(Common.Buildings.First(b => b.Id == buildingId));
				}

				// Set city wonders
				foreach (IWonder wonder in Common.Wonders)
				{
					if (gameData.Wonders[wonder.Id - 1] != cityData.Id) continue;
					city.AddWonder(wonder);
				}
				
				_cities.Add(city);

				foreach (byte fortifiedUnit in cityData.FortifiedUnits)
				{
					IUnit unit = CreateUnit((UnitType)fortifiedUnit, city.X, city.Y);
					unit.Status = (byte)(1 << 3);
					unit.Owner = city.Owner;
					unit.SetHome(city);
					_units.Add(unit);
				}

				cityList.Add(cityData.Id, city);
			}

			UnitData[][] unitData = gameData.Units;
			for (byte p = 0; p < 8; p++)
			{
				if (!gameData.ActiveCivilizations[p]) continue;
				foreach (UnitData data in unitData[p])
				{
					IUnit unit = CreateUnit((UnitType)data.TypeId, data.X, data.Y);
					if (unit == null) continue;
					unit.Status = data.Status;
					unit.Owner = p;
					unit.PartMoves = (byte)(data.RemainingMoves % 3);
					unit.MovesLeft = (byte)((data.RemainingMoves - unit.PartMoves) / 3);
					if (data.GotoX != 0xFF) unit.Goto = new Point(data.GotoX, data.GotoY);
					if (cityList.ContainsKey(data.HomeCityId))
					{
						unit.SetHome(cityList[data.HomeCityId]);
					}
					_units.Add(unit);
				}
			}

			// Game Settings
			bool[] options = gameData.GameOptions;
			Settings.InstantAdvice = options[0];
			Settings.AutoSave = options[1];
			Settings.EndOfTurn = options[2];
			Settings.Animations = options[3];
			Settings.Sound = options[4];
			Settings.EnemyMoves = options[5];
			Settings.CivilopediaText = options[6];
			// Settings.Palace = options[7];

			_currentPlayer = gameData.HumanPlayer;
			for (int i = 0; i < _units.Count(); i++)
			{
				if (_units[i].Owner != gameData.HumanPlayer || _units[i].Busy) continue;
				_activeUnit = i;
				if (_units[i].MovesLeft > 0) break;
			}
		}
	}
}