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
using CivOne.Interfaces;
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
		
		private Bitmap GetTile16(ITile tile)
		{
			Picture output = new Picture(16, 16);
			
			bool altTile = ((tile.X + tile.Y) % 2 == 1);
			
			// Set tile base
			if (tile.Type != Terrain.Ocean)
			{
				output.AddLayer(GetPart("SPRITES", 0, 80, 16, 16));
			}
			
			// Add tile terrain
			switch (tile.Type)
			{
				case Terrain.Ocean:
				case Terrain.River:
					bool ocean = (tile.Type == Terrain.Ocean);
					output.AddLayer(GetPart("SPRITES", tile.Borders * 16, (ocean ? 64 : 80), 16, 16));
					break;
				default:
					int terrainId = (int)tile.Type;
					if (tile.Type == Terrain.Grassland1) altTile = false;
					else if (tile.Type == Terrain.Grassland2) { altTile = true; terrainId = (int)Terrain.Grassland1; }
					output.AddLayer(GetPart("SPRITES", terrainId * 16, (altTile ? 0 : 16), 16, 16));
					break;
			}
			
			// Add special resources
			if (tile.Special)
			{
				int terrainId = (int)tile.Type;
				Bitmap resource = GetPart("SPRITES", terrainId * 16, 112, 16, 16);
				Picture.ReplaceColours(resource, 3, 0);
				output.AddLayer(resource);
			}
			
			return output.Image;
		}
		
		private Bitmap GetTile256(ITile tile)
		{
			Picture output = new Picture(16, 16);
			
			// Set tile base
			switch (tile.Type)
			{
				case Terrain.Ocean: output.AddLayer(GetPart("TER257", 0, 160, 16, 16)); break;
				default: output.AddLayer(GetPart("SP257", 0, 64, 16, 16)); break;
			}
			
			// Add tile terrain
			switch (tile.Type)
			{
				case Terrain.Ocean:
					output.AddLayer(GetPart("TER257", tile.Borders * 16, 160, 16, 16));
					break;
				case Terrain.River:
					output.AddLayer(GetPart("SP257", tile.Borders * 16, 80, 16, 16));
					break;
				default:
					int terrainId = (int)tile.Type;
					if (tile.Type == Terrain.Grassland2) { terrainId = (int)Terrain.Grassland1; }
					output.AddLayer(GetPart("TER257", tile.Borders * 16, terrainId * 16, 16, 16));
					break;
			}
			
			// Add special resources
			if (tile.Special)
			{
				int terrainId = (int)tile.Type;
				Bitmap resource = GetPart("SP257", terrainId * 16, 112, 16, 16);
				Picture.ReplaceColours(resource, 3, 0);
				output.AddLayer(resource);
			}
			
			return output.Image;
		}
		
		public Bitmap GetTile(ITile tile)
		{
			if (Settings.Instance.GraphicsMode == GraphicsMode.Graphics16)
			{
				return GetTile16(tile);
			}
			return GetTile256(tile);
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