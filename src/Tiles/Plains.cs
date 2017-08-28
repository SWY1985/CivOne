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
	internal class Plains : BaseTile
	{
		public override byte Movement => 1;
		public override byte Defense => 2;
		public override sbyte Food => (sbyte)(1 + (Irrigation ? 1 : 0));
		public override sbyte Shield => (sbyte)(Special ? 3 : 1);
		public override sbyte Trade => (sbyte)((Road || RailRoad) ? 1 : 0);
		public override sbyte IrrigationFoodBonus => -2;
		public override byte IrrigationCost => 5;
		public override sbyte MiningShieldBonus => 2;
		public override byte MiningCost => 15;
		
		public Plains(int x, int y, bool special) : base(x, y, special)
		{
			Type = Terrain.Plains;
			Name = "Plains";
		}
		public Plains() : this(-1, -1, false)
		{
		}
	}
}