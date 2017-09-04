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
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.IO;

namespace CivOne.UserInterface
{
	public class Button : Element, IMouseElement
	{
		private readonly TextSettings _textSettings;

		private int FontHeight => Resources.Instance.GetFontHeight(_textSettings.FontId);

		private byte _colour, _colourLight, _colourDark;
		private string _text;

		public event EventHandler Clicked;

		public TextSettings TextSettings => _textSettings;

		public byte Colour
		{
			get
			{
				return _colour;
			}
			set
			{
				if (_colour == value) return;
				_colour = value;
				Draw();
			}
		}

		public byte ColourLight
		{
			get
			{
				return _colourLight;
			}
			set
			{
				if (_colourLight == value) return;
				_colourLight = value;
				Draw();
			}
		}

		public byte ColourDark
		{
			get
			{
				return _colourDark;
			}
			set
			{
				if (_colourDark == value) return;
				_colourDark = value;
				Draw();
			}
		}

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				if (_text == value) return;
				_text = value;
				Draw();
			}
		}

		private void Draw(object sender = null, EventArgs args = null)
		{
			using (Picture picture = new Picture(Width, Height))
			{
				picture.FillRectangle(1, 1, Width - 2, Height - 2, _colour)
					.DrawRectangle(0, 0, Width - 1, 1, _colourLight)
					.DrawRectangle(0, 1, 1, Height - 1, _colourLight)
					.DrawRectangle(Width - 1, 0, 1, Height, _colourDark)
					.DrawRectangle(1, Height - 1, Width - 1, 1, _colourDark)
					.DrawText(Text, (int)Math.Ceiling((float)Width / 2), (int)Math.Ceiling(((float)Height - FontHeight) / 2), _textSettings);
				Bitmap.AddLayer(picture.Bitmap);
			}
		}

		public bool MouseDown(int left, int top)
		{
			if (Clicked != null)
			{
				Clicked.Invoke(this, EventArgs.Empty);
				return true;
			}
			return false;
		}

		public bool MouseUp(int left, int top) => (Clicked != null);

		public bool MouseDrag(int left, int top) => false;

		private static Button ColourTemplate(string text, int left, int top, int width, int height, byte colour, byte colourDark, EventHandler click = null)
		{
			Button output = new Button(text, left, top, width, height)
			{
				_colour = colour,
				_colourDark = colourDark
			};
			output._textSettings.Colour = colourDark;
			output.Draw();
			output.Clicked += click;
			return output;
		}

		public static Button Blue(string text, int left, int top, int width, int height = -1, EventHandler click = null) => ColourTemplate(text, left, top, width, height, 9, 1, click);
		public static Button Green(string text, int left, int top, int width, int height = -1, EventHandler click = null) => ColourTemplate(text, left, top, width, height, 10, 2, click);
		public static Button Red(string text, int left, int top, int width, int height = -1, EventHandler click = null) => ColourTemplate(text, left, top, width, height, 12, 4, click);
		
		private Button(string text, int left, int top, int width, int height = -1)
		{
			_text = text;
			_colourLight = 7;

			if (height == -1) height = Resources.Instance.GetFontHeight(1) + 3;

			Bitmap = new Bytemap(width, height);
			Left = left;
			Top = top;
			
			_textSettings = new TextSettings()
			{
				FontId = 1,
				Alignment = TextAlign.Center
			};
			_textSettings.Changed += Draw;
		}
	}
}