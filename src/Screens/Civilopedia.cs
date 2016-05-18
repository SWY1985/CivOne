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
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;
using CivOne.Tiles;

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
		
		private bool _update = true;
		private int _startIndex = 0;
		private byte _pageNumber = 1;
		
		private void DrawPageTitle()
		{
			if (_singlePage == null) return;
			
			int titleX = 204, iconX = 8, iconY = 8;
			string category = "(unknown)";
			if (typeof(ITile).IsAssignableFrom(_singlePage.GetType())) { category = "Terrain Type"; iconX = 23; iconY = 4; }
			else if (typeof(IBuilding).IsAssignableFrom(_singlePage.GetType())) { category = "City Improvement"; iconX = 36; iconY = 16; }
			else if (typeof(IWonder).IsAssignableFrom(_singlePage.GetType())) { category = "Wonder of the World"; titleX = 160; }
			else if (typeof(IUnit).IsAssignableFrom(_singlePage.GetType())) { category = "Military Units"; titleX = 224; }
			else if (typeof(IAdvance).IsAssignableFrom(_singlePage.GetType())) { category = "Civilization Advance"; }
			else if (typeof(IConcept).IsAssignableFrom(_singlePage.GetType())) { category = "Game Concepts"; titleX = 160; }
			
			if (_singlePage.Icon != null)
				AddLayer(_singlePage.Icon, iconX, iconY);
			_canvas.DrawText(_singlePage.Name.ToUpper(), 5, 5, titleX, 20, TextAlign.Center);
			_canvas.DrawText(category, 6, 7, titleX, 36, TextAlign.Center);
		}
		
		private void DrawPage(byte pageNumber)
		{
			if (_singlePage == null) return;
			
			DrawTerrainText();
			AddLayer(_singlePage.DrawPage(pageNumber));
		}
		
		private bool NextPage()
		{
			if (_singlePage != null && _pageNumber < _singlePage.PageCount)
			{
				_canvas.FillRectangle(15, 8, 8, 304, 184);
				DrawPageTitle();
				DrawPage(++_pageNumber);
				return true;
			}
			return false;
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (!_update) return false;
			
			if (_singlePage == null)
			{
				_canvas = new Picture(320, 200, Resources.WorldMapTiles.Image.Palette.Entries);
				
				_canvas.FillRectangle(14, 0, 0, 320, 200);
				_canvas.FillRectangle(15, 60, 2, 200, 9);
				_canvas.FillRectangle(15, 2, 14, 316, 184);
				
				_canvas.DrawText("ENCYCLOPEDIA of CIVILIZATION", 0, 5, 67, 4);
				_canvas.DrawText("ENCYCLOPEDIA of CIVILIZATION", 0, 10, 66, 3);
				
				if (_pages.Length > 78)
				{
					_canvas.DrawText("MORE", 0, 12, 8, 4);
				}
				_canvas.DrawText("EXIT", 0, 12, 286, 4);
				
				int xx = 10, yy = 16;
				int columns = (int)Math.Ceiling((float)_pages.Length / 26);
				for (int i = _startIndex; i < _pages.Length; i++)
				{
					string name = _pages[i].Name;
					if (columns >= 3 && name.Length >= 18) name = string.Format("{0}.", name.Substring(0, 17)); 
					_canvas.DrawText(name, 0, 5, xx, yy);
					
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
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (_singlePage != null && NextPage())
			{
				return true;
			}
			Destroy();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (_singlePage != null)
			{
				if (!NextPage()) Destroy();
				return true;
			}
			
			if (args.Y < 16)
			{
				if (args.X < 160)
				{
					if (_pages.Length <= 78) return false;
					_startIndex += 78;
					if (_startIndex >= _pages.Length) _startIndex = 0;
					_update = true;
					return true;
				}
				else
				{
					Destroy();
					return true;
				}
			}
			
			if (args.X < 10 || args.X > 310) return false;
			int xx = 10, yy = 16;
			int columns = (int)Math.Ceiling((float)_pages.Length / 26);
			int columnWidth = (columns < 3) ? 150 : 100;
			for (int i = _startIndex; i < _pages.Length; i++)
			{
				if (args.X > xx + columnWidth) { i += 25; xx += columnWidth; continue; }
				if (args.Y >= yy && args.Y <= yy + 7)
				{
					Console.WriteLine("Opening Civilopedia page: {0}", _pages[i].Name);
					Common.AddScreen(new Civilopedia(_pages[i]));
					return true;
				}
				
				yy += 7;
				if (yy <= 192) continue;
				
				xx += (columns < 3) ? 150 : 100;
				if (xx >= 300) break;
				yy = 16;
			}
			return false;
		}
		
		private void DrawTerrainTextValues(ref int y, string name, string food = null, string production = null, string trade = null, string foodIrrigation = null, string productionMining = null, string tradeRoads = null)
		{
			string foodFormat = "Food: {0} units.";
			string productionFormat = "Production: {0} units.";
			string tradeFormat = "Trade: {0}";
			
			_canvas.DrawText(name, 6, 1, 12, y);
			y += 8;
			if (food != null)
			{
				if (foodIrrigation != null)
					food = string.Format("{0} ({1} with Irrigation)", food, foodIrrigation);
				_canvas.DrawText(string.Format(foodFormat, food), 6, 9, 16, y);
				y += 8;
			}
			if (production != null)
			{
				if (productionMining != null)
					production = string.Format("{0} ({1} with Mining)", production, productionMining);
				_canvas.DrawText(string.Format(productionFormat, production), 6, 9, 16, y);
				y += 8;
			}
			if (trade != null)
			{
				if (tradeRoads != null)
					trade = string.Format("{0} ({1} with Roads)", trade, tradeRoads);
				_canvas.DrawText(string.Format(tradeFormat, trade), 6, 9, 16, y);
				y += 8;
			}			
			if (food == null && production == null && trade == null)
			{
				_canvas.DrawText("nothing", 6, 9, 16, y);
				y += 8;
			}
			y += 4;
		}
		
		private void DrawTerrainText()
		{
			if (!typeof(ITile).IsAssignableFrom(_singlePage.GetType())) return;
			
			string foodFormat = "Food: {0} units.";
			string productionFormat = "Production: {0} units.";
			string tradeFormat = "Trade: {0}";
			
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
			
			_canvas.DrawText("*  -1 if government is Despotism/Anarchy.", 6, 9, 16, yy); yy += 8;
			_canvas.DrawText("%  +1 if government is Republic/Democracy.", 6, 9, 16, yy); yy += 12;
			
			_canvas.DrawText(string.Format("Movement cost: {0} MP", move), 6, 12, 12, yy); yy += 8;
			_canvas.DrawText(string.Format("Defense bonus: +{0}%", defense), 6, 12, 12, yy);
		}
		
		public Civilopedia(ICivilopedia[] pages)
		{
			Cursor = MouseCursor.Pointer;
			
			_pages = pages;
		}
		
		public Civilopedia(ICivilopedia page)
		{
			_update = false;
			_singlePage = page;
			Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			if (page.Icon != null) palette = Resources.PaletteCombine(palette, page.Icon.Image.Palette.Entries, 16);
			_canvas = new Picture(320, 200, palette);
			
			int border = Common.Random.Next(2);
			Bitmap[] borders = new Bitmap[8];
			int index = 0;
			for (int y = 0; y < 2; y++)
			{
				for (int x = 0; x < 4; x++)
				{
					borders[index] = (Bitmap)Resources.Instance.GetPart("SP299", ((border == 0) ? 192 : 224) + (8 * x), 120 + (8 * y), 8, 8).Clone();
					index++;
				}
			}
			
			_canvas.FillRectangle(15, 0, 0, 320, 200);
			for (int x = 8; x < 312; x += 8)
			{
				AddLayer(borders[4], x, 0);
				AddLayer(borders[6], x, 192);
			}
			for (int y = 8; y < 192; y += 8)
			{
				AddLayer(borders[7], 0, y);
				AddLayer(borders[5], 312, y);
			}
			AddLayer(borders[0], 0, 0);
			AddLayer(borders[1], 312, 0);
			AddLayer(borders[2], 0, 192);
			AddLayer(borders[3], 312, 192);
			
			DrawPageTitle();
			DrawPage(_pageNumber);
		}
	}
}