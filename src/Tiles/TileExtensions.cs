// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Units;

namespace CivOne.Tiles
{
	public static class Extensions
	{
		private static Game Game => Game.Instance;
		private static Resources Resources => Resources.Instance;
		private static Palette Palette => Resources["SP257"].Palette;

		public static Picture ToPicture(this ITile[,] tiles, TileSettings settings = null, Player player = null)
		{
			if (settings == null) settings = TileSettings.Default;

			Picture output = new Picture(16 * tiles.GetLength(0), 16 * tiles.GetLength(1), Palette);

			for (int yy = 0; yy < tiles.GetLength(1); yy++)
			for (int xx = 0; xx < tiles.GetLength(0); xx++)
			{
				ITile tile = tiles[xx, yy];
				if (tile == null || player != null && !player.Visible(tile)) continue;

				int x = (xx * 16), y = (yy * 16);
				output.AddLayer(tile.ToPicture(settings), x, y, dispose: true);

				if (player != null)
				{
					foreach (Direction direction in new[] { Direction.West, Direction.North, Direction.East, Direction.South })
					{
						if (player.Visible(tile, direction)) continue;
						output.AddLayer(Resources.GetFog(direction), x, y);
					}
				}
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
					output.DrawText(label, 0, 5, x, y + 1);
					output.DrawText(label, 0, 11, x, y);
				}
			}

			return output;
		}

		public static Picture ToPicture(this ITile tile, TileSettings settings = null, Player player = null)
		{
			if (settings == null) settings = TileSettings.Default;

			Picture output = new Picture(16, 16, Palette);

			output.AddLayer(Resources[tile, settings.Improvements, settings.Roads], dispose: true);

			if (settings.Cities && tile.City != null)
			{
				output.AddLayer(Icons.City(tile.City, smallFont: settings.CitySmallFonts));
				if (settings.ActiveUnit && tile.Units.Any(u => u == Game.ActiveUnit && u.Owner != Game.PlayerNumber(player)))
				{
					output.AddLayer(tile.UnitsToPicture(), -1, -1, dispose: true);
				}
			}
			else if (settings.EnemyUnits || settings.Units)
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
				output.AddLayer(unitPicture);
				if (stack)
					output.AddLayer(unitPicture, -1, -1);
			}
			return output;
		}
	}
}