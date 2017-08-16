// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Graphics;

namespace CivOne.Screens.CityManagerPanels
{
	internal class CityFoodStorage : BaseScreen
	{
		private readonly City _city;

		private readonly Picture _background;
		
		private bool _update = true;
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				this.Tile(_background)
					.DrawRectangle(colour: 1)
					.FillRectangle(1, 1, 1, 89, 8)
					.DrawText($"Food Storage", 1, 17, 6, 2, TextAlign.Left);

				int foodPerLine = (_city.Size + 1);
				int foodWidth = 8;
				int foodHeight = 8;
				// if (_city.Size > 10) foodWidth /= 4;
				for (int i = 0; i < 7; i++)
				{
					if ((_city.Size * foodWidth) <= 80) break;
					foodWidth--;
				}
				int width = 8 + (_city.Size * foodWidth);
				if (width < 88)
					this.FillRectangle(1, 2 + width, 9, 88 - width, 82);
				
				if (_city.Buildings.Any(b => (b is Granary)))
				{
					this.FillRectangle(1, 3, 49, width - 3, 1);
				}

				for (int i = 0; i < _city.Food; i++)
				{
					int x = 1 + (foodWidth * (i % foodPerLine));
					int y = 9 + (((i - (i % foodPerLine)) / foodPerLine) * foodHeight);
					this.AddLayer(Icons.Food, x, y);
				}
				
				_update = false;
			}
			return true;
		}

		public void Update()
		{
			_update = true;
		}

		public CityFoodStorage(City city, Picture background) : base(91, 92)
		{
			_city = city;
			_background = background;

			Palette = background.Palette.Copy();
		}
	}
}