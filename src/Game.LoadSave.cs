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
using CivOne.Players;
using CivOne.Units;
using CivOne.Wonders;

namespace CivOne
{
	internal partial class Game : BaseInstance
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
				Log($"Game instance loaded (difficulty: {_instance.Difficulty}, competition: {_instance._competition}");
			}
		}

		public void Save(string sveFile, string mapFile)
		{
			using (IGameData gameData = new SaveDataAdapter())
			{
				Map.Instance.SaveMap(mapFile);

				gameData.GameTurn = Data.GameTurn;
				gameData.HumanPlayer = Data.HumanPlayer;
				gameData.RandomSeed = Data.RandomSeed;
				gameData.Difficulty = Data.Difficulty;
				gameData.ActiveCivilizations = Data.ActiveCivilizations;
				gameData.CivilizationIdentity = Data.CivilizationIdentity;
				gameData.CurrentResearch = Data.CurrentResearch;
				gameData.DiscoveredAdvanceIDs = Data.DiscoveredAdvanceIDs;
				gameData.LeaderNames = Data.LeaderNames;
				gameData.CivilizationNames = Data.CivilizationNames;
				gameData.CitizenNames = Data.CitizenNames;
				gameData.CityNames = CityNames;
				gameData.PlayerGold = Data.PlayerGold;
				gameData.ResearchProgress = Data.ResearchProgress;
				gameData.TaxRate = Data.TaxRate;
				gameData.ScienceRate = Data.ScienceRate;
				gameData.StartingPositionX = Data.StartingPositionX;
				gameData.Government = Data.Government;
				gameData.Cities = Data.Cities;
				gameData.Units = Data.Units;
				gameData.Wonders = Data.Wonders;
				gameData.TileVisibility = Data.TileVisibility;
				gameData.AdvanceFirstDiscovery = Data.AdvanceFirstDiscovery;
				gameData.GameOptions = Data.GameOptions;
				gameData.NextAnthologyTurn = Data.NextAnthologyTurn;
				gameData.OpponentCount = (ushort)(_players.Length - 2);
				gameData.ReplayData = Data.ReplayData;
				File.WriteAllBytes(sveFile, gameData.GetBytes());
			}
		}

		private static IPlayer CreatePlayer(ICivilization civilization, string leaderName, string citizenName, string civilizationName, bool isHuman)
		{
			if (isHuman)
				return new HumanPlayer(civilization, leaderName, civilizationName, citizenName);
			if (civilization is Barbarian)
				return new BarbarianPlayer();
			return new ComputerPlayer(civilization, leaderName, civilizationName, citizenName);
		}

		private Game(IGameData gameData) : this()
		{
			Data.Difficulty = gameData.Difficulty;
			_competition = (gameData.OpponentCount + 1);
			_cities = new List<City>();
			_units = new List<IUnit>();

			ushort[] advanceFirst = gameData.AdvanceFirstDiscovery;
			Data.TileVisibility = gameData.TileVisibility;
			for (int i = 0; i < _players.Length; i++)
			{
				ICivilization[] civs = Common.Civilizations.Where(c => c.PreferredPlayerNumber == i).ToArray();
				ICivilization civ = civs[gameData.CivilizationIdentity[i] % civs.Length];
				IPlayer player = (_players[i] = CreatePlayer(civ, gameData.LeaderNames[i], gameData.CitizenNames[i], gameData.CivilizationNames[i], (i == gameData.HumanPlayer)));
				player.Government = Reflect.GetGovernments().FirstOrDefault(x => x.Id == gameData.Government[i]);

				player.TaxesRate = gameData.TaxRate[i];
				player.LuxuriesRate = 10 - gameData.ScienceRate[i] - player.TaxesRate;
				player.StartX = (short)gameData.StartingPositionX[i];

				_players[i] = player;
			}

			Data.DiscoveredAdvanceIDs = gameData.DiscoveredAdvanceIDs;
			Data.PlayerGold = gameData.PlayerGold;
			Data.ResearchProgress = gameData.ResearchProgress;
			Data.AdvanceFirstDiscovery = gameData.AdvanceFirstDiscovery;
			Data.TileVisibility = gameData.TileVisibility;

			Data.GameTurn = gameData.GameTurn;
			Data.CityNames = gameData.CityNames;
			HumanPlayer.CurrentResearch = Common.Advances.FirstOrDefault(a => a.Id == gameData.CurrentResearch);
		
			Data.NextAnthologyTurn = gameData.NextAnthologyTurn;

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
					if (gameData.Wonders[wonder.Id] != cityData.Id) continue;
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

			Data.ReplayData = gameData.ReplayData;

			Data.GameOptions = gameData.GameOptions;

			_players.SetCurrentPlayer(gameData.HumanPlayer);
			for (int i = 0; i < _units.Count(); i++)
			{
				if (_units[i].Owner != gameData.HumanPlayer || _units[i].Busy) continue;
				_activeUnit = i;
				if (_units[i].MovesLeft > 0) break;
			}
		}
	}
}