// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Linq;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.Units;

namespace CivOne
{
	internal class City
	{
		internal byte X;
		internal byte Y;
		internal byte Owner;
		internal string Name;
		internal byte Size;
		internal int Shields { get; private set; }
		internal IProduction CurrentProduction { get; private set; }

		internal IEnumerable<ITile> ResourceTiles
		{
			get
			{
				ITile[,] tiles = CityRadius;
				for (int xx = 0; xx < 5; xx++)
				for (int yy = 0; yy < 5; yy++)
				{
					if (tiles[xx, yy] == null) continue;
					
					ITile tile = tiles[xx, yy];
					if (tile.X == X && tile.Y == Y) yield return tile;
				}
			}
		}

		private Player Player
		{
			get
			{
				return Game.Instance.GetPlayer(Owner);
			}
		}

		public IEnumerable<IProduction> AvailableProduction
		{
			get
			{
				foreach (IUnit unit in Reflect.GetUnits().Where(u => Player.ProductionAvailable(u)))
				{
					if (unit.Class == UnitClass.Water && !Map.Instance.GetTile(X, Y).GetBorderTiles().Any(t => t.IsOcean)) continue;
					yield return unit;
				}
			}
		}

		public void SetProduction(IProduction production)
		{
			CurrentProduction = production;
		}

		public void Buy()
		{
			// DEBUG CODE
			Shields = (int)CurrentProduction.Price * 10;
		}

		public int Population
		{
			get
			{
				int output = 0;
				for (int i = 1; i <= Size; i++)
				{
					output += 10000 * i;
				}
				return output;
			}
		}

		public ITile[,] CityRadius
		{
			get
			{
				Player player = Game.Instance.GetPlayer(Owner);
				ITile[,] tiles = Map.Instance.GetMapPart(X - 2, Y - 2, 5, 5);
				for (int xx = 0; xx < 5; xx++)
				for (int yy = 0; yy < 5; yy++)
				{
					ITile tile = tiles[xx, yy];
					if (tile == null) continue;
					if ((xx == 0 || xx == 4) && (yy == 0 || yy == 4)) tiles[xx, yy] = null;
					if (!player.Visible(tile)) tiles[xx, yy] = null;
				}
				return tiles;
			}
		}

		internal void NewTurn()
		{
			// Temporary code
			Shields++;
			if (Shields >= (int)CurrentProduction.Price * 10)
			{
				Shields = 0;
				if (CurrentProduction is IUnit)
				{
					Game.Instance.CreateUnit((CurrentProduction as IUnit).Type, X, Y, Owner);
				}
			}
		}

		internal City()
		{
			CurrentProduction = new Militia();
		}
	}
}