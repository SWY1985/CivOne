// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.IO;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Credits : BaseScreen
	{
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
		private int _noiseCounter = 33;
		
		private bool _done = false;
		private bool _showIntroLine = false;
		private bool _introSkipped = false;
		private int _introLine = -1;
		
		private void HandleIntroText()
		{
			_introLeft--;
			if (SHOW_INTRO_LINE.Contains(_introLeft))
			{
				_showIntroLine = true;
				_introLine++;
			}
			else if (HIDE_INTRO_LINE.Contains(_introLeft))
			{
				_showIntroLine = false;
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
            if (_done) return false;	
			
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
			
			if (_noiseCounter == 0)
			{				
				Common.AddScreen(Menus[0]);
			}
			
			// Drawing
			_canvas.FillRectangle(0, 0, 0, 320, 200);
            if (_introLeft < 320 && _introLeft > 0)
			{
				_canvas.AddLayer(_pictures[0].GetPart(0, 0, 320 - _introLeft, 200), _introLeft);
			}
			if (_introLeft > -320 && _introLeft <= 0)
			{
				_canvas.AddLayer(_pictures[0].GetPart(-_introLeft, 0, 320 + _introLeft, 200), 0);
				if (_introLeft < 0)
					_canvas.AddLayer(_pictures[1].GetPart(0, 0, -_introLeft, 200), _introLeft + 320);
			}
			if (_introLeft > -320 && _showIntroLine)
			{
				_canvas.DrawText(_introText[_introLine], 4, _textColours[0], 160, 82, TextAlign.Center);
				_canvas.DrawText(_introText[_introLine], 4, _textColours[1], 160, 84, TextAlign.Center);
				_canvas.DrawText(_introText[_introLine], 4, _textColours[2], 160, 83, TextAlign.Center);
			}
			if (_introLeft == -320 && _noiseCounter > 0)
            {
                if (!_introSkipped) _canvas.AddLayer(_pictures[1].Image);
				if (_logoSwipe < 320)
				{
					if (_logoSwipe > 0)
					{
						_canvas.AddLayer(_pictures[2].GetPart(0, 0, _logoSwipe, 200));
					}
				}
				else
				{
					_canvas.AddLayer(_pictures[2].Image);	
				}
				if (_introSkipped) _canvas.AddLayer(_pictures[1].Image);
            }
			else if (_noiseCounter == 0)
			{
				_canvas.AddLayer(_pictures[2].Image);
				_canvas.ResetPalette();
				_done = true;
				
				// Draw menu background
				_canvas.FillRectangle(_menuColours[0], 101, 141, 120, 47);
				_canvas.FillRectangle(_menuColours[1], 101, 142, 119, 46);
				_canvas.FillRectangle(_menuColours[2], 102, 142, 118, 45);
			}
			return true;
		}
		
		public bool SkipIntro()
		{
			if (_introSkipped || _noiseCounter < 33) return false;
			
			_showIntroLine = false;
			_introLeft = -320;
			_logoSwipe = 320;
			_cycleCounter = 98;
			_pictures[1].FillRectangle(5, 0, 0, 320, 200);
			_introSkipped = true;
			return true;
		}
		
		private void StartNewGame(object sender, EventArgs args)
		{
			Destroy();
			Common.AddScreen(new Intro());
			Console.WriteLine("Main Menu: Start a New Game");
		}
		
		private void Earth(object sender, EventArgs args)
		{
			Destroy();
			Common.AddScreen(new Intro());
			Console.WriteLine("Main Menu: EARTH");
		}
		
		private void CustomizeWorld(object sender, EventArgs args)
		{
			Destroy();
			Common.AddScreen(new CustomizeWorld());
			Console.WriteLine("Main Menu: Customize World");
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			return SkipIntro();
		}
		
		public override bool MouseDown(MouseEventArgs args)
		{
			return SkipIntro();
		}
		
		public Credits()
		{
            _introText = TextFile.Instance.LoadArray("credits");
            _pictures = new Picture[3];
            for (int i = 0; i < 2; i++)
                _pictures[i] = Resources.Instance.LoadPIC(string.Format("BIRTH{0}", i), true);
            _pictures[2] = Resources.Instance.LoadPIC("LOGO", true);
            _noiseMap = new byte[320, 200];
            for (int x = 0; x < 320; x++)
            {
                for (int y = 0; y < 200; y++)
                {
                    _noiseMap[x, y] = (byte)Common.Random.Next(1, _noiseCounter);
                }
            }
			switch (Settings.Instance.GraphicsMode)
			{
				case GraphicsMode.Graphics256:
					_textColours = new byte[] { 248, 242, 244 };
					break;
				case GraphicsMode.Graphics16:
					_textColours = new byte[] { 15, 15, 7 };
					break;
			}
			_menuColours = new byte[] { 8, 15, 7 };
			
			_canvas = new Picture(320, 200, _pictures[2].Image.Palette.Entries);
			
			Menu menu = new Menu(Canvas.Image.Palette.Entries)
			{
				X = 103,
				Y = 144,
				Width = 116,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 8,
				FontId = 0
			};
			Menu.Item[] menuItems = new Menu.Item[5];
			menu.Items.Add(menuItems[0] = new Menu.Item("Start a New Game"));
			menu.Items.Add(menuItems[1] = new Menu.Item("Load a Saved Game") { Enabled = false });
			menu.Items.Add(menuItems[2] = new Menu.Item("EARTH"));
			menu.Items.Add(menuItems[3] = new Menu.Item("Customize World"));
			menu.Items.Add(menuItems[4] = new Menu.Item("View Hall of Fame") { Enabled = false });
			
			menuItems[0].Selected += StartNewGame;
			menuItems[2].Selected += Earth;
			menuItems[3].Selected += CustomizeWorld;
			
			Menus.Add(menu);
		}
	}
}