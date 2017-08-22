// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Graphics.Sprites;
using CivOne.Tiles;

namespace CivOne.Graphics
{
	internal static class TileResources
	{
		private static Resources Resources => Resources.Instance;
		
		private static IBitmap[] _icons = new Picture[12];
		
		private static void DrawIrrigation(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.Irrigation) return;
			if (Game.Instance != null && tile.City == null)
			
			output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"][64, 32, 16, 16], 0, 0);
		}
		
		private static void DrawMine(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.Mine) return;
			
			output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"][80, 32, 16, 16], 0, 0);
		}
		
		private static void DrawFortress(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.Fortress) return;
			if (Game.Instance != null && tile.City == null)
			
			output.AddLayer(Icons.Fortress, 0, 0);
		}
		
		private static void DrawRoad(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.Road || tile.RailRoad) return;
						
			bool connected = false;
			ITile borderTile = null;
			Direction[] directions = new [] { Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest };
			for (int i = 0; i < directions.Length; i++)
			{
				if ((borderTile = tile.GetBorderTile(directions[i])) == null) continue;
				if (!(borderTile.Road || borderTile.RailRoad) || (tile.City != null && borderTile.City != null)) continue;
				output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"][(i * 16), 48, 16, 16], 0, 0);
				connected = true;
			}
			if (connected) return;
			output.FillRectangle(7, 7, 2, 2, 6);
		}
		
		private static void DrawRailRoad(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.RailRoad && tile.City == null) return;
			
			bool connected = false;
			ITile borderTile = null;
			Direction[] directions = new [] { Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest };
			for (int i = 0; i < directions.Length; i++)
			{
				if ((borderTile = tile.GetBorderTile(directions[i])) == null) continue;
				// if (!borderTile.Road || borderTile.RailRoad || (tile.City != null && borderTile.City != null)) continue;
				if (!borderTile.Road || borderTile.RailRoad || borderTile.City != null) continue;
				output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"][(i * 16), 48, 16, 16], 0, 0);
				connected = true;
			}
			for (int i = 0; i < directions.Length; i++)
			{
				if ((borderTile = tile.GetBorderTile(directions[i])) == null) continue;
				if (!borderTile.RailRoad && tile.City == null && borderTile.City == null) continue;
				if (tile.City != null && borderTile.City == null && !borderTile.RailRoad) continue;
				output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"][128 + (i * 16), 96, 16, 16], 0, 0);
				connected = true;
			}
			if (connected) return;
			output.FillRectangle(7, 7, 2, 2, 5);
		}
		
		private static void DrawHut(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.Hut) return;
			
			output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"][240, 112, 16, 16].ColourReplace(3, 0), 0, 0);
		}
		
		internal static Picture GetTile16(ITile tile, bool improvements, bool roads)
		{
			Picture output = new Picture(16, 16);
			
			output.AddLayer(MapTile.TileBase(tile).Bitmap);
			output.AddLayer(MapTile.TileLayer(tile).Bitmap);
			
			// Add special resources
			if (tile.Special)
			{
				int terrainId = (int)tile.Type;
				output.AddLayer(Resources["SPRITES"][terrainId * 16, 112, 16, 16].ColourReplace(3, 0));
			}
			
			// Add tile improvements
			if (improvements && tile.Type != Terrain.River)
			{
				DrawIrrigation(ref output, tile, true);
				DrawMine(ref output, tile, true);
			}
			if (roads)
			{
				DrawRoad(ref output, tile, true);
				DrawRailRoad(ref output, tile, true);
			}
			DrawFortress(ref output, tile, true);
			DrawHut(ref output, tile, true);
			
			return output;
		}
		
		internal static Picture GetTile256(ITile tile, bool improvements, bool roads)
		{
			Picture output = new Picture(16, 16);

			output.AddLayer(MapTile.TileBase(tile).Bitmap);

			if (improvements) DrawIrrigation(ref output, tile);
			output.AddLayer(MapTile.TileLayer(tile).Bitmap);
			
			// Add special resources
			if (!Map.TileIsType(tile, Terrain.Grassland1, Terrain.Grassland2) && tile.Special)
			{
				int terrainId = (int)tile.Type;
				output.AddLayer(Resources["SP257"][terrainId * 16, 112, 16, 16].ColourReplace(3, 0));
			}
			else if (tile.Type == Terrain.Grassland2)
			{
				output.AddLayer(Resources["SP257"][152, 40, 8, 8].ColourReplace(3, 0), 4, 4);
			}
			
			// Add tile improvements
			if (tile.Type != Terrain.River && improvements)
			{
				DrawMine(ref output, tile);
			}
			if (roads)
			{
				DrawRoad(ref output, tile);
				DrawRailRoad(ref output, tile, true);
			}
			DrawFortress(ref output, tile, true);
			DrawHut(ref output, tile);
			
			return output;
		}
		
		internal static IBitmap GetIcon(Terrain terrain)
		{
			int terrainId = (int)terrain;
			if (terrainId == 12) terrainId = 2;
			if (_icons[terrainId] != null)
				return _icons[terrainId];
			
			switch (terrain)
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
			return _icons[terrainId];
		}
	}
}