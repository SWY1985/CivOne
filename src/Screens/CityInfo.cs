// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
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
		
		private void DrawButton(string text, int x, int width, bool selected)
		{
			_canvas.FillRectangle(7, x + 0, 0, width, 1);
			_canvas.FillRectangle(7, x + 0, 1, 1, 8);
			_canvas.FillRectangle(1, x + 1, 8, width - 1, 1);
			_canvas.FillRectangle(1, x + width - 1, 0, 1, 8);
			_canvas.FillRectangle((byte)(selected ? 15 : 9), x + 1, 1, width - 2, 7);
			_canvas.DrawText(text, 1, 1, x + (width / 2), 2, TextAlign.Center);
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

				_update = false;
			}
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (args.Y > 9) return false;
			if (args.X < 34)
			{
				_choice = CityInfoChoice.Info;
				_update = true;
				return true;
			}
			else if (args.X < 66)
			{
				_choice = CityInfoChoice.Happy;
				_update = true;
				return true;
			}
			else if (args.X < 99)
			{
				_choice = CityInfoChoice.Map;
				_update = true;
				return true;
			}
			else if (args.X < 132)
			{
				// TODO: Open city view
				_choice = CityInfoChoice.Info;
				_update = true;
				Common.AddScreen(new CityView(_city));
				return true;
			}
			return false;
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