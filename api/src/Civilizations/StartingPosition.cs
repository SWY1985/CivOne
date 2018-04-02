// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Linq;

namespace CivOne.Civilizations
{
	public class StartingPosition : BaseAttribute
	{
		private static bool InRange(object value) => ((Point)value) != Point.Empty && ((Point)value).X >= 0 && ((Point)value).Y >= 0 && ((Point)value).X <= 79 && ((Point)value).Y <= 49;

		public Point Location => GetValue<Point>();

		/// <summary>
		/// Modify the civilization starting position on the EARTH map.
		/// </summary>
		/// <param name="x">The X coordinate on the EARTH map where the player will start. Must be a value in the range 0 to 79.</param>
		/// <param name="y">The Y coordinate on the EARTH map where the player will start. Must be a value in the range 0 to 49.</param>
		public StartingPosition(int x, int y) : base(typeof(Point), new Point(x, y), InRange)
		{
		}
	}
}