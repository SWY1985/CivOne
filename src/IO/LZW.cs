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

namespace CivOne.IO
{
	public static class LZW
	{
		private class ByteList
		{
			private readonly List<byte> _byteList = new List<byte>();
			private sbyte _byteNumber = 0;
			private byte _byte = 0;

			public void Add(int entry, int dictionarySize)
			{
				int codeLength = CodeLength(dictionarySize - 1);
				for (int bit = 0; bit < codeLength; bit++)
				{
					int outputBit = (entry & (0x01 << bit)) >> bit;
					_byte |= (byte)(outputBit << _byteNumber++);
					if (_byteNumber < 8) continue;
					_byteList.Add(_byte);
					_byteNumber = 0;
					_byte = 0;
				}
			}

			public byte[] ToArray()
			{
				return _byteList.ToArray();
			}
		}

		private static byte CodeLength(int input)
		{
			for (int i = 31; i >= 0; i--)
			{
				if (((input >> i) & 1) == 1) return (byte)(i + 1);
			}
			return 1;
		}

		public static byte[] Encode(byte[] input, bool clearEnd = false, int maxBits = 11)
		{
			Dictionary<string, int> dictionary = Enumerable.Range(0, 256).ToDictionary(x => x.ToString(), x => x);
			ByteList byteList = new ByteList();

			if (clearEnd)
			{
				dictionary.Add("CLR", dictionary.Count);
				dictionary.Add("END", dictionary.Count);
				byteList.Add(dictionary["CLR"], dictionary.Count);
			}
			
			byte[] entry = new byte[0];
			for (int i = 0; i < input.Length; i++)
			{
				byte[] newEntry = entry.Append(input[i]).ToArray();
				if (dictionary.ContainsKey(string.Join(",", newEntry)))
				{
					entry = newEntry;
					continue;
				}
				byteList.Add(dictionary[string.Join(",", entry)], dictionary.Count);
				if (dictionary.Count < ((0x01 << maxBits) - 1))
				{
					dictionary.Add(string.Join(",", newEntry), dictionary.Count);
				}
				entry = new byte[] { input[i] };
			}

			if (entry.Length > 0)
			{
				byteList.Add(dictionary[string.Join(",", entry)], dictionary.Count);
			}

			if (clearEnd)
			{
				byteList.Add(dictionary["END"], dictionary.Count);
			}

			return byteList.ToArray();
		}
	}
}