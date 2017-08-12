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
using System.Linq;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Units;
using CivOne.UserInterface;
using CivOne.Wonders;

namespace CivOne.Screens
{
	internal class CityChooseProduction : BaseScreen
	{
		private readonly City _city;

		private readonly IProduction[] _availableProduction;
		private readonly int _fontId = 0;
		private readonly List<IProduction[]> _pages = new List<IProduction[]>();

		private bool _update = true;
		private int _menuHeight;
		private int _page = 0;
		
		private void MenuCancel(object sender, EventArgs args)
		{
			CloseMenus();
			Destroy();
		}

		private void ProductionChoice(object sender, EventArgs args)
		{
			if (_pages.Count > 1 && ((sender as MenuItem<int>).Value == _pages[_page].Length))
			{
				CloseMenus();
				_page++;
				if (_page >= _pages.Count) _page = 0;
				_menuHeight = Resources.Instance.GetFontHeight(1) * (_pages[_page].Length + 1);
				_update = true;
				return;
			}
			_city.SetProduction(_pages[_page].ToArray()[(sender as MenuItem<int>).Value]);
			MenuCancel(sender, args);
		}

		private void ProductionContext(object sender, EventArgs args)
		{
			if (_pages.Count > 1 && ((sender as MenuItem<int>).Value == _pages[_page].Length))
			{
				ProductionChoice(sender, args);
				return;
			}
			ICivilopedia page = (_pages[_page][(sender as MenuItem<int>).Value] as ICivilopedia);
			Common.AddScreen(new Civilopedia(page, icon: false));
		}

		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas.FillRectangle(0, 0, 0, 320, 200);

				List<string> menuItems = new List<string>();
				string menuHeaderText = $"What shall we build in {_city.Name}?";
				int itemWidth = Resources.Instance.GetTextSize(_fontId, menuHeaderText).Width;
				foreach (IProduction production in _pages[_page])
				{
					string menuText = string.Empty;
					if (production is IUnit)
					{
						IUnit unit = (production as IUnit);
						int turns = ((int)unit.Price * 10) - _city.Shields;
						if (_city.ShieldIncome > 1)
							turns = (int)Math.Ceiling((double)turns / _city.ShieldIncome);
						if (turns < 1) turns = 1;
						menuText = $"{unit.Name} ({turns} turns, ADM:{unit.Attack}/{unit.Defense}/{unit.Move})";
						if (Resources.Instance.GetTextSize(_fontId, menuText).Width > itemWidth) itemWidth = Resources.Instance.GetTextSize(_fontId, menuText).Width;
					}
					if (production is IBuilding)
					{
						IBuilding building = (production as IBuilding);
						int turns = ((int)building.Price * 10) - _city.Shields;
						if (_city.ShieldIncome > 1)
							turns = (int)Math.Ceiling((double)turns / _city.ShieldIncome);
						if (turns < 1) turns = 1;
						menuText = $"{building.Name} ({turns} turns)";
						if (Resources.Instance.GetTextSize(_fontId, menuText).Width > itemWidth) itemWidth = Resources.Instance.GetTextSize(_fontId, menuText).Width;
					}
					if (production is IWonder)
					{
						IWonder wonder = (production as IWonder);
						int turns = ((int)wonder.Price * 10) - _city.Shields;
						if (_city.ShieldIncome > 1)
							turns = (int)Math.Ceiling((double)turns / _city.ShieldIncome);
						if (turns < 1) turns = 1;
						menuText = $"{wonder.Name} ({turns} turns)";
						if (Game.WonderObsolete(wonder)) menuText = $"*{menuText}";
						if (Resources.Instance.GetTextSize(_fontId, menuText).Width > itemWidth) itemWidth = Resources.Instance.GetTextSize(_fontId, menuText).Width;
					}
					menuItems.Add(menuText);
				}
				if (_pages.Count > 1)
				{
					menuItems.Add("More...");
				}
				itemWidth += 10;

				int width = itemWidth + 14;
				int height = _menuHeight + 10 + Resources.Instance.GetFontHeight(_fontId);
				Picture menuGfx = new Picture(width, height);
				menuGfx.Tile(Patterns.PanelGrey);
				menuGfx.AddBorder(15, 8, 0, 0, width, height);
				menuGfx.DrawText(menuHeaderText, _fontId, 15, 4, 4);
				menuGfx.DrawText($"(Help available)", 1, 10, width, height - Resources.Instance.GetFontHeight(1), TextAlign.Right);

				_canvas.FillRectangle(5, 80, 8, width + 2, height + 2);
				AddLayer(menuGfx, 81, 9);
				
				Picture background = menuGfx.GetPart(2, 3 + Resources.Instance.GetFontHeight(_fontId), itemWidth, Resources.Instance.GetFontHeight(_fontId) * menuItems.Count + 4);
				Picture.ReplaceColours(background, new byte[] { 7, 22 }, new byte[] { 11, 3 });

				Menu menu = new Menu(Palette, background)
				{
					X = 83,
					Y = 12 + Resources.Instance.GetFontHeight(_fontId),
					Width = itemWidth,
					ActiveColour = 11,
					TextColour = 5,
					FontId = _fontId
				};

				int i = 0;
				foreach (string item in menuItems)
				{
					menu.Items.Add(item, i++)
						.OnSelect(ProductionChoice)
						.OnContext(ProductionContext);
				}
				menu.Width += 10;
				menu.MissClick += MenuCancel;
				menu.Cancel += MenuCancel;

				AddMenu(menu);
				
				_update = false;
			}
			return true;
		}

		public CityChooseProduction(City city) : base(MouseCursor.Pointer)
		{
			_city = city;

			_canvas = new Picture(320, 200, Common.DefaultPalette);

			_availableProduction = _city.AvailableProduction.ToArray();
			_menuHeight = Resources.Instance.GetFontHeight(0) * _availableProduction.Length;
			if (_menuHeight > 170)
			{
				_fontId = 1;
				_menuHeight = Resources.Instance.GetFontHeight(1) * _availableProduction.Length;
				if (_menuHeight > 170)
				{
					_pages.Add(_availableProduction.Where(p => (p is IUnit)).Take(28).ToArray());
					if (_availableProduction.Count(p => (p is IBuilding || p is IWonder)) > 28)
					{
						_pages.Add(_availableProduction.Where(p => (p is IBuilding)).Take(28).ToArray());
						_pages.Add(_availableProduction.Where(p => (p is IWonder)).Take(28).ToArray());
					}
					else
					{
						_pages.Add(_availableProduction.Where(p => (p is IBuilding || p is IWonder)).Take(28).ToArray());
					}
					_menuHeight = Resources.Instance.GetFontHeight(1) * (_pages[0].Length + 1);
					return;
				}
			}
			_pages.Add(_availableProduction);
		}
	}
}