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
	public delegate void ResizeEventHandler(object sender, ResizeEventArgs args);

	public class ResizeEventArgs : EventArgs
	{
		public int Width { get; private set; }
		public int Height { get; private set; }
		
		public Size Size => new Size(Width, Height);
		
		public ResizeEventArgs(int width, int height)
		{
			Width = width;
			Height = height;
		}
	}
}