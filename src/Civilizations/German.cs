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
	internal class German : ICivilization
	{
		public string Name
		{
			get { return "German"; }
		}

		public string NamePlural
		{
			get { return "Germans"; }
		}

		public string LeaderName
		{
			get { return "Frederick"; }
		}

		public byte PreferredPlayerNumber
		{
			get { return 3; }
		}
		
		public string[] CityNames
		{
			get
			{
				return new string[]
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
}