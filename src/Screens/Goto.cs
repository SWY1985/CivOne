// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne.Screens
{
	internal class Goto : BaseScreen
	{
		private readonly int _x, _y;
		
		private bool _update = true;

		public int X { get; private set; }
		public int Y { get; private set; }
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;
				return true;
			}
			return false;
		}
		
		private void KeyDown(object sender, KeyboardEventArgs args)
		{
			args.Handled = true;
			Destroy();
		}
		
		private void MouseDown(object sender, ScreenEventArgs args)
		{
			int offsetX = 80;
			if (Settings.RightSideBar) offsetX = 0;
			int offsetY = 8;
			
			int xx = (int)Math.Floor((double)(args.X - offsetX) / 16);
			int yy = (int)Math.Floor((double)(args.Y - offsetY) / 16);

			if (xx >= 0 && yy >= 0)
			{
				X = _x + xx;
				Y = _y + yy;
				while (X < 0) X += Map.WIDTH;
				while (X >= Map.WIDTH) X -= Map.WIDTH;
			}

			Destroy();
			args.Handled = true;
		}

		internal Goto(int x, int y) : base(MouseCursor.Goto)
		{
			_x = x;
			_y = y;
			X = -1;
			Y = -1;
			
			Palette = Common.TopScreen.Palette;

			OnKeyDown += KeyDown;
			OnMouseDown += MouseDown;
		}
	}
}