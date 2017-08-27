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

		private static bool DrawRoad(ITile tile) => tile.Road && (!tile.RailRoad || tile.BorderRoads() != tile.BorderRailRoads());
		private static bool DrawIrrigation(ITile tile) => tile.Irrigation && tile.City == null;
		private static bool DrawMine(ITile tile) => tile.Mine;
		private static bool DrawFortress(ITile tile) => tile.Fortress && tile.City == null;
		private static bool DrawHut(ITile tile) => tile.Hut;
		
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
		
		internal static Picture GetTile16(ITile tile, bool improvements, bool roads)
		{
			Picture output = new Picture(16, 16);
			
			output.AddLayer(MapTile.TileBase(tile));
			output.AddLayer(MapTile.TileLayer(tile));
			output.AddLayer(MapTile.TileSpecial(tile));
			
			// Add tile improvements
			if (improvements && tile.Type != Terrain.River)
			{
				if (DrawIrrigation(tile)) output.AddLayer(MapTile.Irrigation);
				if (DrawMine(tile)) output.AddLayer(MapTile.Mine);
			}
			if (roads)
			{
				if (DrawRoad(tile)) output.AddLayer(MapTile.Road[tile.DrawRoadDirections()]);
				DrawRailRoad(ref output, tile, true);
			}
			if (DrawFortress(tile)) output.AddLayer(MapTile.Fortress);
			if (DrawHut(tile)) output.AddLayer(MapTile.Hut);
			
			return output;
		}
		
		internal static Picture GetTile256(ITile tile, bool improvements, bool roads)
		{
			Picture output = new Picture(16, 16);

			output.AddLayer(MapTile.TileBase(tile));
			if (improvements && DrawIrrigation(tile)) output.AddLayer(MapTile.Irrigation);
			output.AddLayer(MapTile.TileLayer(tile));
			output.AddLayer(MapTile.TileSpecial(tile));
			
			// Add tile improvements
			if (tile.Type != Terrain.River && improvements)
			{
				if (DrawMine(tile)) output.AddLayer(MapTile.Mine);
			}
			if (roads)
			{
				// DrawRoad(ref output, tile);
				if (DrawRoad(tile)) output.AddLayer(MapTile.Road[tile.DrawRoadDirections()]);
				DrawRailRoad(ref output, tile, true);
			}
			if (DrawFortress(tile)) output.AddLayer(MapTile.Fortress);
			if (DrawHut(tile)) output.AddLayer(MapTile.Hut);
			
			return output;
		}
	}
}