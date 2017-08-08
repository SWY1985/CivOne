// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Interfaces;

namespace CivOne.GFX
{
	public static class Extensions
	{
		private static Game Game => Game.Instance;
		private static Resources Resources => Resources.Instance;
		private static Color[] Palette => Resources["SP257"].Palette;

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
				output.AddLayer(tile.ToPicture(settings), x, y);

				if (player != null)
				{
					foreach (Direction direction in new[] { Direction.West, Direction.North, Direction.East, Direction.South })
					{
						if (player.Visible(tile, direction)) continue;
						output.AddLayer(Resources.GetFog(direction), x, y);
					}
				}

				if (settings.EnemyUnits || settings.Units)
				{
					int unitCount = tile.Units.Count(u => settings.Units || player == null || u.Owner != Game.PlayerNumber(player));
					if (unitCount == 0) continue;
					output.AddLayer(tile.UnitsToPicture(), x, y);
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

		public static Picture ToPicture(this ITile tile, TileSettings settings = null)
		{
			if (settings == null) settings = TileSettings.Default;

			Picture output = new Picture(16, 16, Palette);

			output.AddLayer(Resources.GetTile(tile, settings.Improvements, settings.Roads));

			if (settings.Cities && tile.City != null)
			{
				output.AddLayer(Icons.City(tile.City, smallFont: settings.CitySmallFonts));
			}

			return output;
		}

		public static Picture UnitsToPicture(this ITile tile)
		{
			if (tile == null || tile.Units.Length == 0) return null;
			
			bool stack = (tile.Units.Length > 1);
			Picture unitGfx = tile.Units[0].GetUnit(tile.Units[0].Owner);
			
			Picture output = new Picture(16, 16, Palette);
			output.AddLayer(unitGfx, 1, 1);
			if (stack)
				output.AddLayer(unitGfx);
			return output;
		}
	}
}