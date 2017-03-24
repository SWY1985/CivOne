// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class WorldMap : BaseScreen
	{
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update) return false;
			_update = false;
			return true;
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

		private int VisibleTop
		{
			get
			{
				Player player = Game.Human;
				for(int yy = 0; yy < Map.HEIGHT; yy++)
				for(int xx = 0; xx < Map.WIDTH; xx++)
				{
					if (player.Visible(xx, yy)) return yy;
				}
				return 0;
			}
		}

		private int VisibleBottom
		{
			get
			{
				Player player = Game.Human;
				for(int yy = Map.HEIGHT - 1; yy >= 0; yy--)
				for(int xx = 0; xx < Map.WIDTH; xx++)
				{
					if (player.Visible(xx, yy)) return yy;
				}
				return 0;
			}
		}
		
		public WorldMap()
		{
			_canvas = new Picture(320, 200, Resources.WorldMapTiles.Palette);
			_canvas.FillRectangle(5, 0, 0, 320, 200);

			int startX = Game.Human.StartX - 40;
			// int startY = (VisibleTop - Map.HEIGHT) + VisibleBottom; //(int)Math.Floor((double)(Map.HEIGHT - (VisibleBottom - VisibleTop)) / 2);
			int startY = ((Map.HEIGHT - (VisibleBottom - VisibleTop)) / 2) - VisibleTop;
			if (Settings.RevealWorld) startX = 0;
			if (Settings.RevealWorld || VisibleTop == 0 || VisibleBottom == Map.HEIGHT) startY = 0;
			
			for (int x = 0; x < Map.WIDTH; x++)
			for (int y = 0; y < Map.HEIGHT; y++)
			{
				if (!Settings.RevealWorld && !Human.Visible(x, y)) continue;

				City city = null;
				IUnit[] units;
				ITile tile = Map[x, y];
				Terrain type = tile.Type;
				if (type == Terrain.Grassland2) type = Terrain.Grassland1;
				bool altTile = ((x + y) % 2 == 1);
				int xx = (((int)type) * 4);
				int yy = altTile ? 4 : 0;
				
				int dx = (x - startX) * 4;
				int dy = (y + startY) * 4;
				if (dy < 0 || dy >= 200) continue;

				while (dx > 320) dx -= 320;
				while (dx < 0) dx += 320;

				AddLayer(Resources.WorldMapTiles.GetPart(xx, yy, 4, 4), dx, dy);
				
				if ((city = tile.City) != null && city.Size > 0)
				{
					_canvas.FillRectangle(Common.ColourLight[city.Owner], dx, dy, 4, 4);
				}
				else if ((units = tile.Units).Length > 0)
				{
					_canvas.FillRectangle(5, dx + 1, dy + 1, 3, 3);
					_canvas.FillRectangle(Common.ColourLight[units[0].Owner], dx, dy, 3, 3);
				}
			}
		}
	}
}