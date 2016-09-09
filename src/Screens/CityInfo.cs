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
	internal class CityInfo : BaseScreen
	{
		private readonly City _city;

		private readonly Bitmap _background;
		
		private CityInfoChoice _choice = CityInfoChoice.Info;
		private bool _update = true;

		private Picture InfoFrame
		{
			get
			{
				Picture output = new Picture(144, 83);
				IUnit[] units = _city.Tile.Units;
				for (int i = 0; i < units.Length; i++)
				{
					int xx = 4 + ((i % 6) * 18);
					int yy = 0 + (((i - (i % 6)) / 6) * 16);

					output.AddLayer(units[i].GetUnit(units[i].Owner).Image, xx, yy);
					output.DrawText($"{units[i].Home.Name.Substring(0, 3)}.", 1, 5, xx, yy + 16);
				}
				return output;
			}
		}

		private Picture HappyFrame
		{
			get
			{
				//TODO: Draw happiness data/stats
				Picture output = new Picture(144, 83);
				output.FillRectangle(1, 5, 15, 122, 1);
				output.FillRectangle(1, 5, 31, 122, 1);
				
				for (int yy = 1; yy < 30; yy+= 16)
				for (int i = 0; i < _city.Size; i++)
				{
					if (i < _city.ResourceTiles.Count() - 1)
					{
						output.AddLayer(Icons.Population((i % 2 == 0) ? Population.ContentMale : Population.ContentFemale), 7 + (8 * i), yy);
						continue;
					}
					output.AddLayer(Icons.Population(Population.Entertainer), 7 + (8 * i), yy);
				}
				return output;
			}
		}
		
		private Picture MapFrame
		{
			get
			{
				//TODO: Draw map
				Picture output = new Picture(144, 83);
				output.FillRectangle(9, 5, 2, 122, 1);
				output.FillRectangle(9, 5, 3, 1, 74);
				output.FillRectangle(9, 126, 3, 1, 74);
				output.FillRectangle(9, 5, 77, 122, 1);
				output.FillRectangle(5, 6, 3, 120, 74);
				return output;
			}
		}
		
		private void DrawButton(string text, int x, int width, bool selected)
		{
			_canvas.FillRectangle(7, x + 0, 0, width, 1);
			_canvas.FillRectangle(7, x + 0, 1, 1, 8);
			_canvas.FillRectangle(1, x + 1, 8, width - 1, 1);
			_canvas.FillRectangle(1, x + width - 1, 0, 1, 8);
			_canvas.FillRectangle((byte)(selected ? 15 : 9), x + 1, 1, width - 2, 7);
			_canvas.DrawText(text, 1, 1, x + (int)Math.Ceiling((double)width / 2), 2, TextAlign.Center);
		}

		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas.FillLayerTile(_background);
				_canvas.AddBorder(1, 1, 0, 0, 133, 92);
				_canvas.FillRectangle(0, 133, 0, 3, 92);
				
				DrawButton("Info", 0, 34, (_choice == CityInfoChoice.Info));
				DrawButton("Happy", 34, 32, (_choice == CityInfoChoice.Happy));
				DrawButton("Map", 66, 33, (_choice == CityInfoChoice.Map));
				DrawButton("View", 99, 33, false);

				switch (_choice)
				{
					case CityInfoChoice.Info:
						AddLayer(InfoFrame, 0, 9);
						break;
					case CityInfoChoice.Happy:
						AddLayer(HappyFrame, 0, 9);
						break;
					case CityInfoChoice.Map:
						AddLayer(MapFrame, 0, 9);
						break;
				}

				_update = false;
			}
			return true;
		}

		private bool GotoInfo()
		{
			_choice = CityInfoChoice.Info;
			_update = true;
			return true;
		}

		private bool GotoHappy()
		{
			_choice = CityInfoChoice.Happy;
			_update = true;
			return true;
		}

		private bool GotoMap()
		{
			_choice = CityInfoChoice.Map;
			_update = true;
			return true;
		}

		private bool GotoView()
		{
			_choice = CityInfoChoice.Info;
			_update = true;
			Common.AddScreen(new CityView(_city));
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			switch (args.KeyChar)
			{
				case 'I':
					return GotoInfo();
				case 'H':
					return GotoHappy();
				case 'M':
					return GotoMap();
				case 'V':
					return GotoView();
			}
			return false;
		}

		private bool InfoClick(ScreenEventArgs args)
		{
			IUnit[] units = _city.Tile.Units;
			for (int i = 0; i < units.Length; i++)
			{
				int xx = 4 + ((i % 6) * 18);
				int yy = 0 + (((i - (i % 6)) / 6) * 16);

				if (new Rectangle(xx, yy, 16, 16).Contains(args.Location))
				{
					units[i].Sentry = false;
					_update = true;
					break;
				}
			}
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (args.Y < 10)
			{
				if (args.X < 34) return GotoInfo();
				else if (args.X < 66) return GotoHappy();
				else if (args.X < 99) return GotoMap();
				else if (args.X < 132) return GotoView();
			}
			
			switch (_choice)
			{
				case CityInfoChoice.Info:
					MouseArgsOffset(ref args, 0, 9);
					return InfoClick(args);
				case CityInfoChoice.Happy:
				case CityInfoChoice.Map:
					break;
			}
			return true;
		}

		public void Close()
		{
			Destroy();
		}

		public CityInfo(City city, Bitmap background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(136, 92, background.Palette.Entries);
		}
	}
}