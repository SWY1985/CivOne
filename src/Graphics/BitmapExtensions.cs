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
using CivOne.IO;

namespace CivOne.Graphics
{
	public static class BitmapExtensions
	{
		private static bool OutBoundX(this IBitmap bitmap, int x) => (x < 0 || x >= bitmap.Bitmap.Width);
		private static bool OutBoundY(this IBitmap bitmap, int y) => (y < 0 || y >= bitmap.Bitmap.Height);

		public static int GetHeight(this IBitmap bitmap) => bitmap.Bitmap.Height;
		public static int GetWidth(this IBitmap bitmap) => bitmap.Bitmap.Width;

		public static IBitmap FillRectangle(this IBitmap bitmap, byte colour, Rectangle rectangle) => FillRectangle(bitmap, colour, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		public static IBitmap FillRectangle(this IBitmap bitmap, byte colour, Point point, Size size) => FillRectangle(bitmap, colour, point.X, point.Y, size.Width, size.Height);
		public static IBitmap FillRectangle(this IBitmap bitmap, byte colour, int left, int top, int width, int height)
		{
			for (int yy = top; yy < (top + height); yy++)
			{
				if (yy >= bitmap.GetHeight()) break;
				if (bitmap.OutBoundY(yy)) continue;
				for (int xx = left; xx < (left + width); xx++)
				{
					if (xx >= bitmap.GetWidth()) break;
					if (bitmap.OutBoundX(xx)) continue;
					bitmap.Bitmap[xx, yy] = colour;
				}
			}
			return bitmap;
		}
		
		public static IBitmap AddLayer(this IBitmap bitmap, IBitmap layer, Point point, bool dispose = false) => AddLayer(bitmap, layer, point.X, point.Y, dispose);
		public static IBitmap AddLayer(this IBitmap bitmap, IBitmap layer, int left = 0, int top = 0, bool dispose = false)
		{
			if (layer == null) return bitmap;
			AddLayer(bitmap, layer.Bitmap, left, top, false);
			if (dispose) layer.Dispose();
			return bitmap;
		}
		public static IBitmap AddLayer(this IBitmap bitmap, Bytemap layer, Point point, bool dispose = false) => AddLayer(bitmap, layer, point.X, point.Y, dispose);
		public static IBitmap AddLayer(this IBitmap bitmap, Bytemap layer, int left = 0, int top = 0, bool dispose = false)
		{
			if (layer == null) return bitmap;
			for (int yy = 0; yy < layer.Height; yy++)
			{
				if (top + yy >= bitmap.GetHeight()) break;
				if (bitmap.OutBoundY(top + yy)) continue;
				for (int xx = 0; xx < layer.Width; xx++)
				{
					if (left + xx >= bitmap.GetWidth()) break;
					if (layer[xx, yy] == 0 || bitmap.OutBoundX(left + xx)) continue;
					bitmap.Bitmap[left + xx, top + yy] = layer[xx, yy];
				}
			}
			if (dispose) layer.Dispose();
			return bitmap;
		}

		public static IBitmap Tile(this IBitmap bitmap, IBitmap layer, Rectangle rectangle) => Tile(bitmap, layer, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		public static IBitmap Tile(this IBitmap bitmap, IBitmap layer, Point point, Size size) => Tile(bitmap, layer, point.X, point.Y, size.Width, size.Height);
		public static IBitmap Tile(this IBitmap bitmap, IBitmap layer, int left = 0, int top = 0, int width = -1, int height = -1)
		{
			if (layer == null) return bitmap;
			if (width == -1) width = bitmap.GetWidth() - left;
			if (height == -1) height = bitmap.GetHeight() - top;
			for (int yy = 0; yy < height; yy++)
			{
				if (top + yy >= bitmap.GetHeight()) break;
				if (bitmap.OutBoundY(top + yy)) continue;
				for (int xx = 0; xx < width; xx++)
				{
					if (left + xx >= bitmap.GetWidth()) break;
					if (layer.Bitmap[xx % layer.GetWidth(), yy % layer.GetHeight()] == 0 || bitmap.OutBoundX(left + xx)) continue;
					bitmap.Bitmap[left + xx, top + yy] = layer.Bitmap[xx % layer.GetWidth(), yy % layer.GetHeight()];
				}
			}
			return bitmap;
		}

		public static void DrawText(this IBitmap bitmap, string text, int font, byte colour, int x, int y, TextAlign align = TextAlign.Left) => DrawText(bitmap, text, font, colour, colour, x, y, align);
		public static void DrawText(this IBitmap bitmap, string text, int font, byte firstLetterColour, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			if (string.IsNullOrWhiteSpace(text)) return;
			Bytemap textLayer = Resources.Instance.GetText(text, font, firstLetterColour, colour).Bitmap;
			switch(align)
			{
				case TextAlign.Center: x -= (textLayer.Width + 1) / 2; break;
				case TextAlign.Right: x -= textLayer.Width; break;
			}
			AddLayer(bitmap, textLayer, x, y, dispose: true);
		}

		public static Bytemap Crop(this IBitmap bitmap, int left, int top, int width, int height) => bitmap.Bitmap[left, top, width, height];
	}
}