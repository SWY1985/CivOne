// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityProduction : BaseScreen
	{
		private readonly City _city;

		private readonly Bitmap _background;
		
		private bool _update = true;
		
		private void DrawButton(string text, int x, int width, bool blink = false)
		{
			_canvas.FillRectangle(7, x + 0, 7, width, 1);
			_canvas.FillRectangle(7, x + 0, 8, 1, 8);
			_canvas.FillRectangle(1, x + 1, 15, width - 1, 1);
			_canvas.FillRectangle(1, x + width - 1, 7, 1, 8);
			_canvas.FillRectangle((byte)(blink ? 14 : 9), x + 1, 8, width - 2, 7);
			_canvas.DrawText(text, 1, 1, x + (int)Math.Ceiling((double)width / 2), 9, TextAlign.Center);
		}

		private void ForceUpdate(object sender, EventArgs args)
		{
			_update = true;
		}

		private void DrawShields()
		{
			double lineHeight = 1;
			int totalShields = (int)_city.CurrentProduction.Price * 10;
			int shieldWidth = 8;
			int shieldHeight = 8;
			int shieldsPerLine = 10;
			for (int i = 100; i <= 400; i *= 2)
			{
				if (totalShields <= i) break;
				shieldWidth /= 2;
				shieldsPerLine *= 2;
				lineHeight /= 2;
			}

			for (int i = 0; i < _city.Shields; i++)
			{
				int x = 1 + (shieldWidth * (i % shieldsPerLine));
				int y = 17 + (((i - (i % shieldsPerLine)) / shieldsPerLine) * shieldHeight);
				AddLayer(Icons.Shield, x, y);
			}
		}

		private bool ProductionInvalid
		{
			get
			{
				if (_city.CurrentProduction is IBuilding)
				{
					return _city.Buildings.Any(b => b.Id == (_city.CurrentProduction as IBuilding).Id); 
				}
				if (_city.CurrentProduction is IWonder)
				{
					return Game.Instance.BuiltWonders.Any(w => w.Id == (_city.CurrentProduction as IWonder).Id); 
				}
				return false;
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update || ProductionInvalid)
			{
				int totalShields = (int)_city.CurrentProduction.Price * 10;
				int width = 83;
				int shieldsPerLine = 10;
				if (totalShields > 100) { width += 4; shieldsPerLine *= 2; }
				if (totalShields > 200) { width += 1; shieldsPerLine *= 2; }
				if (totalShields > 400) { shieldsPerLine *= 2; }
				int height = 8 * ((totalShields - (totalShields % shieldsPerLine)) / shieldsPerLine);

				_canvas.FillLayerTile(_background);
				_canvas.AddBorder(1, 1, 0, 0, width, 19 + height);
				_canvas.FillRectangle(1, 1, 1, (width - 2), 16);
				if (width < 88)
				{
					_canvas.FillRectangle(5, width, 0, 88 - width, 99);
				}
				if (height < 80)
				{
					_canvas.FillRectangle(5, 0, 19 + height, width, 80 - height);
				}
				bool blink = false;
				if (ProductionInvalid)
					blink = (gameTick % 4 > 1);
				DrawButton("Change", 1, 33, blink);
				DrawButton("Buy", 64, 18);

				//AddLayer(Icons.Shield, 1, 17);
				DrawShields();

				if (_city.CurrentProduction is IUnit)
				{
					IUnit unit = (_city.CurrentProduction as IUnit);
					AddLayer(unit.GetUnit(_city.Owner), 33, 0);
				}
				else if (_city.CurrentProduction is IBuilding)
				{
					IBuilding building = (_city.CurrentProduction as IBuilding);
					_canvas.DrawText(building.Name, 1, 15, 44, 1, TextAlign.Center); 
				}
				else if (_city.CurrentProduction is IWonder)
				{
					IWonder wonder = (_city.CurrentProduction as IWonder);
					_canvas.DrawText(wonder.Name, 1, 15, 44, 1, TextAlign.Center); 
				}
				
				_update = false;
			}
			return true;
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
			_city.Buy();
			_update = true;
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

		public void Close()
		{
			Destroy();
		}

		public CityProduction(City city, Bitmap background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(88, 99, background.Palette.Entries);
		}
	}
}