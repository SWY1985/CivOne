// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.Enums
{
	public static class DirectionExtensions
	{
		public static bool And(this Direction direction, Direction compare)
		{
			return (direction & compare) == compare;
		}

		public static bool Not(this Direction direction, Direction compare)
		{
			return (direction & compare) == 0;
		}
	}
}