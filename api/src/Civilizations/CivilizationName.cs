// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.Civilizations
{
	public struct CivilizationName
	{
		internal bool Valid
		{
			get
			{
				if (Name.Length < 1 || Plural.Length < 1) return false;
				if (Name.Length > 11 || Plural.Length > 12) return false;
				return true;
			}
		}

		public string Name { get; private set; }
		public string Plural { get; private set; }

		internal CivilizationName(string name, string plural)
		{
			Name = name;
			Plural = plural;
		}
	}
}