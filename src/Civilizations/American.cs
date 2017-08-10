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
	internal class American : BaseCivilization<Lincoln>
	{
		public American() : base(5, 5, "American", "Americans", "linc")
		{
			StartX = 12;
			StartY = 18;
			CityNames = new string[]
			{
				"Washington",
				"New York",
				"Boston",
				"Philadelphia",
				"Atlanta",
				"Chicago",
				"Buffalo",
				"St. Louis",
				"Detroit",
				"New Orleans",
				"Baltimore",
				"Denver",
				"Cincinnati",
				"Dallas",
				"Los Angeles",
				"Las Vegas"
			};
		}
	}
}