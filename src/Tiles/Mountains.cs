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
	internal class Mountains : BaseTile
	{
		public override byte Movement
		{
			get
			{
				return 3;
			}
		}
		
		public override byte Defense
		{
			get
			{
				return 6;
			}
		}
		
		public override sbyte Food
		{
			get
			{
				return 0; 
			}
		}
		
		public override sbyte Shield
		{
			get
			{
				sbyte output = (sbyte)(1 + (Mine ? 1 : 0));
				if (RailRoad) output = (sbyte)Math.Floor((double)output * 1.5);
				return output;
			}
		}
		
		public override sbyte Trade
		{
			get
			{
				return (sbyte)(Special ? 5 + (MonarchyCommunist ? 1 : 0) + (RepublicDemocratic ? 2 : 0) : 0);
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
				return -2;
			}
		}
		
		public override byte MiningCost
		{
			get
			{
				return 10;
			}
		}
		
		public Mountains(int x, int y, bool special) : base(x, y, special)
		{
			Type = Terrain.Mountains;
			Name = "Mountains";
		}
		public Mountains() : this(-1, -1, false)
		{
		}
	}
}