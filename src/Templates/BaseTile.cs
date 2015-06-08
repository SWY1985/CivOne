// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseTile : ITile
	{
		public virtual Picture Icon
		{
			get
			{
				return TileResources.GetIcon(Type);
			}
		}
		public Terrain Type { get; protected set; }
		public string Name { get; protected set; }
		public byte PageCount
		{
			get
			{
				return 1;
			}
		}
		public Picture DrawPage(byte pageNumber)
		{
			return new Picture(320, 200);
		}
		
		public int X { get; private set; }
		public int Y { get; private set; }
		public bool Special { get; private set; }
		
		public ITile GetBorderTile(Direction direction)
		{
			Map map = Map.Instance;
			switch (direction)
			{
				case Direction.North: return map.GetTile(X, Y - 1);
				case Direction.East: return map.GetTile(X + 1, Y);
				case Direction.South: return map.GetTile(X, Y + 1);
				case Direction.West: return map.GetTile(X - 1, Y);
				case Direction.NorthWest: return map.GetTile(X - 1, Y - 1);
				case Direction.NorthEast: return map.GetTile(X + 1, Y - 1);
				case Direction.SouthWest: return map.GetTile(X - 1, Y + 1);
				case Direction.SouthEast: return map.GetTile(X + 1, Y + 1);
			}
			return null;
		}
		
		public Terrain GetBorderType(Direction direction)
		{
			ITile tile = GetBorderTile(direction);
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
		
		public virtual bool Road { get; set; }
		public virtual bool Irrigation { get; set; }
		public virtual bool Mine { get; set; }
		public virtual bool Hut { get; set; }
		
		protected BaseTile(int x, int y, bool special = false)
		{
			X = x;
			Y = y;
			Special = special;
		}
	}
}