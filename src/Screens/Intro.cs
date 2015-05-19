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
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.IO;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Intro : BaseScreen
	{
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
			
			ColorPalette palette = _pictures[_introPicture].Image.Palette;
			for (int i = 1; i < 256; i++)
				palette.Entries[i] = FadeColour(Color.Black, _pictures[_introPicture].OriginalColours[i]);
			_canvas.SetPalette(palette);
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			bool update = true;
			if (_introPicture == _introPictureNext && FadeStep < 1.0F)
			{
				FadeStep += 0.0625F;
				FadeColours();
			}
			else if (_introPicture != _introPictureNext)
			{
				if (FadeStep > 0.0F)
				{
					FadeStep -= 0.0625F;
					FadeColours();
				}
				else
				{
					_introPicture = _introPictureNext;
					_canvas = new Picture(320, 200, _pictures[_introPicture].Image.Palette.Entries);
					FadeColours();
				}
			}
			else if (gameTick % 2 == 0)
			{
				_introTicks++;
				if (_introTicks % 30 == 0)
				{
					_introLine++;
					if (_introLine >= _introText.Length)
					{
						Destroy();
						Common.AddScreen(new Demo());
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
					case 0: _canvas.Cycle(184, 176); break;
					case 1: _canvas.Cycle(32, 47).Cycle(48, 63).Cycle(64, 79); break;
					case 2: _canvas.Cycle(80, 95).Cycle(96, 111).Cycle(112, 127); break;
					case 3: _canvas.Cycle(134, 139).Cycle(245, 250); break;
					case 4: _canvas.Cycle(96, 102).Cycle(135, 140); break;
					case 5: _canvas.Cycle(136, 138).Cycle(129, 130).Cycle(250, 254); break;
					case 6: _canvas.Cycle(132, 134).Cycle(135, 138).Cycle(208, 210).Cycle(245, 249); break;
					case 7: _canvas.Cycle(132, 134).Cycle(208, 210).Cycle(246, 249); break;
				}
			}
			else
			{
				update = false;
			}
			if (!update) return false;
			
			_canvas.AddLayer(_pictures[_introPicture].Image);
			
			if (_fadeStep < 1.0F) return true;
			
			int previousText = 0;
			string introLine = _introText[_introLine];
			while (introLine == string.Empty)
				introLine = _introText[_introLine - (++previousText)];
			byte textColour = 0;
			if (_introTicks % 30 > 1 && _introTicks % 30 < 29 || ((_introLine + 1) < _introText.Length && _introText[_introLine + 1] == string.Empty))
			{
				textColour = 11;
			}
			else if (_introTicks % 30 == 1 || _introTicks % 30 == 29)
			{
				if (_introTicks % 30 == 1) Console.WriteLine(_introText[_introLine]);
				textColour = 3;
			}
			if (textColour > 0)
				_canvas.DrawText(introLine, 6, textColour, 160, 160, TextAlign.Center);
			
			return true;
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			if (args.Shift)
			{
				if (_fadeStep < 1.0F) return false;
				if (args.KeyCode == Keys.Left)
				{
					if (_introLine <= 1) return false;
					
					_introLine--;
					if (_introText[_introLine] == "_")
					{
						_introLine--;
						_introTicks = 1;
						IntroPicture--;
					}
					return true;
				}
				if (args.KeyCode == Keys.Right)
				{
					if (_introLine >= _introText.Length - 1) return false;
					
					_introLine++;
					if (_introText[_introLine] == "_")
					{
						_introLine++;
						_introTicks = 1;
						IntroPicture++;
					}
					return true;
				}
			}
			return false;
		}
		
		public Intro()
		{
			Cursor = MouseCursor.None;
			
            _introText = TextFile.Instance.LoadArray("story");
			_pictures = new Picture[8];
			for (int i = 0; i < _pictures.Length; i++)
				_pictures[i] = Resources.Instance.LoadPIC(string.Format("BIRTH{0}", i + 1), true);
			
			_canvas = new Picture(320, 200, _pictures[0].Image.Palette.Entries);
			_canvas.FillRectangle(1, 0, 0, 320, 200);
		}
	}
}