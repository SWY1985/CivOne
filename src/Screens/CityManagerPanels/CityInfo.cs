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
using CivOne.Graphics;
using CivOne.Graphics.Sprites;
using CivOne.Units;
using CivOne.UserInterface;

namespace CivOne.Screens.CityManagerPanels
{
	internal class CityInfo : BaseScreen
	{
		private readonly City _city;
		private readonly Button _info, _happy, _map;
		
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

					output.AddLayer(units[i].ToBitmap(), xx, yy);
					string homeCity = "NON.";
					if (units[i].Home != null)
						homeCity = $"{units[i].Home.Name.Substring(0, 3)}.";
					output.DrawText(homeCity, 1, 5, xx, yy + 16);
				}
				return output;
			}
		}

		private Picture HappyFrame
		{
			get
			{
				//TODO: Draw happiness data/stats
				Picture output = new Picture(144, 83)
					.FillRectangle(5, 15, 122, 1, 1)
					.FillRectangle(5, 31, 122, 1, 1)
					.As<Picture>();
				
				for (int yy = 1; yy < 30; yy+= 16)
				for (int i = 0; i < _city.Size; i++)
				{
					if (i < _city.ResourceTiles.Count() - 1)
					{
						output.AddLayer(Icons.Citizen((i % 2 == 0) ? Citizen.ContentMale : Citizen.ContentFemale), 7 + (8 * i), yy);
						continue;
					}
					output.AddLayer(Icons.Citizen(Citizen.Entertainer), 7 + (8 * i), yy);
				}
				return output;
			}
		}
		
		private Picture MapFrame
		{
			get
			{
				//TODO: Draw map
				Picture output = new Picture(144, 83)
					.FillRectangle(5, 2, 122, 1, 9)
					.FillRectangle(5, 3, 1, 74, 9)
					.FillRectangle(126, 3, 1, 74, 9)
					.FillRectangle(5, 77, 122, 1, 9)
					.FillRectangle(6, 3, 120, 74, 5)
					.As<Picture>();
				return output;
			}
		}

		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				this.Tile(Pattern.PanelBlue)
					.DrawRectangle(colour: 1);
				
				_info.Colour = (byte)((_choice == CityInfoChoice.Info) ? 15 : 9);
				_happy.Colour = (byte)((_choice == CityInfoChoice.Happy) ? 15 : 9);
				_map.Colour = (byte)((_choice == CityInfoChoice.Map) ? 15 : 9);

				switch (_choice)
				{
					case CityInfoChoice.Info:
						this.AddLayer(InfoFrame, 0, 9);
						break;
					case CityInfoChoice.Happy:
						this.AddLayer(HappyFrame, 0, 9);
						break;
					case CityInfoChoice.Map:
						this.AddLayer(MapFrame, 0, 9);
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

		private void InfoClick(object sender, EventArgs args) => GotoInfo();

		private void HappyClick(object sender, EventArgs args) => GotoHappy();

		private void MapClick(object sender, EventArgs args) => GotoMap();

		private void ViewClick(object sender, EventArgs args) => GotoView();
		
		private void KeyDown(object sender, KeyboardEventArgs args)
		{
			switch (args.KeyChar)
			{
				case 'I':
					args.Handled = GotoInfo();
					break;
				case 'H':
					args.Handled = GotoHappy();
					break;
				case 'M':
					args.Handled = GotoMap();
					break;
				case 'V':
					args.Handled = GotoView();
					break;
			}
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
					units[i].Busy = false;
					_update = true;
					break;
				}
			}
			return true;
		}
		
		private void MouseDown(object sender, ScreenEventArgs args)
		{
			if (args.Y < 10) return;
			
			switch (_choice)
			{
				case CityInfoChoice.Info:
					MouseArgsOffset(ref args, 0, 9);
					args.Handled = InfoClick(args);
					return;
				case CityInfoChoice.Happy:
				case CityInfoChoice.Map:
					break;
			}
			args.Handled = true;
		}

		public CityInfo(City city) : base(133, 92)
		{
			_city = city;

			OnKeyDown += KeyDown;
			OnMouseDown += MouseDown;

			Elements.AddRange(new [] {
				_info = Button.Blue("Info", 0, 0, 34, click: InfoClick),
				_happy = Button.Blue("Happy", 34, 0, 32, click: HappyClick),
				_map = Button.Blue("Map", 66, 0, 33, click: MapClick),
				Button.Blue("View", 99, 0, 33, click: ViewClick)
			});
		}
	}
}