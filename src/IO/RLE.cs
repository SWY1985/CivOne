// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.IO;
using System.Linq;

namespace CivOne.IO
{
	internal class RLE
	{
		private const int RLE_REPEAT = 0x90;
		private const int RLE_ESCAPE = 0x00;

		public static byte[] Decode(byte[] input)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				byte value = input[0];
				for (int i = 0; i < input.Length; i++)
				{
					if (input[i] != RLE_REPEAT || input[i + 1] == RLE_ESCAPE)
					{
						value = input[i];
						ms.WriteByte(value);
						if (input[i] == RLE_REPEAT && input[i + 1] == RLE_ESCAPE) i++;
						continue;
					}

					int repeat = input[i + 1];
					ms.Write(Enumerable.Repeat(value, repeat).ToArray(), 0, repeat - 1);
					i++;
				}
				return ms.ToArray();
			}
		}

		public static byte[] Encode(byte[] input)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				for (int i = 0; i < input.Length; i++)
				{
					byte value = input[i];
					byte repeat = 1;
					for (int r = 1; r < 255 && (i + r) < input.Length; r++)
					{
						if (input[i + r] != value) break;
						repeat++;
					}

					ms.WriteByte(value);
					if (value == RLE_REPEAT) ms.WriteByte(RLE_ESCAPE);
					if (repeat == 1) continue;
					ms.WriteByte(RLE_REPEAT);
					ms.WriteByte(repeat);
					if (repeat == RLE_REPEAT) ms.WriteByte(RLE_ESCAPE);
					i += (repeat - 1);
				}

				return ms.ToArray();
			}
		}
	}
}