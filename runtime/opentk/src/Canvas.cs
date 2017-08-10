// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Graphics;

namespace CivOne
{
	internal class Canvas : IBitmap
	{
		public Color[] Palette { get; private set; }

		public byte[,] Bitmap { get; private set; }

		internal int Width => Bitmap.GetLength(0);
		internal int Height => Bitmap.GetLength(1);

		private int GetColourInt(Color colour)
		{
			return ((int)colour.A << 24) + ((int)colour.B << 16) + ((int)colour.G << 8) + ((int)colour.R);
		}

		public int[] ColourMap
		{
			get
			{
				int[] output = new int[Width * Height];
				int i = 0;
				for (int yy = Bitmap.GetUpperBound(1); yy >= 0; yy--)
				for (int xx = 0; xx <= Bitmap.GetUpperBound(0); xx++)
				{
					output[i++] = GetColourInt(Palette[Bitmap[xx, yy]]);
				}
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