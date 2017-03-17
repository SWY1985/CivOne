// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Templates;

namespace CivOne.Tiles
{
	internal class Jungle : BaseTile
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
				return 1;
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
				return (sbyte)(Special ? 3 : 0);
			}
		}
		
		public override sbyte IrrigationFoodBonus
		{
			get
			{
				return 10;
			}
		}
		
		public override byte IrrigationCost
		{
			get
			{
				return 15;
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
				return 15;
			}
		}
		
		public Jungle(int x, int y, bool special) : base(x, y, special)
		{
			Type = Terrain.Jungle;
			Name = "Jungle";
		}
		public Jungle() : this(-1, -1, false)
		{
		}
	}
}