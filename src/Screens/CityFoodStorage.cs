// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityFoodStorage : BaseScreen
	{
		private readonly City _city;

		private readonly Bitmap _background;
		
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update || (gameTick % 2 == 0))
			{
				_canvas.FillLayerTile(_background);
				_canvas.AddBorder(1, 1, 0, 0, 91, 92);
				_canvas.FillRectangle(1, 1, 1, 89, 8);
				_canvas.FillRectangle(0, 91, 0, 1, 92);
				_canvas.DrawText($"Food Storage", 1, 17, 6, 2, TextAlign.Left);
				
				_update = false;
				return true;
			}
			return false;
		}

		public void Close()
		{
			Destroy();
		}

		public CityFoodStorage(City city, Bitmap background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(92, 92, background.Palette.Entries);
		}
	}
}