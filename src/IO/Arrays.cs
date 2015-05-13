// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;

namespace CivOne.IO
{
	internal class Arrays
	{
		internal const int DEFAULT_LENGTH = 2000;

		internal static void Expand(ref int[] array, int expandSize = DEFAULT_LENGTH)
		{
			int[] output = new int[array.Length + expandSize];
			Array.Copy(array, output, array.Length);
			array = output;
		}

		internal static void Truncate(ref int[] array, int truncateSize)
		{
			int[] output = new int[array.Length - truncateSize];
			Array.Copy(array, truncateSize, output, 0, array.Length - truncateSize);
			array = output;
		}
	}
}