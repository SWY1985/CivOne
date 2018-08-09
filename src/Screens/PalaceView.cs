// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Graphics.Sprites;
using CivOne.IO;

namespace CivOne.Screens
{
	internal class PalaceView : BaseScreen
	{
		private enum Stage
		{
			View,
			Message,
			SelectPart,
			SelectStyle,
			Morph
		}

		private const int NOISE_COUNT = 40;

		private readonly Picture _background;
		private readonly byte[,] _noiseMap;

		private Picture _palaceMorph = null;
		private int _noiseCounter = NOISE_COUNT + 5;

		private Stage _currentStage = Stage.View;

		private bool _update = true;

		private Picture DrawPalace()
		{
			PalaceData palace = Human.Palace;
			Picture picture = new Picture(320, 200);
			picture.AddLayer(_background);
			switch (palace.GetGardenLevel(1))
			{
				case 1: picture.AddLayer(Resources["CBACKS1"], 0, 135); break;
				case 2: picture.AddLayer(Resources["CBACKS2"], 0, 135); break;
				case 3: picture.AddLayer(Resources["CBACKS3"], 0, 135); break;
			}

			for (int i = palace.PalaceLeft; i <= palace.PalaceRight; i++)
			{
				if (i == 3) continue;

				byte level = palace.GetPalaceLevel(i);
				PalaceStyle style = palace.GetPalaceStyle(i);
				PalacePart part = PalacePart.None;

				if (level == 0 && (i < 2 || i > 4)) continue;

				int xx = 17;
				switch (i)
				{
					case 1:
					case 2: xx = 17 + (48 * i) - 33; break;
					//case 3: i = 17; break;
					case 4:
					case 5:
					case 6: xx = 185 + ((i - 4) * 48); break;
				}
				
				switch (i)
				{
					case 0:
						xx = 9;
						part = PalacePart.LeftTower;
						break;
					case 1:
					case 2:
						xx = 17 + (48 * i);
						if (palace.GetPalaceLevel(i - 1) > 0)
						{
							part = PalacePart.Wall;
							xx -= 24;
							break;
						}
						part = PalacePart.LeftTowerWall;
						xx -= 33;
						break;
					case 4:
						xx = 185 + ((i - 4) * 48);
						if (palace.GetPalaceLevel(i + 1) > 0)
						{
							part = PalacePart.WallShadow;
							break;
						}
						part = PalacePart.RightTowerWallShadow;
						break;
					case 5:
						xx = 185 + ((i - 4) * 48);
						if (palace.GetPalaceLevel(i + 1) > 0)
						{
							part = PalacePart.Wall;
							break;
						}
						part = PalacePart.RightTowerWall;
						break;
					case 6:
						xx = 185 + ((i - 4) * 48) - 3;
						part = PalacePart.RightTower;
						break;
				}

				picture.AddLayer(Resources.GetPalace(palace.GetPalaceStyle(i), part, palace.GetPalaceLevel(i)), xx, 37);
			}

			// Draw palace middle
			picture.AddLayer(Resources.GetPalace(palace.GetPalaceStyle(3), PalacePart.Center, palace.GetPalaceLevel(3)), 135, palace.GetPalaceLevel(3) == 0 ? 37 : 38);
			
			switch (palace.GetGardenLevel(0))
			{
				case 1: picture.AddLayer(Resources["CBRUSH0"], 0, 105); break;
				case 2: picture.AddLayer(Resources["CBRUSH2"], 0, 94); break;
				case 3: picture.AddLayer(Resources["CBRUSH4"], 0, 94); break;
			}
			switch (palace.GetGardenLevel(2))
			{
				case 1: picture.AddLayer(Resources["CBRUSH1"], 184, 105); break;
				case 2: picture.AddLayer(Resources["CBRUSH3"], 184, 94); break;
				case 3: picture.AddLayer(Resources["CBRUSH5"], 184, 94); break;
			}
			return picture;
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				this.AddLayer(DrawPalace());

				switch (_currentStage)
				{
					case Stage.Message:
						{
							Picture message = new Picture(269, 39)
								.Tile(Pattern.PanelGrey)
								.DrawRectangle3D()
								.As<Picture>();
							int yy = 4;
							foreach (string line in TextFile.Instance.GetGameText("KING/PALACE"))
							{
								message.DrawText(line.Trim('^'), 0, 15, 4, yy);
								yy += 8;
							}
							this.FillRectangle(20, 16, 271, 41, 5)
								.AddLayer(message, 21, 17);
						}
						break;
					case Stage.SelectPart:
						{
							Picture message = new Picture(180, 15)
								.Tile(Pattern.PanelGrey)
								.DrawRectangle3D()
								.DrawText("Which section shall we improve?", 0, 15, 4, 4)
								.As<Picture>();
							this.FillRectangle(40, 16, 182, 17, 5)
								.AddLayer(message, 41, 17);

							for (int i = 0; i < 7; i++)
							{
								if (Human.Palace.GetPalaceLevel(i) >= 4) continue;

								int xx = 12 + (48 * i);
								this.DrawText($"{i + 1}", 0, 5, xx, 145)
									.DrawText($"{i + 1}", 0, 14, xx, 144);
							}
							for (int i = 0; i < 3; i++)
							{
								if (Human.Palace.GetGardenLevel(i) >= 3) continue;

								int xx = 40 + (120 * i);
								this.DrawText($"{(char)('A' + i)}", 0, 5, xx, 161)
									.DrawText($"{(char)('A' + i)}", 0, 14, xx, 160);
							}
						}
						break;
					case Stage.Morph:
						if (_noiseCounter > 0)
						{
							_palaceMorph.ApplyNoise(_noiseMap, _noiseCounter--);
							this.AddLayer(DrawPalace())
								.AddLayer(_palaceMorph);
							return true;
						}
						_currentStage = Stage.View;
						return true;
					case Stage.SelectStyle:
						break;
				}

				_update = false;
				return true;
			}
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			PalaceData palace = Human.Palace;

			switch (_currentStage)
			{
				case Stage.Message:
					_currentStage = Stage.SelectPart;
					_update = true;
					break;
				case Stage.SelectPart:
					bool morph = false;
					_palaceMorph = DrawPalace();
					
					try
					{
						switch (args.KeyChar)
						{
							case 'A': 
							case 'B': 
							case 'C':
								{
									int index = (int)(args.KeyChar - 'A');
									byte level = (byte)(palace.GetGardenLevel(index) + 1);
									if (level > 2) break;
									morph = true; palace.SetGarden(index, (byte)(Human.Palace.GetGardenLevel(2) + 1));
								}
								break;
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
								{
									int index = (int)(args.KeyChar - '1');
									byte level = (byte)(palace.GetPalaceLevel(index) + 1);
									if (level > 4) break;
									morph = true; palace.SetPalace(index, 1, level);
								}
								break;
						};
					}
					catch
					{
						// TODO: Check for valid choice before handling keypress
						_currentStage = Stage.View;
						_update = true;
						break;
					}
					if (morph)
					{
						_update = true;
						_currentStage = Stage.Morph;
						break;
					}
					_palaceMorph = null;
					break;
				case Stage.View:
					Destroy();
					break;
			}
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			switch (_currentStage)
			{
				case Stage.Message:
					_currentStage = Stage.SelectPart;
					_update = true;
					break;
				case Stage.SelectPart:
					// _currentStage = Stage.View;
					// _update = true;
					break;
				case Stage.View:
					Destroy();
					break;
			}
			return true;
		}
		
		public PalaceView(bool build = false)
		{
			_noiseMap = new byte[320, 200];
			for (int x = 0; x < 320; x++)
			for (int y = 0; y < 200; y++)
			{
				_noiseMap[x, y] = (byte)Common.Random.Next(1, NOISE_COUNT);
			}
			
			_background = Resources["CBACK"];
			Palette = _background.Palette;
			if (build) _currentStage = Stage.Message;
		}
	}
}