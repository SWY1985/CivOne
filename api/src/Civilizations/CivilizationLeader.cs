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

namespace CivOne.Civilizations
{
	public class CivilizationLeader : BaseAttribute
	{
		private static bool InRange(object value) => Enum.IsDefined(typeof(Leader), value) && ((Leader)value) != Leader.Atilla;

		public Leader Value => GetValue<Leader>();

		/// <summary>
		/// Modify the civilization leader.
		/// </summary>
		/// <param name="leader">The new leader for this civilization.</param>
		public CivilizationLeader(Leader leader) : base(typeof(Leader), leader, InRange)
		{
		}
	}
}