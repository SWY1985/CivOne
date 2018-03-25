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
	public class Price : BaseAttribute
	{
		private static bool InRange(object value) => (byte)value > 0 && (byte)value <= 40;

		public byte Value => GetValue<byte>();

		/// <summary>
		/// Modify the unit price.
		/// </summary>
		/// <param name="value">The new price for the unit. Must be multiplied by 10 for human player price in shields. (valid range: 1 to 40)</param>
		public Price(byte value) : base(typeof(byte), value, InRange)
		{
		}
	}
}