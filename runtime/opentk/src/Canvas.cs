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
using System.Runtime.InteropServices;
using CivOne.Graphics;
using CivOne.IO;

namespace CivOne
{
	internal class Canvas : IBitmap
	{
		public Palette Palette { get; private set; }

		public Bytemap Bitmap { get; private set; }

		internal int Width => Bitmap.Width;
		internal int Height => Bitmap.Height;

		private int[] PaletteArray
		{
			get
			{
				int[] output = new int[Palette.Length];
				IntPtr ptr = Marshal.AllocHGlobal(Palette.Length * 4);
				for (int i = 0; i < output.Length; i++)
				{
					Colour colour = Palette[i];
					Marshal.WriteInt32(ptr, (i * 4), ((int)colour.A << 24) + ((int)colour.B << 16) + ((int)colour.G << 8) + ((int)colour.R));
				}
				Marshal.Copy(ptr, output, 0, output.Length);
				Marshal.FreeHGlobal(ptr);
				return output;
			}
		}

		public int[] ColourMap => Bitmap.ToColourMap(PaletteArray, bottomToTop: true);

		internal Canvas(IBitmap bitmap)
		{
			if (bitmap == null) return;
			Palette = bitmap.Palette;
			Bitmap = bitmap.Bitmap;
		}

		public void Dispose() => Bitmap?.Dispose();
	}
}