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

		private static bool DrawRoad(ITile tile) => (tile.Road || tile.RailRoad) && (!tile.RailRoad || (tile.RailRoad && tile.BorderRoads() != tile.BorderRailRoads()));
		private static bool DrawRailRoad(ITile tile) => tile.RailRoad;
		private static bool DrawIrrigation(ITile tile) => tile.Irrigation && tile.City == null;
		private static bool DrawMine(ITile tile) => tile.Mine;
		private static bool DrawFortress(ITile tile) => tile.Fortress && tile.City == null;
		private static bool DrawHut(ITile tile) => tile.Hut;
		
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
				if (DrawRailRoad(tile)) output.AddLayer(MapTile.RailRoad[tile.DrawRailRoadDirections()]);
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
				if (DrawRoad(tile)) output.AddLayer(MapTile.Road[tile.DrawRoadDirections()]);
				if (DrawRailRoad(tile)) output.AddLayer(MapTile.RailRoad[tile.DrawRailRoadDirections()]);
			}
			if (DrawFortress(tile)) output.AddLayer(MapTile.Fortress);
			if (DrawHut(tile)) output.AddLayer(MapTile.Hut);
			
			return output;
		}
	}
}