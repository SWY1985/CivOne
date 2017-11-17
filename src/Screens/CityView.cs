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
using CivOne.Graphics;
using CivOne.Graphics.Sprites;
using CivOne.Wonders;

using UniversityBuilding = CivOne.Buildings.University;

namespace CivOne.Screens
{
	[Modal]
	internal class CityView : BaseScreen
	{
		private const float FADE_STEP = 0.1f;
		private const int NOISE_COUNT = 40;

		private readonly TextSettings _dialogText;

		private readonly City _city;
		private readonly IProduction _production;
		private readonly Picture _background;
		private readonly bool _founded;
		private readonly bool _firstView;
		private readonly bool _captured;
		private readonly byte[,] _noiseMap;
		
		private int _noiseCounter = NOISE_COUNT + 15;

		private int _houseType = 0;

		private readonly Picture _overlay;
		private readonly Picture[] _invaders;

		private bool _update = true;
		
		private int _x = 80, _y = 138;
		private float _fadeStep = 1.0f;
		private bool _skip = false;

		private string _buildingFile = null;

		public event EventHandler Skipped;
		
		private Colour FadeColour(Colour colour1, Colour colour2)
		{
			int r = (int)(((float)colour1.R * (1.0F - _fadeStep)) + ((float)colour2.R * _fadeStep));
			int g = (int)(((float)colour1.G * (1.0F - _fadeStep)) + ((float)colour2.G * _fadeStep));
			int b = (int)(((float)colour1.B * (1.0F - _fadeStep)) + ((float)colour2.B * _fadeStep));
			return new Colour(r, g, b);
		}
		
		private void FadeColours()
		{
			if (Settings.GraphicsMode != GraphicsMode.Graphics256) return;
			
			Palette palette = _background.Palette;
			for (int i = 1; i < 256; i++)
				palette[i] = FadeColour(new Colour(0, 0, 0), _background.OriginalColours[i]);
			this.SetPalette(palette);
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (gameTick % 4 == 0)
			{
				this.Cycle(64, 79);
				_update = true;
			}

			if (_captured)
			{
				this.AddLayer(_background);
				int frame = (_x % 30) / 3;
				for (int i = 7; i >= 0; i--)
				{
					int xx = (_x - 65) - (48 * i);
					if (xx + 78 <= 0) continue;
					this.AddLayer(_invaders[frame], xx, _y);
				}
				_x++;
				return true;
			}

			if (_noiseMap != null)
			{
				if (_noiseCounter > 0)
				{
					_overlay.ApplyNoise(_noiseMap, _noiseCounter--);
					this.AddLayer(_background)
						.AddLayer(_overlay);
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
				this.AddLayer(_background)
					.DrawText($"{_city.Name} founded: {Game.GameYear}.", 5, 5, 161, 3, TextAlign.Center);

				int frame = (_x % 4);
				this.AddLayer(Resources["SETTLERS"][1, 1 + (16 * frame), 48, 15], _x, 120)
					.AddLayer(Resources["SETTLERS"][1, 1 + (16 * ((frame + 2) % 4)), 48, 15], _x + 27, 125)
					.AddLayer(Resources["SETTLERS"][1, 1 + (16 * ((frame + 3) % 4)), 48, 15], _x + 14, 131)
					.AddLayer(Resources["SETTLERS"][1, 1 + (16 * ((frame + 1) % 4)), 48, 15], _x + 40, 135);

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
			if (_fadeStep != 0.0F && _fadeStep != 1.0F) return false;
			if (_noiseCounter > 0 && _noiseCounter < NOISE_COUNT) return false;

			Destroy();
			if (Skipped != null)
				Skipped(this, null);
			else
				HandleClose();
			return true;
		}
		
		private void KeyDown(object sender, KeyboardEventArgs args)
		{
			args.Handled = SkipAction();
		}
		
		private void MouseDown(object sender, ScreenEventArgs args)
		{
			args.Handled = SkipAction();
		}

		private void DrawWonder<T>(Picture picture = null, int x = -1, int y = -1) where T : IWonder
		{
			if (picture == null) picture = _background;
			if (typeof(T) == typeof(Pyramids))
			{
				picture.AddLayer(Resources["WONDERS2"][131, 54, 187, 29], 133, 0);
				picture.AddLayer(Resources["WONDERS2"][318, 54, 1, 29], 0, 0);
			}
			if (typeof(T) == typeof(Colossus))
			{
				picture.AddLayer(Resources["WONDERS2"][88, 97, 124, 39], 170, 0);
			}
			if (typeof(T) == typeof(GreatWall))
			{
				picture.AddLayer(Resources["WONDERS2"][1, 38, 66, 81], 0, 0);
			}
			if (typeof(T) == typeof(HooverDam))
			{
				picture.AddLayer(Resources["WONDERS2"][1, 14, 147, 20], 1, 9);
			}
			if (typeof(T) == typeof(Lighthouse))
			{
				picture.AddLayer(Resources["WONDERS"][229, 116, 40, 83], x, y);
			}
			if (typeof(T) == typeof(HangingGardens))
			{
				picture.AddLayer(Resources["WONDERS"][159, 149, 69, 50], x, y);
			}
			if (typeof(T) == typeof(Oracle))
			{
				picture.AddLayer(Resources["WONDERS"][164, 97, 64, 51], x, y);
			}
			if (typeof(T) == typeof(DarwinsVoyage))
			{
				picture.AddLayer(Resources["WONDERS"][40, 69, 62, 47], x, y);
			}
		}

		private void DrawWonderOverlay<T>(int x, int y, int offset) where T : IWonder
		{
			DrawWonder<T>(x: x, y: y + offset);
			if (!(_production is T))
				DrawWonder<T>(_overlay, x, y + offset);
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
				picture.AddLayer(Resources[_buildingFile][51, 151, 49, 49], 0, 72);
			}

			if (typeof(T) == typeof(CityWalls))
			{
				Picture wall = Resources[_buildingFile][251, 101, 43, 49];
				Picture door = Resources[_buildingFile][51, 101, 49, 49];

				for (int xx = 0; xx < 142; xx += 43)
					picture.AddLayer(wall, xx, 108);
				picture.AddLayer(door, 142, 108);
				for (int xx = 191; xx < 320; xx += 43)
					picture.AddLayer(wall, xx, 108);
			}

			if (typeof(T) == typeof(Barracks))
				picture.AddLayer(Resources[_buildingFile][1, 1, 49, 49], x, y);
			if (typeof(T) == typeof(Granary))
				picture.AddLayer(Resources[_buildingFile][1, 51, 49, 49], x, y);
			if (typeof(T) == typeof(Temple))
				picture.AddLayer(Resources[_buildingFile][1, 101, 49, 49], x, y);
			if (typeof(T) == typeof(MarketPlace))
				picture.AddLayer(Resources[_buildingFile][1, 151, 49, 49], x, y);
			if (typeof(T) == typeof(Library))
				picture.AddLayer(Resources[_buildingFile][51, 1, 49, 49], x, y);
			if (typeof(T) == typeof(Courthouse))
				picture.AddLayer(Resources[_buildingFile][51, 51, 49, 49], x, y);
			if (typeof(T) == typeof(Bank))
				picture.AddLayer(Resources[_buildingFile][101, 1, 49, 49], x, y);
			if (typeof(T) == typeof(Cathedral))
				picture.AddLayer(Resources[_buildingFile][101, 51, 49, 49], x, y);
			if (typeof(T) == typeof(UniversityBuilding))
				picture.AddLayer(Resources[_buildingFile][101, 101, 49, 49], x, y);
			if (typeof(T) == typeof(Colosseum))
				picture.AddLayer(Resources[_buildingFile][151, 1, 49, 49], x, y);
			if (typeof(T) == typeof(Factory))
				picture.AddLayer(Resources[_buildingFile][151, 51, 49, 49], x, y);
			if (typeof(T) == typeof(MfgPlant))
				picture.AddLayer(Resources[_buildingFile][151, 101, 49, 49], x, y);
			if (typeof(T) == typeof(SdiDefense))
				picture.AddLayer(Resources[_buildingFile][151, 151, 49, 49], x, y);
			if (typeof(T) == typeof(RecyclingCenter))
				picture.AddLayer(Resources[_buildingFile][201, 1, 49, 49], x, y);
			if (typeof(T) == typeof(NuclearPlant))
				picture.AddLayer(Resources[_buildingFile][201, 151, 49, 49], x, y);
		}

		private void DrawBuildingOverlay<T>(int x, int y, int offset = -18) where T : IBuilding
		{
			DrawBuilding<T>(x: x, y: y + offset);
			if (!(_production is T))
				DrawBuilding<T>(_overlay, x, y + offset);
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

				
				foreach (Type type in new Type[] { typeof(Barracks), typeof(Granary), typeof(Temple), typeof(MarketPlace), typeof(Library), typeof(Courthouse), typeof(Bank), typeof(Cathedral), typeof(UniversityBuilding), typeof(Colosseum), typeof(Factory), typeof(MfgPlant), typeof(SdiDefense), typeof(RecyclingCenter), typeof(NuclearPlant), typeof(Lighthouse), typeof(HangingGardens), typeof(Oracle), typeof(DarwinsVoyage) })
				{
					if (_city.HasBuilding(type) || _city.HasWonder(type))
					{
						int sizeX = 2, sizeY = 2;

						CityViewMap id;
						if (type == typeof(Barracks)) id = CityViewMap.Barracks;
						else if (type == typeof(Granary)) id = CityViewMap.Granary;
						else if (type == typeof(Temple)) id = CityViewMap.Temple;
						else if (type == typeof(MarketPlace)) id = CityViewMap.MarketPlace;
						else if (type == typeof(Library)) id = CityViewMap.Library;
						else if (type == typeof(Courthouse)) id = CityViewMap.Courthouse;
						else if (type == typeof(Bank)) id = CityViewMap.Bank;
						else if (type == typeof(Cathedral)) id = CityViewMap.Cathedral;
						else if (type == typeof(UniversityBuilding)) id = CityViewMap.University;
						else if (type == typeof(Colosseum)) id = CityViewMap.Colosseum;
						else if (type == typeof(Factory)) id = CityViewMap.Factory;
						else if (type == typeof(MfgPlant)) id = CityViewMap.MfgPlant;
						else if (type == typeof(SdiDefense)) id = CityViewMap.SdiDefense;
						else if (type == typeof(RecyclingCenter)) id = CityViewMap.RecyclingCenter;
						else if (type == typeof(NuclearPlant)) id = CityViewMap.NuclearPlant;
						else if (type == typeof(Lighthouse)) id = CityViewMap.Lighthouse;
						else if (type == typeof(HangingGardens)) { id = CityViewMap.HangingGardens; sizeX = 3; sizeY = 3; }
						else if (type == typeof(Oracle)) { id = CityViewMap.Oracle; sizeX = 3; sizeY = 3; }
						else if (type == typeof(DarwinsVoyage)) { id = CityViewMap.DarwinsVoyage; sizeX = 3; sizeY = 3; }
						else continue;

						for (int i = 0; i < 1000; i++)
						{
							int xx = Common.Random.Next(15) + 1;
							int yy = Common.Random.Next(10);
							if (xx == 6 || xx == 11 || yy == 2 || yy == 6) continue;
							if (xx == 5 || xx == 10 || yy == 1 || yy == 5) continue;
							if (xx + sizeX > cityMap.GetLength(0) || yy + sizeY > cityMap.GetLength(1)) continue;
							if ((int)cityMap[xx, yy] > 3) continue;
							bool invalid = false;
							for (int oy = 0; oy < sizeY; oy++)
							for (int ox = 0; ox < sizeX; ox++)
							{
								if ((int)cityMap[xx + ox, yy + oy] <= 3) continue;
								invalid = true;
								break; 
							}
							if (invalid) continue;

							cityMap[xx, yy] = id;
							for (int oy = 0; oy < sizeY; oy++)
							for (int ox = 0; ox < sizeX; ox++)
							{
								if (ox == 0 && oy == 0) continue;
								cityMap[xx + ox, yy + oy] = CityViewMap.Occupied;
							}
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
			if (_city.Wonders.Any(b => b is HooverDam))
			{
				DrawWonder<HooverDam>();
				if (!(_production is HooverDam))
					DrawWonder<HooverDam>(_overlay);
			}

			if (_city.Buildings.Any(b => b is Aqueduct))
			{
				DrawBuilding<Aqueduct>();
				if (!(_production is Aqueduct))
					DrawBuilding<Aqueduct>(_overlay);
			}

			int stage = (int)Math.Floor((double)(Game.GetPlayer(_city.Owner).Advances.Count() - 9) / 2);
			for (int xx = 0; xx < 18; xx++)
			for (int yy = 10; yy >= 0; yy--)
			{
				int dx = 0 + (16 * xx) + (yy * 8);
				int dy = 106 - (yy * 8);
				Picture building;
				switch (cityMap[xx, yy])
				{
					case CityViewMap.House:
						int centerDistance = Math.Max(Math.Abs(9 - xx), yy);
						if (stage >= 20)
						{
							if (_city.Size > 8 && Common.Random.Next((_city.Size - 7) * 2) > centerDistance)
							{
								if (Common.Random.Next(10) > 5)
								{
									building = Resources["CITYPIX1"][1 + (32 * 8), (Common.Random.Next(10) > 5) ? 1 : 33, 31, 31];
								}
								else
								{
									building = Resources["CITYPIX1"][1 + (32 * 9), (Common.Random.Next(10) > 5) ? 1 : 33, 31, 31];
								}
							}
							else
							{
								if (Common.Random.Next(10) > 5)
								{
									building = Resources["CITYPIX1"][1 + (32 * 6), 33, 31, 31];
								}
								else
								{
									building = Resources["CITYPIX1"][1 + (32 * 7), 33, 31, 31];
								}
							}
						}
						else if (stage >= 16)
						{
							if (Common.Random.Next(stage - 16) > centerDistance)
							{
								if (Common.Random.Next(10) > 5)
								{
									building = Resources["CITYPIX1"][1 + (32 * 6), 1, 31, 31];
								}
								else
								{
									building = Resources["CITYPIX1"][1 + (32 * 7), 1, 31, 31];
								}
							}
							else
							{
								if (Common.Random.Next(10) > 5)
								{
									building = Resources["CITYPIX1"][1 + (32 * 4), 33, 31, 31];
								}
								else
								{
									building = Resources["CITYPIX1"][1 + (32 * 5), 33, 31, 31];
								}
							}
						}
						else if (stage >= 7)
						{
							if (Common.Random.Next(stage - 7) > centerDistance)
							{
								if (Common.Random.Next(10) > 5)
								{
									building = Resources["CITYPIX1"][1 + (32 * 4), 1, 31, 31];
								}
								else
								{
									building = Resources["CITYPIX1"][1 + (32 * 5), 1, 31, 31];
								}
							}
							else
							{
								if (Common.Random.Next(10) > 5)
								{
									building = Resources["CITYPIX1"][1 + (32 * 2), 33, 31, 31];
								}
								else
								{
									building = Resources["CITYPIX1"][1 + (32 * 3), 33, 31, 31];
								}
							}
						}
						else if (stage >= 1)
						{
							if (Common.Random.Next(stage) > centerDistance)
							{
								if (Common.Random.Next(10) > 5)
								{
									if (Common.Random.Next((stage - 5) * 4) > centerDistance)
									{
										building = Resources["CITYPIX1"][1 + (32 * 2), 33, 31, 31];
									}
									else
									{
										building = Resources["CITYPIX1"][1 + (32 * 2), 1, 31, 31];
									}
								}
								else
								{
									if (Common.Random.Next((stage - 5) * 4) > centerDistance)
									{
										building = Resources["CITYPIX1"][1 + (32 * 3), 33, 31, 31];
									}
									else
									{
										building = Resources["CITYPIX1"][1 + (32 * 3), 1, 31, 31];
									}
								}
							}
							else
							{
								building = Resources["CITYPIX1"][1 + (32 * _houseType), 33, 31, 31];
							}
						}
						else
						{
							if (Common.Random.Next((-3 - stage)) > centerDistance)
							{
								building = Resources["CITYPIX1"][1 + (32 * _houseType), 33, 31, 31];
							}
							else
							{
								building = Resources["CITYPIX1"][1 + (32 * _houseType), 1, 31, 31];
							}
						}
						break;
					case CityViewMap.Tree:
						building = Resources["CITYPIX1"][0, 65, 24, 8];
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
						building = Resources["CITYPIX1"][sx, sy, 24, 8];
						dx -= 5;
						dy += 24;
						break;
					case CityViewMap.Barracks:
						DrawBuildingOverlay<Barracks>(dx, dy);
						continue;
					case CityViewMap.Granary:
						DrawBuildingOverlay<Granary>(dx, dy);
						continue;
					case CityViewMap.Temple:
						DrawBuildingOverlay<Temple>(dx, dy);
						continue;
					case CityViewMap.MarketPlace:
						DrawBuildingOverlay<MarketPlace>(dx, dy);
						continue;
					case CityViewMap.Library:
						DrawBuildingOverlay<Library>(dx, dy);
						continue;
					case CityViewMap.Courthouse:
						DrawBuildingOverlay<Courthouse>(dx, dy);
						continue;
					case CityViewMap.Bank:
						DrawBuildingOverlay<Bank>(dx, dy);
						continue;
					case CityViewMap.Cathedral:
						DrawBuildingOverlay<Cathedral>(dx, dy);
						continue;
					case CityViewMap.University:
						DrawBuildingOverlay<UniversityBuilding>(dx, dy);
						continue;
					case CityViewMap.Colosseum:
						DrawBuildingOverlay<Colosseum>(dx, dy);
						continue;
					case CityViewMap.Factory:
						DrawBuildingOverlay<Factory>(dx, dy);
						continue;
					case CityViewMap.MfgPlant:
						DrawBuildingOverlay<MfgPlant>(dx, dy);
						continue;
					case CityViewMap.SdiDefense:
						DrawBuildingOverlay<SdiDefense>(dx, dy);
						continue;
					case CityViewMap.RecyclingCenter:
						DrawBuildingOverlay<RecyclingCenter>(dx, dy);
						continue;
					case CityViewMap.NuclearPlant:
						DrawBuildingOverlay<NuclearPlant>(dx, dy);
						continue;
					case CityViewMap.Lighthouse:
						DrawWonderOverlay<Lighthouse>(dx, dy, -52);
						continue;
					//
					case CityViewMap.HangingGardens:
						DrawWonderOverlay<HangingGardens>(dx, dy, -19);
						continue;
					case CityViewMap.Oracle:
						DrawWonderOverlay<Oracle>(dx, dy, -20);
						continue;
					case CityViewMap.DarwinsVoyage:
						DrawWonderOverlay<DarwinsVoyage>(dx, dy, -16);
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
			_dialogText = TextSettings.ShadowText(15, 5);
			_dialogText.FontId = 5;
			
			_city = city;
			_production = production;
			_background = new Picture(Resources["HILL"]);
			_founded = founded;
			_firstView = firstView;
			
			Palette = _background.Palette;
			_overlay = new Picture(_background);

			OnKeyDown += KeyDown;
			OnMouseDown += MouseDown;
			
			if (founded) return;

			DrawBuildings();
			this.AddLayer(_background);
			
			if (_captured = captured)
			{
				Picture invaders;
				int xx = 0, yy = 2, ww = 78, hh = 60;
				if (Game.CurrentPlayer.HasAdvance<Conscription>())
				{
					invaders = Resources["INVADERS"];
				}
				else if (Game.CurrentPlayer.HasAdvance<Gunpowder>())
				{
					invaders = Resources["INVADER2"];
				}
				else
				{
					invaders = Resources["INVADER3"];
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
					_invaders[ii] = invaders[xx + (frameX * (ww + 1)), yy + (frameY * (hh + 1)), ww, hh];
				}
				_x = 0;

				int totalLuxuries = Game.GetPlayer(_city.Owner).Cities.Sum(x => x.Luxuries);
				int totalGold = Game.GetPlayer(_city.Owner).Gold;
				int cityLuxuries = _city.Luxuries;
				if (cityLuxuries == 0) cityLuxuries = 1;
				int captureGold = (int)Math.Floor(((float)totalGold / totalLuxuries) * cityLuxuries);
				if (captureGold < 0) captureGold = 0;

				Game.GetPlayer(_city.Owner).Gold -= (short)captureGold;
				Game.CurrentPlayer.Gold += (short)captureGold;
				
				string[] lines =  new [] { $"{Game.CurrentPlayer.TribeNamePlural} capture", $"{city.Name}. {captureGold} gold", "pieces plundered." };
				int width = lines.Max(l => Resources.GetTextSize(5, l).Width) + 12;
				Picture dialog = new Picture(width, 54)
					.Tile(Pattern.PanelGrey, 1, 1)
					.DrawRectangle()
					.DrawRectangle3D(1, 1, width - 2, 52)
					.DrawText(lines[0], 5, 6, _dialogText)
					.DrawText(lines[1], 5, 21, _dialogText)
					.DrawText(lines[2], 5, 36, _dialogText)
					.As<Picture>();

				_background.AddLayer(dialog, 80, 8);
			}

			if (production != null)
			{
				_noiseMap = new byte[320, 200];
				for (int x = 0; x < 320; x++)
				for (int y = 0; y < 200; y++)
				{
					_noiseMap[x, y] = (byte)Common.Random.Next(1, NOISE_COUNT);
				}

				string[] lines =  new [] { $"{_city.Name} builds", $"{(production as ICivilopedia).Name}." };
				int width = lines.Max(l => Resources.GetTextSize(5, l).Width) + 12;
				Picture dialog = new Picture(width, 39)
					.Tile(Pattern.PanelGrey, 1, 1)
					.DrawRectangle()
					.DrawRectangle3D(1, 1, width - 2, 37)
					.DrawText(lines[0], 5, 6, _dialogText)
					.DrawText(lines[1], 5, 21, _dialogText)
					.As<Picture>();

				foreach (Picture picture in new[] { _background, _overlay })
				{
					picture.AddLayer(dialog, 80, 10);
				}
				return;
			}

			if (captured) return;
			
			this.DrawText(_city.Name, 5, 5, 161, 3, TextAlign.Center)
				.DrawText(_city.Name, 5, 15, 160, 2, TextAlign.Center)
				.DrawText(Game.GameYear, 5, 5, 161, 16, TextAlign.Center)
				.DrawText(Game.GameYear, 5, 15, 160, 15, TextAlign.Center);
			
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
				this.AddLayer(Resources["POP"][sx, sy, sw, sh], dx, dy);
			}
		}
	}
}