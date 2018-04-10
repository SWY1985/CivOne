// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Leaders;

namespace CivOne.Civilizations
{
	internal class Barbarian : BaseCivilization<Atilla>
	{
		internal static bool IsSeaSpawnTurn => Game.Started && (Game.GameTurn % 8 == 0) && (Game.GameTurn > 150 || Game.GameTurn >= (5 - Game.Difficulty) * 32);

		public Barbarian() : base(Civilization.Barbarians, "Barbarian", "Barbarians")
		{
			StartX = 255;
			StartY = 255;
			CityNames = new string[]
			{
				"Mecca",
				"Naples",
				"Sidon",
				"Tyre",
				"Tarsus",
				"Issus",
				"Cunaxa",
				"Cremona",
				"Cannae",
				"Capua",
				"Turin",
				"Genoa",
				"Utica",
				"Crete",
				"Damascus",
				"Verona",
				"Salamis",
				"Lisbon",
				"Hamburg",
				"Prague",
				"Salzburg",
				"Bergen",
				"Venice",
				"Milan",
				"Ghent",
				"Pisa",
				"Cordoba",
				"Seville",
				"Dublin",
				"Toronto",
				"Melbourne",
				"Sydney"
			};
		}
	}
}