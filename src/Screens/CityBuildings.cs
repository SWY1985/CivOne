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
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityBuildings : BaseScreen
	{
		private readonly City _city;

		private readonly Bitmap _background;
		
		private bool _update = true;

		private void DrawWonders(int offset = 0)
		{
			IWonder[] wonders = _city.Wonders.ToArray();
			for (int i = 0; i < wonders.Length; i++)
			{
				int ii = (i + offset);
				int xx = (ii % 2 == 0) ? 21 : 1;
				int yy = -1 + (6 * ii);
				if (yy < 0)
					AddLayer(wonders[i].SmallIcon.GetPart(0, Math.Abs(yy), wonders[i].SmallIcon.Image.Width, wonders[i].SmallIcon.Image.Height + yy), xx, 0);
				else
					AddLayer(wonders[i].SmallIcon, xx, yy);
				_canvas.DrawText(wonders[i].Name, 1, 15, 42, 3 + (6 * ii));
			}
		}

		private void DrawBuildings(int offset = 0)
		{
			IBuilding[] buildings = _city.Buildings.ToArray();
			for (int i = 0; i < buildings.Length; i++)
			{
				int ii = (i + offset);
				int xx = (ii % 2 == 0) ? 21 : 1;
				int yy = -1 + (6 * ii);
				if (yy < 0)
					AddLayer(buildings[i].SmallIcon.GetPart(0, Math.Abs(yy), buildings[i].SmallIcon.Image.Width, buildings[i].SmallIcon.Image.Height + yy), xx, 0);
				else
					AddLayer(buildings[i].SmallIcon, xx, yy);
				_canvas.DrawText(buildings[i].Name, 1, 15, 42, 3 + (6 * ii));
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas.FillLayerTile(_background);
				_canvas.FillRectangle(0, 107, 0, 1, 97);

				DrawWonders();
				DrawBuildings(_city.Wonders.Length);

				_canvas.AddBorder(1, 1, 0, 0, 107, 97);
				
				_update = false;
			}
			return true;
		}

		public void Close()
		{
			Destroy();
		}

		public CityBuildings(City city, Bitmap background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(108, 97, background.Palette.Entries);
		}
	}
}