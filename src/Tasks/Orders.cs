// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Advances;
using CivOne.Interfaces;
using CivOne.IO;
using CivOne.Screens;
using CivOne.Units;

namespace CivOne.Tasks
{
	internal class Orders : GameTask
	{
		private enum Order
		{
			None,
			NewCity,
			Sentry,
			Fortify,
			Road,
			Irrigate,
			Mines,
			Fortress,
			Wait,
			Skip,
			Unload,
			Disband
		}

		private City _city;
		private Player _player;
		private IUnit _unit = null;
		private int _x, _y;
		private Order _order;

		private void Error(string error)
		{
			GameTask.Enqueue(Message.Error("-- Civilization Note --", TextFile.Instance.GetGameText($"ERROR/{error}")));
		}
		
		private void CityManagerClosed(object sender, EventArgs args)
		{
			if (_unit != null)
			{
				Game.DisbandUnit(_unit);
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
			_city = Game.AddCity(_player, name, _x, _y); 
			if (_city != null)
			{
				if (_player.IsHuman)
				{
					CityView cityView = new CityView(_city, founded: true);
					cityView.Closed += CityFounded;
					cityView.Skipped += CityViewed;
					Common.AddScreen(cityView);
					return;
				}
				if (_unit != null)
				{
					Game.DisbandUnit(_unit);
				}
			}
			EndTask();
		}

		private void CreateCity(Player player, int x, int y)
		{
			string name = Game.CityName(player);
			if (player.IsHuman)
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
			if (_unit != null && !(_unit is Settlers))
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

			if (Map[_x, _y].IsOcean)
			{
				EndTask();
				return;
			}

			if (Map[_x, _y].City != null)
			{
				// There is already a city here
				if (_unit is Settlers)
				{
					if (Map[_x, _y].City.Size >= 10)
					{
						// City is 10 or larger, can not join city
						Error("ADDCITY");
						EndTask();
						return;
					}
					Map[_x, _y].City.Size++;
					Game.DisbandUnit(_unit);
				}
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

		private void Mines()
		{
			if (!(_unit is Settlers))
			{
				Error("SETTLERS");
				EndTask();
				return;
			}
			(_unit as Settlers).BuildMines();
			EndTask();
		}

		private void Fortress()
		{
			if (!(_unit is Settlers))
			{
				Error("SETTLERS");
				EndTask();
				return;
			}
			if (Game.GetPlayer(_unit.Owner).HasAdvance<Construction>())
			{
				(_unit as Settlers).BuildFortress();
			}
			EndTask();
		}

		private void Road()
		{
			if (!(_unit is Settlers))
			{
				Error("SETTLERS");
				EndTask();
				return;
			}
			(_unit as Settlers).BuildRoad();
			EndTask();
		}

		private void UnitWait()
		{
			Game.UnitWait();
			EndTask();
		}

		public override void Run()
		{
			switch (_order)
			{
				case Order.NewCity:
					CreateCity();
					break;
				case Order.Irrigate:
					Irrigate();
					break;
				case Order.Mines:
					Mines();
					break;
				case Order.Fortress:
					Fortress();
					break;
				case Order.Road:
					Road();
					break;
				case Order.Wait:
					UnitWait();
					break;
				default:
					EndTask();
					break;
			}
		}

		public static Orders FoundCity(IUnit unit = null)
		{
			return new Orders()
			{
				_unit = unit,
				_order = Order.NewCity
			};
		}

		public static Orders NewCity(Player player, int x, int y)
		{
			return new Orders()
			{
				_player = player,
				_order = Order.NewCity,
				_x = x,
				_y = y
			};
		}

		public static Orders BuildIrrigation(IUnit unit)
		{
			return new Orders()
			{
				_unit = unit,
				_order = Order.Irrigate
			};
		}

		public static Orders BuildMines(IUnit unit)
		{
			return new Orders()
			{
				_unit = unit,
				_order = Order.Mines
			};
		}

		public static Orders BuildFortress(IUnit unit)
		{
			return new Orders()
			{
				_unit = unit,
				_order = Order.Fortress
			};
		}

		public static Orders BuildRoad(IUnit unit)
		{
			return new Orders()
			{
				_unit = unit,
				_order = Order.Road
			};
		}

		public static Orders Wait(IUnit unit)
		{
			return new Orders()
			{
				_unit = unit,
				_order = Order.Wait
			};
		}

		private Orders()
		{
			
		}
	}
}