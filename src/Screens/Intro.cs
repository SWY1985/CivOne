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
using CivOne.Graphics;
using CivOne.IO;

namespace CivOne.Screens
{
	[Expand]
	internal class Intro : BaseScreen
	{
		private const float FADE_STEP = 0.0625F;
		
		private readonly string[] _introText;
		private readonly Picture[] _pictures;

		private float _fadeStep = 0.0F;
		private int _introTicks = 0;
		private int _introLine = 1;
		
		private int _introPicture = 0;
		private int _introPictureNext = 0;
		
		private float FadeStep
		{
			get
			{
				return _fadeStep;
			}
			set
			{
				_fadeStep = value;
				if (_fadeStep < 0.0F) _fadeStep = 0.0F;
				if (_fadeStep > 1.0F) _fadeStep = 1.0F;
			}
		}
		
		private int IntroPicture
		{
			get
			{
				return _introPicture;
			}
			set
			{
				_introPictureNext = value;
			}
		}
		
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
			
			using (Palette palette = _pictures[_introPicture].Palette.Copy())
			{
				for (int i = 1; i < 256; i++)
					palette[i] = FadeColour(new Colour(0, 0, 0), _pictures[_introPicture].OriginalColours[i]);
				this.SetPalette(palette);
			}
		}
		
		private bool HandleScreenFadeIn()
		{
			if (FadeStep >= 1.0F) return false;
			FadeStep += FADE_STEP;
			FadeColours();
			return true;
		}
		
		private bool HandleScreenFadeOut()
		{
			if (_introPicture == _introPictureNext) return false;
			if (FadeStep > 0.0F)
			{
				FadeStep -= FADE_STEP;
				FadeColours();
			}
			else
			{
				_introPicture = _introPictureNext;
				Palette = _pictures[_introPicture].Palette;
				FadeColours();
			}
			return true;
		}
		
		private bool HandleScreenFade()
		{
			if (_introPicture == _introPictureNext && HandleScreenFadeIn())
				return true;
			return HandleScreenFadeOut();
		}
		
		private void LogIntroText()
		{
			Log(@"Intro: ""{0}""", _introText[_introLine]);
		}
		
		private byte TextColour
		{
			get
			{
				if (_introTicks % 30 > 1 && _introTicks % 30 < 29 || ((_introLine + 1) < _introText.Length && _introText[_introLine + 1] == string.Empty)) return 11;
				if (_introTicks % 30 == 1 || _introTicks % 30 == 29) return 3;
				return 0;
			}
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			bool update = HandleScreenFade();
			if (!update && gameTick % 2 == 0)
			{
				_introTicks++;
				if (_introTicks % 30 == 0)
				{
					_introLine++;
					if (_introLine >= _introText.Length)
					{
						Destroy();
						Common.AddScreen(new NewGame());
						return true;
					}
					if (_introText[_introLine] == "_")
					{
						IntroPicture++;
						_introLine++;
					}
				}
				
				switch (_introPicture)
				{
					case 0: this.Cycle(184, 176); break;
					case 1: this.Cycle(32, 47).Cycle(48, 63).Cycle(64, 79); break;
					case 2: this.Cycle(80, 95).Cycle(96, 111).Cycle(112, 127); break;
					case 3: this.Cycle(134, 139).Cycle(245, 250); break;
					case 4: this.Cycle(96, 102).Cycle(135, 140); break;
					case 5: this.Cycle(136, 138).Cycle(129, 130).Cycle(250, 254); break;
					case 6: this.Cycle(132, 134).Cycle(135, 138).Cycle(208, 210).Cycle(245, 249); break;
					case 7: this.Cycle(132, 134).Cycle(208, 210).Cycle(246, 249); break;
				}
			}
			else if (!update)
			{
				return false;
			}
			
			int x = (Width - 320) / 2;
			int y = (Height - 200) / 2;
			if (x != 0 || y != 0)
			{
				this.Clear(_pictures[_introPicture].Bitmap[0, 0])
					.FillRectangle(x, y, 320, 200, _pictures[_introPicture].Bitmap[10, 100])
					.AddLayer(_pictures[_introPicture], x, y);
			}
			else
			{
				this.AddLayer(_pictures[_introPicture]);
			}
			
			if (_fadeStep < 1.0F) return true;
			
			int previousText = 0;
			string introLine = _introText[_introLine];
			while (introLine == string.Empty)
				introLine = _introText[_introLine - (++previousText)];
			this.DrawText(introLine, 6, TextColour, x + 160, y + 160, TextAlign.Center);
			
			if (_introTicks % 30 == 1) LogIntroText();
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (args.Shift)
			{
				if (_fadeStep < 1.0F) return false;
				if (args.Key == Key.Left)
				{
					if (_introLine <= 1) return false;
					
					Log("Intro: <<");
					
					_introLine--;
					if (_introText[_introLine] == "_")
					{
						_introLine--;
						_introTicks = 0;
						IntroPicture--;
					}
					else
					{
						LogIntroText();
					}
					return true;
				}
				if (args.Key == Key.Right)
				{
					if (_introLine >= _introText.Length - 1) return false;
					
					Log("Intro: >>");
					
					_introLine++;
					if (_introText[_introLine] == "_")
					{
						_introLine++;
						_introTicks = 0;
						IntroPicture++;
					}
					else
					{
						LogIntroText();	
					}
					return true;
				}
			}
			if (args.Key == Key.Space || args.Key == Key.Enter)
			{
				Destroy();
				Common.AddScreen(new NewGame());
				return true;
			}
			return false;
		}

		public void Resize(object sender, ResizeEventArgs args)
		{
			Bitmap.Clear();
			HasUpdate(0);
		}
		
		public Intro()
		{
			OnResize += Resize;
			
			_introText = TextFile.Instance.LoadArray("STORY");
			if (_introText.Length == 0)
			{
				_introText = new string[16];
				for (int i = 0; i < 16; i++)
				{
					_introText[i] = (i % 2) == 0 ? "MISSING TEXT" : "_";
				}
			}
			_pictures = new Picture[8];
			for (int i = 0; i < _pictures.Length; i++)
				_pictures[i] = Resources[$"BIRTH{(i + 1)}"];
			
			Palette = _pictures[0].Palette;
		}
	}
}