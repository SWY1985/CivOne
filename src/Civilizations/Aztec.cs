// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Interfaces;

namespace CivOne.Civilizations
{
	internal class Aztec : ICivilization
	{
		public string Name
		{
			get { return "Aztec"; }
		}

		public string NamePlural
		{
			get { return "Aztecs"; }
		}

		public string LeaderName
		{
			get { return "Montezuma"; }
		}

		public byte PreferredPlayerNumber
		{
			get { return 4; }
		}
		
		public string[] CityNames
		{
			get
			{
				return new string[]
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
}