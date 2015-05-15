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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using CivOne.Enums;

namespace CivOne.GFX
{
	public class Picture
	{
		private readonly Color[] _originalColours;
		private readonly ColorPalette _palette;
		private readonly Dictionary<string, Bitmap> _cache;
		private readonly Bitmap _image;
		private readonly byte[,] _bitmap;
				
		public Bitmap Image
		{
			get
			{
				return (Bitmap)_image.Clone();
			}
		}

		public Color[] OriginalColours
		{
			get
			{
				return _originalColours;
			}
		}

		public byte[,] GetBitmap
		{
			get
			{
				return _bitmap;
			}
		}

		public static void ReplaceColours(Bitmap image, byte colourFrom, byte colourTo)
		{
			ReplaceColours(image, new[] { colourFrom }, new[] { colourTo });
		}
		public static void ReplaceColours(Bitmap image, byte[] coloursFrom, byte[] coloursTo)
		{
			byte[] pixels = new byte[image.Width * image.Height];

			BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

			IntPtr pointer = bmpData.Scan0;
			Marshal.Copy(pointer, pixels, 0, pixels.Length);
			for (int j = 0; j < coloursFrom.Length && j < coloursTo.Length; j++)
			{
				for (int i = 0; i < pixels.Length; i++)
					if (pixels[i] == coloursFrom[j])
						pixels[i] = coloursTo[j];
			}
			Marshal.Copy(pixels, 0, pointer, pixels.Length);
			image.UnlockBits(bmpData);
		}
		public static void Clip(Bitmap image, int top, int right, int bottom, int left)
		{
			byte[] pixels = new byte[image.Width * image.Height];

			BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

			IntPtr pointer = bmpData.Scan0;
			Marshal.Copy(pointer, pixels, 0, pixels.Length);
			int index = 0;
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					if (x < left || x > (image.Width - 1 - right) || y < top || y > (image.Height - 1 - bottom)) pixels[index] = 0;
					index++;
				}
			}
			Marshal.Copy(pixels, 0, pointer, pixels.Length);
			image.UnlockBits(bmpData);
		}

		public void FillRectangle(byte colour, int left, int top, int width, int height)
		{
			byte[] pixels = new byte[width];
			for (int i = 0; i < pixels.Length; i++)
				pixels[i] = colour;

			for (int y = 0; y < height; y++)
			{
				BitmapData bmpData = _image.LockBits(new Rectangle(left, top + y, width, 1), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
				Marshal.Copy(pixels, 0, bmpData.Scan0, width);
				_image.UnlockBits(bmpData);
			}
		}

		public static void FillRectangle(Bitmap image, byte colour, int top, int left, int width, int height)
		{
			byte[] pixels = new byte[image.Width * image.Height];

			BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

			IntPtr pointer = bmpData.Scan0;
			Marshal.Copy(pointer, pixels, 0, pixels.Length);
			int index = 0;
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					if (x >= left && x < (left + width) && y >= top && y < (top + height)) pixels[index] = colour;
					index++;
				}
			}
			Marshal.Copy(pixels, 0, pointer, pixels.Length);
			image.UnlockBits(bmpData);
		}

		public static void ReplaceColoursClip(Bitmap image, byte colourFrom, byte colourTo, int top, int right, int bottom, int left)
		{
			byte[] pixels = new byte[image.Width * image.Height];

			BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

			IntPtr pointer = bmpData.Scan0;
			Marshal.Copy(pointer, pixels, 0, pixels.Length);
			int index = 0;
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					if ((x < left || x > (image.Width - 1 - right) || y < top || y > (image.Height - 1 - bottom)) && pixels[index] == colourFrom) pixels[index] = colourTo;
					index++;
				}
			}
			Marshal.Copy(pixels, 0, pointer, pixels.Length);
			image.UnlockBits(bmpData);
		}
		
		public void DrawText(string text, int font, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			DrawText(text, font, colour, colour, x, y, align);
		}
		public void DrawText(string text, int font, byte firstLetterColour, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			Bitmap textImage = Resources.Instance.GetText(text, font, firstLetterColour, colour);
			DrawText(textImage, align, x, y);
		}
		private void DrawText(Bitmap textImage, TextAlign align, int x, int y)
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
				_palette.Entries[i] = _originalColours[i];
		}

		public void SetPalette(ColorPalette palette)
		{
			_image.Palette = palette;
		}

		public Picture Cycle(int colour, ref Color[] colours)
		{
			Color reserve = _palette.Entries[colour];
			_palette.Entries[colour] = colours[0];
			for (int i = 0; i < colours.Length - 1; i++)
				colours[i] = colours[i + 1];
			colours[colours.Length - 1] = reserve;
			_image.Palette = _palette;
			return this;
		}

		public Picture Cycle(int start, int end)
		{
			if (start > end) return CycleReverse(end, start);

			Color reserve = _palette.Entries[end];
			for (int i = end; i > start; i--)
				_palette.Entries[i] = _palette.Entries[i - 1];
			_palette.Entries[start] = reserve;
			_image.Palette = _palette;
			return this;
		}

		private Picture CycleReverse(int start, int end)
		{
			Color reserve = _palette.Entries[start];
			for (int i = start; i < end; i++)
				_palette.Entries[i] = _palette.Entries[i + 1];
			_palette.Entries[end] = reserve;
			_image.Palette = _palette;
			return this;
		}

		public Bitmap GetPart(int x, int y, int width, int height)
		{
			Bitmap output;
			string key = string.Format("{0}|{1}|{2}|{3}", x, y, width, height);
			if (_cache.ContainsKey(key))
			{
				output = _cache[key];
			}
			else
			{
				byte[] sourcePixels = new byte[_image.Width * _image.Height];
				BitmapData bmpData = _image.LockBits(new Rectangle(0, 0, _image.Width, _image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
				Marshal.Copy(bmpData.Scan0, sourcePixels, 0, sourcePixels.Length);
				_image.UnlockBits(bmpData);

				int ww = width;
				int hh = height;
				byte[] pixels = new byte[ww * hh];
				for (int yy = 0; yy < hh; yy++)
				{
					int sourceIndex = (y * bmpData.Stride) + (yy * bmpData.Stride) + x;
					Array.Copy(sourcePixels, sourceIndex, pixels, yy * ww, ww);
				}

				output = new Bitmap(ww, hh, PixelFormat.Format8bppIndexed);
				for (int yy = 0; yy < output.Height; yy++)
				{
					bmpData = output.LockBits(new Rectangle(0, yy, output.Width, 1), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
					Marshal.Copy(pixels, (output.Width * yy), bmpData.Scan0, output.Width);
					output.UnlockBits(bmpData);
				}
				
				_cache.Add(key, output);
			}
			output.Palette = _palette;
			return output;
		}

		public static Bitmap Combine(Bitmap baseImage, Bitmap layer)
		{
			return Combine(baseImage, layer, new Point(0, 0));
		}
		public static Bitmap Combine(Bitmap baseImage, Bitmap layer, int x, int y)
		{
			return Combine(baseImage, layer, new Point(x, y));
		}
		public static Bitmap Combine(Bitmap baseImage, Bitmap layer, Point offset)
		{
			//TODO: check larger layer than base image resolutions
			BitmapData bmpData;
			List<byte[]> bytemaps = new List<byte[]>();
			foreach (Bitmap image in new [] { baseImage, layer })
			{
				byte[] pixels = new byte[image.Width * image.Height];
				for (int y = 0; y < image.Height; y++)
				{
					bmpData = image.LockBits(new Rectangle(0, y, image.Width, 1), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
					Marshal.Copy(bmpData.Scan0, pixels, image.Width * y, image.Width);
					image.UnlockBits(bmpData);
				}

				bytemaps.Add(pixels);
			}

			offset = new Point(offset.X, offset.Y);

			byte[] imgData = bytemaps[0];            
			int index = 0;
			for (int yy = 0; yy < layer.Height; yy++)
			{
				int destIndex = (offset.Y * baseImage.Width) + (yy * baseImage.Width) + offset.X;
				for (int xx = 0; xx < layer.Width; xx++)
				{
					if (bytemaps[1][index] > 0)
					{
						imgData[destIndex] = bytemaps[1][index];
					}
					destIndex++;
					index++;
				}
			}

			Bitmap output = new Bitmap(baseImage.Width, baseImage.Height, PixelFormat.Format8bppIndexed);
			for (int y = 0; y < output.Height; y++)
			{
				bmpData = output.LockBits(new Rectangle(0, y, output.Width, 1), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
				Marshal.Copy(imgData, (output.Width * y), bmpData.Scan0, output.Width);
				output.UnlockBits(bmpData);
			}

			output.Palette = baseImage.Palette;

			return output;
		}
		
		public void AddLayer(Bitmap layer, int x = 0, int y = 0)
		{
			AddLayer(layer, new Point(x, y));
		}
		public void AddLayer(Bitmap layer, Point offset)
		{
			if (offset.X < 0 || offset.Y < 0 || offset.X + layer.Width > _image.Width || offset.Y + layer.Height > _image.Height) return;

			int layerWidth = layer.Width;
			int layerHeight = layer.Height;
			int imageWidth = _image.Width;
			int imageHeight = _image.Height;

			//TODO: check larger layer than base image resolutions
			BitmapData bmpData;

			byte[] bytemap = new byte[layerWidth * layerHeight];
			for (int y = 0; y < layer.Height; y++)
			{
				bmpData = layer.LockBits(new Rectangle(0, y, layerWidth, 1), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
				Marshal.Copy(bmpData.Scan0, bytemap, layerWidth * y, layerWidth);
				layer.UnlockBits(bmpData);
			}

			byte[] imgData = new byte[_image.Width * _image.Height];
			for (int y = 0; y < imageHeight; y++)
			{
				bmpData = _image.LockBits(new Rectangle(0, y, imageWidth, 1), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
				Marshal.Copy(bmpData.Scan0, imgData, imageWidth * y, imageWidth);
				_image.UnlockBits(bmpData);
			}

			int index = 0;
			for (int yy = 0; yy < layerHeight; yy++)
			{
				int destIndex = (offset.Y * imageWidth) + (yy * imageWidth) + offset.X;
				for (int xx = 0; xx < layerWidth; xx++)
				{
					if (bytemap[index] > 0)
					{
						imgData[destIndex] = bytemap[index];
					}
					destIndex++;
					index++;
				}
			}

			bmpData = _image.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			Marshal.Copy(imgData, 0, bmpData.Scan0, imgData.Length);
			_image.UnlockBits(bmpData);
		}

		public void ApplyNoise(byte[,] noiseMap, int step)
		{
			int imageWidth = _image.Width;
			int imageHeight = _image.Height;

			BitmapData bmpData;
			byte[] imgData = new byte[imageWidth * imageHeight];
			for (int y = 0; y < imageHeight; y++)
			{
				bmpData = _image.LockBits(new Rectangle(0, y, imageWidth, 1), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
				Marshal.Copy(bmpData.Scan0, imgData, imageWidth * y, imageWidth);
				_image.UnlockBits(bmpData);
			}

			for (int x = 0; x < _image.Width; x++)
			{
				for (int y = 0; y < _image.Height; y++)
				{
					if (noiseMap[x, y] < step) continue;
					imgData[(y * imageWidth) + x] = 0;
				}
			}

			bmpData = _image.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			Marshal.Copy(imgData, 0, bmpData.Scan0, imgData.Length);
			_image.UnlockBits(bmpData);
		}

		public Picture(byte[,] bytes, Color[] colours)
		{
			_cache = new Dictionary<string, Bitmap>();
			_originalColours = colours;

			_image = new Bitmap(bytes.GetLength(0), bytes.GetLength(1), PixelFormat.Format8bppIndexed);
			for (int y = 0; y < _image.Height; y++)
			{
				byte[] imgData = new byte[_image.Width];
				for (int x = 0; x < _image.Width; x++)
				{
					imgData[x] = bytes[x, y];
				}
				BitmapData bmpData = _image.LockBits(new Rectangle(0, y, _image.Width, 1), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
				Marshal.Copy(imgData, 0, bmpData.Scan0, _image.Width);
				_image.UnlockBits(bmpData);
			}

			_palette = _image.Palette;
			for (int i = 0; i < colours.Length; i++)
				_palette.Entries[i] = colours[i];
			_image.Palette = _palette;
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
		
		public Picture(int width, int height) : this(width, height, EmptyPalette)
		{
		}

		public Picture(int width, int height, Color[] colours)
		{
			_cache = new Dictionary<string, Bitmap>();
			_originalColours = colours;

			_image = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

			_palette = _image.Palette;
			for (int i = 0; i < colours.Length; i++)
				_palette.Entries[i] = colours[i];
			_image.Palette = _palette;
		}

		public Picture(int width, int height, byte[] bytes, Color[] colours)
		{
			_cache = new Dictionary<string, Bitmap>();
			_originalColours = colours;

			_bitmap = new byte[width, height];
			int ind = 0;
			for (int y = 0; (y < height); y++)
				for (int x = 0; (x < width); x++)
					_bitmap[x, y] = bytes[ind++];

			byte[] pixels = new byte[width * height];
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					pixels[(y * width) + x] = _bitmap[x, y];

			_image = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

			BitmapData bmpData = _image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			IntPtr pointer = bmpData.Scan0;
			Marshal.Copy(pixels, 0, pointer, pixels.Length);
			_image.UnlockBits(bmpData);

			_palette = _image.Palette;
			for (int i = 0; i < colours.Length; i++)
				_palette.Entries[i] = colours[i];
			_image.Palette = _palette;
		}
	}
}