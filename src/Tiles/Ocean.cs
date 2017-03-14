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
	internal class Ocean : BaseTile
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
				sbyte output = (sbyte)(Special ? (AnarchyDespotism ? 2 : 3) : 1); 
				if (RailRoad) output = (sbyte)Math.Floor((double)output * 1.5);
				return output;
			}
		}
		
		public override sbyte Shield
		{
			get
			{
				return 0;
			}
		}
		
		public override sbyte Trade
		{
			get
			{
				return (sbyte)(BaseTrade + SpecialTrade);
			}
		}

		public override sbyte BaseTrade
		{
			get
			{
				return (sbyte)(2 + (RepublicDemocratic ? 1 : 0) + (Road ? 1 : 0));
			}
		}		
		
		public override sbyte IrrigationFoodBonus
		{
			get
			{
				return -1;
			}
		}
		
		public override byte IrrigationCost
		{
			get
			{
				return 0;
			}
		}
		
		public override sbyte MiningShieldBonus
		{
			get
			{
				return -1;
			}
		}
		
		public override byte MiningCost
		{
			get
			{
				return 0;
			}
		}
		
		public override bool IsOcean
		{
			get
			{
				return true;
			}
		}
		
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