// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;

namespace CivOne.Graphics
{
	internal class DefaultFont : IFont
	{
		private const bool B0 = false, B1 = true;
		private readonly Dictionary<char, bool[,]> _characters;

		public int FontHeight
		{
			get
			{
				return 8;
			}
		}

		public byte FirstChar
		{
			get
			{
				return 32;
			}
		}

		public byte LastChar
		{
			get
			{
				return 127;
			}
		}

		public Picture GetLetter(char character, byte colour)
		{
			if (!_characters.ContainsKey(character)) return new Picture(7, 7);

			bool[,] pixels = _characters[character];
			Picture output = new Picture(pixels.GetLength(0), pixels.GetLength(1));
			for (int yy = 0; yy < pixels.GetLength(1); yy++)
			for (int xx = 0; xx < pixels.GetLength(0); xx++)
			{
				output[xx, yy] = (byte)(pixels[xx, yy] ? colour : 0);
			}
			return output;
		}

		public DefaultFont()
		{
			_characters = new Dictionary<char, bool[,]>();
			_characters.Add((char)32, new bool[,] { { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 } });
			_characters.Add((char)33, new bool[,] { { B0, B1, B1, B1, B0, B1, B0 } });
			_characters.Add((char)34, new bool[,] { { B1, B1, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B1, B1, B0, B0, B0, B0, B0 } });
			_characters.Add((char)35, new bool[,] { { B0, B0, B1, B0, B1, B0, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B0, B1, B0, B1, B0, B0 } });
			_characters.Add((char)36, new bool[,] { { B0, B0, B0, B1, B1, B0, B0 }, { B0, B0, B1, B1, B0, B1, B0 }, { B0, B0, B1, B0, B1, B1, B0 }, { B0, B0, B0, B1, B1, B0, B0 } });
			_characters.Add((char)37, new bool[,] { { B0, B1, B0, B0, B0, B1, B0 }, { B0, B0, B0, B0, B1, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B1, B0, B0, B0, B1, B0 } });
			_characters.Add((char)38, new bool[,] { { B0, B0, B0, B0, B1, B0, B0 }, { B0, B1, B0, B1, B0, B1, B0 }, { B1, B0, B1, B0, B0, B1, B0 }, { B0, B1, B0, B1, B1, B0, B0 }, { B0, B0, B0, B0, B1, B1, B0 } });
			_characters.Add((char)39, new bool[,] { { B1, B1, B0, B0, B0, B0, B0 } });
			_characters.Add((char)40, new bool[,] { { B0, B1, B1, B1, B1, B1, B0 }, { B1, B0, B0, B0, B0, B0, B1 } });
			_characters.Add((char)41, new bool[,] { { B1, B0, B0, B0, B0, B0, B1 }, { B0, B1, B1, B1, B1, B1, B0 } });
			_characters.Add((char)42, new bool[,] { { B0, B0, B1, B0, B1, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 } });
			_characters.Add((char)43, new bool[,] { { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 } });
			_characters.Add((char)44, new bool[,] { { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B0, B0, B0, B1, B0 } });
			_characters.Add((char)45, new bool[,] { { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 } });
			_characters.Add((char)46, new bool[,] { { B0, B0, B0, B0, B0, B1, B0 } });
			_characters.Add((char)47, new bool[,] { { B0, B0, B0, B0, B0, B1, B1 }, { B0, B0, B1, B1, B1, B0, B0 }, { B1, B1, B0, B0, B0, B0, B0 } });
			_characters.Add((char)48, new bool[,] { { B0, B1, B1, B1, B1, B1, B0 }, { B1, B0, B0, B0, B0, B0, B1 }, { B1, B0, B0, B0, B0, B0, B1 }, { B0, B1, B1, B1, B1, B1, B0 } });
			_characters.Add((char)49, new bool[,] { { B0, B1, B0, B0, B0, B0, B0 }, { B1, B1, B1, B1, B1, B1, B1 } });
			_characters.Add((char)50, new bool[,] { { B0, B1, B0, B0, B0, B1, B1 }, { B1, B0, B0, B0, B1, B0, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B0, B1, B1, B0, B0, B0, B1 } });
			_characters.Add((char)51, new bool[,] { { B0, B1, B0, B0, B0, B1, B0 }, { B1, B0, B0, B0, B0, B0, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B0, B1, B1, B0, B1, B1, B0 } });
			_characters.Add((char)52, new bool[,] { { B0, B0, B0, B1, B1, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B1, B0, B0, B1, B0, B0 }, { B1, B1, B1, B1, B1, B1, B1 }, { B0, B0, B0, B0, B1, B0, B0 } });
			_characters.Add((char)53, new bool[,] { { B1, B1, B1, B0, B0, B1, B0 }, { B1, B0, B1, B0, B0, B0, B1 }, { B1, B0, B1, B0, B0, B0, B1 }, { B1, B0, B0, B1, B1, B1, B0 } });
			_characters.Add((char)54, new bool[,] { { B0, B1, B1, B1, B1, B1, B0 }, { B1, B0, B0, B1, B0, B0, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B0, B1, B0, B0, B1, B1, B0 } });
			_characters.Add((char)55, new bool[,] { { B1, B0, B0, B0, B0, B0, B0 }, { B1, B0, B0, B0, B0, B1, B1 }, { B1, B0, B1, B1, B1, B0, B0 }, { B1, B1, B0, B0, B0, B0, B0 } });
			_characters.Add((char)56, new bool[,] { { B0, B1, B1, B0, B1, B1, B0 }, { B1, B0, B0, B1, B0, B0, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B0, B1, B1, B0, B1, B1, B0 } });
			_characters.Add((char)57, new bool[,] { { B0, B1, B1, B0, B0, B1, B0 }, { B1, B0, B0, B1, B0, B0, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B0, B1, B1, B1, B1, B1, B0 } });
			_characters.Add((char)58, new bool[,] { { B0, B0, B0, B1, B0, B1, B0 } });
			_characters.Add((char)59, new bool[,] { { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B0, B1, B0, B1, B0 } });
			_characters.Add((char)60, new bool[,] { { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B1, B0, B0, B0, B1, B0 } });
			_characters.Add((char)61, new bool[,] { { B0, B0, B1, B0, B1, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 } });
			_characters.Add((char)62, new bool[,] { { B0, B1, B0, B0, B0, B1, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 } });
			_characters.Add((char)63, new bool[,] { { B0, B1, B0, B0, B0, B0, B0 }, { B1, B0, B0, B0, B0, B0, B0 }, { B1, B0, B0, B1, B0, B1, B0 }, { B0, B1, B1, B0, B0, B0, B0 } });
			_characters.Add((char)64, new bool[,] { { B0, B0, B1, B1, B1, B0, B0 }, { B0, B1, B0, B0, B0, B1, B0 }, { B1, B0, B0, B1, B0, B0, B1 }, { B1, B0, B1, B0, B1, B0, B1 }, { B1, B0, B1, B1, B1, B0, B1 }, { B1, B1, B0, B0, B1, B0, B1 }, { B0, B0, B1, B1, B0, B0, B0 } });
			_characters.Add((char)65, new bool[,] { { B0, B0, B0, B0, B0, B1, B1 }, { B0, B0, B0, B1, B1, B0, B0 }, { B0, B1, B1, B0, B1, B0, B0 }, { B1, B0, B0, B0, B1, B0, B0 }, { B0, B1, B1, B0, B1, B0, B0 }, { B0, B0, B0, B1, B1, B0, B0 }, { B0, B0, B0, B0, B0, B1, B1 } });
			_characters.Add((char)66, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B0, B1, B1, B0, B1, B1, B0 } });
			_characters.Add((char)67, new bool[,] { { B0, B1, B1, B1, B1, B1, B0 }, { B1, B0, B0, B0, B0, B0, B1 }, { B1, B0, B0, B0, B0, B0, B1 }, { B1, B0, B0, B0, B0, B0, B1 }, { B0, B1, B0, B0, B0, B1, B0 } });
			_characters.Add((char)68, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B1, B0, B0, B0, B0, B0, B1 }, { B1, B0, B0, B0, B0, B0, B1 }, { B0, B1, B0, B0, B0, B1, B0 }, { B0, B0, B1, B1, B1, B0, B0 } });
			_characters.Add((char)69, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B1, B0, B0, B1, B0, B0, B1 } });
			_characters.Add((char)70, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B1, B0, B0, B1, B0, B0, B0 }, { B1, B0, B0, B1, B0, B0, B0 }, { B1, B0, B0, B1, B0, B0, B0 } });
			_characters.Add((char)71, new bool[,] { { B0, B1, B1, B1, B1, B1, B0 }, { B1, B0, B0, B0, B0, B0, B1 }, { B1, B0, B0, B0, B0, B0, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B0, B1, B0, B1, B1, B1, B1 } });
			_characters.Add((char)72, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B1, B1, B1, B1, B1, B1, B1 } });
			_characters.Add((char)73, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 } });
			_characters.Add((char)74, new bool[,] { { B0, B0, B0, B0, B0, B1, B0 }, { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B0, B0, B0, B0, B1 }, { B1, B1, B1, B1, B1, B1, B0 } });
			_characters.Add((char)75, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B1, B0, B0, B0, B1, B0 }, { B1, B0, B0, B0, B0, B0, B1 } });
			_characters.Add((char)76, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B0, B0, B0, B0, B1 } });
			_characters.Add((char)77, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B0, B0, B1, B1, B0, B0 }, { B0, B0, B0, B0, B0, B1, B0 }, { B0, B0, B0, B1, B1, B0, B0 }, { B0, B0, B1, B0, B0, B0, B0 }, { B1, B1, B1, B1, B1, B1, B1 } });
			_characters.Add((char)78, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B0, B1, B1, B0, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B0, B1, B1, B0 }, { B1, B1, B1, B1, B1, B1, B1 } });
			_characters.Add((char)79, new bool[,] { { B0, B1, B1, B1, B1, B1, B0 }, { B1, B0, B0, B0, B0, B0, B1 }, { B1, B0, B0, B0, B0, B0, B1 }, { B1, B0, B0, B0, B0, B0, B1 }, { B0, B1, B1, B1, B1, B1, B0 } });
			_characters.Add((char)80, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B1, B0, B0, B1, B0, B0, B0 }, { B1, B0, B0, B1, B0, B0, B0 }, { B1, B0, B0, B1, B0, B0, B0 }, { B0, B1, B1, B0, B0, B0, B0 } });
			_characters.Add((char)81, new bool[,] { { B0, B1, B1, B1, B1, B1, B0 }, { B1, B0, B0, B0, B0, B0, B1 }, { B1, B0, B0, B0, B1, B0, B1 }, { B1, B0, B0, B0, B0, B1, B1 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B0, B0, B0, B0, B0, B1 } });
			_characters.Add((char)82, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B1, B0, B0, B1, B0, B0, B0 }, { B1, B0, B0, B1, B0, B0, B0 }, { B1, B0, B0, B1, B1, B0, B0 }, { B0, B1, B1, B0, B0, B1, B1 } });
			_characters.Add((char)83, new bool[,] { { B0, B1, B1, B0, B0, B1, B0 }, { B1, B0, B0, B1, B0, B0, B1 }, { B1, B0, B0, B1, B0, B0, B1 }, { B0, B1, B0, B0, B1, B1, B0 } });
			_characters.Add((char)84, new bool[,] { { B1, B0, B0, B0, B0, B0, B0 }, { B1, B0, B0, B0, B0, B0, B0 }, { B1, B1, B1, B1, B1, B1, B1 }, { B1, B0, B0, B0, B0, B0, B0 }, { B1, B0, B0, B0, B0, B0, B0 } });
			_characters.Add((char)85, new bool[,] { { B1, B1, B1, B1, B1, B1, B0 }, { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B0, B0, B0, B0, B1 }, { B1, B1, B1, B1, B1, B1, B0 } });
			_characters.Add((char)86, new bool[,] { { B1, B1, B1, B0, B0, B0, B0 }, { B0, B0, B0, B1, B1, B1, B0 }, { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B0, B1, B1, B1, B0 }, { B1, B1, B1, B0, B0, B0, B0 } });
			_characters.Add((char)87, new bool[,] { { B1, B1, B0, B0, B0, B0, B0 }, { B0, B0, B1, B1, B0, B0, B0 }, { B0, B0, B0, B0, B1, B1, B1 }, { B0, B0, B1, B1, B0, B0, B0 }, { B1, B1, B0, B0, B0, B0, B0 }, { B0, B0, B1, B1, B0, B0, B0 }, { B0, B0, B0, B0, B1, B1, B1 }, { B0, B0, B1, B1, B0, B0, B0 }, { B1, B1, B0, B0, B0, B0, B0 } });
			_characters.Add((char)88, new bool[,] { { B1, B0, B0, B0, B0, B0, B1 }, { B0, B1, B0, B0, B0, B1, B0 }, { B0, B0, B1, B1, B1, B0, B0 }, { B0, B0, B1, B1, B1, B0, B0 }, { B0, B1, B0, B0, B0, B1, B0 }, { B1, B0, B0, B0, B0, B0, B1 } });
			_characters.Add((char)89, new bool[,] { { B1, B1, B0, B0, B0, B0, B0 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B0, B0, B1, B1, B1, B1 }, { B0, B0, B1, B0, B0, B0, B0 }, { B1, B1, B0, B0, B0, B0, B0 } });
			_characters.Add((char)90, new bool[,] { { B1, B0, B0, B0, B0, B1, B1 }, { B1, B0, B0, B1, B1, B0, B1 }, { B1, B0, B1, B0, B0, B0, B1 }, { B1, B1, B0, B0, B0, B0, B1 } });
			_characters.Add((char)91, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B1, B0, B0, B0, B0, B0, B1 } });
			_characters.Add((char)92, new bool[,] { { B0, B1, B0, B0, B0, B0, B0 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B0, B1, B0, B0 }, { B0, B0, B0, B0, B0, B1, B0 } });
			_characters.Add((char)93, new bool[,] { { B1, B0, B0, B0, B0, B0, B1 }, { B1, B1, B1, B1, B1, B1, B1 } });
			_characters.Add((char)94, new bool[,] { { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B0, B1, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B1, B0, B0, B0, B0, B0 } });
			_characters.Add((char)95, new bool[,] { { B0, B0, B1, B1, B1, B0, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B0, B1, B1, B1, B0, B0 } });
			_characters.Add((char)96, new bool[,] { { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 } });
			_characters.Add((char)97, new bool[,] { { B0, B0, B1, B0, B0, B1, B0 }, { B0, B0, B1, B0, B1, B0, B1 }, { B0, B0, B1, B0, B1, B0, B1 }, { B0, B0, B0, B1, B1, B1, B1 } });
			_characters.Add((char)98, new bool[,] { { B1, B1, B1, B1, B1, B1, B1 }, { B0, B0, B1, B0, B0, B0, B1 }, { B0, B0, B1, B0, B0, B0, B1 }, { B0, B0, B0, B1, B1, B1, B0 } });
			_characters.Add((char)99, new bool[,] { { B0, B0, B0, B1, B1, B1, B0 }, { B0, B0, B1, B0, B0, B0, B1 }, { B0, B0, B1, B0, B0, B0, B1 }, { B0, B0, B0, B1, B0, B1, B0 } });
			_characters.Add((char)100, new bool[,] { { B0, B0, B0, B0, B1, B1, B0 }, { B0, B0, B0, B1, B0, B0, B1 }, { B0, B0, B0, B1, B0, B0, B1 }, { B0, B1, B1, B1, B1, B1, B1 } });
			_characters.Add((char)101, new bool[,] { { B0, B0, B0, B1, B1, B1, B0 }, { B0, B0, B1, B0, B1, B0, B1 }, { B0, B0, B1, B0, B1, B0, B1 }, { B0, B0, B0, B1, B1, B0, B1 } });
			_characters.Add((char)102, new bool[,] { { B0, B0, B0, B1, B1, B1, B1 }, { B0, B0, B1, B0, B1, B0, B0 } });
			_characters.Add((char)103, new bool[,] { { B0, B0, B0, B1, B0, B0, B1 }, { B0, B0, B1, B0, B1, B0, B1 }, { B0, B0, B1, B0, B1, B0, B1 }, { B0, B0, B1, B1, B1, B1, B0 } });
			_characters.Add((char)104, new bool[,] { { B0, B1, B1, B1, B1, B1, B1 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B0, B0, B1, B1, B1 } });
			_characters.Add((char)105, new bool[,] { { B0, B1, B0, B1, B1, B1, B1 } });
			_characters.Add((char)106, new bool[,] { { B0, B0, B0, B0, B0, B0, B1 }, { B0, B1, B0, B1, B1, B1, B0 } });
			_characters.Add((char)107, new bool[,] { { B0, B1, B1, B1, B1, B1, B1 }, { B0, B0, B0, B0, B1, B0, B0 }, { B0, B0, B0, B1, B0, B1, B0 }, { B0, B0, B1, B0, B0, B0, B1 } });
			_characters.Add((char)108, new bool[,] { { B0, B1, B1, B1, B1, B1, B1 } });
			_characters.Add((char)109, new bool[,] { { B0, B0, B1, B1, B1, B1, B1 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B0, B1, B1, B1, B1, B1 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B0, B0, B1, B1, B1, B1 } });
			_characters.Add((char)110, new bool[,] { { B0, B0, B1, B1, B1, B1, B1 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B0, B1, B0, B0, B0, B0 }, { B0, B0, B0, B1, B1, B1, B1 } });
			_characters.Add((char)111, new bool[,] { { B0, B0, B0, B1, B1, B1, B0 }, { B0, B0, B1, B0, B0, B0, B1 }, { B0, B0, B1, B0, B0, B0, B1 }, { B0, B0, B0, B1, B1, B1, B0 } });
			_characters.Add((char)112, new bool[,] { { B0, B0, B1, B1, B1, B1, B1 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B0, B0, B1, B0, B0, B0 } });
			_characters.Add((char)113, new bool[,] { { B0, B0, B0, B1, B0, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B0, B1, B0, B1, B0, B0 }, { B0, B0, B1, B1, B1, B1, B1 } });
			_characters.Add((char)114, new bool[,] { { B0, B0, B1, B1, B1, B1, B1 }, { B0, B0, B1, B0, B0, B0, B0 } });
			_characters.Add((char)115, new bool[,] { { B0, B0, B0, B1, B0, B0, B1 }, { B0, B0, B1, B0, B1, B0, B1 }, { B0, B0, B1, B0, B1, B0, B1 }, { B0, B0, B1, B0, B0, B1, B0 } });
			_characters.Add((char)116, new bool[,] { { B0, B1, B1, B1, B1, B1, B0 }, { B0, B0, B0, B1, B0, B0, B1 }, { B0, B0, B0, B0, B0, B0, B1 } });
			_characters.Add((char)117, new bool[,] { { B0, B0, B1, B1, B1, B1, B0 }, { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B1, B1, B1, B1, B1 } });
			_characters.Add((char)118, new bool[,] { { B0, B0, B1, B1, B0, B0, B0 }, { B0, B0, B0, B0, B1, B1, B0 }, { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B0, B0, B1, B1, B0 }, { B0, B0, B1, B1, B0, B0, B0 } });
			_characters.Add((char)119, new bool[,] { { B0, B0, B1, B1, B1, B0, B0 }, { B0, B0, B0, B0, B0, B1, B1 }, { B0, B0, B1, B1, B1, B0, B0 }, { B0, B0, B0, B0, B0, B1, B1 }, { B0, B0, B1, B1, B1, B0, B0 } });
			_characters.Add((char)120, new bool[,] { { B0, B0, B1, B1, B0, B1, B1 }, { B0, B0, B0, B0, B1, B0, B0 }, { B0, B0, B1, B1, B0, B1, B1 } });
			_characters.Add((char)121, new bool[,] { { B0, B0, B0, B0, B0, B0, B1 }, { B0, B0, B1, B1, B0, B0, B1 }, { B0, B0, B0, B0, B1, B1, B0 }, { B0, B0, B0, B0, B1, B0, B0 }, { B0, B0, B1, B1, B0, B0, B0 } });
			_characters.Add((char)122, new bool[,] { { B0, B0, B1, B0, B0, B1, B1 }, { B0, B0, B1, B0, B1, B0, B1 }, { B0, B0, B1, B0, B1, B0, B1 }, { B0, B0, B1, B1, B0, B0, B1 } });
			_characters.Add((char)123, new bool[,] { { B0, B1, B1, B0, B0, B0, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B1, B1, B1, B1, B0 }, { B0, B1, B1, B1, B0, B0, B0 } });
			_characters.Add((char)124, new bool[,] { { B0, B1, B1, B1, B1, B0, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B1, B1, B1, B1, B1, B0 }, { B0, B1, B1, B1, B1, B0, B0 } });
			_characters.Add((char)125, new bool[,] { { B0, B0, B0, B1, B0, B0, B0 }, { B1, B0, B1, B0, B1, B1, B1 }, { B1, B1, B0, B0, B0, B1, B1 }, { B1, B1, B1, B0, B1, B0, B1 }, { B0, B0, B0, B1, B0, B0, B0 } });
			_characters.Add((char)126, new bool[,] { { B0, B1, B1, B1, B0, B0, B0 }, { B1, B0, B0, B0, B1, B1, B0 }, { B1, B0, B1, B1, B1, B1, B0 }, { B1, B0, B0, B0, B1, B1, B0 }, { B0, B1, B1, B1, B0, B0, B0 } });
			_characters.Add((char)127, new bool[,] { { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 }, { B0, B0, B0, B0, B0, B0, B0 } });
		}
	}
}