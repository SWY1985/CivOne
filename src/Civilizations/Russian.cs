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
	internal class Russian : BaseCivilization<Stalin>
	{
		public Russian() : base(Civilization.Russians, "Russian", "Russians", "stal")
		{
			StartX = 44;
			StartY = 12;
			CityNames = new string[]
			{
				"Moscow",
				"Leningrad",
				"Kiev",
				"Minsk",
				"Smolensk",
				"Odessa",
				"Sevastopol",
				"Tblisi",
				"Sverdlovsk",
				"Yakutsk",
				"Vladivostok",
				"Novograd",
				"Krasnoyarsk",
				"Riga",
				"Rostov",
				"Atrakhan"
			};
		}
	}
}