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
using System.Reflection;
using CivOne.Advances;
using CivOne.Buildings;
using CivOne.Concepts;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.Wonders;

namespace CivOne.Screens
{
	internal class Civilopedia : BaseScreen
	{
		internal static ICivilopedia[] Advances = Reflect.GetCivilopediaAdvances().OrderBy(x => x.Name).ToArray();
		internal static ICivilopedia[] Improvements = Reflect.GetCivilopediaCityImprovements().OrderBy(x => x.Name).ToArray();
		internal static ICivilopedia[] Units = Reflect.GetCivilopediaUnits().OrderBy(x => x.Name).ToArray();
		internal static ICivilopedia[] TerrainType = Reflect.GetCivilopediaTerrainTypes().OrderBy(x => x.Name).ToArray();
		internal static ICivilopedia[] Misc = Reflect.GetConcepts().OrderBy(x => x.Name).ToArray();
		internal static ICivilopedia[] Complete = Reflect.GetCivilopediaAll().OrderBy(x => x.Name).ToArray();
		
		private readonly ICivilopedia[] _pages;
		private readonly ICivilopedia _singlePage;
		private readonly bool _discovered;
		private readonly bool _icon;
		
		private bool _update = true;
		private int _startIndex = 0;
		private byte _pageNumber = 1;

		private bool _closing = false;
		
		private void DrawPageTitle()
		{
			if (_singlePage == null) return;
			
			int titleX = 204, iconX = 8, iconY = 8;
			string category = "(unknown)";
			if (typeof(ITile).GetTypeInfo().IsAssignableFrom(_singlePage.GetType().GetTypeInfo())) { category = "Terrain Type"; iconX = 23; iconY = 4; }
			else if (typeof(IBuilding).GetTypeInfo().IsAssignableFrom(_singlePage.GetType().GetTypeInfo())) { category = "City Improvement"; iconX = 36; iconY = 16; }
			else if (typeof(IWonder).GetTypeInfo().IsAssignableFrom(_singlePage.GetType().GetTypeInfo())) { category = "Wonder of the World"; titleX = 160; }
			else if (typeof(IUnit).GetTypeInfo().IsAssignableFrom(_singlePage.GetType().GetTypeInfo())) { category = "Military Units"; titleX = 224; }
			else if (typeof(IAdvance).GetTypeInfo().IsAssignableFrom(_singlePage.GetType().GetTypeInfo())) { category = "Civilization Advance"; }
			else if (typeof(IConcept).GetTypeInfo().IsAssignableFrom(_singlePage.GetType().GetTypeInfo())) { category = "Game Concepts"; titleX = 160; }
			
			if (!_icon) titleX = 160;
			if (_singlePage.Icon != null && _icon)
			{
				this.AddLayer(_singlePage.Icon, iconX, iconY);
			}
			this.DrawText(_singlePage.Name.ToUpper(), 5, 5, titleX, 20, TextAlign.Center)
				.DrawText(category, 6, 7, titleX, 36, TextAlign.Center);
			if (_pageNumber == 2 && _discovered)
			{
				this.DrawText("(Discovered)", 6, 7, titleX, 48, TextAlign.Center);
			}
		}
		
		private void DrawPage(byte pageNumber)
		{
			if (_singlePage == null) return;
			
			DrawTerrainText();
			using (IBitmap page = _singlePage.DrawPage(pageNumber))
			{
				this.AddLayer(page);
			}
		}
		
		private bool NextPage()
		{
			if (_singlePage != null && _pageNumber < _singlePage.PageCount)
			{
				this.FillRectangle(8, 8, 304, 184, 15);
				DrawPage(++_pageNumber);
				DrawPageTitle();
				return true;
			}
			return false;
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_closing)
			{
				if((_singlePage != null && !_discovered) || !HandleScreenFadeOut())
				{
					Close();
				}
				return true;
			}

			if (!_update) return false;
			
			if (_singlePage == null)
			{
				Palette = Resources.WorldMapTiles.Palette.Copy();
				
				this.Clear(14)
					.FillRectangle(60, 2, 200, 9, 15)
					.FillRectangle(2, 14, 316, 184, 15);
				
				this.DrawText("ENCYCLOPEDIA of CIVILIZATION", 0, 5, 67, 4)
					.DrawText("ENCYCLOPEDIA of CIVILIZATION", 0, 10, 66, 3);
				
				if (_pages.Length > 78)
				{
					this.DrawText("MORE", 0, 12, 8, 4);
				}
				this.DrawText("EXIT", 0, 12, 286, 4);
				
				int xx = 10, yy = 16;
				int columns = (int)Math.Ceiling((float)_pages.Length / 26);
				for (int i = _startIndex; i < _pages.Length; i++)
				{
					string name = _pages[i].Name;
					if (columns >= 3 && name.Length >= 18) name = $"{name.Substring(0, 17)}."; 
					this.DrawText(name, 0, 5, xx, yy);
					
					yy += 7;
					if (yy <= 192) continue;
					
					xx += (columns < 3) ? 150 : 100;
					if (xx >= 300) break;
					yy = 16;
				}
			}
			
			_update = false;
			return true;
		}
		
		private void KeyDown(object sender, KeyboardEventArgs args)
		{
			if (_closing) return;

			args.Handled = true;
			_closing = !(_singlePage != null && NextPage());
		}
		
		private void MouseDown(object sender, ScreenEventArgs args)
		{
			if (_closing) return;
			if (_singlePage != null)
			{
				if (!NextPage()) 
					_closing = true;
				args.Handled = true;
				return;
			}
			
			if (args.Y < 16)
			{
				if (args.X < 160)
				{
					if (_pages.Length <= 78) return;
					_startIndex += 78;
					if (_startIndex >= _pages.Length) _startIndex = 0;
					_update = true;
				}
				else
				{
					_closing = true;
				}
				args.Handled = true;
				return;
			}
			
			if (args.X < 10 || args.X > 310) return;
			int xx = 10, yy = 16;
			int columns = (int)Math.Ceiling((float)_pages.Length / 26);
			int columnWidth = (columns < 3) ? 150 : 100;
			for (int i = _startIndex; i < _pages.Length; i++)
			{
				if (args.X > xx + columnWidth) { i += 25; xx += columnWidth; continue; }
				if (args.Y >= yy && args.Y <= yy + 7)
				{
					Log("Opening Civilopedia page: {0}", _pages[i].Name);
					Common.AddScreen(new Civilopedia(_pages[i]));
					args.Handled = true;
					return;
				}
				
				yy += 7;
				if (yy <= 192) continue;
				
				xx += (columns < 3) ? 150 : 100;
				if (xx >= 300) break;
				yy = 16;
			}
		}
		
		private void DrawTerrainTextValues(ref int y, string name, string food = null, string production = null, string trade = null, string foodIrrigation = null, string productionMining = null, string tradeRoads = null)
		{
			string foodFormat = "Food: {0} units.";
			string productionFormat = "Production: {0} units.";
			string tradeFormat = "Trade: {0}";
			
			this.DrawText(name, 6, 1, 12, y);
			y += 8;
			if (food != null)
			{
				if (foodIrrigation != null)
					food = string.Format("{0} ({1} with Irrigation)", food, foodIrrigation);
				this.DrawText(string.Format(foodFormat, food), 6, 9, 16, y);
				y += 8;
			}
			if (production != null)
			{
				if (productionMining != null)
					production = string.Format("{0} ({1} with Mining)", production, productionMining);
				this.DrawText(string.Format(productionFormat, production), 6, 9, 16, y);
				y += 8;
			}
			if (trade != null)
			{
				if (tradeRoads != null)
					trade = string.Format("{0} ({1} with Roads)", trade, tradeRoads);
				this.DrawText(string.Format(tradeFormat, trade), 6, 9, 16, y);
				y += 8;
			}			
			if (food == null && production == null && trade == null)
			{
				this.DrawText("nothing", 6, 9, 16, y);
				y += 8;
			}
			y += 4;
		}
		
		private void DrawTerrainText()
		{
			if (!typeof(ITile).GetTypeInfo().IsAssignableFrom(_singlePage.GetType().GetTypeInfo())) return;
			
			ITile tile = (ITile)_singlePage;
			int move = 1, defense = 0;
			
			int yy = 84;
			
			switch (tile.Type)
			{
				case Terrain.Arctic:
					DrawTerrainTextValues(ref yy, "Arctic");
					DrawTerrainTextValues(ref yy, "Seals", "2");
					move = 2;
					break;
				case Terrain.Desert:
					DrawTerrainTextValues(ref yy, "Desert", "0", "1", "0", "1", "2", "1%");
					DrawTerrainTextValues(ref yy, "Oasis", "3*", "1", "0", "4*", "2", "1%");
					break;
				case Terrain.Forest:
					DrawTerrainTextValues(ref yy, "Forest", "1", "2");
					DrawTerrainTextValues(ref yy, "Game", "3*", "2");
					move = 2;
					defense = 50;
					break;
				case Terrain.Grassland1:
				case Terrain.Grassland2:
					DrawTerrainTextValues(ref yy, "Grassland", "2", "0/1", "0", "3*", null, "1%");
					break;
				case Terrain.Hills:
					DrawTerrainTextValues(ref yy, "Hills", "1", "0", null, "2", "3*");
					DrawTerrainTextValues(ref yy, "Coal", "1", "2", null, "2", "5*");
					move = 2;
					defense = 100;
					break;
				case Terrain.Jungle:
					DrawTerrainTextValues(ref yy, "Jungle", "1");
					DrawTerrainTextValues(ref yy, "Gems", "1", null, "4%*");
					move = 2;
					defense = 50;
					break;
				case Terrain.Mountains:
					DrawTerrainTextValues(ref yy, "Mountains", null, "1", null, null, "2");
					DrawTerrainTextValues(ref yy, "Gold", null, "1", "6%*", null, "2");
					move = 3;
					defense = 200;
					break;
				case Terrain.Ocean:
					DrawTerrainTextValues(ref yy, "Ocean", "1", null, "2%");
					DrawTerrainTextValues(ref yy, "Fish", "3*", null, "2%");
					break;
				case Terrain.Plains:
					DrawTerrainTextValues(ref yy, "Plains", "1", "1", "0", "2", null, "1%");
					DrawTerrainTextValues(ref yy, "Horses", "1", "3", "0", "2", null, "1%");
					break;
				case Terrain.River:
					DrawTerrainTextValues(ref yy, "River", "2", "0/1", "1%", "3*");
					defense = 50;
					break;
				case Terrain.Swamp:
					DrawTerrainTextValues(ref yy, "Swamp", "1");
					DrawTerrainTextValues(ref yy, "Oil", "1", "4");
					move = 2;
					defense = 50;
					break;
				case Terrain.Tundra:
					DrawTerrainTextValues(ref yy, "Tundra", "1");
					DrawTerrainTextValues(ref yy, "Game", "3*");
					break;
			}
			
			this.DrawText("*  -1 if government is Despotism/Anarchy.", 6, 9, 16, yy); yy += 8;
			this.DrawText("%  +1 if government is Republic/Democracy.", 6, 9, 16, yy); yy += 12;
			
			this.DrawText($"Movement cost: {move} MP", 6, 12, 12, yy); yy += 8;
			this.DrawText($"Defense bonus: +{defense}%", 6, 12, 12, yy);
		}

		private void Close()
		{
			Destroy();
		}
		
		public Civilopedia(ICivilopedia[] pages) : base(MouseCursor.Pointer)
		{
			OnMouseDown += MouseDown;
			
			Palette = Common.DefaultPalette;
			_pages = pages;
		}
		
		public Civilopedia(ICivilopedia page, bool discovered = false, bool icon = true)
		{
			OnKeyDown += KeyDown;
			OnMouseDown += MouseDown;

			_discovered = discovered;
			_icon = icon;

			_update = false;
			_singlePage = page;
			Palette = Common.DefaultPalette;
			if (page.Icon != null) Palette.MergePalette(page.Icon.Palette, 16);
			
			this.Clear(15);
			DrawBorder(Common.Random.Next(2));

			if (_singlePage != null && !Settings.CivilopediaText) _pageNumber++;
			
			DrawPageTitle();
			DrawPage(_pageNumber);
		}
	}
}