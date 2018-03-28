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

namespace CivOne.Leaders
{
	public class Aggression : BaseAttribute
	{
		private static bool InRange(object value) => Enum.IsDefined(typeof(AggressionLevel), value);

		public string Value => GetValue<string>();

		/// <summary>
		/// Modify the leader aggression level.
		/// </summary>
		/// <param name="value">The new agression level for the leader.</param>
		public Aggression(AggressionLevel value) : base(typeof(AggressionLevel), value, InRange)
		{
		}
	}
}