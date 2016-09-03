// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityMap : BaseScreen
	{
		private readonly City _city;

		private readonly Bitmap _background;
		
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas.FillLayerTile(_background);
				_canvas.AddBorder(1, 1, 0, 0, 82, 82);
				_canvas.FillRectangle(0, 82, 0, 2, 82);
				
				ITile[,] tiles = _city.CityRadius;
				for (int xx = 0; xx < 5; xx++)
				for (int yy = 0; yy < 5; yy++)
				{
					ITile tile = tiles[xx, yy];
					if (tile == null) continue;
					AddLayer(Resources.Instance.GetTile(tile), (xx * 16) + 1, (yy * 16) + 1);
					if (Settings.Instance.RevealWorld) continue;
					
					if (!HumanPlayer.Visible(tile, Direction.West)) AddLayer(Resources.Instance.GetFog(Direction.West), (xx * 16) + 1, (yy * 16) + 1);
					if (!HumanPlayer.Visible(tile, Direction.North)) AddLayer(Resources.Instance.GetFog(Direction.North), (xx * 16) + 1, (yy * 16) + 1);
					if (!HumanPlayer.Visible(tile, Direction.East)) AddLayer(Resources.Instance.GetFog(Direction.East), (xx * 16) + 1, (yy * 16) + 1);
					if (!HumanPlayer.Visible(tile, Direction.South)) AddLayer(Resources.Instance.GetFog(Direction.South), (xx * 16) + 1, (yy * 16) + 1);
				}
				
				_update = false;
			}
			return true;
		}

		public void Close()
		{
			Destroy();
		}

		public CityMap(City city, Bitmap background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(84, 82, background.Palette.Entries);
		}
	}
}