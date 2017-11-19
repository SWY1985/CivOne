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
using CivOne.Screens;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.Units;

namespace CivOne
{
    public class GameState
    {
        public Settings Settings { get; set; }
        public readonly string[] CityNames;
        public readonly bool[] CityNameUsed;
        public readonly int Difficulty;
        public readonly int Competition;
        public readonly Player[] Players;
        public readonly List<City> Cities;
        public readonly List<IUnit> Units;
        public int CurrentPlayerId = 0;
        public int ActiveUnitId;
        public ushort AnthologyTurn;
        public Dictionary<byte, byte> AdvanceOrigin;
        public Player HumanPlayer { get; set; }
        public Map Map;

        public GameState()
        {
            CityNames = Common.AllCityNames.ToArray();
            CityNameUsed = new bool[Common.AllCityNames.Count()];
            AdvanceOrigin = new Dictionary<byte, byte>();
            Cities = new List<City>();
            Units = new List<IUnit>();
            Settings = Settings.Instance;
            Map = Map.Instance;
        }

        public GameState(
            int difficulty
            , int competition
        ) : this()
        {
            Difficulty = difficulty;
            Competition = competition;
            Players = new Player[competition + 1];
        }

        public GameState(
            int difficulty
            , int competition
            , ICivilization tribe
            , string leaderName
            , string tribeName
            , string tribeNamePlural
        ) : this(difficulty, competition)
        {
            Logger.Log("Game instance created (difficulty: {0}, competition: {1})", Difficulty, Competition);

            Settings.Animations = true;
            Settings.CivilopediaText = true;
            Settings.Sound = true;
            Settings.EnemyMoves = true;

            for (int i = 0; i <= competition; i++)
            {
                if (i == tribe.PreferredPlayerNumber)
                {
                    Players[i] = new Player(tribe, leaderName, tribeName, tribeNamePlural);
                    HumanPlayer = Players[i];
                    if (difficulty == 0)
                    {
                        // Chieftain starts with 50 Gold
                        HumanPlayer.Gold = 50;
                        Settings.InstantAdvice = true;
                    }
                    Logger.Log($"- Player {i} is {Players[i].LeaderName} of the {Players[i].TribeNamePlural} (human)");
                    continue;
                }

                ICivilization[] civs = Common.Civilizations.Where(civ => civ.PreferredPlayerNumber == i).ToArray();
                int r = Common.Random.Next(civs.Length);

                Players[i] = new Player(civs[r]);

                Logger.Log($"- Player {i} is {Players[i].LeaderName} of the {Players[i].TribeNamePlural}");
            }

            Logger.Log("Adding starting units...");
            for (byte i = 1; i <= competition; i++)
            {
                AddStartingUnits(i);
            }

            Logger.Log("Calculate players handicap...");
            for (byte i = 1; i <= competition; i++)
            {
                CalculateHandicap(i);
            }

            Logger.Log("Apply players bonus...");
            for (byte i = 1; i <= competition; i++)
            {
                ApplyBonus(i);
            }

            // Number of turns to next antholoy needs to be checked


            AnthologyTurn = (ushort)Common.Random.Next(1, 128);
        }

        public ushort __gameTurn;
        public ushort _gameTurn
        {
            get
            {
                return __gameTurn;
            }
            set
            {
                __gameTurn = value;
                Logger.Log($"Turn {__gameTurn}: {GameYear}");
                if (AnthologyTurn >= __gameTurn)
                {
                    //TODO: Show anthology
                    AnthologyTurn = (ushort)(__gameTurn + 20 + Common.Random.Next(40));
                    Logger.Log($"New anthology turn is ${AnthologyTurn}");
                }
            }
        }

        internal string GameYear
        {
            get
            {
                return Common.YearString(_gameTurn);
            }
        }

        internal Player CurrentPlayer
        {
            get
            {
                return Players[CurrentPlayerId];

            }
        }

        internal byte PlayerNumber(Player player)
        {
            byte i = 0;
            foreach (Player p in Players)
            {
                if (p == player)
                    return i;
                i++;
            }
            return 0;
        }

        public void SetAdvanceOrigin(IAdvance advance, Player player)
        {
            if (AdvanceOrigin.ContainsKey(advance.Id))
                return;

            byte playerNumber = 0;

            if (player != null)
                playerNumber = PlayerNumber(player);

            AdvanceOrigin.Add(advance.Id, playerNumber);
        }

        public bool GetAdvanceOrigin(IAdvance advance, Player player)
        {
            if (AdvanceOrigin.ContainsKey(advance.Id))
                return (AdvanceOrigin[advance.Id] == PlayerNumber(player));

            return false;
        }

        public Player GetPlayer(byte number)
        {
            if (Players.Length < number)
                return null;
            return Players[number];
        }

        internal IUnit[] GetUnits(int x, int y)
        {
            while (x < 0)
                x += Map.WIDTH;
            while (x >= Map.WIDTH)
                x -= Map.WIDTH;

            if (y < 0) return null;
            if (y >= Map.HEIGHT) return null;

            return Units.Where(u => u.X == x && u.Y == y).OrderBy(u => (u == ActiveUnit) ? 0 : (u.Fortify || u.FortifyActive ? 1 : 2)).ToArray();
        }

        internal IUnit[] GetUnits()
        {
            return Units.ToArray();
        }

        public IUnit ActiveUnit
        {
            get
            {
                if (Units.Count(u => u.Owner == CurrentPlayerId && !u.Busy) == 0)
                    return null;

                // If the unit counter is too high, return to 0
                if (ActiveUnitId >= Units.Count)
                    ActiveUnitId = 0;

                // Does the current unit still have moves left?
                if (Units[ActiveUnitId].Owner == CurrentPlayerId
                    && (Units[ActiveUnitId].MovesLeft > 0 || Units[ActiveUnitId].PartMoves > 0)
                    && !Units[ActiveUnitId].Sentry && !Units[ActiveUnitId].Fortify
                )
                {
                    return Units[ActiveUnitId];
                }

                // Task busy, don't change the active unit
                if (GameTask.Any())
                    return Units[ActiveUnitId];

                // Check if any units are still available for this player
                if (!Units.Any(u => u.Owner == CurrentPlayerId && (u.MovesLeft > 0 || u.PartMoves > 0) && !u.Busy))
                {
                    if (CurrentPlayer == HumanPlayer && !Settings.Instance.EndOfTurn && !GameTask.Any() && (Common.TopScreen is GamePlay))
                    {
                        GameTask.Enqueue(Turn.End());
                    }
                    return null;
                }

                // Loop through units
                while (
                    Units[ActiveUnitId].Owner != CurrentPlayerId 
                    || (Units[ActiveUnitId].MovesLeft == 0 && Units[ActiveUnitId].PartMoves == 0) 
                    || (Units[ActiveUnitId].Sentry || Units[ActiveUnitId].Fortify))
                {
                    ++ActiveUnitId;
                    if (ActiveUnitId >= Units.Count)
                        ActiveUnitId = 0;
                }
                return Units[ActiveUnitId];
            }

            internal set
            {
                if (value == null || value.MovesLeft == 0 && value.PartMoves == 0)
                    return;
                value.Sentry = false;
                value.Fortify = false;
                ActiveUnitId = Units.IndexOf(value);
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

                if (Map.FixedStartPositions && _gameTurn == 0)
                {
                    // Map position is fixed, don't check anything
                    x = Players[player].Civilization.StartX;
                    y = Players[player].Civilization.StartY;
                    if (Map[x, y].Hut)
                        Map[x, y].Hut = false;
                }
                else
                {
                    ITile tile = Map[x, y];

                    if (tile.IsOcean)
                        continue;

                    if (tile.Hut)
                        continue;

                    // Is there already a unit on this tile?
                    if (Units.Any(u => u.X == x || u.Y == y)) 
                        continue;

                    // Is the land value high enough?
                    if (tile.LandValue < (12 - (loopCounter / 32))) 
                        continue;

                    // Distance to other cities
                    if (Cities.Any(c => Common.DistanceToTile(x, y, c.X, c.Y) < (10 - (loopCounter / 64)))) 
                        continue;

                    // Distance to other settlers
                    if (Units.Any(u => (u is Settlers) && Common.DistanceToTile(x, y, u.X, u.Y) < (10 - (loopCounter / 64))))
                        continue;

                    // Check buildable tiles on continent
                    if (Map.ContinentTiles(tile.ContinentId).Count(t => Map.TileIsType(t, Terrain.Plains, Terrain.Grassland1, Terrain.Grassland2, Terrain.River)) < (32 - (_gameTurn / 16)))
                        continue; 

                    // After 0 AD, don't spawn a Civilization on a continent that already contains cities.
                    if (Common.TurnToYear(_gameTurn) >= 0 && Map.ContinentTiles(tile.ContinentId).Any(t => t.City != null))
                        continue;

                    Logger.Log(loopCounter.ToString());
                }

                // Starting position found, add Settlers
                IUnit unit = UnitsFactory.CreateUnit(UnitType.Settlers, x, y);
                unit.Owner = player;
                Units.Add(unit);

                Players[player].StartX = (short)x;

                return;
            }
        }

        private void CalculateHandicap(byte player)
        {
            // Translated drom this post by Gowron:
            // http://forums.civfanatics.com/showthread.php?t=494994

            // All Handicap values start from 0.
            byte handicap = 0;
            IUnit startUnit = Units.FirstOrDefault(u => u.Owner == player);
            if (startUnit == null) return;
            int x = startUnit.X, y = startUnit.Y;

            ITile[] continent = Map.ContinentTiles(Map[x, y].ContinentId).ToArray();
            IUnit[] unitsOnContinent = Units.Where(u => continent.Any(c => (c.X == u.X && c.Y == u.Y))).ToArray();

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

            Players[player].Handicap = handicap;
        }

        private void ApplyBonus(byte player)
        {
            byte bonus = (byte)(Players.Max(p => p.Handicap) - Players[player].Handicap);
            IUnit startUnit = Units.Where(u => u.Owner == player).FirstOrDefault();
            if (startUnit == null) return;
            int x = startUnit.X, y = startUnit.Y;

            if (bonus >= 4)
            {
                // If the Bonus value of the civ is 4 or higher, then the civ is granted an extra Settlers unit, for a total of two Settlers units.
                // In this case, the Bonus value is reduced by 3 afterwards.
                IUnit unit = UnitsFactory.CreateUnit(UnitType.Settlers, x, y);
                unit.Owner = player;
                Units.Add(unit);

                bonus -= 3;
            }

            // If the Bonus value is (still) greater than zero, then the civ gains a number of technologies equal to the Bonus value.
            while (bonus > 0)
            {
                IAdvance[] available = Players[player].AvailableResearch.ToArray();
                int advanceId = Common.Random.Next(0, 72);
                for (int i = 0; i < 1000; ++i)
                {
                    if (!available.Any(a => a.Id == (advanceId + i) % 72))
                        continue;

                    IAdvance advance = available.First(a => a.Id == (advanceId + i) % 72);
                    SetAdvanceOrigin(advance, null);
                    Players[player].AddAdvance(advance, false);

                    break;
                }

                --bonus;
            }
        }
    }
}
