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
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.IO;
using CivOne.Templates;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	internal class Credits : BaseScreen
	{
		private const int NOISE_COUNT = 20;
		
		private readonly int[] SHOW_INTRO_LINE = new[] { 312, 279, 254, 221, 196, 171, 146, 121, 96, 71, 46, 21, -4, -37, -62, -95, -120, -145, -170, -195, -220, -245, -270, -295 };
		private readonly int[] HIDE_INTRO_LINE = new[] { 287, 229, -29, -87, -315 };
		
		private readonly byte[] _textColours;
		private readonly byte[] _menuColours;
		private readonly string[] _introText;
		private readonly Picture[] _pictures;
		private readonly byte[,] _noiseMap;
		
		private int _introLeft = 320;
		private int _logoSwipe = 0;
		private int _cycleCounter = 0;
		private int _noiseCounter = NOISE_COUNT;
		
		private bool _done = false;
		private bool _showIntroLine = false;
		private bool _introSkipped = false;
		private int _introLine = -1;
		
		private IScreen _overlay = null;

		private IScreen _nextScreen = null;
		
		private void HandleIntroText()
		{
			_introLeft--;
			if (SHOW_INTRO_LINE.Contains(_introLeft))
			{
				_showIntroLine = true;
				_introLine++;
				Console.WriteLine(@"Credits: ""{0}""", _introText[_introLine]);
			}
			else if (HIDE_INTRO_LINE.Contains(_introLeft))
			{
				_showIntroLine = false;
				Resources.Instance.ClearTextCache();
			}
		}
		
		private bool LoadGameCancel
		{
			get
			{
				return _overlay != null && (_overlay.GetType() == typeof(LoadGame) && ((LoadGame)_overlay).Cancel);
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_nextScreen != null)
			{
				if (!HandleScreenFadeOut(Speed.Slow))
				{
					Common.AddScreen(_nextScreen);
					Destroy();
					return true;
				}
				return true;
			}

			if (_done && (_overlay == null || !_overlay.HasUpdate(gameTick))) return false;	
			
			// Updates
			if (_introLeft > -320)
			{
				HandleIntroText();
			}
			else if (_logoSwipe < 320)
			{
				_canvas.Cycle(224, 254);
				_logoSwipe += 16;
			}
			else if (_cycleCounter < 98)
			{
				_canvas.Cycle(224, 254);
				_cycleCounter++;
			}
			else if (_noiseCounter > 0)
			{
				_pictures[1].ApplyNoise(_noiseMap, --_noiseCounter);
			}
			
			if (_noiseCounter == 0 && HasMenu && !Common.HasScreenType<Menu>() && (_overlay == null || LoadGameCancel))
			{
				CreateMenu();
			}
			
			// Drawing
			_canvas.FillRectangle(0, 0, 0, 320, 200);
			if (_introLeft < 320 && _introLeft > 0)
			{
				AddLayer(_pictures[0].GetPart(0, 0, 320 - _introLeft, 200), _introLeft);
			}
			if (_introLeft > -320 && _introLeft <= 0)
			{
				AddLayer(_pictures[0].GetPart(-_introLeft, 0, 320 + _introLeft, 200), 0);
				if (_introLeft < 0)
					AddLayer(_pictures[1].GetPart(0, 0, -_introLeft, 200), _introLeft + 320);
			}
			if (_introLeft > -320 && _showIntroLine)
			{
				_canvas.DrawText(_introText[_introLine], 4, _textColours[0], 160, 82, TextAlign.Center);
				_canvas.DrawText(_introText[_introLine], 4, _textColours[1], 160, 84, TextAlign.Center);
				_canvas.DrawText(_introText[_introLine], 4, _textColours[2], 160, 83, TextAlign.Center);
			}
			if (_introLeft == -320 && _noiseCounter > 0)
			{
				if (!_introSkipped) AddLayer(_pictures[1]);
				if (_logoSwipe < 320)
				{
					if (_logoSwipe > 0)
					{
						AddLayer(_pictures[2].GetPart(0, 0, _logoSwipe, 200));
					}
				}
				else
				{
					AddLayer(_pictures[2]);	
				}
				if (_introSkipped) AddLayer(_pictures[1]);
			}
			else if (_noiseCounter == 0)
			{
				AddLayer(_pictures[2]);
				_canvas.ResetPalette();
				_done = true;
				
				if (_overlay != null)
				{
					AddLayer(_overlay);
					if (_overlay.GetType() == typeof(LoadGame) && ((LoadGame)_overlay).Cancel)
					{
						CreateMenu();
					}
					if (!HasMenu) return true;
				}
				
				// Draw menu background
				_canvas.FillRectangle(5, 100, 140, 122, 49);
				_canvas.FillRectangle(_menuColours[0], 101, 141, 120, 47);
				_canvas.FillRectangle(_menuColours[1], 101, 142, 119, 46);
				_canvas.FillRectangle(_menuColours[2], 102, 142, 118, 45);
				
				CreateMenu();
			}
			return true;
		}
		
		public bool SkipIntro()
		{
			if (_introSkipped || _noiseCounter < NOISE_COUNT) return false;
			
			_showIntroLine = false;
			_introLeft = -320;
			_logoSwipe = 320;
			_cycleCounter = 98;
			_pictures[1].FillRectangle(5, 0, 0, 320, 200);
			_introSkipped = true;
			return true;
		}
		
		private void CreateMenu()
		{
			if (HasMenu) return;
			Menu menu = new Menu(Canvas.Palette)
			{
				X = 103,
				Y = 144,
				Width = 116,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 8,
				FontId = 0
			};
			menu.Items.Add("Start a New Game").OnSelect(StartNewGame);
			menu.Items.Add("Load a Saved Game").OnSelect(LoadSavedGame);
			menu.Items.Add("EARTH").OnSelect(Earth);
			menu.Items.Add("Customize World").OnSelect(CustomizeWorld);
			menu.Items.Add("View Hall of Fame").Disable();
			
			AddMenu(menu);
		}

		private void StartIntro()
		{
			Cursor = MouseCursor.None;
			foreach (IScreen menu in _menus)
				AddLayer(menu.Canvas);
			CloseMenus();
			_nextScreen = new Intro();
		}
		
		private void StartNewGame(object sender, EventArgs args)
		{
			
			Console.WriteLine("Main Menu: Start a New Game");
			Map.Generate();
			StartIntro();
		}
		
		private void LoadSavedGame(object sender, EventArgs args)
		{
			_overlay = null;
			Console.WriteLine("Main Menu: Load a Saved Game");
			CloseMenus();
			
			_overlay = new LoadGame(_canvas.Palette);
		}
		
		private void Earth(object sender, EventArgs args)
		{
			Console.WriteLine("Main Menu: EARTH");
			Map.LoadMap();
			StartIntro();
		}
		
		private void CustomizeWorld(object sender, EventArgs args)
		{
			Console.WriteLine("Main Menu: Customize World");
			Destroy();
			Common.AddScreen(new CustomizeWorld());
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (_done && _overlay != null)
				return _overlay.KeyDown(args);
			return SkipIntro();
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (_done && _overlay != null)
				return _overlay.MouseDown(args);
			return SkipIntro();
		}
		
		public override bool MouseUp(ScreenEventArgs args)
		{
			if (_done && _overlay != null)
				return _overlay.MouseUp(args);
			return false;
		}
		
		public override bool MouseDrag(ScreenEventArgs args)
		{
			if (_done && _overlay != null)
				return _overlay.MouseDrag(args);
			return false;
		}
		
		public override MouseCursor Cursor
		{
			get
			{
				if (_overlay != null && !LoadGameCancel)
					return _overlay.Cursor;
				return base.Cursor;
			}
		}
		
		public Credits()
		{
			_introText = TextFile.Instance.LoadArray("CREDITS");
			if (_introText.Length == 0) _introText = new string[25];
			_pictures = new Picture[3];
			for (int i = 0; i < 2; i++)
				_pictures[i] = Resources.Instance.LoadPIC(string.Format("BIRTH{0}", i), true);
			_pictures[2] = Resources.Instance.LoadPIC("LOGO", true);
			_noiseMap = new byte[320, 200];
			for (int x = 0; x < 320; x++)
			for (int y = 0; y < 200; y++)
			{
				_noiseMap[x, y] = (byte)Common.Random.Next(1, _noiseCounter);
			}
			switch (Settings.GraphicsMode)
			{
				case GraphicsMode.Graphics256:
					_textColours = new byte[] { 248, 242, 244 };
					break;
				case GraphicsMode.Graphics16:
					_textColours = new byte[] { 15, 7, 15 };
					break;
			}
			_menuColours = new byte[] { 8, 15, 7 };
			
			_canvas = new Picture(320, 200, _pictures[2].Palette);

			Sound.Play("opening");
		}
	}
}