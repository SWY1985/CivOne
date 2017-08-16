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
using CivOne.IO;

namespace CivOne.Graphics
{
	public static class BitmapExtensions
	{
		private static bool OutBoundX(this IBitmap bitmap, int x) => (x < 0 || x >= bitmap.Bitmap.Width);
		private static bool OutBoundY(this IBitmap bitmap, int y) => (y < 0 || y >= bitmap.Bitmap.Height);

		public static int GetHeight(this IBitmap bitmap) => bitmap.Bitmap.Height;
		public static int GetWidth(this IBitmap bitmap) => bitmap.Bitmap.Width;

		public static T As<T>(this IBitmap bitmap) where T : class, IBitmap => (bitmap as T);

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

		public static IBitmap DrawRectangle(this IBitmap bitmap, int left = 0, int top = 0, int width = -1, int height = -1, byte colour = 5) => DrawRectangle3D(bitmap, left, top, width, height, colour, colour);
		public static IBitmap DrawRectangle3D(this IBitmap bitmap, int left = 0, int top = 0, int width = -1, int height = -1, byte colourLight = 15, byte colourDark = 8)
		{
			if (width < 0) width = bitmap.GetWidth() - left;
			if (height < 0) height = bitmap.GetHeight() - top;
			int ww = (left + width - 1), hh = (top + height - 1);
			for (int yy = top; yy <= hh; yy++)
			{
				if (yy >= bitmap.GetHeight()) break;
				if (bitmap.OutBoundY(yy)) continue;
				for (int xx = left; xx <= ww; xx++)
				{
					if (xx >= bitmap.GetWidth()) break;
					if (bitmap.OutBoundX(xx)) continue;
					if (yy == top || xx == ww)
						bitmap.Bitmap[xx, yy] = colourDark;
					else if (yy == hh || xx == left)
						bitmap.Bitmap[xx, yy] = colourLight;
				}
			}
			return bitmap;
		}
		
		public static IBitmap DrawLine(this IBitmap bitmap, Point from, Point to, byte colour) => DrawLine(bitmap, from.X, from.Y, to.X, to.Y, colour = 5);
		public static IBitmap DrawLine(this IBitmap bitmap, int x1, int y1, int x2, int y2, byte colour = 5)
		{
			int difX = (x2 - x1), difY = (y2 - y1);
			float xx = x1, yy = y1, incX, incY;
			int steps;
			if (Math.Abs(difX) < Math.Abs(difY))
			{
				incX = ((float)difX / difY);
				incY = difY < 0 ? -1 : 1;
				if (difY < 0) incX = -incX;
				steps = difY;
			}
			else
			{
				incX = difX < 0 ? -1 : 1;
				incY = ((float)difY / difX);
				if (difX < 0) incY = -incY;
				steps = difX;
			}
			for (int i = 0; i < Math.Abs(steps); i++)
			{
				bitmap.Bitmap[(int)Math.Round(xx), (int)Math.Round(yy)] = colour;
				xx += incX;
				yy += incY;
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

		public static IBitmap DrawText(this IBitmap bitmap, string text, int font, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			if (string.IsNullOrWhiteSpace(text)) return bitmap;
			Bytemap textLayer = Resources.Instance.GetText(text, font, colour).Bitmap;
			switch(align)
			{
				case TextAlign.Center: x -= (textLayer.Width + 1) / 2; break;
				case TextAlign.Right: x -= textLayer.Width; break;
			}
			AddLayer(bitmap, textLayer, x, y, dispose: true);
			return bitmap;
		}
		public static IBitmap DrawText(this IBitmap bitmap, string text, int x = 0, int y = 0, TextSettings settings = null)
		{
			if (string.IsNullOrWhiteSpace(text)) return bitmap;
			if (settings == null)
			{
				if (bitmap is IDefaultTextSettings)
					settings = (bitmap as IDefaultTextSettings).DefaultTextSettings;
				else
					settings = new TextSettings();
			}
			
			Size textSize = Resources.Instance.GetTextSize(settings.FontId, text);
			Bytemap textLayer;
			if (settings.FirstLetterColour != 0)
			{
				textLayer = Resources.Instance.GetText(text, settings.FontId, settings.FirstLetterColour, settings.Colour).Bitmap;
			}
			else if (settings.TopColour != 0 && settings.BottomColour != 0)
			{
				textLayer = new Picture(textSize.Width, textSize.Height + 2)
					.AddLayer(Resources.Instance.GetText(text, settings.FontId, settings.TopColour))
					.AddLayer(Resources.Instance.GetText(text, settings.FontId, settings.BottomColour), top: 2)
					.AddLayer(Resources.Instance.GetText(text, settings.FontId, settings.Colour), top: 1)
					.Bitmap;
			}
			else if (settings.BottomColour != 0)
			{
				textLayer = new Picture(textSize.Width, textSize.Height + 1)
					.AddLayer(Resources.Instance.GetText(text, settings.FontId, settings.BottomColour), top: 1)
					.AddLayer(Resources.Instance.GetText(text, settings.FontId, settings.Colour))
					.Bitmap;
			}
			else
			{
				textLayer = Resources.Instance.GetText(text, settings.FontId, settings.Colour).Bitmap;
			}

			switch(settings.Alignment)
			{
				case TextAlign.Center: x -= (textLayer.Width + 1) / 2; break;
				case TextAlign.Right: x -= textLayer.Width; break;
			}

			if (settings.VerticalAlignment == VerticalAlign.Bottom)
			{
				y -= Resources.Instance.GetFontHeight(settings.FontId);
			}

			AddLayer(bitmap, textLayer, x, y, dispose: true);
			return bitmap;
		}

		public static Bytemap Crop(this IBitmap bitmap, int left, int top, int width, int height) => bitmap.Bitmap[left, top, width, height];
		
		public static IBitmap ColourReplace(this IBitmap bitmap, byte colourFrom, byte colourTo, int x, int y, int width, int height)
		{
			for (int yy = y; yy < y + height; yy++)
			{
				if (yy >= bitmap.GetHeight()) break;
				if (bitmap.OutBoundY(yy)) continue;
				for (int xx = x; xx < x + width; xx++)
				{
					if (xx >= bitmap.GetWidth()) break;
					if (bitmap.OutBoundX(xx)) continue;
					if (bitmap.Bitmap[xx, yy] != colourFrom) continue;
					bitmap.Bitmap[xx, yy] = colourTo;
				}
			}
			return bitmap;
		}

		// Palette functions
		public static IBitmap Cycle(this IBitmap bitmap, int colour, ref Colour[] colours)
		{
			Colour reserve = bitmap.Palette[colour];
			bitmap.Palette[colour] = colours[0];
			for (int i = 0; i < colours.Length - 1; i++)
				colours[i] = colours[i + 1];
			colours[colours.Length - 1] = reserve;
			for (int i = 0; i < colours.Length && i < bitmap.Palette.Length; i++)
				bitmap.Palette[i] = colours[i];
			return bitmap;
		}
		
		public static IBitmap Cycle(this IBitmap bitmap, int start, int end)
		{
			Colour reserve;
			if (start > end)
			{
				reserve = bitmap.Palette[start];
				for (int i = start; i < end; i++)
					bitmap.Palette[i] = bitmap.Palette[i + 1];
				bitmap.Palette[end] = reserve;
				return bitmap;
			}
			reserve = bitmap.Palette[end];
			for (int i = end; i > start; i--)
				bitmap.Palette[i] = bitmap.Palette[i - 1];
			bitmap.Palette[start] = reserve;
			return bitmap;
		}
	}
}