// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.


using System.Collections.Generic;
using System.IO;
using System.Linq;
using CivOne.Advances;
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Units;
using CivOne.Wonders;

namespace CivOne.GameSave
{
    public class OriginalGameFileLoader
    {
        public GameState LoadGame(string sveFile, string mapFile)
        {
            GameState gs;
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
                for (int i = 5384; i < 8968; i += 28)
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
                for (int i = 9920; i < 22208; i += 12)
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

                    IUnit unit = UnitsFactory.CreateUnit((UnitType)type, x, y);
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
                Settings.Instance.InstantAdvice = (settings & (0x01 << 0)) > 0;
                Settings.Instance.AutoSave = (settings & (0x01 << 1)) > 0;
                Settings.Instance.EndOfTurn = (settings & (0x01 << 2)) > 0;
                Settings.Instance.Animations = (settings & (0x01 << 3)) > 0;
                Settings.Instance.Sound = (settings & (0x01 << 4)) > 0;
                Settings.Instance.EnemyMoves = (settings & (0x01 << 5)) > 0;
                Settings.Instance.CivilopediaText = (settings & (0x01 << 6)) > 0;
                // Settings.Palace = (settings & (0x01 << 7)) > 0;

                ushort anthologyTurn = Common.BinaryReadUShort(br, 35778);

                ushort competition = (ushort)(Common.BinaryReadUShort(br, 37820) + 1);
                ushort civIdentity = Common.BinaryReadUShort(br, 37854);

                gs = new GameState(difficulty, competition);
                Logger.Log("Game instance loaded (difficulty: {0}, competition: {1})", difficulty, competition);

                // Load map visibility
                byte[] visibility = Common.BinaryReadBytes(br, 22208, 4000);

                for (int i = 0; i <= competition; i++)
                {
                    int identity = ((civIdentity >> i) & 0x1);
                    ICivilization[] civs = Common.Civilizations.Where(c => c.PreferredPlayerNumber == i).ToArray();
                    ICivilization civ = civs[identity];
                    Player player = (gs.Players[i] = new Player(civ, leaderNames[i], tribeNames[i], tribeNamesPlural[i]));
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

                            if ((tile & (1 << i)) == 0)
                                continue;

                            player.Explore(xx, yy, 0);
                        }

                    // Set civilization advances
                    for (int t = 0; t < 5; t++)
                    {
                        int offset = 1256 + (i * 10) + (t * 2);
                        ushort techFlag = Common.BinaryReadUShort(br, offset);
                        for (int b = 0; b < 16; b++)
                        {
                            if ((techFlag & (1 << b)) == 0)
                                continue;

                            IAdvance advance = Common.Advances.FirstOrDefault(a => a.Id == (16 * t) + b);

                            if (advance == null)
                                continue;

                            player.AddAdvance(advance, false);

                            int originId = Common.BinaryReadUShort(br, 26720 + (advance.Id * 2));
                            if (originId == player.Civilization.Id)
                                Game._instance.GameState.SetAdvanceOrigin(advance, player);
                        }
                    }

                    Logger.Log($"- Player {i} is {player.LeaderName} of the {gs.Players[i].TribeNamePlural}" + ((i == humanPlayer) ? " (human)" : ""));
                }

                gs._gameTurn = Common.BinaryReadUShort(br, 0);
                gs.HumanPlayer = gs.Players[humanPlayer];
                gs.HumanPlayer.CurrentResearch = Common.Advances.FirstOrDefault(a => a.Id == Common.BinaryReadUShort(br, 14));

                gs.AnthologyTurn = anthologyTurn;

                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) <= i)
                        break;

                    gs.Players[i].StartX = (short)Common.BinaryReadUShort(br, 1896 + (i * 2));
                }

                foreach (City city in cities)
                {
                    gs.Cities.Add(city);
                }
                foreach (IUnit unit in units)
                {
                    gs.Units.Add(unit);
                }

                for (int i = 0; i < gs.CityNames.Length; i++)
                {
                    if (!cities.Any(x => x.Name == gs.CityNames[i]))
                        continue;

                    gs.CityNameUsed[i] = true;
                }

                gs.CurrentPlayerId = humanPlayer;
                for (int i = 0; i < gs.Units.Count(); i++)
                {
                    if (gs.Units[i].Owner != humanPlayer)
                        continue;

                    gs.ActiveUnitId = i;

                    if (gs.Units[i].MovesLeft > 0)
                        break;
                }
            }

            return gs;
        }
    }
}