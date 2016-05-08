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
	internal class Zulu : ICivilization
	{
		public string Name
		{
			get { return "Zulu"; }
		}

		public string NamePlural
		{
			get { return "Zulus"; }
		}

		public string LeaderName
		{
			get { return "Shaka"; }
		}

		public byte PreferredPlayerNumber
		{
			get { return 2; }
		}
	}
}