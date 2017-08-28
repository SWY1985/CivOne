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
	internal class Ocean : BaseTile
	{
		public override byte Movement => 1;
		public override byte Defense => 2;
		public override sbyte Food => (sbyte)(Special ? 2 : 1);
		public override sbyte Shield => 0;
		public override sbyte Trade => (sbyte)(2 + (Road || RailRoad ? 1 : 0));
		public override sbyte IrrigationFoodBonus => -1;
		public override byte IrrigationCost => 0;
		public override sbyte MiningShieldBonus => -1;
		public override byte MiningCost => 0;
		public override bool IsOcean => true;
		
		public Ocean(int x, int y, bool special) : base(x, y, special)
		{
			Type = Terrain.Ocean;
			Name = "Ocean";
		}
		public Ocean() : this(-1, -1, false)
		{
		}
	}
}