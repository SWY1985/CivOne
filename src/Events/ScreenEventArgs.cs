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
using CivOne.Enums;

namespace CivOne.Events
{
	public delegate void ScreenEventHandler(object sender, ScreenEventArgs args);

	public class ScreenEventArgs : EventArgs
	{
		public bool Handled { get; set; }
		public int X { get; private set; }
		public int Y { get; private set; }
		public MouseButton Buttons { get; private set; }
		
		public Point Location
		{
			get
			{
				return new Point(X, Y);
			}
		}
		
		public ScreenEventArgs(int x, int y)
		{
			X = x;
			Y = y;
			Buttons = MouseButton.None;
		}
		
		public ScreenEventArgs(int x, int y, MouseButton buttons)
		{
			X = x;
			Y = y;
			Buttons = buttons;
		}
	}
}