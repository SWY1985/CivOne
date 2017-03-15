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
using CivOne.Enums;
using CivOne.GFX.ImageFormats;

namespace CivOne.GFX
{
	public class Picture
	{
		private readonly Color[] _originalColours;
		private readonly Color[] _palette = new Color[256];
		private readonly Dictionary<string, Bitmap> _cache;
		private readonly byte[,] _bitmap;
		
		public Bitmap Image
		{
			get
			{
				// Bitmap output = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
				// for (int y = 0; y < Height; y++)
				// {
				// 	byte[] imgData = new byte[Width];
				// 	for (int x = 0; x < Width; x++)
				// 	{
				// 		imgData[x] = _bitmap[x, y];
				// 	}
				// 	BitmapData bmpData = output.LockBits(new Rectangle(0, y, Width, 1), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
				// 	Marshal.Copy(imgData, 0, bmpData.Scan0, Width);
				// 	output.UnlockBits(bmpData);
				// }
				
				// ColorPalette palette = output.Palette;
				// for (int i = 0; i < Palette.Length; i++)
				// 	palette.Entries[i] = Palette[i];
				// output.Palette = palette;
				// return output;
				throw new NotImplementedException();
			}
		}

		public int Width
		{
			get
			{
				return _bitmap.GetLength(0);
			}
		}

		public int Height
		{
			get
			{
				return _bitmap.GetLength(1);
			}
		}

		public Size Size
		{
			get
			{
				return new Size(_bitmap.GetLength(0), _bitmap.GetLength(1));
			}
		}
		
		public Color[] OriginalColours
		{
			get
			{
				return _originalColours;
			}
		}

		public Color[] Palette
		{
			get
			{
				return _palette;
			}
			set
			{
				for (int i = 0; i < _palette.Length && i < value.Length; i++)
					_palette[i] = value[i];
			}
		}
		
		public byte[,] GetBitmap
		{
			get
			{
				return _bitmap;
			}
		}

		public int[] GetColorMap
		{
			get
			{
				int[] output = new int[Width * Height];
				int i = 0;
				for (int yy = _bitmap.GetUpperBound(1); yy >= 0; yy--)
				for (int xx = 0; xx <= _bitmap.GetUpperBound(0); xx++)
				{
					output[i++] = _palette[_bitmap[xx, yy]].GetHashCode();
				}
				return output;
			}
		}

		public byte[,] ScaleBitmap(int scaleX, int scaleY)
		{
			byte[,] output = new byte[Width * scaleX, Height * scaleY];
			for (int yy = 0; yy < Height; yy++)
			for (int xx = 0; xx < Width; xx++)
			{
				for (int sy = 0; sy < scaleY; sy++)
				for (int sx = 0; sx < scaleX; sx++)
				{
					output[(xx * scaleX) + sx, (yy * scaleY) + sy] = _bitmap[xx, yy];
				}
			}
			return output;
		}
		
		public static void ReplaceColours(Picture image, byte colourFrom, byte colourTo)
		{
			ReplaceColours(image, new[] { colourFrom }, new[] { colourTo });
		}
		public static void ReplaceColours(Picture image, byte[] coloursFrom, byte[] coloursTo)
		{
			for (int yy = 0; yy < image.Height; yy++)
			for (int xx = 0; xx < image.Width; xx++)
			for (int j = 0; j < coloursFrom.Length && j < coloursTo.Length; j++)
			{
				if (image[xx, yy] != coloursFrom[j]) continue;
				image[xx, yy] = coloursTo[j];
			}
		}
		public static void Clip(ref Bitmap image, int top, int right, int bottom, int left)
		{
			// byte[] pixels = new byte[image.Width * image.Height];
			
			// BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
			
			// IntPtr pointer = bmpData.Scan0;
			// Marshal.Copy(pointer, pixels, 0, pixels.Length);
			// int index = 0;
			// for (int y = 0; y < image.Height; y++)
			// {
			// 	for (int x = 0; x < image.Width; x++)
			// 	{
			// 		if (x < left || x > (image.Width - 1 - right) || y < top || y > (image.Height - 1 - bottom)) pixels[index] = 0;
			// 		index++;
			// 	}
			// }
			// Marshal.Copy(pixels, 0, pointer, pixels.Length);
			// image.UnlockBits(bmpData);
			throw new NotImplementedException();
		}
		
		public void FillRectangle(byte colour, int left, int top, int width, int height)
		{
			for (int yy = top; yy < top + height; yy++)
			{
				if(yy < 0 || yy >= Height) continue;
				for (int xx = left; xx < left + width; xx++)
				{
					if(xx < 0 || xx >= Width) continue;
					_bitmap[xx, yy] = colour;
				}
			}
		}
		
		public static void FillRectangle(Bitmap image, byte colour, int top, int left, int width, int height)
		{
			// byte[] pixels = new byte[image.Width * image.Height];
			
			// BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
			
			// IntPtr pointer = bmpData.Scan0;
			// Marshal.Copy(pointer, pixels, 0, pixels.Length);
			// int index = 0;
			// for (int y = 0; y < image.Height; y++)
			// {
			// 	for (int x = 0; x < image.Width; x++)
			// 	{
			// 		if (x >= left && x < (left + width) && y >= top && y < (top + height)) pixels[index] = colour;
			// 		index++;
			// 	}
			// }
			// Marshal.Copy(pixels, 0, pointer, pixels.Length);
			// image.UnlockBits(bmpData);
			throw new NotImplementedException();
		}
		
		public static void ReplaceColoursClip(Bitmap image, byte colourFrom, byte colourTo, int top, int right, int bottom, int left)
		{
			// byte[] pixels = new byte[image.Width * image.Height];
			
			// BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
			
			// IntPtr pointer = bmpData.Scan0;
			// Marshal.Copy(pointer, pixels, 0, pixels.Length);
			// int index = 0;
			// for (int y = 0; y < image.Height; y++)
			// {
			// 	for (int x = 0; x < image.Width; x++)
			// 	{
			// 		if ((x < left || x > (image.Width - 1 - right) || y < top || y > (image.Height - 1 - bottom)) && pixels[index] == colourFrom) pixels[index] = colourTo;
			// 		index++;
			// 	}
			// }
			// Marshal.Copy(pixels, 0, pointer, pixels.Length);
			// image.UnlockBits(bmpData);
			throw new NotImplementedException();
		}
		
		public void DrawText(string text, int font, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			if (colour == 0) return;
			DrawText(text, font, colour, colour, x, y, align);
		}
		public void DrawText(string text, int font, byte firstLetterColour, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			Picture textImage = Resources.Instance.GetText(text, font, firstLetterColour, colour);
			DrawText(textImage, align, x, y);
		}
		private void DrawText(Picture textImage, TextAlign align, int x, int y)
		{
			switch (align)
			{
				case TextAlign.Center:
					x -= (textImage.Width / 2);
					break;
				case TextAlign.Right:
					x -= textImage.Width;
					break;
			}
			AddLayer(textImage, x, y);
		}
		
		public void ResetPalette()
		{
			for (int i = 0; i < 256; i++)
				_palette[i] = _originalColours[i];
		}

		public void SetPalette(Color[] colours)
		{
			for (int i = 1; i < colours.Length && i < 256; i++)
				_palette[i] = colours[i];
		}
		
		public Picture Cycle(int colour, ref Color[] colours)
		{
			Color reserve = Palette[colour];
			Palette[colour] = colours[0];
			for (int i = 0; i < colours.Length - 1; i++)
				colours[i] = colours[i + 1];
			colours[colours.Length - 1] = reserve;
			Palette = colours;
			return this;
		}
		
		public Picture Cycle(int start, int end)
		{
			if (start > end) return CycleReverse(end, start);
			
			Color reserve = _palette[end];
			for (int i = end; i > start; i--)
				_palette[i] = _palette[i - 1];
			_palette[start] = reserve;
			return this;
		}
		
		private Picture CycleReverse(int start, int end)
		{
			Color reserve = _palette[start];
			for (int i = start; i < end; i++)
				_palette[i] = _palette[i + 1];
			_palette[end] = reserve;
			return this;
		}

		public Picture GetPart(int x, int y, int width, int height)
		{
			byte[,] bytes = new byte[width, height];
			for (int yy = y; yy < (y + height) && yy < Height; yy++)
			for (int xx = x; xx < (x + width) && xx < Width; xx++)
			{
				bytes[xx - x, yy - y] = _bitmap[xx, yy];
			}
			return new Picture(bytes, Palette);
		}
		
		public void FillLayerTile(Picture layer, int x = 0, int y = 0)
		{
			for (int xx = x; xx < Width; xx += layer.Width)
			for (int yy = y; yy < Height; yy += layer.Height)
			{
				AddLayer(layer, new Point(xx, yy));
			}
		}
		
		public void AddBorder(byte colourLight, byte colourDark, int x, int y, int width, int height, int depth = 0)
		{
			int w = x + (width - 1);
			int h = y + (height - 1);
			
			FillRectangle(colourLight, x + depth, y + depth, 1, h - (depth * 2));
			FillRectangle(colourLight, x + depth, h - depth, w - (depth * 2), 1);
			FillRectangle(colourDark, x + depth, y + depth, w - (depth * 2), 1);
			FillRectangle(colourDark, w - depth, y + depth, 1, h - (depth * 2) + 1);
		}

		public void AddLine(byte colour, int x1, int y1, int x2, int y2)
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
				_bitmap[(int)Math.Round(xx), (int)Math.Round(yy)] = colour;
				xx += incX;
				yy += incY;
			}
		}
		
		public void AddLayer(Picture layer, int x = 0, int y = 0)
		{
			AddLayer(layer, new Point(x, y));
		}
		public void AddLayer(Picture layer, Point offset)
		{
			if (layer == null) return;
			
			int layerWidth = layer.Width;
			int layerHeight = layer.Height;
			int imageWidth = Width;
			int imageHeight = Height;

			for (int yy = 0; yy < layerHeight; yy++)
			{
				if (yy + offset.Y >= imageHeight) continue;
				for (int xx = 0; xx < layerWidth; xx++)
				{
					if (xx + offset.X >= imageWidth) continue;
					if (layer[xx, yy] == 0) continue;
					if (xx + offset.X < 0 || yy + offset.Y < 0) continue;
					_bitmap[xx + offset.X, yy + offset.Y] = layer[xx, yy];
				}
			}
		}
		
		public void ColourReplace(byte colourFrom, byte colourTo, int x, int y, int width, int height)
		{
			for (int yy = y; yy < y + height; yy++)
			for (int xx = x; xx < x + width; xx++)
			{
				if (_bitmap[xx, yy] != colourFrom) continue;
				_bitmap[xx, yy] = colourTo;
			}
		}
		
		public void ApplyNoise(byte[,] noiseMap, int step)
		{
			for (int y = 0; y < Height; y++)
			for (int x = 0; x < Width; x++)
			{
				if (noiseMap[x, y] < step) continue;
				_bitmap[x, y] = 0;
			}
		}
		
		private static Color[] EmptyPalette
		{
			get
			{
				Color[] colours = new Color[256];
				for (int i = 0; i < colours.Length; i++)
					colours[i] = Color.Black;
				return colours;
			}
		}
		
		public byte this[int x, int y]
		{
			get
			{
				if (x < 0 || x >= Width || y < 0 || y >= Height) return 0;
				
				return _bitmap[x, y];
			}
			internal set
			{
				if (x < 0 || x >= Width || y < 0 || y >= Height) return;

				FillRectangle(value, x, y, 1, 1);
			}
		}
		
		public Picture(byte[,] bytes, Color[] colours)
		{
			_cache = new Dictionary<string, Bitmap>();
			_originalColours = colours;
			for (int i = 0; i < colours.Length; i++)
				_palette[i] = colours[i];
			_bitmap = bytes;
		}
		
		public Picture(Picture picture) : this(picture.Width, picture.Height, picture.Palette)
		{
			AddLayer(picture);
		}
		
		public Picture(int width, int height) : this(width, height, EmptyPalette)
		{
		}
		
		public Picture(int width, int height, byte[] bytes, Color[] colours) : this(width, height, colours)
		{
			for (int yy = 0; yy < height; yy++)
			for (int xx = 0; xx < width; xx++)
			{
				int index = (yy * width) + xx;
				if (index > bytes.Length) continue;
				_bitmap[xx, yy] = bytes[index];
			}
		}
		
		public Picture(int width, int height, Color[] colours)
		{
			_cache = new Dictionary<string, Bitmap>();
			_originalColours = colours;
			for (int i = 0; i < colours.Length; i++)
				_palette[i] = colours[i];
			_bitmap = new byte[width, height];
		}
	}
}