// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Runtime.InteropServices;

namespace CivOne.IO
{
	public class Bytemap : BaseUnmanaged
	{
		public readonly int Width, Height;
		public int Length => base.Size;

		public byte this[int x, int y]
		{
			get => ReadByte((Width * y) + x);
			set => WriteByte((Width * y) + x, value);
		}

		public new void Clear() => base.Clear();

		public int[] ToColourMap(int[] palette, bool rightToLeft = false, bool bottomToTop = false)
		{
			int[] output = new int[Length];
			int i = 0;
			for (int yy = 0; yy < Height; yy++)
			{
				int y = (bottomToTop ? (Height - yy - 1) : yy);
				for (int xx = 0; xx < Width; xx++)
				{
					int x = (rightToLeft ? (Width - xx - 1) : xx);
					output[i++] = palette[this[x, y]];
				}
			}
			return output;
		}

		public new byte[] ToByteArray() => base.ToByteArray();

		public static Bytemap Copy(Bytemap source) => new Bytemap(source);

		private Bytemap(Bytemap source) : base(source)
		{
			Width = source.Width;
			Height = source.Height;
		}

		public Bytemap(int width, int height) : base(width * height, true)
		{
			Width = width;
			Height = height;
		}

		public Bytemap(byte[,] bytes) : this(bytes.GetLength(0), bytes.GetLength(1))
		{
			for (int y = bytes.GetUpperBound(1); y >= 0; y--)
			for (int x = bytes.GetUpperBound(0); x >= 0; x--)
			{
				this[x, y] = bytes[x, y];
			}
		}
	}
}