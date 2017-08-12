// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Runtime.InteropServices;

namespace CivOne.IO
{
	public class Bytemap : IDisposable
	{
		private readonly IntPtr _handle;

		public readonly int Width, Height;

		public byte this[int x, int y]
		{
			get => Marshal.ReadByte(_handle, (Width * y) + x);
			set => Marshal.WriteByte(_handle, (Width * y) + x, value);
		}

		public byte[] ToByteArray()
		{
			byte[] output = new byte[Width * Height];
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
			_handle = Marshal.AllocHGlobal(width * height);
			for (int i = 0; i < (width * height); i++)
			{
				Marshal.WriteByte(_handle, i, 0);
			}
		}

		public Bytemap(byte[,] bytes) : this(bytes.GetLength(0), bytes.GetLength(1))
		{
			for (int y = bytes.GetUpperBound(1); y >= 0; y--)
			for (int x = bytes.GetUpperBound(0); x >= 0; x--)
			{
				this[x, y] = bytes[x, y];
			}
		}

		public void Dispose()
		{
			Marshal.FreeHGlobal(_handle);
		}
	}
}