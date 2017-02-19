// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.IO;
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

			public void Close()
			{
				_byteList.Add(_byte);
			}

			public byte[] ToArray()
			{
				return _byteList.ToArray();
			}
		}

		private class ByteArray
		{
			public byte[] Bytes { get; private set; }
			private readonly string _byteString;
			public override string ToString()
			{
				return _byteString;
			}
			public static ByteArray Empty
			{
				get
				{
					return new ByteArray(new byte[0]);
				}
			}
			
			public override bool Equals (object obj)
			{
				if (obj == null || GetType() != obj.GetType()) return false;

				return (obj as ByteArray).ToString() == ToString();
			}

			// override object.GetHashCode
			public override int GetHashCode()
			{
				return ToString().GetHashCode();
			}

			public ByteArray(byte[] bytes)
			{
				Bytes = bytes;
				_byteString = string.Join(",", bytes);
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

		private static Dictionary<int, ByteArray> DecodeDictionary(bool clearEnd)
		{
			Dictionary<int, ByteArray> output = Enumerable.Range(0, 256).ToDictionary(x => x, x => new ByteArray(new byte[] { (byte)x }));
			output.Add(output.Count, ByteArray.Empty);
			if (clearEnd)
			{
				output.Add(output.Count, ByteArray.Empty);
			}
			return output;
		}
		
		public static byte[] Decode(byte[] input, bool clearEnd = false, bool flushDictionary = true, int maxBits = 11)
		{
			using (MemoryStream ms = new MemoryStream())
			using (BinaryWriter bw = new BinaryWriter(ms))
			{
				Dictionary<int, ByteArray> dictionary = DecodeDictionary(clearEnd);

				int value = 0;
				int counter = 0;
				byte[] entry = new byte[0];
				for (int i = 0; i < input.Length; i++)
				{
					int codeLength = CodeLength(dictionary.Count);
					if (codeLength > maxBits) codeLength = maxBits;
					for (int bit = 0; bit < 8; bit++)
					{
						value |= ((input[i] >> bit) & 0x01) << counter++;
						if (counter != codeLength) continue;
						
						if (!dictionary.ContainsKey(value) && (flushDictionary || dictionary.Count < ((0x01 << maxBits) - 1)))
						{
							dictionary.Add(dictionary.Count, new ByteArray(entry.Append(entry[0]).ToArray()));
						}
						
						ByteArray outVal = dictionary[value];
						ByteArray newEntry = new ByteArray(entry.Append(outVal.Bytes[0]).ToArray());
						bw.Write(outVal.Bytes);

						if (!dictionary.ContainsValue(newEntry) && (flushDictionary || dictionary.Count < ((0x01 << maxBits) - 1)))
						{
							dictionary.Add(dictionary.Count, newEntry);
						}
						entry = outVal.Bytes;
						value = 0;
						counter = 0;
						
						if (flushDictionary && CodeLength(dictionary.Count) > maxBits)
						{
							dictionary = DecodeDictionary(clearEnd);
							entry = new byte[0];
						}
					}
				}

				return ms.ToArray();
			}
		}

		public static byte[] Encode(byte[] input, bool clearEnd = false, int maxBits = 11)
		{
			Dictionary<string, int> dictionary = Enumerable.Range(0, 256).ToDictionary(x => x.ToString(), x => x);
			ByteList byteList = new ByteList();

			dictionary.Add("CLR", dictionary.Count);
			if (clearEnd)
			{
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
			byteList.Close();

			return byteList.ToArray();
		}
	}
}