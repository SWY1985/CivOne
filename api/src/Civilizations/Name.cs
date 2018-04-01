// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;

namespace CivOne.Civilizations
{
	public class Name : BaseAttribute
	{
		public static bool InRange(object value) => ((CivilizationName)value).Valid;

		public CivilizationName Value => GetValue<CivilizationName>();

		/// <summary>
		/// Modifiy the civilization name.
		/// </summary>
		/// <param name="name">The new name for the civilization. (example: Roman)</param>
		/// <param name="namePlural">The new plural name for the civilization. (example: Romans)</param>
		public Name(string name, string namePlural) : base(typeof(CivilizationName), new CivilizationName(name, namePlural), InRange)
		{
		}
	}
}