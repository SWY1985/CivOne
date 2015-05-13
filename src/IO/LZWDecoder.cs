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
using System.IO;

namespace CivOne.IO
{
	/// <remarks>
	/// This code is based on JCivED source code by darkpanda. <http://sourceforge.net/projects/jcived/>
	/// </remarks>
	public class LZWDecoder
	{
		public static int[] Decode(int[] input)
		{
			int[] output = new int[0];

			while (input.Length > 0)
			{
				LZWDictionary dic = new LZWDictionary();
				int prev = input[0];
				int[] buffer = dic.GetEntry(prev);

				int[] data = new int[Arrays.DEFAULT_LENGTH];
				data[0] = prev;
				int length = 1;

				int lastChar = prev;

				int i = 0;
				while (!dic.IsFull && i < input.Length - 1)
				{
					int[] entry;
					if (input[++i] < dic.CursorPosition)
					{
						entry = dic.GetEntry(input[i]);
					}
					else
					{
						entry = dic.GetEntry(prev);
						Arrays.Expand(ref entry, 1);
						entry[entry.Length - 1] = lastChar;
					}

					// Append entry to the result data
					if (data.Length - length < entry.Length)
					{
						Arrays.Expand(ref data);
					}
					length += entry.Length;
					Array.Copy(entry, 0, data, length - entry.Length, entry.Length);

					lastChar = entry[0];

					int[] newEntry = new int[buffer.Length + 1];
					Array.Copy(buffer, newEntry, buffer.Length);
					newEntry[newEntry.Length - 1] = lastChar;
					dic.AddEntry(newEntry);

					prev = input[i];

					buffer = dic.GetEntry(prev);
				}

				// Append data to output and remove processed data from input array
				int appendStart = output.Length;
				Arrays.Expand(ref output, length);
				Array.Copy(data, 0, output, appendStart, length);
				Arrays.Truncate(ref input, (i + 1));
			}
			return output;
		}

		public static int[] ConvertByteStream(BinaryReader br, int remainingCodedBytes, int bits)
		{
			List<int> parsedIndexes = new List<int>();

			int usableBits = 0;
			int usableBitCount = 0;

			int indicatorLength = 1; // to increment with ++; rule is that 8+indicatorLength must be <= bits, otherwise reset
			int indicatorFlag = 0x001; // to increment with <<=1 followed by |= 1
			int nextThreshold = 0x0100; // to increment with <<=1, or *=2 - 256
			int decodedCounter = 0;
			int index = 0;

			while (remainingCodedBytes > 0)
			{
				/* get enough coded bits to work with */
				while (usableBitCount < 8 + indicatorLength)
				{
					usableBits |= (br.ReadByte() << usableBitCount);
					remainingCodedBytes--;
					usableBitCount += 8;
				}

				/* decode bytes and indicators */
				while (usableBitCount >= 8 + indicatorLength)
				{
					index = usableBits & (((indicatorFlag << 8) & 0xFF00) | 0x00FF);

					//int decodedByte = usableBits & 0xFF;
					usableBits >>= 8;
					usableBitCount -= 8;

					//int decodedIndicator = usableBits & indicatorFlag;
					usableBits >>= indicatorLength;
					usableBitCount -= indicatorLength;

					decodedCounter++;

					if (decodedCounter == nextThreshold)
					{
						decodedCounter = 0;
						indicatorLength += 1;
						indicatorFlag <<= 1;
						indicatorFlag |= 1;
						nextThreshold <<= 1;

						if (8 + indicatorLength > bits)
						{
							decodedCounter = 0;
							indicatorLength = 1;
							indicatorFlag = 0x001;
							nextThreshold = 256;
						}
					}
					parsedIndexes.Add(index);
				}
			}

			return parsedIndexes.ToArray();
		}
	}
}