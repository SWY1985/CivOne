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
using CivOne.Events;
using CivOne.IO;

namespace CivOne.UserInterface
{
	public abstract class Element : BaseInstance, IDisposable
	{
		public int Left { get; protected set; }
		public int Top { get; protected set; }

		public Rectangle Bounds => (Bitmap == null ? Rectangle.Empty : new Rectangle(Left, Top, Width, Height));
		public Point Location => new Point(Left, Top);
		public Size Size => (Bitmap == null ? Size.Empty : new Size(Width, Height));
		public int Width => Bitmap.Width;
		public int Height => Bitmap.Height;

		public Bytemap Bitmap { get; protected set; }
		public void Dispose()
		{
			Bitmap?.Dispose();
		}
	}
}