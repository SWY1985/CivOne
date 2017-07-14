// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Interfaces;
using System.Collections.Generic;

namespace CivOne.GameSate
{
    public class GameState
    {
        // Original

        public List<Player> Players { get; set; }
        public List<IUnit> Units { get; set; }
        public ushort GameTurn { get; set; }
        public int CurrentPlayerNumber { get; set; }
        public int Difficulty { get; set; }
        public List<City> Cities { get; set; }
        public Map Map { get; set; }
        public Dictionary<byte, byte> AdvanceOrigin { get; set; }
        public List<string> CityNames { get; set; }
        public Settings Settings { get; set; }
        public ushort AnthologyTurn { get; set; }
        public Player HumanPlayer { get; set; }

        // Extended
    }
}
