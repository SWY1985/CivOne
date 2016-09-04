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
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityProduction : BaseScreen
	{
		private readonly City _city;

		private readonly Bitmap _background;
		
		private bool _update = true;
		
		private void DrawButton(string text, int x, int width)
		{
			_canvas.FillRectangle(7, x + 0, 7, width, 1);
			_canvas.FillRectangle(7, x + 0, 8, 1, 8);
			_canvas.FillRectangle(1, x + 1, 15, width - 1, 1);
			_canvas.FillRectangle(1, x + width - 1, 7, 1, 8);
			_canvas.FillRectangle(9, x + 1, 8, width - 2, 7);
			_canvas.DrawText(text, 1, 1, x + (int)Math.Ceiling((double)width / 2), 9, TextAlign.Center);
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas.FillLayerTile(_background);
				_canvas.AddBorder(1, 1, 0, 0, 88, 99);
				_canvas.FillRectangle(1, 1, 1, 86, 16);
				DrawButton("Change", 1, 33);
				DrawButton("Buy", 64, 18);

				if (_city.CurrentProduction is IUnit)
				{
					IUnit unit = (_city.CurrentProduction as IUnit);
					AddLayer(unit.GetUnit(_city.Owner), 33, 0);
				}
				else
				{
				}
				/*
				_canvas.FillRectangle(0, 123, 0, 1, 38);*/

				
				_update = false;
			}
			return true;
		}

		public void Close()
		{
			Destroy();
		}

		public CityProduction(City city, Bitmap background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(88, 99, background.Palette.Entries);
		}
	}
}