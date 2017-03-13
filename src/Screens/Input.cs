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
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Input : BaseScreen
	{
		public event EventHandler Accept;
		public event EventHandler Cancel;
		
		private string _text;
		private int _fontId;
		private byte _textColour, _cursorColour;
		private int _x, _y, _width, _height;
		private int _maxLength;
		private int _cursorPosition = -1;
		
		public string Text
		{
			get
			{
				return _text;
			}
		}

		private bool AppendCharacter(char character)
		{
			if (!Resources.Instance.ValidCharacter(_fontId, character)) return false;
			
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
		
		public override bool HasUpdate(uint gameTick)
		{
			_canvas.FillRectangle(0, 0, 0, 320, 200);
			int fontHeight = Resources.Instance.GetFontHeight(_fontId);
			int cursorPosition = _cursorPosition;
			if (cursorPosition < 0) cursorPosition = 0;
			
			int xx = _x;
			int yy = _y + (int)Math.Ceiling((float)(_height - fontHeight) / 2);
			
			if (gameTick % 4 < 2)
			{
				for (int i = 0; i <= _text.Length; i++)
				{
					int letterWidth = (_text.Length <= i) ? 7 : Resources.Instance.GetLetterSize(_fontId, _text[i]).Width;
					
					if (i == cursorPosition)
					{
						_canvas.FillRectangle(_cursorColour, xx, _y, letterWidth + 1, _height);
						break;
					}
					
					xx += letterWidth + 1;
				}
			}
			if (_text.Length > 0)
				_canvas.DrawText(_text, _fontId, _textColour, _x, yy);
			
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
					//TODO: Handle delete
					break;
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
		
		public Input(Color[] colours, string text, int fontId, byte textColour, byte cursorColour, int x, int y, int width, int height, int maxLength)
		{
			_canvas = new Picture(320, 200, colours);
			_text = text;
			_fontId = fontId;
			_textColour = textColour;
			_cursorColour = cursorColour;
			_x = x;
			_y = y;
			_width = width;
			_height = height;
			_maxLength = maxLength;
		}
		
		public Input(Color[] colours, int fontId, byte textColour, byte cursorColour, int x, int y, int width, int height, int maxLength)
		{
			_canvas = new Picture(320, 200, colours);
			_text = "";
			_fontId = fontId;
			_textColour = textColour;
			_cursorColour = cursorColour;
			_x = x;
			_y = y;
			_width = width;
			_height = height;
			_maxLength = maxLength;
		}
	}
}