// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;

namespace CivOne.Templates
{
	internal abstract class BaseStatusScreen : BaseScreen
	{
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update) return false;
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public BaseStatusScreen(string title, byte backgroundColour)
		{
			_canvas = new Picture(320, 200, Resources.Instance.LoadPIC("SP299").Image.Palette.Entries);
			
			_canvas.FillRectangle(backgroundColour, 0, 0, 320, 200);
			_canvas.DrawText(title, 0, 15, 160, 2, TextAlign.Center);
			_canvas.DrawText(string.Format("{0} of the {1}", "Empire", Game.Instance.HumanPlayer.TribeNamePlural), 0, 15, 160, 10, TextAlign.Center);
			_canvas.DrawText(string.Format("{0} {1}: {2}", "Emperor", Game.Instance.HumanPlayer.LeaderName, Game.Instance.GameYear), 0, 15, 160, 18, TextAlign.Center);
		}
	}
}