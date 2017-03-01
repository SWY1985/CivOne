// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Events;
using CivOne.Interfaces;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Overlay : BaseScreen
	{
		private bool _update = true;
		private bool _showTerrain = false;

		private int _x, _y;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				if (_showTerrain)
				{
					int cx = Settings.RightSideBar ? 0 : 80;
					int cy = 8;

					for (int yy = 0; yy < 12; yy++)
					for (int xx = 0; xx < 15; xx++)
					{
						ITile tile = Map[_x + xx, _y + yy];
						if (tile == null || !Human.Visible(tile))
						{
							_canvas.FillRectangle(5, cx + (xx * 16), cy + (yy * 16), 16, 16);
							continue;
						}
						AddLayer(Resources.Instance.GetTile(tile, improvements: false), cx + (xx * 16), cy + (yy * 16));
						
						if (!Human.Visible(tile, Direction.West)) AddLayer(Resources.Instance.GetFog(Direction.West), cx + (xx * 16), cy + (yy * 16));
						if (!Human.Visible(tile, Direction.North)) AddLayer(Resources.Instance.GetFog(Direction.North), cx + (xx * 16), cy + (yy * 16));
						if (!Human.Visible(tile, Direction.East)) AddLayer(Resources.Instance.GetFog(Direction.East), cx + (xx * 16), cy + (yy * 16));
						if (!Human.Visible(tile, Direction.South)) AddLayer(Resources.Instance.GetFog(Direction.South), cx + (xx * 16), cy + (yy * 16));
					}
				}

				_update = false;
				return true;
			}
			return false;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			Destroy();
			return true;
		}

		public static Overlay Empty
		{
			get
			{
				return new Overlay();
			}
		}

		public static Overlay Terrain(int x, int y)
		{
			return new Overlay()
			{
				_showTerrain = true,
				_x = x,
				_y = y
			};
		}

		private Overlay()
		{
			Cursor = MouseCursor.Pointer;
			
			_canvas = new Picture(320, 200, Common.TopScreen.Palette);
		}
	}
}