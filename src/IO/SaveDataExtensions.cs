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
using System.Linq;
using System.Runtime.InteropServices;
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

		private static short[] GetShortArray(short* ptr, int length)
		{
			short[] output = new short[length];
			for (int i = 0; i < length; i++)
				output[i] = ptr[i];
			return output;
		}

		private static ushort[] GetUShortArray(ushort* ptr, int length)
		{
			ushort[] output = new ushort[length];
			for (int i = 0; i < length; i++)
				output[i] = ptr[i];
			return output;
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
	}
}