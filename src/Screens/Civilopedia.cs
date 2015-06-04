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
using System.Windows.Forms;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;
using CivOne.Tiles;

namespace CivOne.Screens
{
	internal class Civilopedia : BaseScreen
	{
		internal static ICivilopedia[] Advances = new ICivilopedia[0];
		internal static ICivilopedia[] Improvements = new ICivilopedia[] { new Aqueduct(), new Bank(), new Barracks(), new Cathedral(), new CityWalls(), new Colosseum(), new Courthouse(), new Factory(), new Granary(), new HydroPlant(), new Library(), new Marketplace(), new MassTransit(), new MfgPlant(), new NuclearPlant(), new Palace(), new PowerPlant(), new RecyclingCenter(), new SdiDefense(), new Temple(), new University() };
		internal static ICivilopedia[] Units = new ICivilopedia[0];
		internal static ICivilopedia[] TerrainType = new ICivilopedia[] { new Arctic(), new Desert(), new Forest(), new Grassland(), new Hills(), new Jungle(), new Mountains(), new Ocean(), new Plains(), new River(), new Swamp(), new Tundra() };
		internal static ICivilopedia[] Misc = new ICivilopedia[0]; 
		internal static ICivilopedia[] Complete
		{
			get
			{
				ICivilopedia[] output = new ICivilopedia[0];
				output = output.Concat(Improvements).ToArray();
				output = output.Concat(TerrainType).ToArray();
				return output.OrderBy(x => x.Name).ToArray();
			}
		}
		
		private readonly ICivilopedia[] _pages;
		private readonly ICivilopedia _singlePage;
		
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update) return false;
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public override bool MouseDown(MouseEventArgs args)
		{
			if (_singlePage != null)
			{
				Destroy();
				return true;
			}
			
			if (args.Y < 16)
			{
				if (args.X < 160)
				{
					// MORE
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
			for (int i = 0; i < _pages.Length; i++)
			{
				if (args.X > xx + columnWidth) { i += 26; xx += columnWidth; continue; }
				if (args.Y >= yy && args.Y <= yy + 7)
				{
					Console.WriteLine("Opening Civilopedia page: {0}", _pages[i].Name);
					Common.AddScreen(new Civilopedia(_pages[i]));
					return true;
				}
				
				yy += 7;
				if (yy <= 192) continue;
				
				xx += (columns < 3) ? 150 : 100;
				yy = 16;
			}
			return false;
		}
		
		public Civilopedia(ICivilopedia[] pages)
		{
			Cursor = MouseCursor.Pointer;
			
			_pages = pages;
			
			_canvas = new Picture(320, 200, Resources.WorldMapTiles.Image.Palette.Entries);
			
			_canvas.FillRectangle(14, 0, 0, 320, 200);
			_canvas.FillRectangle(15, 60, 2, 200, 9);
			_canvas.FillRectangle(15, 2, 14, 316, 184);
			
			_canvas.DrawText("ENCYCLOPEDIA of CIVILIZATION", 0, 5, 67, 4);
			_canvas.DrawText("ENCYCLOPEDIA of CIVILIZATION", 0, 10, 66, 3);
			
			_canvas.DrawText("EXIT", 0, 12, 286, 4);
			
			int xx = 10, yy = 16;
			int columns = (int)Math.Ceiling((float)_pages.Length / 26);
			for (int i = 0; i < _pages.Length; i++)
			{
				_canvas.DrawText(_pages[i].Name, 0, 5, xx, yy);
				
				yy += 7;
				if (yy <= 192) continue;
				
				xx += (columns < 3) ? 150 : 100;
				yy = 16;
			}
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
		
		public Civilopedia(ICivilopedia page)
		{
			_singlePage = page;
			Color[] palette = Resources.WorldMapTiles.Image.Palette.Entries;
			if (page.Icon != null) palette = page.Icon.Image.Palette.Entries;
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
			
			_canvas.FillRectangle((byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 16 : 15), 0, 0, 320, 200);
			for (int x = 8; x < 312; x += 8)
			{
				AddLayer(borders[4], x, 0);
				AddLayer(borders[6], x, 192);
			}
			for (int y = 8; y < 192; y += 8)
			{
				AddLayer(borders[5], 0, y);
				AddLayer(borders[7], 312, y);
			}
			AddLayer(borders[0], 0, 0);
			AddLayer(borders[1], 312, 0);
			AddLayer(borders[2], 0, 192);
			AddLayer(borders[3], 312, 192);
			
			string category = "(unknown)";
			if (typeof(ITile).IsAssignableFrom(_singlePage.GetType())) category = "Terrain Type";
			if (typeof(IBuilding).IsAssignableFrom(_singlePage.GetType())) category = "City Improvement";
			if (typeof(IWonder).IsAssignableFrom(_singlePage.GetType())) category = "Wonder of the World";
			
			_canvas.DrawText(page.Name.ToUpper(), 5, 5, 204, 20, TextAlign.Center);
			_canvas.DrawText(category, 6, 7, 204, 36, TextAlign.Center);
			if (page.Icon != null)
				AddLayer(page.Icon, 23, 4);
			
			DrawTerrainText();
		}
	}
}