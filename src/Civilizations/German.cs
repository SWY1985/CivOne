// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Leaders;

namespace CivOne.Civilizations
{
	internal class German : BaseCivilization<Frederick>
	{
		public German() : base(3, 3, "German", "Germans", "fred")
		{
			StartX = 38;
			StartY = 15;
			CityNames = new string[]
			{
				"Berlin",
				"Leipzig",
				"Hamburg",
				"Bremen",
				"Frankfurt",
				"Bonn",
				"Nuremberg",
				"Cologne",
				"Hannover",
				"Munich",
				"Stuttgart",
				"Heidelberg",
				"Salzburg",
				"Konigsberg",
				"Dortmond",
				"Brandenburg"
			};
		}
	}
}