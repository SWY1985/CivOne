// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;

namespace CivOne.Graphics
{
	public static class BitmapExtensions
	{
		private static bool OutBound(this IBitmap bitmap, int dimension, int value) => (value < bitmap.Bitmap.GetLowerBound(dimension) || value > bitmap.Bitmap.GetUpperBound(dimension));
		private static bool OutBoundX(this IBitmap bitmap, int x) => OutBound(bitmap, 0, x);
		private static bool OutBoundY(this IBitmap bitmap, int y) => OutBound(bitmap, 1, y);

		public static int GetHeight(this IBitmap bitmap) => bitmap.Bitmap.GetLength(1);
		public static int GetWidth(this IBitmap bitmap) => bitmap.Bitmap.GetLength(0);

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
		
		public static IBitmap AddLayer(this IBitmap bitmap, IBitmap layer, Point point) => AddLayer(bitmap, layer, point.X, point.Y);
		public static IBitmap AddLayer(this IBitmap bitmap, IBitmap layer, int left = 0, int top = 0)
		{
			if (layer == null) return bitmap;
			for (int yy = 0; yy < layer.GetHeight(); yy++)
			{
				if (top + yy >= bitmap.GetHeight()) break;
				if (bitmap.OutBoundY(top + yy)) continue;
				for (int xx = 0; xx < layer.GetWidth(); xx++)
				{
					if (left + xx >= bitmap.GetWidth()) break;
					if (layer.Bitmap[xx, yy] == 0 || bitmap.OutBoundX(left + xx)) continue;
					bitmap.Bitmap[left + xx, top + yy] = layer.Bitmap[xx, yy];
				}
			}
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
	}
}