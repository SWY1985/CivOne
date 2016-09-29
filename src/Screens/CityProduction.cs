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
					return _city.HasBuilding(_city.CurrentProduction as IBuilding);
				}
				if (_city.CurrentProduction is IWonder)
				{
					return Game.WonderBuilt(_city.CurrentProduction as IWonder);
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
				bool blink = ProductionInvalid && (gameTick % 4 > 1);
				if (!(Common.TopScreen is CityManager)) blink = true;
				DrawButton("Change", (byte)(blink ? 14 : 9), 1, 1, 7, 33);
				DrawButton("Buy", 9, 1, 64, 7, 18);

				DrawShields();

				if (_city.CurrentProduction is IUnit)
				{
					IUnit unit = (_city.CurrentProduction as IUnit);
					AddLayer(unit.GetUnit(_city.Owner), 33, 0);
				}
				else
				{
					string name = (_city.CurrentProduction as ICivilopedia).Name;
					while (Resources.Instance.GetTextSize(1, name).Width > 86)
					{
						name = $"{name.Substring(0, name.Length - 2)}.";
					}
					_canvas.DrawText(name, 1, 15, 44, 1, TextAlign.Center);
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