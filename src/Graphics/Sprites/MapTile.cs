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

		private static bool DrawCoastCorners(ref Picture output, Direction land)
		{
			if (!Resources.Exists("SP299")) return false;

			Picture pic = Resources["SP299"];

			if (land.And(South | East) && land.Not(North | West | SouthWest | NorthEast)) output.AddLayer(pic[224, 100, 16, 16]);
			else if (land.And(North | West) && land.Not(South | East | NorthEast | SouthWest)) output.AddLayer(pic[240, 100, 16, 16]);
			else if (land.And(North | East) && land.Not(South | West | NorthWest | SouthEast)) output.AddLayer(pic[256, 100, 16, 16]);
			else if (land.And(South | West) && land.Not(North | East | SouthEast | NorthWest)) output.AddLayer(pic[272, 100, 16, 16]);
			else return false;
			return true;
		}

		private static void DrawCoastSegments(ref Picture output, Direction land)
		{
			if (!Resources.Exists("TER257")) return;

			Picture pic = Resources["TER257"];
			
			if (land.And(North))
			{
				int xw = land.And(West) ? 80 : land.And(NorthWest) ? 96 : 64;
				int xe = land.And(East) ? 88 : land.And(NorthEast) ? 56 : 24;
				
				output.AddLayer(pic[xw, 176, 8, 8], 0, 0);
				output.AddLayer(pic[xe, 176, 8, 8], 8, 0);
			}
			if (land.And(East))
			{
				int xn = land.And(North) ? 88 : land.And(NorthEast) ? 104 : 72;
				int xs = land.And(South) ? 88 : land.And(SouthEast) ? 56 : 24;
				
				output.AddLayer(pic[xn, 176, 8, 8], 8, 0);
				output.AddLayer(pic[xs, 184, 8, 8], 8, 8);
			}
			if (land.And(South))
			{
				int xw = land.And(West) ? 80 : land.And(SouthWest) ? 48 : 16;
				int xe = land.And(East) ? 88 : land.And(SouthEast) ? 104 : 72;

				output.AddLayer(pic[xw, 184, 8, 8], 0, 8);
				output.AddLayer(pic[xe, 184, 8, 8], 8, 8);
			}
			if (land.And(West))
			{
				int xn = land.And(North) ? 80 : land.And(NorthWest) ? 48 : 16;
				int xs = land.And(South) ? 80 : land.And(SouthWest) ? 96 : 64;
				
				output.AddLayer(pic[xn, 176, 8, 8], 0, 0);
				output.AddLayer(pic[xs, 184, 8, 8], 0, 8);
			}
		}

		private static void DrawCoastDiagonal(ref Picture output, Direction land)
		{
			if (!Resources.Exists("TER257")) return;

			Picture pic = Resources["TER257"];

			if (land.And(NorthWest) && land.Not(North | West)) output.AddLayer(pic[32, 176, 8, 8], 0, 0);
			if (land.And(NorthEast) && land.Not(North | East)) output.AddLayer(pic[40, 176, 8, 8], 8, 0);
			if (land.And(SouthWest) && land.Not(South | West)) output.AddLayer(pic[32, 184, 8, 8], 0, 8);
			if (land.And(SouthEast) && land.Not(South | East)) output.AddLayer(pic[40, 184, 8, 8], 8, 8);
		}

		private static void DrawRiverMouths(ref Picture output, Direction rivers)
		{
			if (!Resources.Exists("TER257")) return;

			Picture pic = Resources["TER257"];

			if (rivers.And(North)) output.AddLayer(pic[128, 176, 16, 16]);
			if (rivers.And(East)) output.AddLayer(pic[144, 176, 16, 16]);
			if (rivers.And(South)) output.AddLayer(pic[160, 176, 16, 16]);
			if (rivers.And(West)) output.AddLayer(pic[176, 176, 16, 16]);
		}

		private static Bytemap GetOceanLayer((Direction Land, Direction Rivers) directions)
		{
			string picFile = (GFX256 ? "TER257" : "SPRITES");
			if (!Resources.Exists(picFile))
				return null;
			if (!GFX256)
				return Resources[picFile].Bitmap[((int)directions.Land & 0xF) * 16, 64, 16, 16];

			Picture output = new Picture(16, 16);
			if (!DrawCoastCorners(ref output, directions.Land))
				DrawCoastSegments(ref output, directions.Land);
			DrawCoastDiagonal(ref output, directions.Land);
			DrawRiverMouths(ref output, directions.Rivers);
			return output.Bitmap;
		}
		
		private static Bytemap GetRiverLayer(Direction directions)
		{
			string picFile = (GFX256 ? "SP257" : "SPRITES");
			if (!Resources.Exists(picFile))
				return null;
			return Resources[picFile].Bitmap[(int)directions * 16, 80, 16, 16];
		}

		private static Bytemap GetTileLayer<T>(Direction directions) where T : ITile, new()
		{
			int terrainId = (int)new T().Type;
			string picFile = (GFX256 ? "TER257" : "SPRITES");
			if (!Resources.Exists(picFile))
			{
				switch (new T().Type)
				{
					case Terrain.Forest: return Free.Forest.Bitmap;
					case Terrain.Plains: return Free.Plains.Bitmap;
				}
				return null;
			}
			if (!GFX256)
				return Resources[picFile].Bitmap[terrainId * 16, (directions == Alternating) ? 0 : 16, 16, 16];
			return Resources[picFile].Bitmap[(int)directions * 16, terrainId * 16, 16, 16];
		}

		private static Bytemap GetSpecial<T>() where T : ITile, new()
		{
			int terrainId = (int)new T().Type;
			string picFile = (GFX256 ? "SP257" : "SPRITES");
			if (!Resources.Exists(picFile))
				return null;
			if (typeof(T) == typeof(Grassland))
				return new Picture(16, 16)
					.AddLayer(Resources[picFile][152, 40, 8, 8].ColourReplace(3, 0), 4, 4).Bitmap;
			return Resources[picFile][terrainId * 16, 112, 16, 16].ColourReplace(3, 0).Bitmap;
		}

		private static Bytemap GetRoad(Direction directions)
		{
			if (directions == Direction.None)
			{
				return new Picture(16, 16).FillRectangle(7, 7, 2, 2, 6).Bitmap;
			}

			string picFile = (GFX256 ? "SP257" : "SPRITES");
			Picture output = new Picture(16, 16);
			Direction[] allDirections = new [] { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };
			for (int i = 0; i < allDirections.Length; i++)
			{
				if (((int)directions & (int)allDirections[i]) == 0) continue;
				output.AddLayer(Resources[picFile][(i * 16), 48, 16, 16], 0, 0);
			}
			return output.Bitmap;
		}

		private static Bytemap GetIrrigation()
		{
			string picFile = (GFX256 ? "SP257" : "SPRITES");
			if (!Resources.Exists(picFile))
				return null;
			return Resources[picFile].Bitmap[64, 32, 16, 16];
		}

		private static Bytemap GetMine()
		{
			string picFile = (GFX256 ? "SP257" : "SPRITES");
			if (!Resources.Exists(picFile))
				return null;
			return Resources[picFile].Bitmap[80, 32, 16, 16];
		}

		private static Bytemap GetFortress()
		{
			string picFile = (GFX256 ? "SP257" : "SPRITES");
			if (!Resources.Exists(picFile))
				return null;
			return Resources[picFile][224, 112, 16, 16]
				.ColourReplace(3, 0).Bitmap;
		}

		private static Bytemap GetHut()
		{
			string picFile = (GFX256 ? "SP257" : "SPRITES");
			if (!Resources.Exists(picFile))
				return null;
			return Resources[picFile][240, 112, 16, 16]
				.ColourReplace(3, 0).Bitmap;
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
		public static readonly ISpriteCollection<(Direction land, Direction rivers)> Ocean = new CachedSpriteCollection<(Direction, Direction)>(GetOceanLayer);
		public static readonly ISpriteCollection<Direction> Plains = new CachedSpriteCollection<Direction>(GetTileLayer<Plains>);
		public static readonly ISpriteCollection<Direction> River = new CachedSpriteCollection<Direction>(GetRiverLayer);
		public static readonly ISpriteCollection<Direction> Swamp = new CachedSpriteCollection<Direction>(GetTileLayer<Swamp>);
		public static readonly ISpriteCollection<Direction> Tundra = new CachedSpriteCollection<Direction>(GetTileLayer<Tundra>);
		public static readonly ISpriteCollection<Direction> Road = new CachedSpriteCollection<Direction>(GetRoad);
		public static readonly ISprite Irrigation = new CachedSprite(GetIrrigation);
		public static readonly ISprite Mine = new CachedSprite(GetMine);
		public static readonly ISprite Fortress = new CachedSprite(GetFortress);
		public static readonly ISprite Hut = new CachedSprite(GetHut);
		public static readonly ISprite Seals = new CachedSprite(GetSpecial<Arctic>);
		public static readonly ISprite Oasis = new CachedSprite(GetSpecial<Desert>);
		public static readonly ISprite Game = new CachedSprite(GetSpecial<Forest>);
		public static readonly ISprite Shield = new CachedSprite(GetSpecial<Grassland>);
		public static readonly ISprite Coal = new CachedSprite(GetSpecial<Hills>);
		public static readonly ISprite Gems = new CachedSprite(GetSpecial<Jungle>);
		public static readonly ISprite Gold = new CachedSprite(GetSpecial<Mountains>);
		public static readonly ISprite Fish = new CachedSprite(GetSpecial<Ocean>);
		public static readonly ISprite Horses = new CachedSprite(GetSpecial<Plains>);
		public static readonly ISprite Oil = new CachedSprite(GetSpecial<Swamp>);
		public static readonly ISprite TundraGame = new CachedSprite(GetSpecial<Tundra>);

		public static ISprite TileBase(ITile tile) => tile.IsOcean ? OceanBase : LandBase;
		public static ISprite TileLayer(ITile tile)
		{
			Direction directions = None, riverDirections = None;
			if (tile is Ocean)
			{
				foreach (Direction direction in new[] { North, East, South, West, NorthWest, NorthEast, SouthWest, SouthEast })
				{
					ITile borderTile = tile.GetBorderTile(direction);
					if (borderTile == null) continue;
					if (borderTile is Ocean) continue;
					directions |= direction;
				}
				foreach (Direction direction in new[] { North, East, South, West })
				{
					ITile borderTile = tile.GetBorderTile(direction);
					if (borderTile == null) continue;
					if (borderTile is River) riverDirections |= direction;
				}
			}
			else
			{
				foreach (Direction direction in new[] { North, East, South, West })
				{
					ITile borderTile = tile.GetBorderTile(direction);
					if (borderTile == null) continue;

					switch (tile)
					{
						case River _:
							if (borderTile is River || borderTile is Ocean) break;
							continue;
						default:
							if (borderTile.GetType() == tile.GetType()) break;
							continue;
					}

					directions |= direction;
				}
			}

			if (!(tile is River || tile is Ocean) && !GFX256 && Resources.Exists("SPRITES"))
			{
				directions = ((tile.X + tile.Y) % 2 == 1) ? Alternating : Direction.None;
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
				case Ocean _: return Ocean[(directions, riverDirections)];
				case Plains _: return Plains[directions];
				case River _: return River[directions];
				case Swamp _: return Swamp[directions];
				case Tundra _: return Tundra[directions];
			}

			return null;
		}
		public static ISprite TileSpecial(ITile tile)
		{
			if (tile is River || (!tile.Special && tile.Type != Terrain.Grassland2)) return null;
			switch (tile)
			{
				case Arctic _: return Seals;
				case Desert _: return Oasis;
				case Forest _: return Game;
				case Grassland _: return Shield;
				case Hills _: return Coal;
				case Jungle _: return Gems;
				case Mountains _: return Gold;
				case Ocean _: return Fish;
				case Plains _: return Horses;
				case Swamp _: return Oil;
				case Tundra _: return TundraGame;
			}
			return null;
		}
	}
}