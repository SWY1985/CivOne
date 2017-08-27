// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Drawing;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Units;

using static CivOne.Enums.Direction;

namespace CivOne.Tiles
{
	internal abstract class BaseTile : BaseInstance, ITile
	{
		private static IBitmap[] _icons = new Picture[12];
		public virtual IBitmap Icon
		{
			get
			{
				int terrainId = (int)Type;
				if (terrainId == 12) terrainId = 2;
				if (_icons[terrainId] == null)
				{
					switch (Type)
					{
						case Terrain.Arctic:
							_icons[terrainId] = Resources["ICONPGT1"][108, 1, 108, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0)
								.FillRectangle(106, 0, 2, 86, 0);
							break;
						case Terrain.Desert:
							_icons[terrainId] = Resources["ICONPGT2"][1, 1, 108, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0)
								.FillRectangle(106, 0, 2, 86, 0);
							break;
						case Terrain.Forest:
							_icons[terrainId] = Resources["ICONPGT2"][215, 1, 104, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
							break;
						case Terrain.Grassland1:
						case Terrain.Grassland2:
							_icons[terrainId] = Resources["ICONPGT2"][108, 1, 108, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0)
								.FillRectangle(106, 0, 2, 86, 0);
							break;
						case Terrain.Hills:
							_icons[terrainId] = Resources["ICONPGT2"][108, 88, 108, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0)
								.FillRectangle(106, 0, 2, 86, 0);
							break;
						case Terrain.Jungle:
							_icons[terrainId] = Resources["ICONPGT1"][1, 88, 108, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0)
								.FillRectangle(106, 0, 2, 86, 0);
							break;
						case Terrain.Mountains:
							_icons[terrainId] = Resources["ICONPGT2"][215, 88, 104, 86]
								.ColourReplace(253, 0);
							break;
						case Terrain.Ocean:
							_icons[terrainId] = Resources["ICONPGT1"][108, 88, 108, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0)
								.FillRectangle(106, 0, 2, 86, 0);
							break;
						case Terrain.Plains:
							_icons[terrainId] = Resources["ICONPGT2"][1, 88, 108, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0)
								.FillRectangle(106, 0, 2, 86, 0);
							break;
						case Terrain.River:
							_icons[terrainId] = Resources["ICONPGT1"][215, 88, 104, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
							break;
						case Terrain.Swamp:
							_icons[terrainId] = Resources["ICONPGT1"][215, 1, 104, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
							break;
						case Terrain.Tundra:
							_icons[terrainId] = Resources["ICONPGT1"][1, 1, 108, 86]
								.ColourReplace((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0)
								.FillRectangle(106, 0, 2, 86, 0);
							break;
					}
				}
				return _icons[terrainId];
			}
		}

		public Terrain Type { get; protected set; }
		public string Name { get; protected set; }
		public byte PageCount => 1;
		public Picture DrawPage(byte pageNumber) => new Picture(320, 200);
		
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
		public virtual sbyte BaseTrade { get; }
		public abstract sbyte IrrigationFoodBonus { get; }
		public abstract byte IrrigationCost { get; }
		public abstract sbyte MiningShieldBonus { get; }
		public abstract byte MiningCost { get; }
		
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
						foreach (Direction direction in new[] { North, East, South, West })
						{
							if (this.GetBorderType(direction) != Terrain.Ocean)
								output += (byte)direction;
						}
						break;
					case Terrain.River:
						foreach (Direction direction in new[] { North, East, South, West })
						{
							Terrain borderType;
							if ((borderType = this.GetBorderType(direction)) == type || borderType == Terrain.Ocean)
								output += (byte)direction;
						}
						break;
					default:
						foreach (Direction direction in new[] { North, East, South, West })
						{
							if (this.GetBorderType(direction) == type)
								output += (byte)direction;
						}
						break;
				}
				return output;
			}
		}
		
		private bool _road;
		public virtual bool Road
		{
			get
			{
				return _road && !RailRoad;
			}
			set
			{
				_road = value;
			}
		}
		public virtual bool RailRoad { get; set; }
		public virtual bool Irrigation { get; set; }
		public virtual bool Mine { get; set; }
		public virtual bool Fortress { get; set; }
		public virtual bool Hut { get; set; }
		public byte Visited { get; private set; }
		public void Visit(byte owner)
		{
			if (((int)Visited & (0x01 << owner)) != 0) return;
			Visited = (byte)(Visited + (0x01 << owner));
		}

		public virtual bool IsOcean => false;
		
		// This method is used to calculate whether a river or grassland tile is special.
		protected bool AlternateSpecial() => ((X + Y) % 4 == 0) || ((X + Y) % 4 == 3);
		public City City => Game?.GetCity(X, Y);
		public IUnit[] Units => Game?.GetUnits(X, Y);

		public ITile this[int relativeX, int relativeY] => Map[X + relativeX, Y + relativeY];
		
		protected BaseTile(int x, int y, bool special = false)
		{
			X = x;
			Y = y;
			Special = special;
		}
	}
}