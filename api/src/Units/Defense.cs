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
	public class Defense : BaseAttribute
	{
		private static bool InRange(object value) => (byte)value >= 0 && (byte)value <= 99;

		public byte Value => GetValue<byte>();

		/// <summary>
		/// Modify the unit defense strength.
		/// </summary>
		/// <param name="value">The new defense strength for the unit. (valid range: 0 to 99)</param>
		public Defense(byte value) : base(typeof(byte), value, InRange)
		{
		}
	}
}