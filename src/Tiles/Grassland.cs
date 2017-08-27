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
	internal class Grassland : BaseTile
	{
		public override byte Movement => 1;
		public override byte Defense => 2;
		public override sbyte Food => 2;
		public override sbyte Shield => (sbyte)(Special ? 1 : 0);
		public override sbyte Trade => (sbyte)((Road || RailRoad) ? 1 : 0);
		public override sbyte IrrigationFoodBonus => -2;
		public override byte IrrigationCost => 5;
		public override sbyte MiningShieldBonus => 2;
		public override byte MiningCost => 10;
		
		private Terrain CalculateTileType()
		{
			if ((((X * 7) + (Y * 11)) & 0x02) == 0)
				return Terrain.Grassland2;
			return Terrain.Grassland1;
		}
		
		public Grassland(int x, int y) : base(x, y, false)
		{
			Type = CalculateTileType();
			Special = AlternateSpecial();
			Name = "Grassland";
		}
		public Grassland() : this(-1, -1)
		{
			Type = Terrain.Grassland1;
		}
	}
}