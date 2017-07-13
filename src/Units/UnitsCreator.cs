// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.


using CivOne.Enums;
using CivOne.Interfaces;

namespace CivOne.Units
{
    public class UnitsCreator
    {
        public static IUnit CreateUnit(Unit type, int x, int y)
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


    }
}
