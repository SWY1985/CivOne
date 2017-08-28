// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Graphics.Sprites;
using CivOne.IO;
using CivOne.Tiles;
using CivOne.Units;

using static CivOne.Enums.Direction;

namespace CivOne.Screens
{
	[Expand]
	internal class Nuke : BaseScreen
	{
		private const int FRAME_COUNT = 28;

		private int _x, _y, _dx, _dy;

		private int _tilesX = 15, _tilesY = 12;
		
		private int _frameCounter = FRAME_COUNT + 2;

		private IBitmap _gameMap;

		private Picture[] _sprites = null;
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_frameCounter < 0)
			{
				Destroy();
				return true;
			}

			int cx = Settings.RightSideBar ? 0 : 80;
			int cy = 8;

			int xx = _dx + cx;
			int yy = _dy + cy;
			
			int step = 28 - _frameCounter--;

			this.AddLayer(_gameMap, cx, cy);
			if (step >= 0 && step < 28)
			{
				this.AddLayer(_sprites[step], xx, yy);
			}

			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args) => false;
		
		public override bool MouseDown(ScreenEventArgs args) => false;

		public void Resize(int width, int height)
		{
			_tilesX = (int)Math.Ceiling((double)(width - 80) / 16);
			_tilesY = (int)Math.Ceiling((double)(height - 8) / 16);

			Bitmap = new Bytemap(width, height);
			
			Player renderPlayer = Settings.RevealWorld ? null : Human;
			_gameMap = Map[_x, _y, _tilesX, _tilesY].ToBitmap(TileSettings.BlinkOff, renderPlayer);
		}

		internal Nuke(int x, int y)
		{
			_x = Common.GamePlay.X;
			_y = Common.GamePlay.Y;

			_dx = x - 14;
			_dy = y - 14;
			
			using (Palette palette = Common.DefaultPalette)
			{
				for (int i = 192; i < 256; i++)
				{
					palette[i] = Resources["NUKE1"].Palette[i];
				}
				Palette = palette;
			}
			Player renderPlayer = Settings.RevealWorld ? null : Human;
			_gameMap = Map[_x, _y, 15, 12].ToBitmap(TileSettings.BlinkOff, renderPlayer);

			_sprites = new Picture[28];
			for (int yy = 0; yy < 4; yy++)
			for (int xx = 0; xx < 7; xx++)
			{
				_sprites[(yy * 7) + xx] = Resources["NUKE1"][1 + (45 * xx), 1 + (45 * yy), 44, 44];
			}
		}
	}
}