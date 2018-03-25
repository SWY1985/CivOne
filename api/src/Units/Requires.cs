// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Enums;

namespace CivOne.Units
{
	public class Requires : BaseAttribute
	{
		public Advance Value => GetValue<Advance>();

		/// <summary>
		/// Modify the advance with which the unit becomes available.
		/// </summary>
		/// <param name="value">The new required advance for the unit.</param>
		public Requires(Advance value) : base(typeof(Advance), value)
		{
		}
	}
}