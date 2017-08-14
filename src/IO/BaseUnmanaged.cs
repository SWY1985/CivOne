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
	public abstract class BaseUnmanaged : IDisposable
	{
		private IntPtr _handle;
		protected int Size { get; private set; }

		protected byte ReadByte(int offset) => Marshal.ReadByte(_handle, offset);
		protected short ReadShort(int offset) => Marshal.ReadInt16(_handle, offset);
		protected int ReadInt(int offset) => Marshal.ReadInt32(_handle, offset);
		protected long ReadLong(int offset) => Marshal.ReadInt64(_handle, offset);

		protected void WriteByte(int offset, byte value) => Marshal.WriteByte(_handle, offset, value);
		protected void WriteShort(int offset, short value) => Marshal.WriteInt16(_handle, offset, value);
		protected void WriteInt(int offset, int value) => Marshal.WriteInt32(_handle, offset, value);
		protected void WriteLong(int offset, long value) => Marshal.WriteInt64(_handle, offset, value);

		protected byte[] ToByteArray()
		{
			byte[] output = new byte[Size];
			Marshal.Copy(_handle, output, 0, output.Length);
			return output;
		}

		protected unsafe BaseUnmanaged(BaseUnmanaged source)
		{
			Size = source.Size;
			_handle = Marshal.AllocHGlobal(Size);
			Buffer.MemoryCopy((byte*)source._handle, (byte*)_handle, source.Size, Size);
		}

		protected BaseUnmanaged(int size, bool initializeZero = false)
		{
			Size = size;
			_handle = Marshal.AllocHGlobal(Size);

			if (!initializeZero) return;
			Marshal.Copy(new byte[size], 0, _handle, size);
		}

		~BaseUnmanaged()
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