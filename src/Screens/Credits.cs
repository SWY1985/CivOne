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
using CivOne.Graphics;
using CivOne.IO;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	[Expand]
	internal class Credits : BaseScreen
	{
		private const int NOISE_COUNT = 20;
		
		private readonly int[] SHOW_INTRO_LINE = new[] { 312, 279, 254, 221, 196, 171, 146, 121, 96, 71, 46, 21, -4, -37, -62, -95, -120, -145, -170, -195, -220, -245, -270, -295 };
		private readonly int[] HIDE_INTRO_LINE = new[] { 287, 229, -29, -87, -315 };
		
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
				Log(@"Credits: ""{0}""", _introText[_introLine]);
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
		
		protected override bool HasUpdate(uint gameTick)
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

			if (_done && (_overlay == null || !_overlay.Update(gameTick))) return false;	
			
			// Updates
			if (_introLeft > -320)
			{
				HandleIntroText();
			}
			else if (_logoSwipe < 320)
			{
				this.Cycle(224, 254);
				_logoSwipe += 16;
			}
			else if (_cycleCounter < 98)
			{
				this.Cycle(224, 254);
				_cycleCounter++;
			}
			else if (_noiseCounter > 0)
			{
				_pictures[0].ApplyNoise(_noiseMap, --_noiseCounter);
				_pictures[1].ApplyNoise(_noiseMap, --_noiseCounter);
			}
			
			if (_noiseCounter == 0 && HasMenu && !Common.HasScreenType<Menu>() && (_overlay == null || LoadGameCancel))
			{
				CreateMenu();
			}
			
			// Drawing
			int ox = (Width - 320), cx = (ox / 2), cy = (Height - 200) / 2;
			this.Clear();
			if (_introLeft > -320)
			{
				this.AddLayer(_pictures[0], _introLeft + ox, cy)
					.AddLayer(_pictures[1], _introLeft + ox + 320, cy);
			}
			if (_introLeft > -320 && _showIntroLine)
			{
				this.DrawText(_introText[_introLine], (Width / 2), (Height / 2) - 16);
			}
			if (_introLeft == -320 && _noiseCounter > 0)
			{
				if (!_introSkipped)
				{
					this.AddLayer(_pictures[0], ox - 320, cy)
						.AddLayer(_pictures[1], ox, cy);
				}
				if (_logoSwipe < 320)
				{
					if (_logoSwipe > 0)
					{
						this.AddLayer(_pictures[2].GetPart(0, 0, _logoSwipe, 200), cx, cy);
					}
				}
				else
				{
					this.AddLayer(_pictures[2], cx, cy);	
				}
				if (_introSkipped)
				{
					this.AddLayer(_pictures[0], ox - 320, cy)
						.AddLayer(_pictures[1], ox, cy);
				}
			}
			else if (_noiseCounter == 0)
			{
				this.AddLayer(_pictures[2], cx, cy);
				this.ResetPalette();
				_done = true;
				
				if (_overlay != null)
				{
					this.AddLayer(_overlay);
					if (_overlay.GetType() == typeof(LoadGame) && ((LoadGame)_overlay).Cancel)
					{
						CreateMenu();
					}
					if (!HasMenu) return true;
				}
				
				// Draw menu background
				int mx = ((Width - 120) / 2), my = Height - 59;
				this.FillRectangle(mx, my, 122, 49, 5)
					.FillRectangle(mx + 1, my + 1, 120, 47, _menuColours[0])
					.FillRectangle(mx + 1, my + 2, 119, 46, _menuColours[1])
					.FillRectangle(mx + 2, my + 2, 118, 45, _menuColours[2]);
				
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
			_pictures[0].Clear(5);
			_pictures[1].Clear(5);
			_introSkipped = true;
			return true;
		}
		
		private void CreateMenu()
		{
			if (HasMenu) return;
			Menu menu = new Menu("MainMenu", Palette)
			{
				X = ((Width - 120) / 2) + 3,
				Y = Height - 55,
				MenuWidth = 116,
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
			foreach (IScreen menu in _menus)
				this.AddLayer(menu);
			CloseMenus();
			if (!Runtime.Settings.ShowIntro)
			{
				_nextScreen = new NewGame();
			}
			else
			{
				_nextScreen = new Intro();
			}
		}
		
		private void StartNewGame(object sender, EventArgs args)
		{
			Log("Main Menu: Start a New Game");
			Map.Generate();
			StartIntro();
		}
		
		private void LoadSavedGame(object sender, EventArgs args)
		{
			_overlay = null;
			Log("Main Menu: Load a Saved Game");
			CloseMenus();
			
			_overlay = new LoadGame(this.Palette);
		}
		
		private void Earth(object sender, EventArgs args)
		{
			Log("Main Menu: EARTH");
			Map.LoadMap();
			StartIntro();
		}
		
		private void CustomizeWorld(object sender, EventArgs args)
		{
			Log("Main Menu: Customize World");
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

		private void Resize(object sender, ResizeEventArgs args)
		{
			_done = false;
			foreach (Menu menu in Common.Screens.Where(x => x is Menu && (x as Menu).Id == "MainMenu"))
			{
				menu.X = ((Width - 120) / 2) + 3;
				menu.Y = Height - 55;
			}
		}
		
		public Credits()
		{
			OnResize += Resize;

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
					DefaultTextSettings = TextSettings.ThreeLayers(244, 248, 242);
					break;
				case GraphicsMode.Graphics16:
					DefaultTextSettings = TextSettings.ThreeLayers(15, 15, 7);
					break;
			}
			DefaultTextSettings.Alignment = TextAlign.Center;
			DefaultTextSettings.FontId = 4;
			
			_menuColours = new byte[] { 8, 15, 7 };
			
			Palette = _pictures[2].Palette;

			Runtime.PlaySound("opening");

			if (!Runtime.Settings.ShowCredits) SkipIntro();
		}
	}
}