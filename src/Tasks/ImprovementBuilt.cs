// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne.Tasks
{
	internal class ImprovementBuilt : GameTask
	{
		private readonly City _city;
		private readonly IProduction _improvement;

		private void ClosedCityView(object sender, EventArgs args)
		{
			if (Common.HasScreenType<CityManager>()) return;
			
			CityManager cityManager = new CityManager(_city);
			cityManager.Closed += (s, a) => EndTask();
			Common.AddScreen(cityManager);
		}

		public override void Run()
		{
			if (Human != _city.Owner)
			{
				Log($"{_city.Name} builds {(_improvement as ICivilopedia).Name}.");
				EndTask();
				return;
			}

			IScreen cityView;
			if (!Settings.Animations)
			{
				cityView = new Newspaper(_city, new string[] { $"{_city.Name} builds", $"{(_improvement as ICivilopedia).Name}." }, showGovernment: false);
			}
			else if (_improvement is IBuilding)
			{
				cityView = new CityView(_city, production: (_improvement as IBuilding));
			}
			else if (_improvement is IWonder)
			{
				cityView = new CityView(_city, production: (_improvement as IWonder));
			}
			else
			{
				EndTask();
				return;
			}
			cityView.Closed += ClosedCityView;
			Common.AddScreen(cityView);
		}

		public ImprovementBuilt(City city, IBuilding building)
		{
			_city = city;
			_improvement = building;
		}

		public ImprovementBuilt(City city, IWonder wonder)
		{
			_city = city;
			_improvement = wonder;
		}
	}
}