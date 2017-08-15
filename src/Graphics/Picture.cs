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
using System.Linq;
using CivOne.Enums;
using CivOne.IO;

namespace CivOne.Graphics
{
	public class Picture : IBitmap
	{
		private readonly Palette _originalColours;
		private readonly Palette _palette = new Palette();
		private readonly Bytemap _bitmap;

		public int Width => _bitmap.Width;
		public int Height => _bitmap.Height;
		public Size Size => new Size(_bitmap.Width, _bitmap.Height);
		public Palette OriginalColours => _originalColours;

		public Palette Palette
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
		
		public Bytemap Bitmap => _bitmap;

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
		
		public void ResetPalette()
		{
			for (int i = 0; i < 256; i++)
				_palette[i] = _originalColours[i];
		}

		public void SetPalette(Palette palette)
		{
			for (int i = 1; i < palette.Length && i < 256; i++)
				_palette[i] = palette[i];
		}
		
		public Picture Cycle(int colour, ref Colour[] colours)
		{
			Colour reserve = Palette[colour];
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
			
			Colour reserve = _palette[end];
			for (int i = end; i > start; i--)
				_palette[i] = _palette[i - 1];
			_palette[start] = reserve;
			return this;
		}
		
		private Picture CycleReverse(int start, int end)
		{
			Colour reserve = _palette[start];
			for (int i = start; i < end; i++)
				_palette[i] = _palette[i + 1];
			_palette[end] = reserve;
			return this;
		}

		public Picture GetPart(int left, int top, int width, int height) => new Picture(_bitmap[left, top, width, height], Palette);
		
		public void AddBorder(byte colourLight, byte colourDark, int x, int y, int width, int height, int depth = 0)
		{
			int w = x + (width - 1);
			int h = y + (height - 1);
			
			this.FillRectangle(colourLight, x + depth, y + depth, 1, h - (depth * 2));
			this.FillRectangle(colourLight, x + depth, h - depth, w - (depth * 2), 1);
			this.FillRectangle(colourDark, x + depth, y + depth, w - (depth * 2), 1);
			this.FillRectangle(colourDark, w - depth, y + depth, 1, h - (depth * 2) + 1);
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
		
		private static Colour[] EmptyPalette = Enumerable.Range(0, 256).Select(_ => new Colour()).ToArray();
		
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

				this.FillRectangle(value, x, y, 1, 1);
			}
		}
		
		public Picture(byte[,] bytes, Palette palette)
		{
			_originalColours = palette.Copy();
			_palette = palette.Copy();
			_bitmap = new Bytemap(bytes);
		}
		
		public Picture(Bytemap bytemap, Palette palette)
		{
			_originalColours = palette.Copy();
			_palette = palette.Copy();
			_bitmap = Bytemap.Copy(bytemap);
		}
		
		public Picture(IBitmap picture) : this(picture.Bitmap, picture.Palette)
		{
		}
		
		public Picture(int width, int height) : this(width, height, EmptyPalette)
		{
		}
		
		public Picture(int width, int height, byte[] bytes, Palette palette) : this(width, height, palette)
		{
			for (int yy = 0; yy < height; yy++)
			for (int xx = 0; xx < width; xx++)
			{
				int index = (yy * width) + xx;
				if (index > bytes.Length) continue;
				_bitmap[xx, yy] = bytes[index];
			}
		}
		
		public Picture(int width, int height, Palette palette)
		{
			_originalColours = palette.Copy();
			_palette = palette.Copy();
			_bitmap = new Bytemap(width, height);
		}

		public void Dispose()
		{
			_bitmap.Dispose();
		}
	}
}