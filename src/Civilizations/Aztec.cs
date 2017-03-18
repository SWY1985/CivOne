// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Leaders;
using CivOne.Templates;

namespace CivOne.Civilizations
{
	internal class Aztec : BaseCivilization<Montezuma>
	{
		public Aztec() : base(11, 4, "Aztec", "Aztecs", "mont")
		{
			StartX = 5;
			StartY = 23;
			CityNames = new string[]
			{
				"Tenochtitlan",
				"Chiauhtia",
				"Chapultapec",
				"Coatepec",
				"Ayontzinco",
				"Itzapalapa",
				"Itzapam",
				"Mitxcoac",
				"Tucubaya",
				"Tecamac",
				"Tepezinco",
				"Ticoman",
				"Tlaxcala",
				"Xaltocan",
				"Xicalango",
				"Zumpanco"
			};
		}
	}
}