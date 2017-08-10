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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using CivOne.Graphics;

namespace CivOne
{
	internal class Canvas : IBitmap
	{
		public Color[] Palette { get; private set; }
		public byte[,] Bitmap { get; private set; }

		internal int Width => Bitmap.GetLength(0);
		internal int Height => Bitmap.GetLength(1);

		internal Bitmap Image
		{
			get
			{
				if (Palette == null || Bitmap == null)
				{
					return new Bitmap(16, 16);
				}

				Bitmap output = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
				for (int y = 0; y < Height; y++)
				{
					byte[] imgData = new byte[Width];
					for (int x = 0; x < Width; x++)
					{
						imgData[x] = Bitmap[x, y];
					}
					BitmapData bmpData = output.LockBits(new Rectangle(0, y, Width, 1), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
					Marshal.Copy(imgData, 0, bmpData.Scan0, Width);
					output.UnlockBits(bmpData);
				}
				
				ColorPalette palette = output.Palette;
				for (int i = 0; i < Palette.Length; i++)
					palette.Entries[i] = Palette[i];
				output.Palette = palette;
				return output;
			}
		}

		internal Canvas(IBitmap bitmap)
		{
			if (bitmap == null) return;
			Palette = bitmap.Palette;
			Bitmap = bitmap.Bitmap;
		}
	}
}