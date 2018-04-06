// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Text;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne.Screens
{
	[Expand]
	internal class Input : BaseScreen
	{
		public event EventHandler Accept;
		public event EventHandler Cancel;
		
		private string _text;
		private int _fontId;
		private byte _textColour, _cursorColour;
		private int _width, _height;
		private int _maxLength;
		private int _cursorPosition = -1;

		public int X { get; set; }
		public int Y { get; set; }
		
		public string Text => _text;

		private bool AppendCharacter(char character)
		{
			if (!Resources.ValidCharacter(_fontId, character)) return false;
			
			if (_cursorPosition == -1)
			{
				_text = string.Empty;
				_cursorPosition = 0;
			}
			
			StringBuilder sb = new StringBuilder(_text);
			if (_text.Length > _cursorPosition)
			{
				sb[_cursorPosition] = character;
			}
			else
			{
				sb.Append(character);
			}
			_text = sb.ToString();
			_cursorPosition++;
			while (_cursorPosition >= _maxLength) _cursorPosition--;
			return true;
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			Bitmap.Clear();
			int fontHeight = Resources.GetFontHeight(_fontId);
			int cursorPosition = _cursorPosition;
			if (cursorPosition < 0) cursorPosition = 0;
			
			int xx = X;
			int yy = Y + (int)Math.Ceiling((float)(_height - fontHeight) / 2);
			
			if (gameTick % 4 < 2)
			{
				for (int i = 0; i <= _text.Length; i++)
				{
					int letterWidth = (_text.Length <= i) ? 7 : Resources.GetLetterSize(_fontId, _text[i]).Width;
					
					if (i == cursorPosition)
					{
						this.FillRectangle(xx, Y, letterWidth + 1, _height, _cursorColour);
						break;
					}
					
					xx += letterWidth + 1;
				}
			}
			if (!string.IsNullOrEmpty(_text))
				this.DrawText(_text, _fontId, _textColour, X, yy);
			
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			StringBuilder sb;
			switch (args.Key)
			{
				case Key.NumPad0:
				case Key.NumPad1:
				case Key.NumPad2:
				case Key.NumPad3:
				case Key.NumPad4:
				case Key.NumPad5:
				case Key.NumPad6:
				case Key.NumPad7:
				case Key.NumPad8:
				case Key.NumPad9:
					return AppendCharacter((char)('0' + (args.Key - Key.NumPad0)));
				case Key.Left:
					if (_cursorPosition > 0) _cursorPosition--;
					else _cursorPosition = 0;
					return true;
				case Key.Right:
					if (_cursorPosition < 0) _cursorPosition = 0;
					if (_cursorPosition < _text.Length) _cursorPosition++;
					else _cursorPosition = _text.Length;
					return true;
				case Key.Escape:
					if (Cancel != null)
						Cancel(this, null);
					break;
				case Key.Enter:
					if (Accept != null)
						Accept(this, null);
					break;
				case Key.Delete:
					if (_cursorPosition < _text.Length)
					{
						sb = new StringBuilder();
						for (int i = 0; i < _text.Length; i++)
						{
							if (i == _cursorPosition) continue;
							sb.Append(_text[i]);
						}
						_text = sb.ToString();
						return true;
					}
					return false;
				case Key.Home:
					_cursorPosition = 0;
					return true;
				case Key.End:
					_cursorPosition = _text.Length;
					return true;
				case Key.Backspace:
					if (_cursorPosition <= 0) return false;
					
					sb = new StringBuilder(_text);
					sb.Remove(--_cursorPosition, 1);
					_text = sb.ToString();
					
					return true;
				case Key.Slash:
					return AppendCharacter((char)'/');
				default:
					char c = args.KeyChar;
					if (!args.Shift) c = Char.ToLower(c);
					if (args.Key == Key.Minus) c = '-';
					if (args.Shift && (c >= '0' && c <= '9'))
					{
						switch (c)
						{
							case '6': c = '^'; break;
							case '7': c = '&'; break;
							case '8': c = '*'; break;
							case '9': c = '('; break;
							case '0': c = ')'; break;
							default: c -= (char)16; break;
						}
					}
					return AppendCharacter(c);
			}
			return false;
		}
		
		public void Close()
		{
			Destroy();
		}

		private void Resize(object sender, ResizeEventArgs args)
		{
			HasUpdate(0);
		}
		
		public Input(Palette palette, string text, int fontId, byte textColour, byte cursorColour, int x, int y, int width, int height, int maxLength)
		{
			OnResize += Resize;

			Palette = palette.Copy();
			_text = text;
			_fontId = fontId;
			_textColour = textColour;
			_cursorColour = cursorColour;
			X = x;
			Y = y;
			_width = width;
			_height = height;
			_maxLength = maxLength;
		}
		
		public Input(Palette palette, int fontId, byte textColour, byte cursorColour, int x, int y, int width, int height, int maxLength)
		{
			OnResize += Resize;

			Palette = palette.Copy();
			_text = "";
			_fontId = fontId;
			_textColour = textColour;
			_cursorColour = cursorColour;
			X = x;
			Y = y;
			_width = width;
			_height = height;
			_maxLength = maxLength;
		}
	}
}