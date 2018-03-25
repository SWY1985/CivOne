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
	public class Moves : BaseAttribute
	{
		private static bool InRange(object value) => (byte)value > 0 && (byte)value <= 16;

		public byte Value => GetValue<byte>();

		/// <summary>
		/// Modify the number of moves a unit gets each turn.
		/// </summary>
		/// <param name="value">The new number of moves for the unit. (valid range: 1 to 16)</param>
		public Moves(byte value) : base(typeof(byte), value, InRange)
		{
		}
	}
}