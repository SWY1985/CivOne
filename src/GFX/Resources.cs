// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CivOne.Enums;
using CivOne.IO;

namespace CivOne.GFX
{
	internal class Resources
	{
		private readonly Dictionary<string, Picture> _cache = new Dictionary<string, Picture>();
		private readonly Dictionary<string, Bitmap> _textCache = new Dictionary<string, Bitmap>();
		private readonly List<Fontset> _fonts = new List<Fontset>();
		
		internal void ClearTextCache()
		{
			_textCache.Clear();
		}
		
		private void LoadFonts()
		{
			byte[] file;
			using (FileStream fs = new FileStream(Path.Combine(Settings.Instance.DataDirectory, "FONTS.CV"), FileMode.Open))
			{
				file = new byte[fs.Length];
				for (int i = 0; i < fs.Length; i++) file[i] = (byte)fs.ReadByte();
			}

			List<ushort> fontOffsets = new List<ushort>();
			int index = 0;
			uint fontCount = BitConverter.ToUInt16(file, index);
			index += 2;

			for (int i = 0; i < fontCount; i++)
			{
				fontOffsets.Add(BitConverter.ToUInt16(file, index));
				index += 2;
			}

			foreach (ushort offset in fontOffsets)
			{
				_fonts.Add(new Fontset(file, offset, LoadPIC("SP257").Image.Palette.Entries));
			}
		}

		public Size GetTextSize(int font, string text)
		{
			int width = 0, height = 0;
			foreach (char c in text)
			{
				Size size = GetLetterSize(font, c);
				width += size.Width + 1;
				if (height < size.Height) height = size.Height;
			}
			return new Size(width, height);
		}

		public Bitmap GetText(string text, int font, byte colour)
		{
			return GetText(text, font, colour, colour);
		}

		public Bitmap GetText(string text, int font, byte colourFirstLetter, byte colour)
		{
			List<Bitmap> letters = new List<Bitmap>();
			bool isFirstLetter = true;
			foreach (char c in text)
			{
				letters.Add(GetLetter(isFirstLetter ? colourFirstLetter : colour, font, c));
				isFirstLetter = false;
			}

			int width = 0, height = 0;
			foreach (Bitmap letter in letters)
			{
				width += letter.Width + 1;
				if (height < letter.Height) height = letter.Height;
			}

			Bitmap output = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

			int xx = 0;
			foreach (Bitmap letter in letters)
			{
				output = Picture.Combine(output, letter, new Point(xx, 0));
				xx += letter.Width + 1;
			}

			output.Palette = letters[0].Palette;

			return output;
		}

		private Size GetLetterSize(int font, char letter)
		{
			return GetLetter(5, font, letter).Size;
		}
		
		public int GetFontHeight(int font)
		{
			return _fonts[font].FontHeight;
		}

		private Bitmap GetLetter(byte colour, int font, char letter)
		{
			string key = string.Format("letter{0}|{1}|{2}", colour, font, letter);
			if (!_textCache.ContainsKey(key))
			{
				_textCache.Add(key, _fonts[font].GetLetter(letter, colour));
			}
			return _textCache[key];
		}
		
		public Bitmap GetPart(string filename, int x, int y, int width, int height)
		{
			return LoadPIC(filename).GetPart(x, y, width, height);
		}

		public Picture LoadPIC(string filename, bool noCache = false)
		{
			string key = filename.ToUpper();
			if (_cache.ContainsKey(key))
			{
				if (!noCache) return _cache[key];
				_cache.Remove(key);
			}
			
			Picture output = null;
			PicFile picFile = new PicFile(filename);
			if ((Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 && picFile.GetPicture256 != null) || picFile.GetPicture16 == null)
			{
				output = new Picture(picFile.GetPicture256, picFile.GetPalette256);
			}
			else
			{
				output = new Picture(picFile.GetPicture16, picFile.GetPalette16);
			}

			if (!noCache) _cache.Add(key, output);
			return output;
		}
		
		private static Resources _instance;
		public static Resources Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new Resources();
				}
				return _instance;
			}
		}

		private Resources()
		{
			LoadFonts();
		}
	}
}