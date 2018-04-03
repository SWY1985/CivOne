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
	internal class English : BaseCivilization<Elizabeth>
	{
		public English() : base(Civilization.English, "English", "English", "eliz")
		{
			StartX = 31;
			StartY = 14;
			CityNames = new string[]
			{
				"London",
				"Coventry",
				"Birmingham",
				"Dover",
				"Nottingham",
				"York",
				"Liverpool",
				"Brighton",
				"Oxford",
				"Reading",
				"Exeter",
				"Cambridge",
				"Hastings",
				"Canterbury",
				"Banbury",
				"Newcastle"
			};
		}
	}
}