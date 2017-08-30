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
using CivOne.UserInterface;

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

		private void RenameClick(object sender, EventArgs args)
		{
			CityName name = new CityName(_city.Name);
			name.Accept += CityRename;
			Common.AddScreen(name);
		}

		private void ExitClick(object sender, EventArgs args) => CloseScreen();
		
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
		
		private void MouseDown(object sender, ScreenEventArgs args)
		{
			_mouseDown = true;
			
			if (new Rectangle(2, 1, 208, 21).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 2, 1);
				args.Handled = _cityHeader.MouseDown(args);
				return;
			}
			if (new Rectangle(127, 23, 82, 82).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 127, 23);
				args.Handled = _cityMap.MouseDown(args);
				return;
			}
			if (new Rectangle(95, 106, 133, 92).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 95, 106);
				args.Handled = _cityInfo.MouseDown(args);
				return;
			}
			if (new Rectangle(211, 1, 107, 97).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 211, 1);
				if (_cityBuildings.MouseDown(args))
				{
					args.Handled = true;
					return;
				}
			}
			if (new Rectangle(230, 99, 88, 99).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 230, 99);
				if (_cityProduction.MouseDown(args))
				{
					args.Handled = true;
					return;
				}
			}
			CloseScreen();
			args.Handled = true;
		}
		
		private void MouseUp(object sender, ScreenEventArgs args)
		{
			if (!_mouseDown)
			{
				args.Handled = true;
				return;
			}

			if (new Rectangle(230, 99, 88, 99).Contains(args.Location))
			{
				MouseArgsOffset(ref args, 230, 99);
				args.Handled = _cityProduction.MouseUp(args);
			}
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

			Elements.AddRange(new [] {
				Button.Blue("Rename", 231, 190, 42, click: RenameClick),
				Button.Red("Exit", 284, 190, 33, click: ExitClick)
			});
			
			Palette = Common.DefaultPalette;

			OnMouseDown += MouseDown;
			OnMouseUp += MouseUp;

			this.Clear(5);
			
			_subScreens.Add(_cityHeader = new CityHeader(_city));
			_subScreens.Add(_cityResources = new CityResources(_city));
			_subScreens.Add(_cityUnits = new CityUnits(_city));
			_subScreens.Add(_cityMap = new CityMap(_city));
			_subScreens.Add(_cityBuildings = new CityBuildings(_city));
			_subScreens.Add(_cityFoodStorage = new CityFoodStorage(_city));
			_subScreens.Add(_cityInfo = new CityInfo(_city));
			_subScreens.Add(_cityProduction = new CityProduction(_city));

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