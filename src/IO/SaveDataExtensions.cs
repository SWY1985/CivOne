// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CivOne.IO
{
	internal unsafe static partial class SaveDataExtensions
	{
		private static byte[] GetByteArray(byte* ptr, int length)
		{
			byte[] output = new byte[length];
			for (int i = 0; i < length; i++)
				output[i] = ptr[i];
			return output;
		}

		private static void SetByteArray(byte *ptr, params byte[] values)
		{
			for (int i = 0; i < values.Length; i++)
				ptr[i] = values[i];
		}

		private static short[] GetShortArray(short* ptr, int length)
		{
			short[] output = new short[length];
			for (int i = 0; i < length; i++)
				output[i] = ptr[i];
			return output;
		}

		private static void SetShortArray(short* ptr, params short[] values)
		{
			for (int i = 0; i < values.Length; i++)
				ptr[i * 2] = values[i];
		}

		private static ushort[] GetUShortArray(ushort* ptr, int length)
		{
			ushort[] output = new ushort[length];
			for (int i = 0; i < length; i++)
				output[i] = ptr[i];
			return output;
		}

		private static void SetUShortArray(ushort* ptr, params ushort[] values)
		{
			for (int i = 0; i < values.Length; i++)
				ptr[i * 2] = values[i];
		}

		private static IEnumerable<byte> GetBitIds(byte[] bytes, int startIndex, int length)
		{
			byte index = 0;
			for (int i = startIndex; i < startIndex + length; i++)
			for (int b = 0; b < 8; b++)
			{
				if ((bytes[i] & (1 << b)) > 0) yield return index;
				index++;
			}
		}

		private static void SetBitIds(ref byte[] bytes, int startIndex, int length, params byte[] values)
		{
			foreach (byte value in values)
			{
				int bitNo = value % 8;
				int byteNo = (value - bitNo) / 8;
				if (length <= byteNo) continue;
				bytes[startIndex + byteNo] |= (byte)(1 << bitNo);
			}
		}

		private static string BytesToString(byte[] bytes, int startIndex, int length)
		{
			StringBuilder output = new StringBuilder();
			for (int i = startIndex; i < startIndex + length; i++)
			{
				if (bytes[i] == 0) break;
				output.Append((char)bytes[i]);
			}
			return output.ToString().Trim();
		}

		private static string[] GetStringArray(byte* ptr, int itemCount, int itemLength)
		{
			byte[] bytes = GetByteArray(ptr, (itemCount * itemLength));
			string[] output = new string[itemCount];
			for (int i = 0; i < itemCount; i++)
				output[i] = BytesToString(bytes, (i * itemLength), itemLength);
			return output;
		}

		private static void SetStringArray(byte *ptr, int itemLength, params string[] values)
		{
			byte[] bytes = new byte[itemLength * values.Length];
			for (int i = 0; i < values.Length; i++)
			for (int c = 0; c < itemLength; c++)
				bytes[(i * itemLength) + c] = (c >= values[i].Length) ? (byte)0 : (byte)values[i][c];
			SetByteArray(ptr, bytes);
		}
	}
}