// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Graphics.Sprites;
using CivOne.Units;

namespace CivOne.Screens.CityManagerPanels
{
	internal class CityInfo : BaseScreen
	{
		private readonly City _city;
		private readonly IUnit[] _units;

		private CityInfoChoice _choice = CityInfoChoice.Info;
		private bool _update = true;

		private Picture InfoFrame
		{
			get
			{
				Picture output = new Picture(144, 83);
				for (int i = 0; i < _units.Length; i++)
				{
					int xx = 4 + ((i % 6) * 18);
					int yy = 0 + (((i - (i % 6)) / 6) * 16);

					output.AddLayer(_units[i].ToBitmap(), xx, yy);
					string homeCity = "NON.";
					if (_units[i].Home != null)
					{
						homeCity = _units[i].Home.Name;
						if (homeCity.Length >= 3)
							homeCity = $"{homeCity.Substring(0, 3)}.";
					}
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
				
				DrawButton("Info", (byte)((_choice == CityInfoChoice.Info) ? 15 : 9), 1, 0, 0, 34);
				DrawButton("Happy", (byte)((_choice == CityInfoChoice.Happy) ? 15 : 9), 1, 34, 0, 32);
				DrawButton("Map", (byte)((_choice == CityInfoChoice.Map) ? 15 : 9), 1, 66, 0, 33);
				DrawButton("View", 9, 1, 99, 0, 33);

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
			for (int i = 0; i < _units.Length; i++)
			{
				int xx = 4 + ((i % 6) * 18);
				int yy = 0 + (((i - (i % 6)) / 6) * 16);

				if (new Rectangle(xx, yy, 16, 16).Contains(args.Location))
				{
					_units[i].Busy = false;
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

		public CityInfo(City city) : base(133, 92)
		{
			_city = city;
			_units = Game.GetUnits().Where(u => u.X == city.X && u.Y == city.Y).ToArray();
		}
	}
}