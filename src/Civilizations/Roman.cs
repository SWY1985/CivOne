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
	internal class Roman : ICivilization
	{
		public string Name
		{
			get { return "Roman"; }
		}

		public string NamePlural
		{
			get { return "Romans"; }
		}

		public string LeaderName
		{
			get { return "Ceasar"; }
		}

		public byte PreferredPlayerNumber
		{
			get { return 1; }
		}
		
		public string[] CityNames
		{
			get
			{
				return new string[]
				{
					"Rome",
					"Caesarea",
					"Carthage",
					"Nicopolis",
					"Byzantium",
					"Brundisium",
					"Syracuse",
					"Antioch",
					"Palmyra",
					"Cyrene",
					"Gordion",
					"Tyrus",
					"Jerusalem",
					"Seleucia",
					"Ravenna",
					"Artaxata"
				};
			}
		}
	}
}