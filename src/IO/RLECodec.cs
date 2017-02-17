// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

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
			if (input == null)
			{
				return null;
			}

			byte[] output = new byte[Arrays.DEFAULT_LENGTH];
			output[0] = input[0];
			int length = 1;

			for (int i = 1; i < input.Length; i++)
			{
				if (input[i] != RLE_REPEAT || input[i + 1] == 0)
				{
					if (output.Length - length < 1)
					{
						Arrays.Expand(ref output);
					}
					output[length++] = input[i];
					if (input[i] == RLE_REPEAT && input[i + 1] == 0) i++;
					continue;
				}

				int repeat = input[i + 1];
				int start = length;
				byte value = output[length - 1];

				if ((output.Length - length) < repeat - 1)
				{
					Arrays.Expand(ref output);
				}
				length += (repeat - 1);

				for (int j = start; j < length; j++)
					output[j] = value;
				i++;
			}

			return output;
		}
	}
}