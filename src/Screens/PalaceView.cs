// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Events;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class PalaceView : BaseScreen
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
		
		public PalaceView()
		{
			Picture background = Resources.Instance.LoadPIC("CBACK");
			
			_canvas = new Picture(320, 200, background.Image.Palette.Entries);
			
			AddLayer(background);
			AddLayer(Resources.Instance.GetPart("CASTLE0", 0, 1, 52, 99), 135, 37);
			AddLayer(Resources.Instance.GetPart("CASTLE0", 53, 1, 26, 99), 185, 37);
			AddLayer(Resources.Instance.GetPart("CASTLE0", 78, 1, 24, 99), 114, 37);
		}
	}
}