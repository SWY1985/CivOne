// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.GFX
{
	public struct Rectangle
	{
		public int X, Y, Width, Height;

		public int Left
		{
			get
			{
				return X;
			}
		}

		public int Top
		{
			get
			{
				return Y;
			}
		}

		public int Right
		{
			get
			{
				return X + Width;
			}
		}
		
		public int Bottom
		{
			get
			{
				return Y + Height;
			}
		}

		private bool Contains(int x, int y)
		{
			return Contains(new Point(x, y));
		}

		public bool Contains(Point point)
		{
			return (point.X >= X && point.X <= Right && point.Y >= Y && point.Y <= Bottom);
		}

		public bool IntersectsWith(Rectangle rectangle)
		{
			return (Contains(rectangle.Left, rectangle.Top) ||
					Contains(rectangle.Right, rectangle.Bottom) ||
					Contains(rectangle.Right, rectangle.Top) ||
					Contains(rectangle.Right, rectangle.Bottom));
		}
		
		public Rectangle(int x, int y, int width, int height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public Rectangle(Point point, Size size)
		{
			X = point.X;
			Y = point.Y;
			Width = size.Width;
			Height = size.Height;
		}
	}
}