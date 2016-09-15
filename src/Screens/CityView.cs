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
using System.Drawing.Imaging;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Interfaces;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityView : BaseScreen, IModal
	{
		private const float FADE_STEP = 0.1f;

		private readonly City _city;
		private readonly Picture _background;
		private readonly bool _founded;
		private readonly bool _firstView;

		private bool _update = true;
		
		private int _x = 80;
		private float _fadeStep = 1.0f;
		private bool _skip = false;

		public event EventHandler Skipped;
		
		private Color FadeColour(Color colour1, Color colour2)
		{
			int r = (int)(((float)colour1.R * (1.0F - _fadeStep)) + ((float)colour2.R * _fadeStep));
			int g = (int)(((float)colour1.G * (1.0F - _fadeStep)) + ((float)colour2.G * _fadeStep));
			int b = (int)(((float)colour1.B * (1.0F - _fadeStep)) + ((float)colour2.B * _fadeStep));
			return Color.FromArgb(r, g, b);
		}
		
		private void FadeColours()
		{
			if (Settings.Instance.GraphicsMode != GraphicsMode.Graphics256) return;
			
			ColorPalette palette = _background.Image.Palette;
			for (int i = 1; i < 256; i++)
				palette.Entries[i] = FadeColour(Color.Black, _background.OriginalColours[i]);
			_canvas.SetPalette(palette);
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_founded && (_skip || _x > 120))
			{
				_fadeStep -= FADE_STEP;
				if (_fadeStep <= 0.0f)
				{
					Close();
					return true;
				}
				FadeColours();
			}
			if (_founded && (gameTick % 3 == 0))
			{
				AddLayer(_background);
				_canvas.DrawText($"{_city.Name} founded: {Game.Instance.GameYear}.", 5, 5, 161, 3, TextAlign.Center);

				int frame = (_x % 4);
				AddLayer(Resources.Instance.GetPart("SETTLERS", 1, 1 + (16 * frame), 48, 15), _x, 120);
				AddLayer(Resources.Instance.GetPart("SETTLERS", 1, 1 + (16 * ((frame + 2) % 4)), 48, 15), _x + 27, 125);
				AddLayer(Resources.Instance.GetPart("SETTLERS", 1, 1 + (16 * ((frame + 3) % 4)), 48, 15), _x + 14, 131);
				AddLayer(Resources.Instance.GetPart("SETTLERS", 1, 1 + (16 * ((frame + 1) % 4)), 48, 15), _x + 40, 135);

				_x++;
				return true;
			}

			if (_firstView && _fadeStep < 1.0f)
			{
				_fadeStep += FADE_STEP;
				if (_fadeStep > 1.0f) _fadeStep = 1.0f;
				FadeColours();
			}

			if (_update)
				_update = false;
			return true;
		}

		private bool SkipAction()
		{
			Destroy();
			if (Skipped != null)
				Skipped(this, null);
			else
				HandleClose();
			return true;
		}

		private void Close()
		{
			Destroy();
			HandleClose();
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			return SkipAction();
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			return SkipAction();
		}
		
		public CityView(City city, bool founded = false, bool firstView = false, IBuilding building = null)
		{
			_city = city;
			_background = Resources.Instance.LoadPIC("HILL");
			_founded = founded;
			_firstView = firstView;
			
			_canvas = new Picture(320, 200, _background.Image.Palette.Entries);

			AddLayer(_background);
			
			if (founded)
			{
				return;
			}

			if (building != null)
			{
				string[] lines =  new [] { $"{_city.Name} builds", $"{building.Name}." };
				int width = lines.Max(l => Resources.Instance.GetTextSize(5, l).Width) + 10;
				int actualWidth = width;
				if (width % 4 > 0) width += (4 - (width % 4));
				Picture dialog = new Picture(width, 37);
				dialog.FillLayerTile((Bitmap)Resources.Instance.GetPart("SP299", 288, 120, 32, 16));
				if (width > actualWidth)
					dialog.FillRectangle(0, actualWidth, 0, width - actualWidth, 37);
				dialog.AddBorder(15, 8, 0, 0, actualWidth, 37);
				dialog.DrawText(lines[0], 5, 5, 4, 5);
				dialog.DrawText(lines[0], 5, 15, 4, 4);
				dialog.DrawText(lines[1], 5, 5, 4, 20);
				dialog.DrawText(lines[1], 5, 15, 4, 19);

				_canvas.FillRectangle(5, 80, 8, actualWidth + 2, 39);
				_canvas.AddLayer(dialog.Image, 81, 9); 
				return;
			}
			
			_canvas.DrawText(_city.Name, 5, 5, 161, 3, TextAlign.Center);
			_canvas.DrawText(_city.Name, 5, 15, 160, 2, TextAlign.Center);
			_canvas.DrawText(Game.Instance.GameYear, 5, 5, 161, 16, TextAlign.Center);
			_canvas.DrawText(Game.Instance.GameYear, 5, 15, 160, 15, TextAlign.Center);
			
			if (firstView)
			{
				_fadeStep = 0.0f;
				FadeColours();
				return;
			}
			//TODO: Render citizens
		}
	}
}