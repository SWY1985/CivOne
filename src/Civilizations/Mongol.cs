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
	internal class Mongol : BaseCivilization<Genghis>
	{
		public Mongol() : base(Civilization.Mongols, "Mongol", "Mongols", "geng")
		{
			StartX = 49;
			StartY = 19;
			CityNames = new string[]
			{
				"Samarkand",
				"Bokhara",
				"Nishapur",
				"Karakorum",
				"Kashgar",
				"Tabriz",
				"Aleppo",
				"Kabul",
				"Ormuz",
				"Basra",
				"Khanbaryk",
				"Khorasan",
				"Shangtu",
				"Kazan",
				"Qyinsay",
				"Kerman"
			};
		}
	}
}