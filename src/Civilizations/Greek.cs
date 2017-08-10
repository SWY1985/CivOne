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
	internal class Greek : BaseCivilization<Alexander>
	{
		public Greek() : base(6, 6, "Greek", "Greeks", "alex")
		{
			StartX = 39;
			StartY = 18;
			CityNames = new string[]
			{
				"Athens",
				"Sparta",
				"Corinth",
				"Delphi",
				"Eretria",
				"Pharsalos",
				"Argos",
				"Mycenae",
				"Herakleia",
				"Antioch",
				"Ephesos",
				"Rhodes",
				"Knossos",
				"Troy",
				"Pergamon",
				"Miletos"
			};
		}
	}
}