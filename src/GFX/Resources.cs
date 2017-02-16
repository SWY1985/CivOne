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
using System.IO;
using CivOne.Enums;
using CivOne.GFX.ImageFormats;
using CivOne.Interfaces;
using CivOne.IO;

namespace CivOne.GFX
{
	internal class Resources
	{
		private readonly Dictionary<string, Picture> _cache = new Dictionary<string, Picture>();
		private readonly Dictionary<string, Picture> _textCache = new Dictionary<string, Picture>();
		private readonly List<Fontset> _fonts = new List<Fontset>();
		private readonly Dictionary<Direction, Picture> _fog = new Dictionary<Direction, Picture>();
		
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
				_fonts.Add(new Fontset(file, offset, LoadPIC("SP257").Palette));
			}
		}
		
		public bool ValidCharacter(int fontId, char c)
		{
			byte asciiChar = (byte)c;
			return (asciiChar >= _fonts[fontId].FirstChar && asciiChar <= _fonts[fontId].LastChar);
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
		
		public Picture GetText(string text, int font, byte colour)
		{
			return GetText(text, font, colour, colour);
		}
		
		public Picture GetText(string text, int font, byte colourFirstLetter, byte colour)
		{
			List<Picture> letters = new List<Picture>();
			bool isFirstLetter = true;
			foreach (char c in text)
			{
				letters.Add(GetLetter(isFirstLetter ? colourFirstLetter : colour, font, c));
				isFirstLetter = false;
			}
			
			int width = 0, height = 0;
			foreach (Picture letter in letters)
			{
				width += letter.Width + 1;
				if (height < letter.Height) height = letter.Height;
			}
			
			Picture output = new Picture(width, height);
			
			int xx = 0;
			foreach (Picture letter in letters)
			{
				output.AddLayer(letter, xx, 0);
				xx += letter.Width + 1;
			}
			
			output.Palette = letters[0].Palette;
			
			return output;
		}
		
		internal Size GetLetterSize(int font, char letter)
		{
			return GetLetter(5, font, letter).Size;
		}
		
		public int GetFontHeight(int font)
		{
			return _fonts[font].FontHeight;
		}
		
		private Picture GetLetter(byte colour, int font, char letter)
		{
			string key = string.Format("letter{0}|{1}|{2}", colour, font, letter);
			if (!_textCache.ContainsKey(key))
			{
				_textCache.Add(key, _fonts[font].GetLetter(letter, colour));
			}
			return _textCache[key];
		}
		
		public Picture GetPart(string filename, int x, int y, int width, int height)
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
		
		internal string[] GetCivilopediaText(string name)
		{
			List<string> textLines = new List<string>();
			string text = string.Join(" ", TextFile.Instance.GetGameText(name));
			string t = "";
			while (text.Length > 0)
			{
				if (text.IndexOf(' ') == -1)
				{
					if (t.Length > 0 && GetTextSize(6, string.Join(" ", t, text)).Width < 294)
						text = string.Join(" ", t, text);
					else if (t.Length > 0)
						textLines.Add(t);
					t = text;
					text = "";
				}
				else if (GetTextSize(6, t + text.Substring(0, text.IndexOf(' '))).Width < 294)
				{
					if (t.Length > 0) t += " ";
					t += text.Substring(0, text.IndexOf(' '));
					text = text.Substring(text.IndexOf(' ')).Trim();
					continue;
				}
				textLines.Add(t);
				t = "";
			}
			return textLines.ToArray();
		}
		
		private static Picture _worldMapTiles;
		public static Picture WorldMapTiles
		{
			get
			{
				if (_worldMapTiles == null)
				{
					Picture sp299 = Resources.Instance.LoadPIC("SP299");
					_worldMapTiles = new Picture(48, 8, sp299.Palette);
					_worldMapTiles.AddLayer(sp299.GetPart(160, 111, 48, 8));
				}
				return _worldMapTiles;
			}
		}
		
		public static Color[] PaletteCombine(Color[] palette1, Color[] palette2, byte start = 0, byte end = 255)
		{
			Color invisible = new Color(252, 84, 252);
			for (int i = start; i < end; i++)
			{
				if (palette2[i] == invisible) continue;
				palette1[i] = palette2[i];
			}
			return palette1;
		}
		
		public Picture GetTile(ITile tile, bool improvements = true, bool roads = true)
		{
			if (Settings.Instance.GraphicsMode == GraphicsMode.Graphics16)
			{
				return TileResources.GetTile16(tile, improvements, roads);
			}
			return TileResources.GetTile256(tile, improvements, roads);
		}

		public Picture GetFog(Direction direction)
		{
			if (!_fog.ContainsKey(direction)) return null;
			return _fog[direction];
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
			_fog.Add(Direction.West, GetPart("SP257", 128, 128, 16, 16));
			_fog.Add(Direction.South, GetPart("SP257", 112, 128, 16, 16));
			_fog.Add(Direction.East, GetPart("SP257", 96, 128, 16, 16));
			_fog.Add(Direction.North, GetPart("SP257", 80, 128, 16, 16));
		}
	}
}