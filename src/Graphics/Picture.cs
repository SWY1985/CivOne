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

		public Picture GetPart(int left, int top, int width, int height) => new Picture(_bitmap[left, top, width, height], Palette.Copy());
		
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

				Bitmap[x, y] = value;
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