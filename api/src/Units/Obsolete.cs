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
	public class Obsolete : BaseAttribute
	{
		public Advance Value => GetValue<Advance>();

		/// <summary>
		/// Modify the advance with which the unit becomes obsolete.
		/// </summary>
		/// <param name="value">The new obsolete advance for the unit.</param>
		public Obsolete(Advance value) : base(typeof(Advance), value)
		{
		}
	}
}