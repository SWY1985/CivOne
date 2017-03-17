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
using CivOne.Templates;

namespace CivOne.Tiles
{
	internal class Grassland : BaseTile
	{
		public override byte Movement
		{
			get
			{
				return 1;
			}
		}
		
		public override byte Defense
		{
			get
			{
				return 2;
			}
		}
		
		public override sbyte Food
		{
			get
			{
				sbyte output = (sbyte)(2 + (Irrigation && !AnarchyDespotism ? 1 : 0)); 
				if (RailRoad) output = (sbyte)Math.Floor((double)output * 1.5);
				return output;
			}
		}
		
		public override sbyte Shield
		{
			get
			{
				sbyte output = (sbyte)(Special ? 1 : 0);
				if (RailRoad) output = (sbyte)Math.Floor((double)output * 1.5);
				return output;
			}
		}
		
		public override sbyte Trade
		{
			get
			{
				return (sbyte)((Road ? 1 : 0) + (RailRoad ? 2 : 0) + (RepublicDemocratic ? 1 : 0));
			}
		}
		
		public override sbyte IrrigationFoodBonus
		{
			get
			{
				return -2;
			}
		}
		
		public override byte IrrigationCost
		{
			get
			{
				return 5;
			}
		}
		
		public override sbyte MiningShieldBonus
		{
			get
			{
				return 2;
			}
		}
		
		public override byte MiningCost
		{
			get
			{
				return 10;
			}
		}
		
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
		}
	}
}