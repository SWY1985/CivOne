// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using Xunit;

namespace CivOne.UnitTests
{
	public class SampleTests
	{
		[Fact]
		public void CityNamesCount()
		{
			int expectedCount = 256;
			int count = Common.AllCityNames.Count();
			bool result = (count == expectedCount);

			Assert.True(result, $"Common.AllCityNames should have {expectedCount} entries, returns {count}.");
		}

		[Fact]
		public void PaletteColourCount()
		{
			int expectedCount = 256;
			int count = Common.DefaultPalette.Count();
			bool result = (count == expectedCount);
			
			Assert.True(result, $"Common.DefaultPalette should have {expectedCount} entries, returns {count}.");
		}
	}
}