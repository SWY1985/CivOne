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
	internal class Chinese : BaseCivilization<Mao>
	{
		public Chinese() : base(12, 5, "Chinese", "Chinese", "mao")
		{
			StartX = 66;
			StartY = 19;
			CityNames = new string[]
			{
				"Peking",
				"Shanghai",
				"Canton",
				"Nanking",
				"Tsingtao",
				"Hangchow",
				"Tientsin",
				"Tatung",
				"Macao",
				"Anyang",
				"Shantung",
				"Chinan",
				"Kaifeng",
				"Ningpo",
				"Paoting",
				"Yangchow"
			};
		}
	}
}