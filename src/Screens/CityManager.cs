// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityManager : BaseScreen
	{
		private readonly City _city;

		private readonly Bitmap _background;

		private readonly CityHeader _cityHeader;
		private readonly CityResources _cityResources;
		private readonly CityUnits _cityUnits;
		private readonly CityMap _cityMap;
		private readonly CityBuildings _cityBuildings;
		private readonly CityFoodStorage _cityFoodStorage;
		private readonly CityInfo _cityInfo;
		private readonly CityProduction _cityProduction;
		
		private bool _update = true;
		private bool _redraw = false;

		private List<IScreen> _subScreens = new List<IScreen>();

		private void CloseScreen()
		{
			_cityHeader.Close();
			Destroy();
			HandleClose();
		}
		
		private void DrawLayer(IScreen layer, uint gameTick, int x, int y)
		{
			if (layer == null) return;
			if (!layer.HasUpdate(gameTick) && !_redraw) return;
			AddLayer(layer, x, y);
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_cityHeader.HasUpdate(gameTick)) _update = true;
			if (_cityResources.HasUpdate(gameTick)) _update = true;
			if (_cityUnits.HasUpdate(gameTick)) _update = true;
			if (_cityMap.HasUpdate(gameTick)) _update = true;
			if (_cityBuildings.HasUpdate(gameTick)) _update = true;
			if (_cityFoodStorage.HasUpdate(gameTick)) _update = true;
			if (_cityInfo.HasUpdate(gameTick)) _update = true;
			if (_cityProduction.HasUpdate(gameTick)) _update = true;
			
			if (_update)
			{
				DrawLayer(_cityHeader, gameTick, 2, 1);
				DrawLayer(_cityResources, gameTick, 2, 23);
				DrawLayer(_cityUnits, gameTick, 2, 67);
				DrawLayer(_cityMap, gameTick, 127, 23);
				DrawLayer(_cityBuildings, gameTick, 211, 1);
				DrawLayer(_cityFoodStorage, gameTick, 2, 106);
				DrawLayer(_cityInfo, gameTick, 95, 106);
				DrawLayer(_cityProduction, gameTick, 230, 99);

				// Draw exit button
				_canvas.FillRectangle(7, 284, 190, 33, 1);
				_canvas.FillRectangle(7, 284, 191, 1, 8);
				_canvas.FillRectangle(4, 285, 198, 32, 1);
				_canvas.FillRectangle(4, 316, 190, 1, 8);
				_canvas.FillRectangle(12, 285, 191, 31, 7);
				_canvas.DrawText("Exit", 1, 4, 302, 192, TextAlign.Center);

				_update = false;
				return true;
			}
			return false;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			foreach (IScreen screen in _subScreens)
			{
				if (!screen.KeyDown(args)) continue;
				return true;
			}
			CloseScreen();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (new Rectangle(127, 23, 82, 82).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 127, 23);
				return _cityMap.MouseDown(args);
			}
			if (new Rectangle(95, 106, 133, 92).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 95, 106);
				return _cityInfo.MouseDown(args);
			}
			if (new Rectangle(230, 99, 88, 99).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 230, 99);
				if (_cityProduction.MouseDown(args))
					return true;
			}
			CloseScreen();
			return true;
		}
		
		public override bool MouseUp(ScreenEventArgs args)
		{
			if (new Rectangle(230, 99, 88, 99).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 230, 99);
				return _cityProduction.MouseUp(args);
			}
			return false;
		}

		private void MapUpdate(object sender, EventArgs args)
		{
			_cityHeader.Update();
			_cityResources.Update();
		}

		public CityManager(City city)
		{
			_city = city;
			_background = (Bitmap)Resources.Instance.GetPart("SP299", 288, 120, 32, 16).Clone();
			Picture.ReplaceColours(_background, new byte[] { 7, 22 }, new byte[] { 57, 9 });

			Cursor = MouseCursor.Pointer;
			
			Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			
			_canvas = new Picture(320, 200, palette);
			_canvas.FillRectangle(5, 0, 0, 320, 200);
			
			_subScreens.Add(_cityHeader = new CityHeader(_city, _background));
			_subScreens.Add(_cityResources = new CityResources(_city, _background));
			_subScreens.Add(_cityUnits = new CityUnits(_city, _background));
			_subScreens.Add(_cityMap = new CityMap(_city, _background));
			_subScreens.Add(_cityBuildings = new CityBuildings(_city, _background));
			_subScreens.Add(_cityFoodStorage = new CityFoodStorage(_city, _background));
			_subScreens.Add(_cityInfo = new CityInfo(_city, _background));
			_subScreens.Add(_cityProduction = new CityProduction(_city, _background));

			_cityMap.MapUpdate += MapUpdate;
		}
	}
}