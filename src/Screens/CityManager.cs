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
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Screens.CityManagerPanels;

namespace CivOne.Screens
{
	internal class CityManager : BaseScreen
	{
		private readonly City _city;
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
		private bool _mouseDown = false;

		private List<IScreen> _subScreens = new List<IScreen>();

		private void CloseScreen()
		{
			_cityHeader.Close();
			Destroy();
		}
		
		private void DrawLayer(IScreen layer, uint gameTick, int x, int y)
		{
			if (layer == null) return;
			if (!layer.Update(gameTick) && !_redraw) return;
			this.AddLayer(layer, x, y);
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (Common.Screens.Count(x => x is CityManager) > 1)
			{
				Destroy();
				return false;
			}

			if (_cityHeader.Update(gameTick)) _update = true;
			if (_cityResources.Update(gameTick)) _update = true;
			if (_cityUnits.Update(gameTick)) _update = true;
			if (_cityMap.Update(gameTick)) _update = true;
			if (_cityBuildings.Update(gameTick)) _update = true;
			if (_cityFoodStorage.Update(gameTick)) _update = true;
			if (_cityInfo.Update(gameTick)) _update = true;
			if (_cityProduction.Update(gameTick)) _update = true;
			
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

				DrawButton("Rename", 9, 1, 231, 190, 42);
				DrawButton("Exit", 12, 4, 284, 190, 33);

				_update = false;
				return true;
			}
			return false;
		}

		private void CityRename(object sender, EventArgs args)
		{
			if (!(sender is CityName)) return;

			_city.Name = (sender as CityName).Value;
			_cityHeader.Update();
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
			_mouseDown = true;
			
			if (new Rectangle(231, 190, 42, 10).Contains(args.Location))
			{
				// Rename button
				CityName name = new CityName(_city.Name);
				name.Accept += CityRename;
				Common.AddScreen(name);
				return true;
			}
			if (new Rectangle(2, 1, 208, 21).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 2, 1);
				return _cityHeader.MouseDown(args);
			}
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
			if (new Rectangle(211, 1, 107, 97).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 211, 1);
				if (_cityBuildings.MouseDown(args))
					return true;
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
			if (!_mouseDown) return true;

			if (new Rectangle(230, 99, 88, 99).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 230, 99);
				return _cityProduction.MouseUp(args);
			}
			return false;
		}

		private void BuildingUpdate(object sender, EventArgs args)
		{
			_cityFoodStorage.Update();
			_cityHeader.Update();
			_cityMap.Update();
			_cityProduction.Update();
		}

		private void HeaderUpdate(object sender, EventArgs args)
		{
			_cityResources.Update();
		}

		private void MapUpdate(object sender, EventArgs args)
		{
			_cityHeader.Update();
			_cityResources.Update();
		}

		public CityManager(City city) : base(MouseCursor.Pointer)
		{
			_city = city;

			_city.UpdateResources();
			
			_canvas = new Picture(320, 200, Common.DefaultPalette);
			this.FillRectangle(5, 0, 0, 320, 200);
			
			_subScreens.Add(_cityHeader = new CityHeader(_city, Patterns.PanelBlue));
			_subScreens.Add(_cityResources = new CityResources(_city, Patterns.PanelBlue));
			_subScreens.Add(_cityUnits = new CityUnits(_city, Patterns.PanelBlue));
			_subScreens.Add(_cityMap = new CityMap(_city, Patterns.PanelBlue));
			_subScreens.Add(_cityBuildings = new CityBuildings(_city, Patterns.PanelBlue));
			_subScreens.Add(_cityFoodStorage = new CityFoodStorage(_city, Patterns.PanelBlue));
			_subScreens.Add(_cityInfo = new CityInfo(_city, Patterns.PanelBlue));
			_subScreens.Add(_cityProduction = new CityProduction(_city, Patterns.PanelBlue));

			_cityBuildings.BuildingUpdate += BuildingUpdate;
			_cityHeader.HeaderUpdate += HeaderUpdate;
			_cityMap.MapUpdate += MapUpdate;
		}

		public override void Dispose()
		{
			_subScreens.ForEach(x => x.Dispose());
			_subScreens.Clear();
			base.Dispose();
		}
	}
}