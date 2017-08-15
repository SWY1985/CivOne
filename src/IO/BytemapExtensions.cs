// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.IO
{
	public static class BytemapExtensions
	{
		public static Bytemap Crop(this Bytemap bytemap, int left, int top, int width, int height)
		{
			if (bytemap == null) return null;
			return bytemap[left, top, width, height];
		}
	}
}