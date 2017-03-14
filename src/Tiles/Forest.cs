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
	internal class Forest : BaseTile
	{
		public override byte Movement
		{
			get
			{
				return 2;
			}
		}
		
		public override byte Defense
		{
			get
			{
				return 3;
			}
		}
		
		public override sbyte Food
		{
			get
			{
				sbyte output = (sbyte)(Special ? 2 + (AnarchyDespotism ? 0 : 1) : 1);
				if (RailRoad) output = (sbyte)Math.Floor((double)output * 1.5);
				return output;
			}
		}
		
		public override sbyte Shield
		{
			get
			{
				sbyte output = 2;
				if (RailRoad) output = (sbyte)Math.Floor((double)output * 1.5);
				return output;
			}
		}
		
		public override sbyte Trade
		{
			get
			{
				return 0;
			}
		}
		
		public override sbyte IrrigationFoodBonus
		{
			get
			{
				return 6;
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