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
	internal class English : ICivilization
	{
		public string Name
		{
			get { return "English"; }
		}

		public string NamePlural
		{
			get { return "English"; }
		}

		public string LeaderName
		{
			get { return "Elizabeth I"; }
		}

		public byte PreferredPlayerNumber
		{
			get { return 6; }
		}

		public byte StartX
		{
			get { return 31; }
		}

		public byte StartY
		{
			get { return 14; }
		}
		
		public string[] CityNames
		{
			get
			{
				return new string[]
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
}