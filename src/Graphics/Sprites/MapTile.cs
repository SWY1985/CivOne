// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.IO;
using CivOne.Tiles;

using static CivOne.Enums.Direction;

namespace CivOne.Graphics.Sprites
{
	public static class MapTile
	{
		private static Free Free => Free.Instance;
		private static Resources Resources => Resources.Instance;
		private static Settings Settings => Settings.Instance;

		private static bool GFX256 => (Settings.GraphicsMode == GraphicsMode.Graphics256);

		private static Bytemap GetLandBase()
		{
			string picFile = (GFX256 ? "SP257" : "SPRITES");
			if (!Resources.Exists(picFile))
				return Free.LandBase.Bitmap;
			if (!GFX256)
				return Resources[picFile].Bitmap[0, 80, 16, 16];
			return Resources[picFile].Bitmap[0, 64, 16, 16];
		}

		private static Bytemap GetOceanBase()
		{
			string picFile = (GFX256 ? "TER257" : "SPRITES");
			if (!Resources.Exists(picFile))
				return Free.OceanBase.Bitmap;
			if (!GFX256)
				return null;
			return Resources[picFile].Bitmap[0, 160, 16, 16];
		}
		
		private static Bytemap GetRiver(Direction directions)
		{
			string picFile = (GFX256 ? "SP257" : "SPRITES");
			if (!Resources.Exists(picFile))
				return null;
			return Resources[picFile].Bitmap[(int)directions * 16, 80, 16, 16];
		}

		private static Bytemap GetTileLayer<T>(Direction directions) where T : ITile, new()
		{
			int terrainId = (int)new T().Type;
			string picFile = (GFX256 ? "SP257" : "SPRITES");
			if (!Resources.Exists(picFile))
			{
				switch (new T().Type)
				{
					case Terrain.Forest: return Free.Forest.Bitmap;
					case Terrain.Plains: return Free.Plains.Bitmap;
				}
				return null;
			}
			return Resources["TER257"].Bitmap[(int)directions * 16, terrainId * 16, 16, 16];
		}

		public static readonly ISprite LandBase = new CachedSprite(GetLandBase);
		public static readonly ISprite OceanBase = new CachedSprite(GetOceanBase);
		public static readonly ISpriteCollection<Direction> Arctic = new CachedSpriteCollection<Direction>(GetTileLayer<Arctic>);
		public static readonly ISpriteCollection<Direction> Desert = new CachedSpriteCollection<Direction>(GetTileLayer<Desert>);
		public static readonly ISpriteCollection<Direction> Forest = new CachedSpriteCollection<Direction>(GetTileLayer<Forest>);
		public static readonly ISpriteCollection<Direction> Grassland = new CachedSpriteCollection<Direction>(GetTileLayer<Grassland>);
		public static readonly ISpriteCollection<Direction> Hills = new CachedSpriteCollection<Direction>(GetTileLayer<Hills>);
		public static readonly ISpriteCollection<Direction> Jungle = new CachedSpriteCollection<Direction>(GetTileLayer<Jungle>);
		public static readonly ISpriteCollection<Direction> Mountains = new CachedSpriteCollection<Direction>(GetTileLayer<Mountains>);
		public static readonly ISpriteCollection<Direction> Plains = new CachedSpriteCollection<Direction>(GetTileLayer<Plains>);
		public static readonly ISpriteCollection<Direction> River = new CachedSpriteCollection<Direction>(GetRiver);
		public static readonly ISpriteCollection<Direction> Swamp = new CachedSpriteCollection<Direction>(GetTileLayer<Swamp>);
		public static readonly ISpriteCollection<Direction> Tundra = new CachedSpriteCollection<Direction>(GetTileLayer<Tundra>);

		public static ISprite TileBase(ITile tile) => tile.IsOcean ? OceanBase : LandBase;
		public static ISprite TileLayer(ITile tile)
		{
			Direction directions = 0;
			foreach (Direction direction in new[] { North, East, South, West })
			{
				ITile borderTile = tile.GetBorderTile(direction);
				if (borderTile == null) continue;

				switch (tile)
				{
					case Ocean _:
						continue;
					case River _:
						if (borderTile is River || borderTile is Ocean) break;
						continue;
					default:
						if (borderTile.GetType() == tile.GetType()) break;
						continue;
				}

				directions |= direction;
			}

			switch (tile)
			{
				case Arctic _: return Arctic[directions];
				case Desert _: return Desert[directions];
				case Forest _: return Forest[directions];
				case Grassland _: return Grassland[directions];
				case Hills _: return Hills[directions];
				case Jungle _: return Jungle[directions];
				case Mountains _: return Mountains[directions];
				case Plains _: return Plains[directions];
				case River _: return River[directions];
				case Swamp _: return Swamp[directions];
				case Tundra _: return Tundra[directions];
			}

			return null;
		}
	}
}