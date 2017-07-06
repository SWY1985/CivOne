// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.IO;
using CivOne.Templates;

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
			Picture picture = new Picture(320, 200);
			picture.AddLayer(_background);
			switch (Human.Palace.GetGardenLevel(1))
			{
				case 1: picture.AddLayer(Resources.Instance["CBACKS1"], 0, 135); break;
				case 2: picture.AddLayer(Resources.Instance["CBACKS2"], 0, 135); break;
				case 3: picture.AddLayer(Resources.Instance["CBACKS3"], 0, 135); break;
			}

			picture.AddLayer(Resources.Instance["CASTLE0"].GetPart(0, 1, 52, 99), 135, 37);
			picture.AddLayer(Resources.Instance["CASTLE0"].GetPart(53, 1, 26, 99), 185, 37);
			picture.AddLayer(Resources.Instance["CASTLE0"].GetPart(78, 1, 24, 99), 114, 37);
			
			switch (Human.Palace.GetGardenLevel(0))
			{
				case 1: picture.AddLayer(Resources.Instance["CBRUSH0"], 0, 105); break;
				case 2: picture.AddLayer(Resources.Instance["CBRUSH2"], 0, 94); break;
				case 3: picture.AddLayer(Resources.Instance["CBRUSH4"], 0, 94); break;
			}
			switch (Human.Palace.GetGardenLevel(2))
			{
				case 1: picture.AddLayer(Resources.Instance["CBRUSH1"], 184, 105); break;
				case 2: picture.AddLayer(Resources.Instance["CBRUSH3"], 184, 94); break;
				case 3: picture.AddLayer(Resources.Instance["CBRUSH5"], 184, 94); break;
			}
			return picture;
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				AddLayer(DrawPalace());

				switch (_currentStage)
				{
					case Stage.Message:
						{
							Picture message = new Picture(269, 39);
							message.FillLayerTile(Patterns.PanelGrey);
							message.AddBorder(15, 8, 0, 0, 269, 39);
							int yy = 4;
							foreach (string line in TextFile.Instance.GetGameText("KING/PALACE"))
							{
								message.DrawText(line.Trim('^'), 0, 15, 4, yy);
								yy += 8;
							}
							_canvas.FillRectangle(5, 20, 16, 271, 41);
							AddLayer(message, 21, 17);
						}
						break;
					case Stage.SelectPart:
						{
							Picture message = new Picture(180, 15);
							message.FillLayerTile(Patterns.PanelGrey);
							message.AddBorder(15, 8, 0, 0, 180, 15);
							message.DrawText("Which section shall we improve?", 0, 15, 4, 4);
							_canvas.FillRectangle(5, 40, 16, 182, 17);
							AddLayer(message, 41, 17);

							for (int i = 0; i < 7; i++)
							{
								int xx = 12 + (48 * i);
								_canvas.DrawText($"{i + 1}", 0, 5, xx, 145);
								_canvas.DrawText($"{i + 1}", 0, 14, xx, 144);
							}
							for (int i = 0; i < 3; i++)
							{
								if (Human.Palace.GetGardenLevel((byte)i) >= 3) continue;

								int xx = 40 + (120 * i);
								_canvas.DrawText($"{(char)('A' + i)}", 0, 5, xx, 161);
								_canvas.DrawText($"{(char)('A' + i)}", 0, 14, xx, 160);
							}
						}
						break;
					case Stage.Morph:
						if (_noiseCounter > 0)
						{
							_palaceMorph.ApplyNoise(_noiseMap, _noiseCounter--);
							AddLayer(DrawPalace());
							AddLayer(_palaceMorph);
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
							case 'A': morph = true; Human.Palace.SetGarden(0, (byte)(Human.Palace.GetGardenLevel(0) + 1)); break;
							case 'B': morph = true; Human.Palace.SetGarden(1, (byte)(Human.Palace.GetGardenLevel(1) + 1)); break;
							case 'C': morph = true; Human.Palace.SetGarden(2, (byte)(Human.Palace.GetGardenLevel(2) + 1)); break;
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
			
			_background = Resources.Instance.LoadPIC("CBACK");
			
			_canvas = new Picture(320, 200, _background.Palette);
			if (build) _currentStage = Stage.Message;
		}
	}
}