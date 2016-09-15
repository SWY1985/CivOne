// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Linq;
using CivOne.Interfaces;
using CivOne.IO;
using CivOne.Screens;
using CivOne.Units;

namespace CivOne.Tasks
{
	internal class Orders : GameTask
	{
		private City _city;
		private Player _player;
		private IUnit _unit = null;
		private int _x, _y;
		private bool _newCity = false;
		private bool _irrigate = false;

		private void Error(string error)
		{
			GameTask.Enqueue(Message.Error("-- Civilization Note --", TextFile.Instance.GetGameText($"ERROR/{error}")));
		}
		
		private void CityManagerClosed(object sender, EventArgs args)
		{
			if (_unit != null)
			{
				Game.Instance.DisbandUnit(_unit);
			}
			EndTask();
		}

		private void CityViewed(object sender, EventArgs args)
		{
			CityManager cityManager = new CityManager(_city);
			cityManager.Closed += CityManagerClosed;
			Common.AddScreen(cityManager);
		}

		private void CityFounded(object sender, EventArgs args)
		{
			CityView cityView = new CityView(_city, firstView: true);
			cityView.Closed += CityViewed;
			Common.AddScreen(cityView);
		}

		private void CityNameAccept(object sender, EventArgs args)
		{
			string name = (sender as CityName).Value;
			CreateCity(name);
			EndTask();
		}

		private void Cancel(object sender, EventArgs args)
		{
			EndTask();
		}

		private void CreateCity(string name)
		{
			_city = Game.Instance.AddCity(_player, name, _x, _y); 
			if (_city != null)
			{
				if (_player.Human)
				{
					CityView cityView = new CityView(_city, founded: true);
					cityView.Closed += CityFounded;
					cityView.Skipped += CityViewed;
					Common.AddScreen(cityView);
					return;
				}
				if (_unit != null)
				{
					Game.Instance.DisbandUnit(_unit);
				}
			}
			EndTask();
		}

		private void CreateCity(Player player, int x, int y)
		{
			string name = Game.Instance.CityName(player);
			if (player.Human)
			{
				CityName cityName = new CityName(name);
				cityName.Accept += CityNameAccept;
				cityName.Cancel += Cancel;
				Common.AddScreen(cityName);
				return;
			}
			
			CreateCity(name);
		}

		private void CreateCity()
		{
			if (!(_unit is Settlers))
			{
				Error("SETTLERS");
				EndTask();
				return;
			}

			Settlers settlers = (_unit as Settlers);
			if (settlers != null)
			{
				_player = settlers.Player;
				_x = settlers.X;
				_y = settlers.Y;
			}

			if (Map.Instance[_x, _y].City != null)
			{
				// There is already a city here, abort!
				EndTask();
				return;
			}

			CreateCity(_player, _x, _y);
		}

		private void Irrigate()
		{
			if (!(_unit is Settlers))
			{
				Error("SETTLERS");
				EndTask();
				return;
			}
			(_unit as Settlers).BuildIrrigation();
			EndTask();
		}

		public override void Run()
		{
			if (_newCity)
			{
				CreateCity();
			}
			else if (_irrigate)
			{
				Irrigate();
			}
		}

		public static Orders NewCity(IUnit unit = null)
		{
			return new Orders()
			{
				_unit = unit,
				_newCity = true
			};
		}

		public static Orders NewCity(Player player, int x, int y)
		{
			return new Orders()
			{
				_player = player,
				_newCity = true,
				_x = x,
				_y = y
			};
		}

		public static Orders Irrigate(IUnit unit)
		{
			return new Orders()
			{
				_unit = unit,
				_irrigate = true
			};
		}

		private Orders()
		{
			
		}
	}
}