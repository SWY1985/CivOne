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
	internal class CityResources : BaseScreen
	{
		private readonly City _city;

		private readonly Bitmap _background;
		
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas.FillLayerTile(_background);
				_canvas.AddBorder(1, 1, 0, 0, 123, 43);
				_canvas.FillRectangle(1, 1, 1, 121, 8);
				_canvas.FillRectangle(0, 123, 0, 1, 43);
				_canvas.DrawText($"City Resources", 1, 17, 6, 2, TextAlign.Left);
				
				_update = false;
			}
			return true;
		}

		public void Close()
		{
			Destroy();
		}

		public CityResources(City city, Bitmap background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(124, 43, background.Palette.Entries);
		}
	}
}