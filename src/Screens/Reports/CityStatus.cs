// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne.Screens.Reports
{
	internal class CityStatus : BaseReport
	{
		private const char FOOD = '{';
		private const char SHIELD = '|';
		private const char TRADE = '}';
		private const byte FONT_ID = 0;

		private readonly City[] _cities;

		private bool _update = true;
		private int _page = 0;

		protected override bool HasUpdate(uint gameTick)
		{
			if (!_update) return false;

			this.FillRectangle(8, 0, 32, 320, 168);

			int fontHeight = Resources.Instance.GetFontHeight(FONT_ID);
			int yy = 32;
			for (int i = (_page++ * 20); i < _cities.Length && i < (_page * 20); i++)
			{
				City city = _cities[i];

				string production = (city.CurrentProduction as ICivilopedia).Name;
				int productionWidth = Resources.Instance.GetTextSize(1, production).Width;

				this.DrawText(city.Name, FONT_ID, 15, 8, yy)
					.DrawText($"{city.Size}-{city.FoodTotal}{FOOD} {city.ShieldTotal}{SHIELD} {city.TradeTotal}{TRADE}", FONT_ID, 15, 80, yy)
					.DrawText(production, FONT_ID, 15, 172, yy)
					.DrawText($"({city.Shields}/{city.CurrentProduction.Price * 10})", FONT_ID, 7, 172 + productionWidth + 7, yy);
				yy += fontHeight;
			}

			_update = false;
			return true;
		}

		private bool NextPage()
		{
			if ((_page * 20) < _cities.Length)
			{
				_update = true;
			}
			else
			{
				Destroy();
			}
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			return NextPage();
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			return NextPage();
		}

		public CityStatus() : base("CITY STATUS", 8)
		{
			_cities = Game.GetCities().Where(c => Human == c.Owner && c.Size > 0).ToArray();
		}
	}
}