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
	internal class CityHeader : BaseScreen
	{
		private readonly City _city;

		private readonly Bitmap _background;
		
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				string population = $"{_city.Population:n0}".Replace(".", ",");

				_canvas.FillLayerTile(_background);
				_canvas.AddBorder(1, 1, 0, 0, 207, 21);
				_canvas.FillRectangle(0, 207, 0, 1, 21);
				_canvas.DrawText($"{_city.Name} (Pop: {population})", 1, 17, 104, 1, TextAlign.Center);
				
				_update = false;
			}
			return true;
		}

		public void Close()
		{
			Destroy();
		}

		public CityHeader(City city, Bitmap background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(208, 21, background.Palette.Entries);
		}
	}
}