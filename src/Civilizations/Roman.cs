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
	internal class Roman : BaseCivilization<Caesar>
	{
		public Roman() : base(1, 1, "Roman", "Romans", "ceas")
		{
			StartX = 36;
			StartY = 19;
			CityNames = new string[]
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