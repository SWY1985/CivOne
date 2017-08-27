// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;

namespace CivOne.Tiles
{
	internal class Forest : BaseTile
	{
		public override byte Movement => 2;
		public override byte Defense => 3;
		public override sbyte Food => (sbyte)(Special ? 2 : 1);
		public override sbyte Shield => 2;
		public override sbyte Trade => 0;
		public override sbyte IrrigationFoodBonus => 6;
		public override byte IrrigationCost => 5;
		public override sbyte MiningShieldBonus => -1;
		public override byte MiningCost => 0;
		
		public Forest(int x, int y, bool special) : base(x, y, special)
		{
			Type = Terrain.Forest;
			Name = "Forest";
		}
		public Forest() : this(-1, -1, false)
		{
		}
	}
}