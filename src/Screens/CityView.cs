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
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityView : BaseScreen
	{
		private readonly City _city;

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
		
		public CityView(City city)
		{
			_city = city;

			Picture background = Resources.Instance.LoadPIC("HILL");
			
			_canvas = new Picture(320, 200, background.Image.Palette.Entries);
			
			AddLayer(background);
			
			_canvas.DrawText(_city.Name, 5, 5, 161, 3, TextAlign.Center);
			_canvas.DrawText(_city.Name, 5, 15, 160, 2, TextAlign.Center);
			_canvas.DrawText(Game.Instance.GameYear, 5, 5, 161, 16, TextAlign.Center);
			_canvas.DrawText(Game.Instance.GameYear, 5, 15, 160, 15, TextAlign.Center);
		}
	}
}