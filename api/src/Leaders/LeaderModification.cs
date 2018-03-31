// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;

namespace CivOne.Leaders
{
	public abstract class LeaderModification : IModification
	{
		public Leader Leader { get; }

		public AttributeValue<string> Name => AttributeValue<string>.Set(this.GetAttribute<Name>());
		public AttributeValue<AggressionLevel> Aggression => AttributeValue<AggressionLevel>.Set(this.GetAttribute<Aggression>());
		public AttributeValue<DevelopmentLevel> Development => AttributeValue<DevelopmentLevel>.Set(this.GetAttribute<Development>());
		public AttributeValue<MilitarismLevel> Militarism => AttributeValue<MilitarismLevel>.Set(this.GetAttribute<Militarism>());

		/// <summary>
		/// Modify an existing leader.
		/// </summary>
		/// <param name="civilizationId">The leader's civilization of the leader to override.</param>
		public LeaderModification(Leader leader)
		{
			Leader = leader;
		}
	}
}