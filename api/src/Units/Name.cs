// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;

namespace CivOne.Units
{
	public class Name : BaseAttribute
	{
		private static bool InRange(object value) => (value as string).Length > 0 && (value as string).Length <= 12;

		public string Value => GetValue<string>();

		/// <summary>
		/// Modify the unit name.
		/// </summary>
		/// <param name="value">The new name for the unit. Must be between 1 and 12 characters long.</param>
		public Name(string value) : base(typeof(string), value, InRange)
		{
		}
	}
}