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
using CivOne.Graphics;
using CivOne.Units;

using static CivOne.Enums.Direction;

namespace CivOne.Tiles
{
	public static class TileExtensions
	{
		private static Game Game => Game.Instance;
		private static Resources Resources => Resources.Instance;
		private static Palette Palette => Resources["SP257"].Palette;

		private static TextSettings CityLabel = TextSettings.ShadowText(11, 5);

		public static ITile GetBorderTile(this ITile tile, Direction direction)
		{
			switch (direction)
			{
				case North: return tile[0, -1];
				case East: return tile[1, 0];
				case South: return tile[0, 1];
				case West: return tile[-1, 0];
				case NorthWest: return tile[-1, -1];
				case NorthEast: return tile[1, -1];
				case SouthWest: return tile[-1, 1];
				case SouthEast: return tile[1, 1];
			}
			return null;
		}
		
		public static IEnumerable<ITile> GetBorderTiles(this ITile tile)
		{
			for (int relY = -1; relY <= 1; relY++)
			for (int relX = -1; relX <= 1; relX++)
			{
				if (relX == 0 && relY == 0) continue;
				if (tile[relX, relY] == null) continue;
				yield return tile[relX, relY];
			}
		}

		public static IEnumerable<ITile> CrossTiles(this ITile tile)
		{
			for (int relY = -1; relY <= 1; relY++)
			for (int relX = -1; relX <= 1; relX++)
			{
				if (relX == 0 && relY == 0) continue;
				if (relX != 0 && relY != 0) continue;
				if (tile[relX, relY] == null) continue;
				yield return tile[relX, relY];
			}
		}

		public static Direction BorderRoads(this ITile tile)
		{
			Direction output = Direction.None;
			for (int i = 1; i <= 128; i *= 2)
			{
				if (!GetBorderTile(tile, (Direction)i).Road && !GetBorderTile(tile, (Direction)i).RailRoad) continue;
				output += i;
			}
			return output;
		}

		public static Direction BorderRailRoads(this ITile tile)
		{
			Direction output = Direction.None;
			for (int i = 1; i <= 128; i *= 2)
			{
				if (!GetBorderTile(tile, (Direction)i).RailRoad) continue;
				output += i;
			}
			return output;
		}

		public static Direction DrawRoadDirections(this ITile tile)
		{
			if (!tile.Road)
				return Direction.None;
			if (!tile.RailRoad)
				return BorderRoads(tile);
			return (Direction)(BorderRoads(tile) - BorderRailRoads(tile));
		}

		public static IBitmap ToBitmap(this ITile[,] tiles, TileSettings settings = null, Player player = null)
		{
			if (settings == null) settings = TileSettings.Default;

			IBitmap output = new Picture(16 * tiles.GetLength(0), 16 * tiles.GetLength(1), Palette);

			for (int yy = 0; yy < tiles.GetLength(1); yy++)
			for (int xx = 0; xx < tiles.GetLength(0); xx++)
			{
				ITile tile = tiles[xx, yy];
				if (tile == null || player != null && !player.Visible(tile)) continue;

				int x = (xx * 16), y = (yy * 16);
				output.AddLayer(tile.ToBitmap(settings, player), x, y, dispose: true);
			}

			if (settings.CityLabels)
			{
				for (int yy = 0; yy < tiles.GetLength(1) - 1; yy++)
				for (int xx = 0; xx < tiles.GetLength(0); xx++)
				{
					ITile tile = tiles[xx, yy];
					if (tile == null || tile.City == null || player != null && !player.Visible(tile)) continue;
					int x = (xx == 0) ? 0 : (xx * 16) - 8;
					int y = (yy * 16) + 16;
					string label = tile.City.Name;
					output.DrawText(label, x, y, CityLabel);
				}
			}

			return output;
		}

		public static IBitmap ToBitmap(this ITile tile, TileSettings settings = null, Player player = null)
		{
			if (settings == null) settings = TileSettings.Default;

			IBitmap output = new Picture(16, 16, Palette);

			output.AddLayer(Resources[tile, settings.Improvements, settings.Roads], dispose: true);

			if (player != null)
			{
				foreach (Direction direction in new[] { West, North, East, South })
				{
					if (player.Visible(tile, direction)) continue;
					output.AddLayer(Resources.GetFog(direction));
				}
			}

			if (settings.Cities && tile.City != null)
			{
				output.AddLayer(Icons.City(tile.City, smallFont: settings.CitySmallFonts));
				if (settings.ActiveUnit && tile.Units.Any(u => u == Game.ActiveUnit && u.Owner != Game.PlayerNumber(player)))
				{
					output.AddLayer(tile.UnitsToPicture(), -1, -1, dispose: true);
				}
			}
			
			if ((settings.EnemyUnits || settings.Units) && (tile.City == null || tile.Units.Any(u => u == Game.ActiveUnit)))
			{
				int unitCount = tile.Units.Count(u => settings.Units || player == null || u.Owner != Game.PlayerNumber(player));
				if (unitCount > 0)
				{
					output.AddLayer(tile.UnitsToPicture(), dispose: true);
				}
			}

			return output;
		}

		public static IBitmap UnitsToPicture(this ITile tile)
		{
			if (tile == null || tile.Units.Length == 0 || (tile.Units.Length == 1 && tile.Units[0] == Game.MovingUnit)) return null;
			
			IUnit[] units = tile.Units.Where(x => x != Game.MovingUnit).ToArray();
			if (units.Length == 0) return null;

			bool stack = (units.Length > 1);
			IUnit unit = units.First();
			
			IBitmap output = new Picture(16, 16, Palette);
			using (IBitmap unitPicture = unit.GetUnit(unit.Owner))
			{
				if (tile.City == null) output.AddLayer(unitPicture);
				if (stack || tile.City != null) output.AddLayer(unitPicture, -1, -1);
			}
			return output;
		}
	}
}