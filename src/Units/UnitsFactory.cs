// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Units;

namespace CivOne.src.Units
{
    static class UnitsFactory
    {
        public static IUnit CreateUnit(UnitType type, int x, int y)
        {
            IUnit unit;
            switch (type)
            {
                case UnitType.Settlers: unit = new Settlers(); break;
                case UnitType.Militia: unit = new Militia(); break;
                case UnitType.Phalanx: unit = new Phalanx(); break;
                case UnitType.Legion: unit = new Legion(); break;
                case UnitType.Musketeers: unit = new Musketeers(); break;
                case UnitType.Riflemen: unit = new Riflemen(); break;
                case UnitType.Cavalry: unit = new Cavalry(); break;
                case UnitType.Knights: unit = new Knights(); break;
                case UnitType.Catapult: unit = new Catapult(); break;
                case UnitType.Cannon: unit = new Cannon(); break;
                case UnitType.Chariot: unit = new Chariot(); break;
                case UnitType.Armor: unit = new Armor(); break;
                case UnitType.MechInf: unit = new MechInf(); break;
                case UnitType.Artillery: unit = new Artillery(); break;
                case UnitType.Fighter: unit = new Fighter(); break;
                case UnitType.Bomber: unit = new Bomber(); break;
                case UnitType.Trireme: unit = new Trireme(); break;
                case UnitType.Sail: unit = new Sail(); break;
                case UnitType.Frigate: unit = new Frigate(); break;
                case UnitType.Ironclad: unit = new Ironclad(); break;
                case UnitType.Cruiser: unit = new Cruiser(); break;
                case UnitType.Battleship: unit = new Battleship(); break;
                case UnitType.Submarine: unit = new Submarine(); break;
                case UnitType.Carrier: unit = new Carrier(); break;
                case UnitType.Transport: unit = new Transport(); break;
                case UnitType.Nuclear: unit = new Nuclear(); break;
                case UnitType.Diplomat: unit = new Diplomat(); break;
                case UnitType.Caravan: unit = new Caravan(); break;
                default: return null;
            }
            unit.X = x;
            unit.Y = y;
            return unit;
        }
    }
}
