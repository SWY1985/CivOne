// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Interfaces;
using CivOne.Wonders;

namespace CivOne.Screens.Reports
{
	[Modal]
	internal class TopCities : BaseScreen
	{
		private bool _update = true;

		private readonly City[] _cities;
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (!_update) return false;

			for (int i = 0; i < _cities.Length; i++)
			{
				City city = _cities[i];

				if (city == null || city.Size == 0) continue;
				byte colour = Common.ColourLight[city.Owner];

				int xx = 8;
				int yy = 32 + (32 * i);
				int ww = 304;
				int hh = 26;

				Player owner = Game.GetPlayer(city.Owner);

				_canvas.FillRectangle(colour, xx, yy, ww, hh);
				_canvas.FillRectangle(3, xx + 1, yy + 1, ww - 2, hh - 2);
				
				int dx = 42;
				int group = -1;
				Citizen[] citizens = city.Citizens.ToArray();
				for (int j = 0; j < city.Size; j++)
				{
					dx += 8;
					if (group != (group = Common.CitizenGroup(citizens[j])) && group > 0 && i > 0)
					{
						dx += 2;
						if (group == 3) dx += 4;
					}
					AddLayer(Icons.Citizen(citizens[j]), dx, yy + 10);
				}

				dx += 16;
				foreach (IWonder wonder in city.Wonders)
				{
					AddLayer(wonder.SmallIcon, dx, yy + 11);
					dx += 19;
				}

				_canvas.DrawText($"{i + 1}. {city.Name} ({owner.Civilization.Name})", 0, 15, 160, yy + 3, TextAlign.Center);
			}

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
		
		public TopCities()
		{
			_canvas = new Picture(320, 200, Common.DefaultPalette);

			// I'm not sure about the order of top 5 cities, but this is pretty close
			_cities = Game.GetCities()
							.Where(c => c.Size > 0)
							.OrderByDescending(c => c.Wonders.Length)
							.ThenByDescending(c => c.Size)
							.ThenByDescending(c => c.Citizens.Count(x => x == Citizen.HappyMale || x == Citizen.HappyFemale))
							.ThenByDescending(c => c.Citizens.Count(x => x == Citizen.ContentMale || x == Citizen.ContentFemale))
							.ThenBy(c => c.Citizens.Count(x => x == Citizen.UnhappyMale || x == Citizen.UnhappyFemale))
							.Take(5)
							.ToArray();
			
			_canvas.FillRectangle(3, 0, 0, 320, 200);
			_canvas.DrawText("The Top Five Cities in the World", 0, 5, 80, 13);
			_canvas.DrawText("The Top Five Cities in the World", 0, 15, 80, 12);
		}
	}
}