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
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens.Reports
{
	internal class AttitudeSurvey : BaseReport
	{
		private const byte FONT_ID = 0;

		private readonly City[] _cities;

		private bool _update = true;
		private int _page = 0;

		private void DrawBuilding<T>(City city, ref int x, int y) where T : IBuilding
		{
			IBuilding building;
			if ((building = city.Buildings.FirstOrDefault(b => b is T)) == null) return;

			AddLayer(building.SmallIcon, x, y - 1);
			x += 18;
		}

		private void DrawCitizens(City city, int x, int y)
		{
			Citizen[] citizens = city.Citizens.ToArray();
			for (int j = 0; j < city.Size; j++)
			{
				x += 8;
				if (j > 0)
				{
					if (citizens[j] == Citizen.Entertainer && citizens[j - 1] != Citizen.Entertainer) x += 6;
				}
				AddLayer(Icons.Citizen(citizens[j]), x, y - 4);
			}
		}

		private void DrawBuildings(City city, int y)
		{
			int x = 212;
			_canvas.FillRectangle(11, x, y - 1, 90, 10);
			DrawBuilding<Temple>(city, ref x, y);
			DrawBuilding<MarketPlace>(city, ref x, y);
			DrawBuilding<Bank>(city, ref x, y);
			DrawBuilding<Cathedral>(city, ref x, y);
			DrawBuilding<Colosseum>(city, ref x, y);
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (!_update) return false;

			_canvas.FillRectangle(9, 0, 32, 320, 168);

			int y = 32;
			for (int i = (_page++ * 16); i < _cities.Length && i < (_page * 16); i++)
			{
				City city = _cities[i];

				_canvas.DrawText($"{city.Name}:", FONT_ID, 15, 16, y);
				
				DrawCitizens(city, (i % 2 == 0) ? 72 : 76, y);
				DrawBuildings(city, y);

				y += 10;
			}
			y += 8;
			if (y <= 190)
			{
				string population = $"{Human.Population:n0}".Replace(".", ",");
				if (Human.Population == 0) population = "00,000";
				int happy = 0;
				int content = 100;
				int unhappy = 0;
				_canvas.DrawText($"Population: {population} Happy:{happy}% Content:{content}% Unhappy:{unhappy}%", 0, 15, 16, y);
			}

			_update = false;
			return true;
		}

		private bool NextPage()
		{
			if ((_page * 16) < _cities.Length)
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

		public AttitudeSurvey() : base("ATTITUDE SURVEY", 9)
		{
			_cities = Game.GetCities().Where(c => Human == c.Owner).ToArray();
		}
	}
}