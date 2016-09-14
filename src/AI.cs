// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Linq;
using CivOne.Interfaces;
using CivOne.Tasks;
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

		internal static void Move(IUnit unit)
		{
			if (unit.Owner == 0)
			{
				// Barbarians
				// Until confrontation has been implemented, barbarians delete themselves
				Game.Instance.DisbandUnit(unit);
			}
			else if (unit is Settlers)
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
				GameTask.Enqueue(Orders.NewCity(unit as Settlers));
			}
			else
			{
				unit.Fortify = true;
			}
		}

		internal static void ChooseResearch(Player player)
		{
			if (player.CurrentResearch != null) return;
			
			IAdvance[] advances = player.AvailableResearch.ToArray();
			
			// No further research possible
			if (advances.Length == 0) return;

			player.CurrentResearch = advances[Common.Random.Next(0, advances.Length)];

			Console.WriteLine($"AI: {player.LeaderName} of the {player.TribeNamePlural} starts researching {player.CurrentResearch.Name}.");
		}
	}
}