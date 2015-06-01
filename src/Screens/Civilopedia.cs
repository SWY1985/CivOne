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
		internal static ICivilopedia[] Improvements = new ICivilopedia[0];
		internal static ICivilopedia[] Units = new ICivilopedia[0];
		internal static ICivilopedia[] Terrain = new ICivilopedia[] { new Arctic(), new Desert(), new Forest(), new Grassland(), new Hills(), new Jungle(), new Mountains(), new Ocean(), new Plains(), new River(), new Swamp(), new Tundra() };
		internal static ICivilopedia[] Misc = new ICivilopedia[0]; 
		internal static ICivilopedia[] Complete
		{
			get
			{
				ICivilopedia[] output = new ICivilopedia[0];
				output = output.Concat(Terrain).ToArray();
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
			
			_canvas.DrawText(page.Name.ToUpper(), 5, 5, 204, 20, TextAlign.Center);
			_canvas.DrawText(category, 6, 7, 204, 36, TextAlign.Center);
			if (page.Icon != null)
				AddLayer(page.Icon, 23, 4);
		}
	}
}