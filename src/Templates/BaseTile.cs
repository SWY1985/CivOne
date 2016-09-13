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
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseTile : ITile
	{
		protected Map Map
		{
			get
			{
				return Map.Instance;
			}
		}

		protected bool AnarchyDespotism
		{
			get
			{
				switch (Game.Instance.CurrentPlayer.Government)
				{
					case Government.Anarchy:
					case Government.Despotism:
						return true;
				}
				return false;
			}
		}

		protected bool MonarchyCommunism
		{
			get
			{
				switch (Game.Instance.CurrentPlayer.Government)
				{
					case Government.Monarchy:
					case Government.Communism:
						return true;
				}
				return false;
			}
		}

		protected bool RepublicDemocracy
		{
			get
			{
				switch (Game.Instance.CurrentPlayer.Government)
				{
					case Government.Republic:
					case Government.Democracy:
						return true;
				}
				return false;
			}
		}

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
		public bool Special { get; protected set; }
		public byte ContinentId { get; set; }
		public byte LandValue { get; set; }
		public byte LandScore
		{
			get
			{
				sbyte score = (sbyte)(Trade + (3 * Food));
				if (!Map.TileIsType(this, Terrain.River, Terrain.Grassland1, Terrain.Grassland2))
				{
					score += (sbyte)(2 * Shield);
				}
				if (MiningShieldBonus < 0)
				{
					score -= (sbyte)(MiningShieldBonus + 1);
				}
				else if (IrrigationFoodBonus < 0)
				{
					score -= (sbyte)(2 * (IrrigationFoodBonus + 1));
				}
				return (byte)score;
			}
		}
		public abstract byte Movement { get; }
		public abstract byte Defense { get; }
		public abstract sbyte Food { get; }
		public abstract sbyte Shield { get; }
		public abstract sbyte Trade { get; }
		public abstract sbyte IrrigationFoodBonus { get; }
		public abstract byte IrrigationCost { get; }
		public abstract sbyte MiningShieldBonus { get; }
		public abstract byte MiningCost { get; }
		
		public ITile GetBorderTile(Direction direction)
		{
			switch (direction)
			{
				case Direction.North: return Map[X, Y - 1];
				case Direction.East: return Map[X + 1, Y];
				case Direction.South: return Map[X, Y + 1];
				case Direction.West: return Map[X - 1, Y];
				case Direction.NorthWest: return Map[X - 1, Y - 1];
				case Direction.NorthEast: return Map[X + 1, Y - 1];
				case Direction.SouthWest: return Map[X - 1, Y + 1];
				case Direction.SouthEast: return Map[X + 1, Y + 1];
			}
			return null;
		}
		
		public IEnumerable<ITile> GetBorderTiles()
		{
			for (int relY = -1; relY <= 1; relY++)
			{
				for (int relX = -1; relX <= 1; relX++)
				{
					if (relX == 0 && relY == 0) continue;
					yield return this[relX, relY];
				}
			}
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
		
		private bool _road = false;
		public virtual bool Road
		{
			get
			{
				if (Game.Started && !_road && City != null) _road = true;
				return _road;
			}
			set
			{
				_road = value;
			}
		}
		private bool _railRoad = false;
		public virtual bool RailRoad
		{
			get
			{
				if (Game.Started && !_railRoad && City != null && Map[X, Y].GetBorderTiles().Any(t => (t as BaseTile)._railRoad || t.City != null)) _railRoad = true;
				return _railRoad;
			}
			set
			{
				_railRoad = value;
			}
		}
		private bool _irrigation = false;
		public virtual bool Irrigation
		{
			get
			{
				if (Game.Started && !_irrigation && City != null) _irrigation = true;
				return _irrigation;
			}
			set
			{
				_irrigation = value;
			}
		}
		public virtual bool Mine { get; set; }
		public virtual bool Hut { get; set; }
		public virtual bool IsOcean
		{
			get
			{
				return false;
			}
		}
		
		// This method is used to calculate whether a river or grassland tile is special.
		protected bool AlternateSpecial()
		{
			return ((X + Y) % 4 == 0) || ((X + Y) % 4 == 3);
		}

		public City City
		{
			get
			{
				return Game.Instance.GetCity(X, Y);
			}
		}

		public IUnit[] Units
		{
			get
			{
				return Game.Instance.GetUnits(X, Y);
			}
		}

		public ITile this[int relativeX, int relativeY]
		{
			get
			{
				return Map[X + relativeX, Y + relativeY];
			}
		}
		
		protected BaseTile(int x, int y, bool special = false)
		{
			X = x;
			Y = y;
			Special = special;
		}
	}
}