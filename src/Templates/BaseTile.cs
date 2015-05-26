// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseTile : ITile
	{
		public Terrain Type { get; protected set; }
		public string Name { get; protected set; }
		public int X { get; private set; }
		public int Y { get; private set; }
		public bool Special { get; private set; }
		
		private Terrain GetBorderType(Direction direction)
		{
			Map map = Map.Instance;
			ITile tile = null;
			switch (direction)
			{
				case Direction.North: tile = map.GetTile(X, Y - 1); break;
				case Direction.East: tile = map.GetTile(X + 1, Y); break;
				case Direction.South: tile = map.GetTile(X, Y + 1); break;
				case Direction.West: tile = map.GetTile(X - 1, Y); break;
			}
			if (tile == null) return Terrain.None;
			if (tile.Type == Terrain.Grassland2) return Terrain.Grassland1;
			return tile.Type;
		}
		
		public byte Borders
		{
			get
			{
				Terrain type = Type;
				if (type == Terrain.Grassland2) type = Terrain.Grassland1;
				
				byte output = 0;
				ITile tile = null;
				switch (type)
				{
					case Terrain.Ocean:
						foreach (Direction direction in new[] { Direction.North, Direction.East, Direction.South, Direction.West })
						{
							if (GetBorderType(direction) != Terrain.Ocean)
								output += (byte)direction;
						}
						break;
					case Terrain.River:
						foreach (Direction direction in new[] { Direction.North, Direction.East, Direction.South, Direction.West })
						{
							Terrain borderType;
							if ((borderType = GetBorderType(direction)) == type || borderType == Terrain.Ocean)
								output += (byte)direction;
						}
						break;
					default:
						foreach (Direction direction in new[] { Direction.North, Direction.East, Direction.South, Direction.West })
						{
							if (GetBorderType(direction) == type)
								output += (byte)direction;
						}
						break;
				}
				return output;
			}
		}
		
		public BaseTile(int x, int y, bool special = false)
		{
			X = x;
			Y = y;
			Special = special;
		}
	}
}