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
using CivOne.Interfaces;
using CivOne.GFX;

namespace CivOne.Templates
{
	public abstract partial class BaseScreen
	{
		protected Picture _canvas = new Picture(320, 200);
		
		protected void AddLayer(IScreen screen, Point point)
		{
			AddLayer(screen, point.X, point.Y);
		}
		protected void AddLayer(IScreen screen, int x = 0, int y = 0)
		{
			_canvas.AddLayer(screen.Canvas, x, y);
		}
		protected void AddLayer(Picture picture, Point point)
		{
			AddLayer(picture, point.X, point.Y);
		}
		protected void AddLayer(Picture picture, int x = 0, int y = 0)
		{
			_canvas.AddLayer(picture, x, y);
		}

		protected void DrawBorder(int border)
		{
			border = (border % 2);
			Picture[] borders = new Picture[8];
			int index = 0;
			for (int yy = 0; yy < 2; yy++)
			for (int xx = 0; xx < 4; xx++)
			{
				borders[index] = Resources.Instance.GetPart("SP299", ((border == 0) ? 192 : 224) + (8 * xx), 120 + (8 * yy), 8, 8);
				index++;
			}
			
			for (int x = 8; x < 312; x += 8)
			{
				AddLayer(borders[4], x, 0);
				AddLayer(borders[6], x, 192);
			}
			for (int y = 8; y < 192; y += 8)
			{
				AddLayer(borders[7], 0, y);
				AddLayer(borders[5], 312, y);
			}
			AddLayer(borders[0], 0, 0);
			AddLayer(borders[1], 312, 0);
			AddLayer(borders[2], 0, 192);
			AddLayer(borders[3], 312, 192);
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
		
		public virtual Picture Canvas
		{
			get
			{
				return _canvas;
			}
		}

		public Color[] Palette
		{
			get
			{
				return Canvas.Palette;
			}
		}
	}
}