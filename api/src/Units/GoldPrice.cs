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
	public class GoldPrice : BaseAttribute
	{
		private static bool InRange(object value) => (short)value >= 0 && (short)value <= 8000;

		public short Value => GetValue<short>();

		/// <summary>
		/// Modify the unit initial gold price.
		/// </summary>
		/// <param name="value">The new gold price for the unit. (valid range: 1 to 8000)</param>
		public GoldPrice(short value) : base(typeof(short), value, InRange)
		{
		}
	}
}