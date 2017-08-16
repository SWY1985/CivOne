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
using System.Runtime.InteropServices;
using CivOne.Graphics;
using CivOne.Screens;

namespace CivOne.IO
{
	public class Bytemap : BaseUnmanaged
	{
		public readonly int Width, Height;
		public new Size Size => new Size(Width, Height);
		public int Length => base.Size;

		public byte this[int x, int y]
		{
			get => ReadByte((Width * y) + x);
			set => WriteByte((Width * y) + x, value);
		}

		internal Bytemap this[int left, int top, int width, int height]
		{
			get
			{
				int dx = 0, dy = 0;
				int sx1 = left, sy1 = top, sx2 = left + width, sy2 = top + height;
				if (sx1 < 0) { dx -= sx1; sx1 = 0; }
				if (sy1 < 0) { dy -= sy1; sy1 = 0; }
				if (sx2 > Width) sx2 = Width;
				if (sy2 > Height) sy2 = Height;

				byte[] buffer = new byte[sx2 - sx1];
				Bytemap output = new Bytemap(width, height);
				for (int yy = sy1; yy < sy2; yy++)
				{
					Marshal.Copy(IntPtr.Add(_handle, (Width * yy) + sx1), buffer, 0, buffer.Length);
					Marshal.Copy(buffer, 0, IntPtr.Add(output._handle, ((yy - top + dy) * buffer.Length) + dx), buffer.Length);
				}
				return output;
			}
		}

		internal void FillRectangle(int left, int top, int width, int height, byte colour)
		{
			if (left < 0) { width -= left; left = 0; }
			if (top < 0) { height -= top; top = 0; }
			if (left + width > Width) width = Width - left;
			if (top + height > Height) height = Height - top;

			byte[] buffer = new byte[width].Clear(colour);
			for (int yy = top; yy < (top + height); yy++)
			{
				Marshal.Copy(buffer, 0, IntPtr.Add(_handle, (Width * yy) + left), buffer.Length);
			}
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