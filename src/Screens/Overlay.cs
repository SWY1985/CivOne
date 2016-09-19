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
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Overlay : BaseScreen
	{
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;
				return true;
			}
			return false;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			HandleClose();
			Destroy();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			HandleClose();
			Destroy();
			return true;
		}

		public static Overlay Empty
		{
			get
			{
				return new Overlay();
			}
		}

		private Overlay()
		{
			Cursor = MouseCursor.Pointer;
			
			_canvas = new Picture(320, 200, Common.TopScreen.Palette);
		}
	}
}