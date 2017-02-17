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
	/// <remarks>
	/// This code is based on JCivED source code by darkpanda. <http://sourceforge.net/projects/jcived/>
	/// </remarks>
	internal class RLECodec
	{
		private const int RLE_REPEAT = 0x90;

		public static byte[] Decode(byte[] input)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				byte value = input[0];
				for (int i = 0; i < input.Length; i++)
				{
					if (input[i] != RLE_REPEAT || input[i + 1] == 0)
					{
						value = input[i];
						ms.WriteByte(value);
						if (input[i] == RLE_REPEAT && input[i + 1] == 0) i++;
						continue;
					}

					int repeat = input[i + 1];

					ms.Write(Enumerable.Repeat(value, repeat).ToArray(), 0, repeat - 1);
					i++;
				}
				return ms.ToArray();
			}
		}
	}
}