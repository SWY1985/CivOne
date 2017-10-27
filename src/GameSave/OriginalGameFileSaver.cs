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
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Units;

namespace CivOne.GameSave
{
    class OriginalGameFileSaver
    {

        public OriginalGameFileSaver(string sveFile, string mapFile)
        {
            _sveFile = sveFile;
            _mapFile = mapFile;
        }

        public void Save(GameState gs)
        {
            // TODO: Implement full save file configuration
            // - http://forums.civfanatics.com/showthread.php?p=12422448
            // - http://forums.civfanatics.com/showthread.php?t=493581
            using (FileStream fs = new FileStream(_sveFile, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                ushort randomSeed = Map.Instance.SaveMap(_mapFile);
                ushort activeCivilizations = 1;
                for (int i = 1; i < gs.Players.Length; i++)
                    if (gs.Players[i].Cities.Any() || gs.GetUnits().Any(x => x.Owner == i))
                        activeCivilizations |= (ushort)(0x01 << i);

                bw.Write(gs._gameTurn);
                bw.Write((ushort)gs.PlayerNumber(gs.HumanPlayer));
                bw.Write((ushort)(0x01 << gs.PlayerNumber(gs.HumanPlayer)));
                bw.Write(randomSeed);
                bw.Write((short)Common.TurnToYear(gs._gameTurn));
                bw.Write((ushort)gs.Difficulty);
                bw.Write(activeCivilizations);
                if (gs.HumanPlayer.CurrentResearch == null)
                    bw.Write((ushort)0x00);
                else
                    bw.Write((ushort)gs.HumanPlayer.CurrentResearch.Id);

                // Leader names
                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        for (int x = 0; x < 14; x++)
                        {
                            bw.Write((byte)0x00);
                        }
                        continue;
                    }
                    bw.Write(gs.Players[i].LeaderName.PadRight(14, (char)0x00).Select(x => (byte)x).ToArray());
                }
                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        for (int x = 0; x < 12; x++)
                        {
                            bw.Write((byte)0x00);
                        }
                        continue;
                    }
                    bw.Write(gs.Players[i].Civilization.NamePlural.PadRight(12, (char)0x00).Select(x => (byte)x).ToArray());
                }
                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        for (int x = 0; x < 11; x++)
                        {
                            bw.Write((byte)0x00);
                        }
                        continue;
                    }
                    bw.Write(gs.Players[i].Civilization.Name.PadRight(11, (char)0x00).Select(x => (byte)x).ToArray());
                }

                // Player gold
                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        bw.Write((short)0);
                        continue;
                    }
                    bw.Write(gs.Players[i].Gold);
                }

                // Research progress
                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        bw.Write((short)0);
                        continue;
                    }
                    bw.Write(gs.Players[i].Science);
                }

                // Units active
                for (int i = 0; i < 8; i++)
                    for (byte unitId = 0; unitId < 28; unitId++)
                    {
                        if (gs.Players.GetUpperBound(0) < i)
                        {
                            bw.Write((short)0);
                            continue;
                        }
                        bw.Write((short)gs.GetUnits().Where(x => x.Owner == i).Count(x =>
                            x.ProductionId == unitId));
                    }

                // Units currently in production
                for (int i = 0; i < 8; i++)
                    for (byte unitId = 0; unitId < 28; unitId++)
                    {
                        if (gs.Players.GetUpperBound(0) < i)
                        {
                            bw.Write((short)0);
                            continue;
                        }
                        bw.Write((short)gs.Players[i].Cities.Count(x =>
                            x.CurrentProduction is IUnit &&
                            (x.CurrentProduction as IUnit).ProductionId == unitId));
                    }

                // Discovered Advances Count
                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        bw.Write((short)0);
                        continue;
                    }
                    bw.Write((short)gs.Players[i].Advances.Count());
                }

                // Set civilization advances
                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        bw.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
                        continue;
                    }
                    for (int techGroup = 0; techGroup < 5; techGroup++)
                    {
                        ushort techFlag = 0;
                        foreach (IAdvance advance in gs.Players[i].Advances.Where(x => ((x.Id - (x.Id % 16)) / 16) == techGroup))
                        {
                            techFlag |= (ushort)(0x01 << (advance.Id % 16));
                        }
                        bw.Write(techFlag);
                    }
                }

                // Civilization Governments
                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        bw.Write((short)0);
                        continue;
                    }
                    bw.Write((short)gs.Players[i].Government.Id);
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
                    bw.Write((short)gs.Cities.Count(x => x.Owner == i));
                }

                // Unit counts
                for (int i = 0; i < 8; i++)
                {
                    bw.Write((short)gs.GetUnits().Count(x => x.Owner == i));
                }

                // TODO: Land counts
                for (int i = 0; i < 8; i++)
                {
                    bw.Write((short)0);
                }

                // Settler counts
                for (int i = 0; i < 8; i++)
                {
                    bw.Write((short)(gs.GetUnits().Count(x => (x is Settlers) && x.Owner == i) + 1));
                }

                // Total Civ size
                for (int i = 0; i < 8; i++)
                {
                    bw.Write((short)gs.Cities.Where(x => x.Owner == i).Sum(x => x.Size));
                }

                // Military power
                for (int i = 0; i < 8; i++)
                {
                    bw.Write((short)gs.GetUnits().Where(x => x.Owner == i).Sum(x => x.Attack + x.Defense));
                }

                // TODO: Civ Rankings
                for (int i = 0; i < 8; i++)
                {
                    bw.Write((short)i);
                }

                // Tax rate
                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        bw.Write((short)0);
                        continue;
                    }
                    bw.Write((short)gs.Players[i].TaxesRate);
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
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        bw.Write((short)0xFF);
                        continue;
                    }
                    bw.Write((short)gs.Players[i].StartX);
                }

                // Leader graphics
                for (int i = 0; i < 8; i++)
                {
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        bw.Write((short)0);
                        continue;
                    }
                    bw.Write((short)gs.Players[i].Civilization.Id);
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
                    bw.Write((short)Map.Instance.AllTiles().Count(x => !x.IsOcean && x.ContinentId == i));
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
                    if (gs.Cities.Count - 1 < i)
                    {
                        for (int b = 0; b < 28; b++)
                        {
                            bw.Write((byte)0);
                        }
                        continue;
                    }
                    City city = gs.Cities[i];
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
                    IUnit unit = UnitsFactory.CreateUnit(((UnitType)i), -1, -1);

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
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        for (int x = 0; x < (12 * 128); x++)
                        {
                            bw.Write((byte)0);
                        }
                        continue;
                    }
                    Player player = gs.Players[i];
                    IUnit[] units = gs.Units.Where(x => x.Owner == i).ToArray();
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
                            if (gs.Players.GetUpperBound(0) < p) continue;
                            if (!gs.Players[p].Visible(unit.X, unit.Y)) continue;
                            visibility |= (byte)(0x01 << p);
                        }
                        byte stack = (byte)i;
                        if (gs.GetUnits(unit.X, unit.Y).Count() > 0)
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
                            if (gs.Players.GetUpperBound(0) < i) continue;
                            if (!gs.Players[i].Visible(xx, yy)) continue;
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
                    if (gs.AdvanceOrigin == null || !gs.AdvanceOrigin.ContainsKey(i))
                    {
                        bw.Write((ushort)0);
                        continue;
                    }
                    bw.Write((ushort)gs.AdvanceOrigin[i]);
                }

                // TODO: Civ-to-Civ destroyed unit counts
                for (int i = 0; i < 128; i++)
                {
                    bw.Write((byte)0);
                }

                // City names
                for (int i = 0; i < 256; i++)
                {
                    if (gs.Cities.Count() - 1 < i)
                    {
                        bw.Write(gs.CityNames[i].PadRight(13, (char)0x00).Select(x => (byte)x).ToArray());
                        continue;
                    }
                    bw.Write(gs.Cities[i].Name.PadRight(13, (char)0x00).Select(x => (byte)x).ToArray());
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
                if (Settings.Instance.InstantAdvice) settings &= (0x01 << 0);
                if (Settings.Instance.AutoSave) settings &= (0x01 << 1);
                if (Settings.Instance.EndOfTurn) settings &= (0x01 << 2);
                if (Settings.Instance.Animations) settings &= (0x01 << 3);
                if (Settings.Instance.Sound) settings &= (0x01 << 4);
                // if (Settings.EnemyMoves) settings &= (0x01 << 5);
                if (Settings.Instance.CivilopediaText) settings &= (0x01 << 6);
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
                    bw.Write((byte)gs.Players.Max(x => x.Advances.Count()));
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
                    if (gs.Players.GetUpperBound(0) < i)
                    {
                        bw.Write((short)0);
                        continue;
                    }
                    bw.Write((short)gs.Players[i].ScienceRate);
                }

                // Next anthology turn
                bw.Write(gs.AnthologyTurn);

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
                    if (gs.Cities.Count - 1 < i)
                    {
                        bw.Write((byte)0xFF);
                        continue;
                    }
                    bw.Write(gs.Cities[i].X);
                }

                // City Y coordinates
                for (int i = 0; i < 256; i++)
                {
                    if (gs.Cities.Count - 1 < i)
                    {
                        bw.Write((byte)0xFF);
                        continue;
                    }
                    bw.Write(gs.Cities[i].Y);
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
                bw.Write((ushort)(gs.Players.Length - 2));

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
                    if (gs.Players.GetUpperBound(0) < i) continue;
                    if (gs.Players[i].Civilization.Id > 7) identity |= (ushort)(0x01 << i);
                }
                bw.Write(identity);
            }
        }

        private readonly string _sveFile;
        private readonly string _mapFile;
    }

}
