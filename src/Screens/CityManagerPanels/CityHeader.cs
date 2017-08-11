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
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne.Screens.CityManagerPanels
{
	internal class CityHeader : BaseScreen
	{
		private readonly City _city;

		private readonly Picture _background;
		
		private bool _update = true;
		
		public event EventHandler HeaderUpdate;

		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				string population = Common.NumberSeperator(_city.Population);

				_canvas.Tile(_background);
				_canvas.AddBorder(1, 1, 0, 0, 207, 21);
				_canvas.FillRectangle(0, 207, 0, 1, 21);
				_canvas.DrawText($"{_city.Name} (Pop:{population})", 1, 17, 104, 1, TextAlign.Center);

				int xx = 0;
				int group = -1;
				Citizen[] citizens = _city.Citizens.ToArray();
				for (int i = 0; i < _city.Size; i++)
				{
					xx += 8;
					if (group != (group = Common.CitizenGroup(citizens[i])) && group > 0 && i > 0)
					{
						xx += 2;
						if (group == 3) xx += 4;
					}
					AddLayer(Icons.Citizen(citizens[i]), xx, 7);
				}

				_update = false;
			}
			return true;
		}

		public override bool MouseDown(ScreenEventArgs args)
		{
			if (args.Y > 6 && args.Y < 20)
			{
				Citizen[] citizens = _city.Citizens.ToArray();
				int xx = 0;
				int group = -1;
				int index = -1;
				for (int i = 0; i < _city.Size; i++)
				{
					xx += 8;
					if ((int)citizens[i] >= 6) index++;
					if (group != (group = Common.CitizenGroup(citizens[i])) && group > 0 && i > 0)
					{
						xx += 2;
						if (group == 3) xx += 4;
					}
					if (args.X < xx || args.X > (xx + 7) || index < 0) continue;

					_city.ChangeSpecialist(index);
					Update();
					return true;
				}
				return false;
			}
			return false;
		}

		public void Update()
		{
			if (HeaderUpdate != null) HeaderUpdate(this, null);
			_update = true;
		}

		public void Close()
		{
			Destroy();
		}

		public CityHeader(City city, Picture background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(208, 21, background.Palette);
		}
	}
}