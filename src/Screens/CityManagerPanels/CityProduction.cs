// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Screens.Dialogs;
using CivOne.Graphics.Sprites;
using CivOne.Units;
using CivOne.Wonders;

namespace CivOne.Screens.CityManagerPanels
{
	internal class CityProduction : BaseScreen
	{
		private const int SHIELD_HEIGHT = 8;

		private readonly City _city;
		private readonly bool _viewCity;
		
		private bool _update = true;

		private int _shieldPrice, _totalShields, _shieldsPerLine;
		private double _shieldWidth;

		private void ForceUpdate(object sender, EventArgs args)
		{
			_update = true;
		}

		private void AcceptBuy(object sender, EventArgs args)
		{
			_city.Buy();
			_update = true;
		}

		private void DrawShields()
		{
			for (int i = 0; i < _city.Shields; i++)
			{
				double x = 1 + (_shieldWidth * (i % _shieldsPerLine));
				int y = 17 + (((i - (i % _shieldsPerLine)) / _shieldsPerLine) * SHIELD_HEIGHT);
				this.AddLayer(Icons.Shield, (int)Math.Floor(x), y);
			}
		}

		private bool ProductionInvalid
		{
			get
			{
				if (_city.CurrentProduction is IBuilding)
				{
					return _city.HasBuilding(_city.CurrentProduction as IBuilding);
				}
				if (_city.CurrentProduction is IWonder)
				{
					return Game.WonderBuilt(_city.CurrentProduction as IWonder);
				}
				return false;
			}
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_update || ProductionInvalid)
			{
				_shieldsPerLine = 10;
				_shieldPrice = (int)_city.CurrentProduction.Price * 10;
				_totalShields = _shieldPrice;
				if (_city.Shields > _totalShields) _totalShields = _city.Shields;
				
				_shieldWidth = 8;
				if (_totalShields > 100)
				{
					_shieldsPerLine = (int)Math.Ceiling((double)_totalShields / 10);
					_shieldWidth = ((double)80 / _shieldsPerLine);
				}

				int width = (int)(_shieldWidth * (_shieldsPerLine - 1)) + 11;
				int height = SHIELD_HEIGHT * ((_shieldPrice - (_shieldPrice % _shieldsPerLine)) / _shieldsPerLine);
				if (height < SHIELD_HEIGHT) height = SHIELD_HEIGHT;

				this.Tile(Pattern.PanelBlue)
					.DrawRectangle(0, 0, width, 19 + height, 1)
					.FillRectangle(1, 1, (width - 2), 16, 1);
				if (width < 88)
				{
					this.FillRectangle(width, 0, 88 - width, 99, 5);
				}
				if (height < 80)
				{
					this.FillRectangle(0, 19 + height, width, 80 - height, 5);
				}
				bool blink = ProductionInvalid && (gameTick % 4 > 1);
				if (!(Common.TopScreen is CityManager)) blink = ProductionInvalid;
				if (!_viewCity)
				{
					DrawButton("Change", (byte)(blink ? 14 : 9), 1, 1, 7, 33);
					DrawButton("Buy", 9, 1, 64, 7, 18);
				}

				DrawShields();

				if (_city.CurrentProduction is IUnit)
				{
					IUnit unit = (_city.CurrentProduction as IUnit);
					this.AddLayer(unit.ToBitmap(_city.Owner), 33, 0);
				}
				else
				{
					string name = (_city.CurrentProduction as ICivilopedia).Name;
					while (Resources.GetTextSize(1, name).Width > 86)
					{
						name = $"{name.Substring(0, name.Length - 2)}.";
					}
					this.DrawText(name, 1, 15, 44, 1, TextAlign.Center);
				}
				
				_update = false;
			}
			return true;
		}

		public void Update()
		{
			_update = true;
		}

		private bool Change()
		{
			CityChooseProduction cityProductionMenu = new CityChooseProduction(_city);
			cityProductionMenu.Closed += ForceUpdate;
			Common.AddScreen(cityProductionMenu);
			return true;
		}

		private bool Buy()
		{
			string name = (_city.CurrentProduction as ICivilopedia).Name;
			short playerGold = Game.CurrentPlayer.Gold;
			short buyPrice = _city.BuyPrice;
			if (playerGold < buyPrice)
			{
				Common.AddScreen(new MessageBox("Cost to complete", $"{name}: ${buyPrice}", $"Treasury: ${playerGold}"));
				return true;
			}

			ConfirmBuy confirmBuy = new ConfirmBuy(name, buyPrice, playerGold);
			confirmBuy.Buy += AcceptBuy;
			Common.AddScreen(confirmBuy);
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			switch (args.KeyChar)
			{
				case 'C':
					return Change();
				case 'B':
					return Buy();
			}
			return false;
		}

		public override bool MouseDown(ScreenEventArgs args)
		{
			if (args.Y < 7 || args.Y > 15) return false;
			if (args.X < 34) return true;
			if (args.X > 63 && args.X < 82) return true;
			return false;
		}

		public override bool MouseUp(ScreenEventArgs args)
		{
			if (args.Y < 7 || args.Y > 15) return false;
			if (args.X < 34) return Change();
			if (args.X > 63 && args.X < 82) return Buy();
			return false;
		}

		public CityProduction(City city, bool viewCity) : base(88, 99)
		{
			_city = city;
			_viewCity = viewCity;
		}
	}
}