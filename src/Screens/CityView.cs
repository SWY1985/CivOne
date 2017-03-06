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

using Bld = CivOne.Buildings;

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
		private readonly bool _captured;
		private readonly byte[,] _noiseMap;
		
		private int _noiseCounter = NOISE_COUNT;

		private int _houseType = 0;

		private readonly Picture _overlay;
		private readonly Picture[] _invaders;

		private bool _update = true;
		
		private int _x = 80, _y = 138;
		private float _fadeStep = 1.0f;
		private bool _skip = false;

		private string _buildingFile = null;

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

			if (_captured)
			{
				AddLayer(_background);
				int frame = (_x % 30) / 3;
				for (int i = 7; i >= 0; i--)
				{
					int xx = (_x - 65) - (48 * i);
					if (xx + 78 <= 0) continue;
					AddLayer(_invaders[frame], xx, _y);
				}
				_x++;
				return true;
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
					Destroy();
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

		private void DrawBuilding<T>(Picture picture = null, int x = -1, int y = -1) where T : IBuilding
		{
			if (_buildingFile == null)
			{
				_buildingFile = Game.GetPlayer(_city.Owner).HasAdvance<Invention>() ? "CITYPIX3" : "CITYPIX2";
			}

			if (picture == null) picture = _background;
			if (typeof(T) == typeof(Aqueduct))
			{
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 51, 151, 49, 49), 0, 72);
			}

			if (typeof(T) == typeof(CityWalls))
			{
				Picture wall = Resources.Instance.GetPart(_buildingFile, 251, 101, 43, 49);
				Picture door = Resources.Instance.GetPart(_buildingFile, 51, 101, 49, 49);

				//0, 108
				for (int xx = 0; xx < 142; xx += 43)
					picture.AddLayer(wall, xx, 108);
				picture.AddLayer(door, 142, 108);
				for (int xx = 191; xx < 320; xx += 43)
					picture.AddLayer(wall, xx, 108);
			}

			if (typeof(T) == typeof(Barracks))
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 1, 1, 49, 49), x, y);
			if (typeof(T) == typeof(Granary))
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 1, 51, 49, 49), x, y);
			if (typeof(T) == typeof(Temple))
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 1, 101, 49, 49), x, y);
			if (typeof(T) == typeof(MarketPlace))
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 1, 151, 49, 49), x, y);
			if (typeof(T) == typeof(Library))
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 51, 1, 49, 49), x, y);
			if (typeof(T) == typeof(Courthouse))
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 51, 51, 49, 49), x, y);
			if (typeof(T) == typeof(Bank))
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 101, 1, 49, 49), x, y);
			if (typeof(T) == typeof(Cathedral))
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 101, 51, 49, 49), x, y);
			if (typeof(T) == typeof(Bld.University))
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 101, 101, 49, 49), x, y);
			if (typeof(T) == typeof(Colosseum))
				picture.AddLayer(Resources.Instance.GetPart(_buildingFile, 151, 1, 49, 49), x, y);
		}

		private CityViewMap[,] GetCityMap
		{
			get
			{
				Common.SetRandomSeedFromName(_city.Name);
				_houseType = Common.Random.Next(2);

				CityViewMap[,] cityMap = new CityViewMap[18,11];
				for (int yy = 0; yy < 11; yy++)
				for (int xx = 0; xx < 18; xx++)
				{
					if (xx == 6 || xx == 11 || yy == 2 || yy == 6)
						cityMap[xx, yy] = CityViewMap.Road;
					if ((xx < 2 && yy < 3) || (xx > 16 && yy > 8))
						cityMap[xx, yy] = CityViewMap.Occupied;
				}
				
				// This is experimental code, not the same as the original game
				int ww = 4 + _city.Size;
				int hh = 4 + (_city.Size - 1);
				if (ww > 18) ww = 18;
				if (hh > 11) hh = 11;

				int bx = (ww / 2) + ((18 - ww) / 2);
				int by = (hh / 2);
				for (int ii = 0; ii < _city.Size; ii++)
				{
					for (int t = 0; t < 16; t++)
					{
						int relX = Common.Random.Next(-1, 2);
						int relY = Common.Random.Next(-1, 2);
						if (relX == 0 && relY == 0) continue;
						bx += relX;
						by += relY;
						while (bx < ((18 - ww) / 2)) bx++;
						while (bx >= ww + ((18 - ww) / 2)) bx--;
						while (by < 0) by++;
						while (by >= hh) by--;
						int type = Common.Random.Next(8);
						if (cityMap[bx, by] != CityViewMap.Empty) continue;
						if (type < 6)
							cityMap[bx, by] = CityViewMap.House;
						else
							cityMap[bx, by] = CityViewMap.Tree;
					}
					for (int i = 0; i < 1000; i++)
					{
						bx = Common.Random.Next(ww) + ((18 - ww) / 2);
						by = Common.Random.Next(hh);
						if (cityMap[bx, by] != CityViewMap.Empty) continue;
						for (int ix = -1; ix < 2; ix++)
						for (int iy = -1; iy < 2; iy++)
						{
							if (Math.Abs(ix) == Math.Abs(iy)) continue;
							if (bx + ix < ((18 - ww) / 2)) continue;
							if (bx + ix >= ww + ((18 - ww) / 2)) continue;
							if (by + iy < 0) continue;
							if (by + iy >= hh) continue;
							if (cityMap[bx + ix, by + iy] != CityViewMap.Empty) { i = 1000; break; }
						}
					}
				}
				
				for (int yy = 0; yy < 11; yy++)
				for (int xx = 0; xx < 18; xx++)
				{
					if ((int)cityMap[xx, yy] > 1)
					{
						if ((xx == 0 || (cityMap[xx - 1, yy] != CityViewMap.House && cityMap[xx - 1, yy] != CityViewMap.Tree)) &&
							(xx == 17 || (cityMap[xx + 1, yy] != CityViewMap.House && cityMap[xx + 1, yy] != CityViewMap.Tree)) &&
							(yy == 0 || (cityMap[xx, yy - 1] != CityViewMap.House && cityMap[xx, yy - 1] != CityViewMap.Tree)) &&
							(yy == 10 || (cityMap[xx, yy + 1] != CityViewMap.House && cityMap[xx, yy + 1] != CityViewMap.Tree))) cityMap[xx, yy] = CityViewMap.Empty;
					}
					if (cityMap[xx, yy] != CityViewMap.Road) continue;
					if ((xx == 0 || (int)cityMap[xx - 1, yy] > 1) ||
						(xx == 17 || (int)cityMap[xx + 1, yy] > 1) ||
						(yy == 0 || (int)cityMap[xx, yy - 1] > 1) ||
						(yy == 10 || (int)cityMap[xx, yy + 1] > 1)) continue;
					cityMap[xx, yy] = CityViewMap.Empty;
				}
				
				for (int yy = 0; yy < 11; yy++)
				for (int xx = 0; xx < 18; xx++)
				{
					if (cityMap[xx, yy] != CityViewMap.Empty) continue;
					if (!(xx == 6 || xx == 11 || yy == 2 || yy == 6)) continue;
					if (((xx == 0 || (int)cityMap[xx - 1, yy] != 1) ? 1 : 0) +
						((xx == 17 || (int)cityMap[xx + 1, yy] != 1) ? 1 : 0) +
						((yy == 0 || (int)cityMap[xx, yy - 1] != 1) ? 1 : 0) +
						((yy == 10 || (int)cityMap[xx, yy + 1] != 1 ? 1 : 0)) > 1) continue;
					cityMap[xx, yy] = CityViewMap.Road;
				}

				
				foreach (Type type in new Type[] { typeof(Barracks), typeof(Granary), typeof(Temple), typeof(MarketPlace), typeof(Library), typeof(Courthouse), typeof(Bank), typeof(Cathedral), typeof(Bld.University), typeof(Colosseum) })
				{
					if (_city.HasBuilding(type))
					{
						CityViewMap id;
						if (type == typeof(Barracks)) id = CityViewMap.Barracks;
						else if (type == typeof(Granary)) id = CityViewMap.Granary;
						else if (type == typeof(Temple)) id = CityViewMap.Temple;
						else if (type == typeof(MarketPlace)) id = CityViewMap.MarketPlace;
						else if (type == typeof(Library)) id = CityViewMap.Library;
						else if (type == typeof(Courthouse)) id = CityViewMap.Courthouse;
						else if (type == typeof(Bank)) id = CityViewMap.Bank;
						else if (type == typeof(Cathedral)) id = CityViewMap.Cathedral;
						else if (type == typeof(Bld.University)) id = CityViewMap.University;
						else if (type == typeof(Colosseum)) id = CityViewMap.Colosseum;
						else continue;

						for (int i = 0; i < 1000; i++)
						{
							int xx = Common.Random.Next(15) + 1;
							int yy = Common.Random.Next(10);
							if (xx == 6 || xx == 11 || yy == 2 || yy == 6) continue;
							if (xx == 5 || xx == 10 || yy == 1 || yy == 5) continue;
							if ((int)cityMap[xx, yy] > 3 ||
								(int)cityMap[xx + 1, yy] > 3 ||
								(int)cityMap[xx, yy + 1] > 3 ||
								(int)cityMap[xx + 1, yy + 1] > 3) continue;

							cityMap[xx, yy] = id;
							cityMap[xx + 1, yy] = CityViewMap.Occupied;
							cityMap[xx, yy + 1] = CityViewMap.Occupied;
							cityMap[xx + 1, yy + 1] = CityViewMap.Occupied;
							break;
						}
					}
				}

				return cityMap;
			}
		}

		private void DrawBuildings()
		{
			CityViewMap[,] cityMap = GetCityMap;

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
					DrawWonder<GreatWall>(_overlay);
			}

			if (_city.Buildings.Any(b => b is Aqueduct))
			{
				DrawBuilding<Aqueduct>();
				if (!(_production is Aqueduct))
					DrawBuilding<Aqueduct>(_overlay);
			}
			
			for (int yy = 10; yy >= 0; yy--)
			for (int xx = 0; xx < 18; xx++)
			{
				int dx = 0 + (16 * xx) + (yy * 8);
				int dy = 106 - (yy * 8);
				Picture building;
				switch (cityMap[xx, yy])
				{
					case CityViewMap.House:
						building = Resources.Instance.GetPart("CITYPIX1", 1 + (32 * _houseType), 1, 31, 31);
						break;
					case CityViewMap.Tree:
						building = Resources.Instance.GetPart("CITYPIX1", 0, 65, 24, 8);
						dx -= 5;
						dy += 24;
						break;
					case CityViewMap.Road:
						Direction road = 0;
						if (yy < cityMap.GetUpperBound(1) && cityMap[xx, yy + 1] == CityViewMap.Road) road |= Direction.North;
						if (xx < cityMap.GetUpperBound(0) && cityMap[xx + 1, yy] == CityViewMap.Road) road |= Direction.East;
						if (yy > 0 && cityMap[xx, yy - 1] == CityViewMap.Road) road |= Direction.South;
						if (xx > 0 && cityMap[xx - 1, yy] == CityViewMap.Road) road |= Direction.West;

						int sx = (int)road;
						int sy = 65;
						if (sx == 0) continue;
						if (sx > 7) sy += 8;
						sx = (sx % 8) * 24;
						if (Game.GetPlayer(_city.Owner).HasAdvance<Automobile>()) sy += 16;
						building = Resources.Instance.GetPart("CITYPIX1", sx, sy, 24, 8);
						dx -= 5;
						dy += 24;
						break;
					case CityViewMap.Barracks:
						dy -= 18;
						DrawBuilding<Barracks>(x: dx, y: dy);
						if (!(_production is Barracks))
							DrawBuilding<Barracks>(_overlay, dx, dy);
						continue;
					case CityViewMap.Granary:
						dy -= 18;
						DrawBuilding<Granary>(x: dx, y: dy);
						if (!(_production is Granary))
							DrawBuilding<Granary>(_overlay, dx, dy);
						continue;
					case CityViewMap.Temple:
						dy -= 18;
						DrawBuilding<Temple>(x: dx, y: dy);
						if (!(_production is Temple))
							DrawBuilding<Temple>(_overlay, dx, dy);
						continue;
					case CityViewMap.MarketPlace:
						dy -= 18;
						DrawBuilding<MarketPlace>(x: dx, y: dy);
						if (!(_production is MarketPlace))
							DrawBuilding<MarketPlace>(_overlay, dx, dy);
						continue;
					case CityViewMap.Library:
						dy -= 18;
						DrawBuilding<Library>(x: dx, y: dy);
						if (!(_production is Library))
							DrawBuilding<Library>(_overlay, dx, dy);
						continue;
					case CityViewMap.Courthouse:
						dy -= 18;
						DrawBuilding<Courthouse>(x: dx, y: dy);
						if (!(_production is Courthouse))
							DrawBuilding<Courthouse>(_overlay, dx, dy);
						continue;
					case CityViewMap.Bank:
						dy -= 18;
						DrawBuilding<Bank>(x: dx, y: dy);
						if (!(_production is Bank))
							DrawBuilding<Bank>(_overlay, dx, dy);
						continue;
					case CityViewMap.Cathedral:
						dy -= 18;
						DrawBuilding<Cathedral>(x: dx, y: dy);
						if (!(_production is Cathedral))
							DrawBuilding<Cathedral>(_overlay, dx, dy);
						continue;
					case CityViewMap.University:
						dy -= 18;
						DrawBuilding<Bld.University>(x: dx, y: dy);
						if (!(_production is Bld.University))
							DrawBuilding<Bld.University>(_overlay, dx, dy);
						continue;
					case CityViewMap.Colosseum:
						dy -= 18;
						DrawBuilding<Colosseum>(x: dx, y: dy);
						if (!(_production is Colosseum))
							DrawBuilding<Colosseum>(_overlay, dx, dy);
						continue;
					default: continue;
				}
				_background.AddLayer(building, dx, dy);
				_overlay.AddLayer(building, dx, dy);
			}

			if (_city.Buildings.Any(b => b is CityWalls))
			{
				DrawBuilding<CityWalls>();
				if (!(_production is CityWalls))
					DrawBuilding<CityWalls>( _overlay);
			}
		}

		public static CityView Capture(City city)
		{
			return new CityView(city, captured: true);
		}
		
		public CityView(City city, bool founded = false, bool firstView = false, IProduction production = null, bool captured = false)
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
			
			if (_captured = captured)
			{
				Picture invaders;
				int xx = 0, yy = 2, ww = 78, hh = 60;
				if (Game.CurrentPlayer.HasAdvance<Conscription>())
				{
					invaders = Resources.Instance.LoadPIC("INVADERS");
				}
				else if (Game.CurrentPlayer.HasAdvance<Gunpowder>())
				{
					invaders = Resources.Instance.LoadPIC("INVADER2");
				}
				else
				{
					invaders = Resources.Instance.LoadPIC("INVADER3");
					xx = 1;
					yy = 1;
					ww = 78;
					hh = 65;
					_y = 133;
				}

				_invaders = new Picture[10];
				for (int ii = 0; ii < 10; ii++)
				{
					int frameX = (ii % 4);
					int frameY = (ii - frameX) / 4;
					_invaders[ii] = invaders.GetPart(xx + (frameX * (ww + 1)), yy + (frameY * (hh + 1)), ww, hh);
				}
				_x = 0;
				
				string[] lines =  new [] { $"{Game.CurrentPlayer.TribeNamePlural} capture", $"{city.Name}. 0 gold", "pieces plundered." };
				int width = lines.Max(l => Resources.Instance.GetTextSize(5, l).Width) + 10;
				if (width % 4 > 0) width += (4 - (width % 4));
				Picture dialog = new Picture(width, 52);
				dialog.FillLayerTile(Resources.Instance.GetPart("SP299", 288, 120, 32, 16));
				dialog.AddBorder(15, 8, 0, 0, width, 52);
				dialog.DrawText(lines[0], 5, 5, 4, 5);
				dialog.DrawText(lines[0], 5, 15, 4, 4);
				dialog.DrawText(lines[1], 5, 5, 4, 20);
				dialog.DrawText(lines[1], 5, 15, 4, 19);
				dialog.DrawText(lines[2], 5, 5, 4, 35);
				dialog.DrawText(lines[2], 5, 15, 4, 34);

				_background.FillRectangle(5, 80, 8, width + 2, 54);
				_background.AddLayer(dialog, 81, 9);
			}

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
				Picture dialog = new Picture(width, 37);
				dialog.FillLayerTile(Resources.Instance.GetPart("SP299", 288, 120, 32, 16));
				dialog.AddBorder(15, 8, 0, 0, width, 37);
				dialog.DrawText(lines[0], 5, 5, 4, 5);
				dialog.DrawText(lines[0], 5, 15, 4, 4);
				dialog.DrawText(lines[1], 5, 5, 4, 20);
				dialog.DrawText(lines[1], 5, 15, 4, 19);

				foreach (Picture picture in new[] { _background, _overlay })
				{
					picture.FillRectangle(5, 80, 8, width + 2, 39);
					picture.AddLayer(dialog, 81, 9);
				}
				return;
			}

			if (captured) return;
			
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