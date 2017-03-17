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
	internal class Tundra : BaseTile
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
				return (sbyte)(Special ? 2 : 1);
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
				return 0;
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
		
		public Tundra(int x, int y, bool special) : base(x, y, special)
		{
			Type = Terrain.Tundra;
			Name = "Tundra";
		}
		public Tundra() : this(-1, -1, false)
		{
		}
	}
}