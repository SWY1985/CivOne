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
using CivOne.Graphics;

namespace CivOne.Screens.CityManagerPanels
{
	internal class CityResources : BaseScreen
	{
		private readonly City _city;

		private readonly Picture _background;
		
		private bool _update = true;

		private void DrawFood()
		{
			int costs = _city.FoodCosts;
			int income = _city.FoodIncome;
			int width = 8;
			for (int i = 0; i < 7; i++)
			{
				if (((costs + Math.Abs(income)) * width) <= 116) break;
				width--;
			}

			for (int i = 0; (i < costs) && (i < costs + income); i++)
			{
				this.AddLayer(Icons.Food, 1 + (width * i), 9);
			}
			for (int i = 0; i < income; i++)
			{
				this.AddLayer(Icons.Food, 5 + (width * costs) + (width * i), 9);
			}
			if (income < 0)
			{
				for (int i = 0; i < -income; i++)
				{
					this.AddLayer(Icons.FoodLoss, 5 + (width * (costs + income)) + (width * i), 9);
				}
			}
		}

		private void DrawShields()
		{
			int costs = _city.ShieldCosts;
			int income = _city.ShieldIncome;
			int width = 8;
			for (int i = 0; i < 7; i++)
			{
				if (((costs + Math.Abs(income)) * width) <= 116) break;
				width--;
			}
			
			for (int i = 0; (i < costs) && (i < costs + income); i++)
			{
				this.AddLayer(Icons.Shield, 1 + (width * i), 17);
			}
			for (int i = 0; i < income; i++)
			{
				this.AddLayer(Icons.Shield, (costs > 0 ? 5 : 1) + (width * costs) + (width * i), 17);
			}
			if (income < 0)
			{
				for (int i = 0; i < -income; i++)
				{
					this.AddLayer(Icons.ShieldLoss, 5 + (8 * (costs + income)) + (width * i), 17);
				}
			}
		}

		private void DrawTrade()
		{
			int width = 8;
			for (int i = 0; i < 7; i++)
			{
				if ((_city.TradeTotal * width) <= 116) break;
				width--;
			}
			
			for (int i = 0; i < _city.TradeTotal; i++)
			{
				this.AddLayer(Icons.Trade, 1 + (width * i), 25);
			}
			
			width = 8;
			for (int i = 0; i < 7; i++)
			{
				if (((_city.Luxuries + _city.Taxes + _city.Science + 8) * width) <= 116) break;
				width--;
			}
			
			int xx = 1;
			for (int i = 0; i < _city.Luxuries; i++)
			{
				this.AddLayer(Icons.Luxuries, xx, 33);
				xx += width;
			}
			if (_city.Luxuries > 0) xx += 4;
			for (int i = 0; i < _city.Taxes; i++)
			{
				this.AddLayer(Icons.Taxes, xx, 33);
				xx += width;
			}
			if (_city.Taxes > 0) xx += 4;
			for (int i = 0; i < _city.Science; i++)
			{
				this.AddLayer(Icons.Science, xx, 33);
				xx += width;
			}
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				this.Tile(_background)
					.DrawRectangle(colour: 1)
					.FillRectangle(1, 1, 1, 121, 8)
					.DrawText($"City Resources", 1, 17, 6, 2, TextAlign.Left);

				DrawFood();
				DrawShields();
				DrawTrade();
				
				_update = false;
			}
			return true;
		}

		public void Update()
		{
			_update = true;
		}

		public CityResources(City city, Picture background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(123, 43, background.Palette);
		}
	}
}