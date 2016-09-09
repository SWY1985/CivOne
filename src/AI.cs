// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Interfaces;
using CivOne.Tiles;
using CivOne.Units;

namespace CivOne
{
	internal class AI
	{
		private static Map Map
		{
			get
			{
				return Map.Instance;
			}
		}

		internal static void MoveUnit(IUnit unit)
		{
			if (unit is Settlers)
			{
				if (!((Map[unit.X, unit.Y] is Grassland) || (Map[unit.X, unit.Y] is River) || (Map[unit.X, unit.Y] is Plains)))
				{
					for (int i = 0; i < 1000; i++)
					{
						int relX = Common.Random.Next(-1, 2);
						int relY = Common.Random.Next(-1, 2);
						if (relX == 0 && relY == 0) continue;
						if (Map[unit.X, unit.Y][relX, relY] is Ocean) continue;
						unit.MoveTo(relX, relY);
						return;
					}
					unit.SkipTurn();
					return;
				} 
				Game.Instance.FoundCity(unit.X, unit.Y);
			}
			else
			{
				unit.Fortify = true;
			}
		}
	}
}