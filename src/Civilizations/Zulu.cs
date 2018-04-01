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
	internal class Zulu : BaseCivilization<Shaka>
	{
		protected override Civilization Civilization => Civilization.Zulus;
		
		public Zulu() : base(9, 2, "Zulu", "Zulus", "shak")
		{
			StartX = 42;
			StartY = 42;
			CityNames = new string[]
			{
				"Zimbabwe",
				"Ulundi",
				"Bapedi",
				"Hlobane",
				"Isandhlwala",
				"Intombe",
				"Mpondo",
				"Ngome",
				"Swazi",
				"Tugela",
				"Umtata",
				"Umfolozi",
				"Ibabanago",
				"Isipezi",
				"Amatikulu",
				"Zunquin"
			};
		}
	}
}