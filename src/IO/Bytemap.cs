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
using System.Runtime.InteropServices;

namespace CivOne.IO
{
	public class Bytemap : IDisposable
	{
		private IntPtr _handle;

		public readonly int Width, Height, Length;

		public byte this[int x, int y]
		{
			get => Marshal.ReadByte(_handle, (Width * y) + x);
			set => Marshal.WriteByte(_handle, (Width * y) + x, value);
		}

		public IEnumerable<byte> ToBytes(bool rightToLeft = false, bool bottomToTop = false)
		{
			for (int yy = 0; yy < Height; yy++)
			{
				int y = (bottomToTop ? (Height - yy - 1) : yy);
				for (int xx = 0; xx < Width; xx++)
				{
					int x = (rightToLeft ? (Width - xx - 1) : xx);
					yield return this[x, y];
				}
			}
		}

		public int[] ToColourMap(int[] palette, bool rightToLeft = false, bool bottomToTop = false)
		{
			int[] output = new int[Length];
			IntPtr ptr = Marshal.AllocHGlobal(Length * 4);
			int i = 0;
			for (int yy = 0; yy < Height; yy++)
			{
				int y = (bottomToTop ? (Height - yy - 1) : yy);
				for (int xx = 0; xx < Width; xx++)
				{
					int x = (rightToLeft ? (Width - xx - 1) : xx);
					Marshal.WriteInt32(ptr, i, palette[this[x, y]]);
					i += 4;
				}
			}
			Marshal.Copy(ptr, output, 0, output.Length);
			Marshal.FreeHGlobal(ptr);
			return output;
		}

		public byte[] ToByteArray()
		{
			byte[] output = new byte[Length];
			Marshal.Copy(_handle, output, 0, output.Length);
			return output;
		}

		public static Bytemap Copy(Bytemap bytemap)
		{
			Bytemap output = new Bytemap(bytemap.Width, bytemap.Height);
			byte[] copy = bytemap.ToByteArray();
			Marshal.Copy(copy, 0, output._handle, copy.Length);
			return output;
		}

		public Bytemap(int width, int height)
		{
			Width = width;
			Height = height;
			Length = (width * height);
			_handle = Marshal.AllocHGlobal(Length);

			if (Length % 8 == 0)
				for (int i = 0; i < Length; i += 8) Marshal.WriteInt64(_handle, i, 0);
			else if (Length % 4 == 0)
				for (int i = 0; i < Length; i += 4) Marshal.WriteInt32(_handle, i, 0);
			else if (Length % 2 == 0)
				for (int i = 0; i < Length; i += 2) Marshal.WriteInt16(_handle, i, 0);
			else
				for (int i = 0; i < Length; i++) Marshal.WriteByte(_handle, i, 0);
		}

		public Bytemap(byte[,] bytes) : this(bytes.GetLength(0), bytes.GetLength(1))
		{
			for (int y = bytes.GetUpperBound(1); y >= 0; y--)
			for (int x = bytes.GetUpperBound(0); x >= 0; x--)
			{
				this[x, y] = bytes[x, y];
			}
		}

		~Bytemap()
		{
			if (_handle == IntPtr.Zero) return;
			Marshal.FreeHGlobal(_handle);
			_handle = IntPtr.Zero;
		}

		public void Dispose()
		{
			if (_handle == IntPtr.Zero) return;
			Marshal.FreeHGlobal(_handle);
			_handle = IntPtr.Zero;
			GC.SuppressFinalize(this);
		}
	}
}