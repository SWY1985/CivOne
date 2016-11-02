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

namespace CivOne.IO
{
	/// <remarks>
	/// This code is based on JCivED source code by darkpanda. <http://sourceforge.net/projects/jcived/>
	/// </remarks>
	public class LZWEncoder
	{
		public static int[] Encode(int[] inputData)
		{
			int[] plainData = inputData.ToArray();
			int[] codedData = new int[0];

			while (plainData.Length > 0)
			{
				LZWDictionary dic = new LZWDictionary();
				int[] buffer = new int[1] { plainData[0] };

				int i = 0;
				while (i < (plainData.Length - 1))
				{
					i++;

					int[] testChunk = new int[buffer.Length + 1];
					Array.Copy(buffer, testChunk, buffer.Length);
					testChunk[testChunk.Length - 1] = plainData[i];
					if (dic.GetIndexOfEntry(testChunk) != -1)
					{
						buffer = testChunk;
						continue;
					}

					Arrays.Expand(ref codedData, 1);
					codedData[codedData.Length - 1] = dic.GetIndexOfEntry(buffer);
					if (dic.IsFull) break;
					buffer = new int[] { plainData[i] };
				}

				if (!dic.IsFull)
				{
					Arrays.Expand(ref codedData, 1);
					codedData[codedData.Length - 1] = dic.GetIndexOfEntry(buffer);
					i++;
				}

				plainData = plainData.Skip(i).ToArray();
			}
			return codedData;
		}
		
		public static byte[] ConvertByteStream(int[] lzwIndexes, int mode)
		{
			List<byte> output = new List<byte>();
			
			int usableBits = 0;
			int usableBitCount = 0;
			
			int indicatorLength = 1; // to increment with ++; rule is that 8+indicatorLength must be <= bits, otherwise reset
			int nextThreshold = 0x0100; // to increment with <<=1, or *=2
			int codedCounter = 0;
			int dicCounter = 0;

			int remainingIndexesToCode = lzwIndexes.Length;
			
			while ((remainingIndexesToCode > 0))
			{
				// get enough coded bits to work with
				while (usableBitCount < 8)
				{
					usableBits = (usableBits | (lzwIndexes[codedCounter] + usableBitCount));
					codedCounter++;

					remainingIndexesToCode--;
					usableBitCount += (8 + indicatorLength);

					dicCounter++;
					if (dicCounter != nextThreshold) continue;
					
					dicCounter = 0;
					indicatorLength++; // to increment with ++; rule is that 8+indicatorLength must be <= ubyte_mode, otherwise reset
					nextThreshold <<= 1;
					
					if ((8 + indicatorLength) <= mode) continue;

					dicCounter = 0;
					indicatorLength = 1;
					nextThreshold = 0x0100;
				}

				while (usableBitCount >=  8)
				{
					byte byteToWrite = (byte)(usableBits & 0xFF);
					output.Add(byteToWrite);
					usableBits >>= 8;
					usableBitCount -= 8;
				}
			}

			// Write remnant bits
			if (usableBitCount > 0)
			{
				byte byteToWrite = (byte)(usableBits & 0xFF);
				output.Add(byteToWrite);
			}

			return output.ToArray();
		}
	}
}