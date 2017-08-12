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

namespace CivOne.Graphics
{
	public class Picture : IBitmap
	{
		private readonly Color[] _originalColours;
		private readonly Color[] _palette = new Color[256];
		private readonly byte[,] _bitmap;

		public int Width => _bitmap.GetLength(0);
		public int Height => _bitmap.GetLength(1);
		public Size Size => new Size(_bitmap.GetLength(0), _bitmap.GetLength(1));
		public Color[] OriginalColours => _originalColours;

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
		
		public byte[,] Bitmap => _bitmap;

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
		
		public void DrawText(string text, int font, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			if (colour == 0 || string.IsNullOrWhiteSpace(text)) return;
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
					x -= (textImage.Width + 1) / 2;
					break;
				case TextAlign.Right:
					x -= textImage.Width;
					break;
			}
			this.AddLayer(textImage, x, y);
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

				this.FillRectangle(value, x, y, 1, 1);
			}
		}
		
		public Picture(byte[,] bytes, Color[] colours)
		{
			_originalColours = colours;
			for (int i = 0; i < colours.Length; i++)
				_palette[i] = colours[i];
			_bitmap = bytes;
		}
		
		public Picture(IBitmap picture) : this(picture.Bitmap, picture.Palette)
		{
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
			_originalColours = colours;
			for (int i = 0; i < colours.Length; i++)
				_palette[i] = colours[i];
			_bitmap = new byte[width, height];
		}
	}
}