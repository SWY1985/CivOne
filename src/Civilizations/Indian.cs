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
	internal class Indian : BaseCivilization<Gandhi>
	{
		public Indian() : base(7, 7, "Indian", "Indians")
		{
			StartX = 57;
			StartY = 24;
			CityNames = new string[]
			{
				"Delhi",
				"Bombay",
				"Madras",
				"Bangalore",
				"Calcutta",
				"Lahore",
				"Karachi",
				"Kolhapur",
				"Jaipur",
				"Hyderbad",
				"Bengal",
				"Chittagong",
				"Punjab",
				"Dacca",
				"Indus",
				"Ganges"
			};
		}
	}
}