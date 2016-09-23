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
	internal class Babylonian : ICivilization
	{
		public int Id
		{
			get
			{
				return 2;
			}
		}

		public string Name
		{
			get { return "Babylonian"; }
		}

		public string NamePlural
		{
			get { return "Babylonians"; }
		}

		public string LeaderName
		{
			get { return "Hammurabi"; }
		}

		public byte PreferredPlayerNumber
		{
			get { return 2; }
		}

		public byte StartX
		{
			get { return 45; }
		}

		public byte StartY
		{
			get { return 22; }
		}
		
		public string[] CityNames
		{
			get
			{
				return new string[]
				{
					"Babylon",
					"Sumer",
					"Uruk",
					"Ninevah",
					"Ashur",
					"Ellipi",
					"Akkad",
					"Eridu",
					"Kish",
					"Nippur",
					"Shuruppak",
					"Zariqum",
					"Izibia",
					"Nimrud",
					"Arbela",
					"Zamua"
				};
			}
		}
	}
}