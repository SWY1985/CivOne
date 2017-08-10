// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.Graphics;

namespace CivOne.Screens
{
	public abstract partial class BaseScreen
	{
		internal static Resources Resources => Resources.Instance;

		protected Picture _canvas = new Picture(320, 200);
		
		protected IBitmap AddLayer(IBitmap bitmap, Point point) => _canvas.AddLayer(bitmap, point.X, point.Y);
		protected IBitmap AddLayer(IBitmap bitmap, int x = 0, int y = 0) => _canvas.AddLayer(bitmap, x, y);

		protected void DrawPanel(int x, int y, int width, int height, bool border = true)
		{
			int xx = x, yy = y, ww = width, hh = height;
			if (border)
			{
				xx++;
				yy++;
				ww -= 2;
				hh -= 2;
				_canvas.FillRectangle(5, x, y, width, height);
			}
			Picture panel = new Picture(ww, hh);
			panel.FillLayerTile(Patterns.PanelGrey);
			panel.AddBorder(15, 8, 0, 0, ww, hh);
			_canvas.AddLayer(panel, xx, yy);
		}

		protected void DrawBorder(int border)
		{
			border = (border % 2);
			Picture[] borders = new Picture[8];
			int index = 0;
			for (int yy = 0; yy < 2; yy++)
			for (int xx = 0; xx < 4; xx++)
			{
				borders[index] = Resources["SP299"].GetPart(((border == 0) ? 192 : 224) + (8 * xx), 120 + (8 * yy), 8, 8);
				index++;
			}
			
			for (int x = 8; x < _canvas.Width - 8; x += 8)
			{
				AddLayer(borders[4], x, 0);
				AddLayer(borders[6], x, _canvas.Height - 8);
			}
			for (int y = 8; y < _canvas.Height - 8; y += 8)
			{
				AddLayer(borders[7], 0, y);
				AddLayer(borders[5], _canvas.Width - 8, y);
			}
			AddLayer(borders[0], 0, 0);
			AddLayer(borders[1], _canvas.Width - 8, 0);
			AddLayer(borders[2], 0, _canvas.Height - 8);
			AddLayer(borders[3], _canvas.Width - 8, _canvas.Height - 8);
		}

		protected void DrawButton(string text, byte colour, byte colourDark, int x, int y, int width)
		{
			_canvas.FillRectangle(7, x, y, width, 1);
			_canvas.FillRectangle(7, x, y + 1, 1, 8);
			_canvas.FillRectangle(colourDark, x + 1, y + 8, width - 1, 1);
			_canvas.FillRectangle(colourDark, x + width - 1, y, 1, 8);
			_canvas.FillRectangle(colour, x + 1, y + 1, width - 2, 7);
			_canvas.DrawText(text, 1, colourDark, x + (int)Math.Ceiling((double)width / 2), y + 2, TextAlign.Center);
		}
		
		public byte[,] Bitmap => _canvas.Bitmap;
		public Color[] Palette => _canvas.Palette;
		public Color[] OriginalColours => _canvas.OriginalColours;
	}
}