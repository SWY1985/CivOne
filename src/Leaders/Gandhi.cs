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
	public class Gandhi : BaseLeader
	{
		protected override Civilization Civilization => Civilization.Indians;

		public Gandhi() : base("M.Gandhi", "KING02", 43, 33)
		{
			Aggression = AggressionLevel.Friendly;
			Development = DevelopmentLevel.Perfectionist;
		}
	}
}