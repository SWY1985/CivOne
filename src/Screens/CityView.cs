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
using CivOne.Advances;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Interfaces;
using CivOne.GFX;
using CivOne.Templates;
using CivOne.Wonders;

namespace CivOne.Screens
{
	internal class CityView : BaseScreen, IModal
	{
		private const float FADE_STEP = 0.1f;
		private const int NOISE_COUNT = 40;

		private readonly City _city;
		private readonly IProduction _production;
		private readonly Picture _background;
		private readonly bool _founded;
		private readonly bool _firstView;
		private readonly byte[,] _noiseMap;
		
		private int _noiseCounter = NOISE_COUNT;

		private int _houseType = 0;

		private readonly Picture _overlay;

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
			return new Color(r, g, b);
		}
		
		private void FadeColours()
		{
			if (Settings.GraphicsMode != GraphicsMode.Graphics256) return;
			
			Color[] palette = _background.Palette;
			for (int i = 1; i < 256; i++)
				palette[i] = FadeColour(Color.Black, _background.OriginalColours[i]);
			_canvas.SetPalette(palette);
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (gameTick % 4 == 0)
			{
				_canvas.Cycle(64, 79);
				_update = true;
			}

			if (_noiseMap != null)
			{
				if (_noiseCounter > 0)
				{
					_overlay.ApplyNoise(_noiseMap, _noiseCounter--);
					AddLayer(_background);
					AddLayer(_overlay);
					return true;
				}
				return false;
			}

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
				_canvas.DrawText($"{_city.Name} founded: {Game.GameYear}.", 5, 5, 161, 3, TextAlign.Center);

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

		private void DrawWonder<T>(Picture picture = null) where T : IWonder
		{
			if (picture == null) picture = _background;
			if (typeof(T) == typeof(Pyramids))
			{
				picture.AddLayer(Resources.Instance.GetPart("WONDERS2", 131, 54, 187, 29), 133, 0);
				picture.AddLayer(Resources.Instance.GetPart("WONDERS2", 318, 54, 1, 29), 0, 0);
			}
			if (typeof(T) == typeof(Colossus))
			{
				picture.AddLayer(Resources.Instance.GetPart("WONDERS2", 88, 97, 124, 39), 170, 0);
			}
			if (typeof(T) == typeof(GreatWall))
			{
				picture.AddLayer(Resources.Instance.GetPart("WONDERS2", 1, 38, 66, 81), 0, 0);
			}
		}

		private void DrawBuilding<T>(Picture picture = null) where T : IBuilding
		{
			if (picture == null) picture = _background;
			if (typeof(T) == typeof(Aqueduct))
			{
				picture.AddLayer(Resources.Instance.GetPart("CITYPIX2", 51, 151, 49, 49), 0, 72);
			}

			if (typeof(T) == typeof(CityWalls))
			{
				Picture wall = Resources.Instance.GetPart("CITYPIX2", 251, 101, 43, 49);
				Picture door = Resources.Instance.GetPart("CITYPIX2", 51, 101, 49, 49);

				//0, 108
				for (int xx = 0; xx < 142; xx += 43)
					picture.AddLayer(wall, xx, 108);
				picture.AddLayer(door, 142, 108);
				for (int xx = 191; xx < 320; xx += 43)
					picture.AddLayer(wall, xx, 108);
			}
		}

		private byte[,] GetCityMap
		{
			get
			{
				Common.SetRandomSeedFromName(_city.Name);
				
				// This is experimental code, not the same as the original game
				int ww = 4 + (_city.Size % 2 * 4);
				int hh = 4 + (((_city.Size - 1) % 2) * 4);
				byte[,] cityMap = new byte[ww, hh];
				_houseType = Common.Random.Next(2);
				for (int ii = 0; ii < _city.Size; ii++)
				for (int t = 0; t < 10; t++)
				{
					int xx = Common.Random.Next(ww);
					int yy = Common.Random.Next(hh);
					int type = Common.Random.Next(8);
					if (type < 3)
					{
						// house 1
						cityMap[xx, yy] = 1;
					}
					else if (type < 6)
					{
						// house 2
						cityMap[xx, yy] = 2;
					}
					else
					{
						// tree
						cityMap[xx, yy] = 3;
					}
				}
				for (int xx = 5; xx < ww; xx += 6)
				{
					bool road = false;
					for (int yy = 0; yy < hh; yy++)
					{
						cityMap[xx, yy] = 0;
						if (cityMap[xx - 1, yy] != 0 || (xx + 1 < ww && cityMap[xx + 1, yy] != 0))
						{
							road = true;
						}
					}
					if (!road) continue;
					for (int yy = 0; yy < hh; yy++)
					{
						byte roadType = 8;
						if (yy == 0) roadType = 4;
						if (yy == (hh - 1)) roadType = 7;
						cityMap[xx, yy] = roadType;
					}
				}

				return cityMap;
			}
		}

		private void DrawBuildings()
		{
			byte[,] cityMap = GetCityMap;
			int ww = cityMap.GetUpperBound(0);
			int hh = cityMap.GetUpperBound(1);

			if (_city.Wonders.Any(b => b is Pyramids))
			{
				DrawWonder<Pyramids>();
				if (!(_production is Pyramids))
					DrawWonder<Pyramids>(_overlay);
			}
			if (_city.Wonders.Any(b => b is Colossus))
			{
				DrawWonder<Colossus>();
				if (!(_production is Colossus))
					DrawWonder<Colossus>(_overlay);
			}
			if (_city.Wonders.Any(b => b is GreatWall))
			{
				DrawWonder<GreatWall>();
				if (!(_production is GreatWall))
					DrawWonder<GreatWall>( _overlay);
			}

			if (_city.Buildings.Any(b => b is Aqueduct))
			{
				DrawBuilding<Aqueduct>();
				if (!(_production is Aqueduct))
					DrawBuilding<Aqueduct>( _overlay);
			}
			
			for (int yy = (hh - 1); yy >= 0; yy--)
			for (int xx = 0; xx < ww; xx++)
			{
				int dx = 96 + (16 * xx) + (yy * 8);
				int dy = 106 - (yy * 8);
				Picture building;
				switch (cityMap[xx, yy])
				{
					case 1:
						building = Resources.Instance.GetPart("CITYPIX1", 1 + (32 * _houseType), 1, 31, 31);
						break;
					case 2:
						building = Resources.Instance.GetPart("CITYPIX1", 1 + (32 * _houseType), 33, 31, 31);
						break;
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
						building = Resources.Instance.GetPart("CITYPIX1", 0 + (24 * (cityMap[xx,yy] - 3)), 65, 24, 8);
						dx -= 5;
						dy += 24;
						break;
					default: continue;
				}
				_background.AddLayer(building, dx, dy);
			}

			if (_city.Buildings.Any(b => b is CityWalls))
			{
				DrawBuilding<CityWalls>();
				if (!(_production is CityWalls))
					DrawBuilding<CityWalls>( _overlay);
			}
		}
		
		public CityView(City city, bool founded = false, bool firstView = false, IProduction production = null)
		{
			_city = city;
			_production = production;
			_background = new Picture(Resources.Instance.LoadPIC("HILL"));
			_founded = founded;
			_firstView = firstView;
			
			_canvas = new Picture(320, 200, _background.Palette);
			_overlay = new Picture(_background);
			
			if (founded) return;

			DrawBuildings();
			AddLayer(_background);

			if (production != null)
			{
				_noiseMap = new byte[320, 200];
				for (int x = 0; x < 320; x++)
				for (int y = 0; y < 200; y++)
				{
					_noiseMap[x, y] = (byte)Common.Random.Next(1, _noiseCounter);
				}

				string[] lines =  new [] { $"{_city.Name} builds", $"{(production as ICivilopedia).Name}." };
				int width = lines.Max(l => Resources.Instance.GetTextSize(5, l).Width) + 10;
				int actualWidth = width;
				if (width % 4 > 0) width += (4 - (width % 4));
				Picture dialog = new Picture(width, 37);
				dialog.FillLayerTile(Resources.Instance.GetPart("SP299", 288, 120, 32, 16));
				if (width > actualWidth)
					dialog.FillRectangle(0, actualWidth, 0, width - actualWidth, 37);
				dialog.AddBorder(15, 8, 0, 0, actualWidth, 37);
				dialog.DrawText(lines[0], 5, 5, 4, 5);
				dialog.DrawText(lines[0], 5, 15, 4, 4);
				dialog.DrawText(lines[1], 5, 5, 4, 20);
				dialog.DrawText(lines[1], 5, 15, 4, 19);

				foreach (Picture picture in new[] { _background, _overlay })
				{
					picture.FillRectangle(5, 80, 8, actualWidth + 2, 39);
					picture.AddLayer(dialog, 81, 9);
				}
				return;
			}
			
			_canvas.DrawText(_city.Name, 5, 5, 161, 3, TextAlign.Center);
			_canvas.DrawText(_city.Name, 5, 15, 160, 2, TextAlign.Center);
			_canvas.DrawText(Game.GameYear, 5, 5, 161, 16, TextAlign.Center);
			_canvas.DrawText(Game.GameYear, 5, 15, 160, 15, TextAlign.Center);
			
			if (firstView)
			{
				_fadeStep = 0.0f;
				FadeColours();
				return;
			}

			int i = 0;
			int group = -1;
			int offsetX = 24;
			bool modern = Human.HasAdvance<Industrialization>();
			foreach (Citizen citizen in _city.Citizens)
			{
				if (group != (group = Common.CitizenGroup(citizen)) && group > 0) offsetX += 8;

				int sx = ((int)(citizen) * 35) + 1, sy = (modern ? 1 : 52);
				int sw = 34, sh = (modern ? 50 : 52);
				int dx = (int)(citizen) + offsetX + (11 * i++), dy = 140;
				AddLayer(Resources.Instance.GetPart("POP", sx, sy, sw, sh), dx, dy);
			}
		}
	}
}