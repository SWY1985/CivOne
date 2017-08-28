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
using System.IO;
using System.Linq;
using System.Text;
using CivOne.Advances;
using CivOne.Buildings;
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Screens;
using CivOne.Wonders;

namespace CivOne
{
	internal class Common
	{
		private static Resources Resources => Resources.Instance;
		private static IRuntime Runtime => RuntimeHandler.Runtime;
		private static Settings Settings => Settings.Instance;
		private static void Log(string text, params object[] parameters) => RuntimeHandler.Runtime.Log(text, parameters);

		public static Random Random = new Random((int)DateTime.Now.Ticks);
		
		public static IAdvance[] Advances = Reflect.GetAdvances().ToArray();
		public static IBuilding[] Buildings = Reflect.GetBuildings().ToArray();
		public static IWonder[] Wonders = Reflect.GetWonders().ToArray();
		public static ICivilization[] Civilizations => Reflect.GetCivilizations().ToArray();
		public static byte[] ColourLight = new byte[] { 12, 15, 10, 9, 14, 11, 13, 7 };
		public static byte[] ColourDark = new byte[] { 4, 7, 2, 1, 10, 3, 4, 8 };
		
		internal static IEnumerable<string> AllCityNames => Civilizations.Select(x => x.CityNames).SelectMany(x => x);

		private static List<IScreen> _screens = new List<IScreen>();
		internal static IScreen[] Screens
		{
			get
			{
				return _screens.ToArray();
			}
		}

		internal static bool HasAttribute<T>(object checkObject) where T : Attribute
		{
			if (checkObject == null)
				return false;
			return Attribute.IsDefined(checkObject.GetType(), typeof(T));
		}

		public static IScreen TopScreen
		{
			get
			{
				if (_screens.Any(x => HasAttribute<Modal>(x)))
					return _screens.Last(x => HasAttribute<Modal>(x));
				return _screens.LastOrDefault();
			}
		}

		public static MouseCursor MouseCursor
		{
			get
			{
				if (TopScreen == null)
					return MouseCursor.None;
				return TopScreen.Cursor;
			}
		}

		public static Palette DefaultPalette
		{
			get
			{
				GamePlay gamePlay = GamePlay;
				if (gamePlay != null)
					return gamePlay.MainPalette.Copy();
				return Resources["SP257"].Palette.Copy();
			}
		}

		public static GamePlay GamePlay => (GamePlay)_screens.FirstOrDefault(x => x is GamePlay);

		public static Palette GamePlayPalette => GamePlay.Palette.Copy();

		internal static void SetRandomSeedFromName(string name)
		{
			short number = 0;
			foreach (byte charByte in name)
			{
				number += charByte;
			}
			SetRandomSeed(number);
		}
		
		internal static void SetRandomSeed(short seed) => Random = new Random(seed);
		
		internal static void AddScreen(IScreen screen) => _screens.Add(screen);
		
		internal static void DestroyScreen(IScreen screen)
		{
			screen.Dispose();
			_screens.Remove(screen);
		}
		
		internal static bool HasScreenType<T>() where T : IScreen
		{
			return _screens.Any(x => x is T);
		}
		
		internal static string CaptureFilename
		{
			get
			{
				for (int i = 1; i < 99999; i++)
				{
					string filename = Path.Combine(Settings.Instance.CaptureDirectory, $"capture{i:00000}.gif");
					if (File.Exists(filename)) continue;
					return filename;
				}
				
				Log("Error: Capture folder is full.");
				return null;
			}
		}
		
		private static bool _reloadSettings;
		internal static bool ReloadSettings
		{
			get
			{
				if (_reloadSettings)
				{
					_reloadSettings = false;
					return true;
				}
				return false;
			}
			set
			{
				_reloadSettings = value;
			}
		}

		internal static string NumberSeperator(int number)
		{
			string input = number.ToString();
			input = input.PadLeft(3 - (input.Length % 3) + input.Length, '0');
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				if (sb.Length > 0 && i % 3 == 0) sb.Append(',');
				sb.Append(input[i]);
			}
			return sb.ToString().TrimStart(new char[] { '0', ',' });
		}

		public static ushort YearToTurn(int year)
		{
			if (year < -4000) return 0;
			if (year < 1000) return (ushort)Math.Floor(((double)year + 4000) / 20);
			if (year < 1500) return (ushort)Math.Floor(((double)year + 1500) / 10);
			if (year < 1750) return (ushort)Math.Floor(((double)year) / 5);
			if (year < 1850) return (ushort)Math.Floor(((double)year - 1050) / 2);
			return (ushort)(year - 1450);
		}
		
		public static int TurnToYear(ushort turn)
		{
			if (turn < 200) return -(200 - turn) * 20;
			else if (turn == 200) return 1;
			else if (turn < 250) return (turn - 200) * 20;
			else if (turn < 300) return ((turn - 250) * 10) + 1000;
			else if (turn < 350) return ((turn - 300) * 5) + 1500;
			else if (turn < 400) return ((turn - 350) * 2) + 1750;
			return (turn - 400) + 1850;
		}
		
		public static string YearString(ushort turn)
		{
			int year = TurnToYear(turn);
			if (year < 0)
				return string.Format("{0} BC", -year);
			return string.Format("{0} AD", year);
		}

		public static string DifficultyName(int difficuly)
		{
			switch (difficuly)
			{
				case 1: return "Lord";
				case 2: return "Prince";
				case 3: return "King";
				case 4: return "Emperor";
				case 5: return "Deity";
				default: return "Chief";
			}
		}

		internal static int CitizenGroup(Citizen citizen)
		{
			int output = (int)citizen;
			output -= (output % 2);
			output /= 2;
			if (output > 3) output = 3;
			return output;
		}
		
		public static bool InCityRange(int x1, int y1, int x2, int y2)
		{
			return new Rectangle(x2 - 2, y2 - 2, 5, 5).IntersectsWith(new Rectangle(x1, y1, 1, 1));
		}
		
		public static int DistanceToTile(int x1, int y1, int x2, int y2)
		{
			return Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2));
		}
		
		public static byte BinaryReadByte(BinaryReader reader, int position)
		{
			if (reader.BaseStream.Position != position)
				reader.BaseStream.Seek(position, SeekOrigin.Begin);
			return reader.ReadByte();
		}
		
		public static ushort BinaryReadUShort(BinaryReader reader, int position)
		{
			if (reader.BaseStream.Position != position)
				reader.BaseStream.Seek(position, SeekOrigin.Begin);
			return reader.ReadUInt16();
		}
		
		public static byte[] BinaryReadBytes(BinaryReader reader, int position, int count)
		{
			if (reader.BaseStream.Position != position)
				reader.BaseStream.Seek(position, SeekOrigin.Begin);
			return reader.ReadBytes(count);
		}
		
		private static string[] BytesToArray(byte[] bytes, int maxLength)
		{
			List<string> output = new List<string>();
			StringBuilder sb = new StringBuilder();
			foreach (byte b in bytes)
			{
				sb.Append((char)b);
				if (sb.Length != maxLength) continue;
				
				output.Add(sb.ToString().Split((char)0)[0].Trim());
				sb.Clear();
			}
			
			return output.ToArray();
		}
		public static string[] BinaryReadStrings(BinaryReader reader, int position, int length, int itemLength)
		{
			if (reader.BaseStream.Position != position)
				reader.BaseStream.Seek(position, SeekOrigin.Begin);
			return BytesToArray(reader.ReadBytes(length), itemLength);
		}
		
		private static Palette _palette16;
		public static Palette GetPalette16
		{
			get
			{
				if (_palette16 == null)
				{
					byte[] shades = new byte[] { 0, 104, 183, 255 };
					_palette16 = new Colour[]
					{
						Colour.Transparent,
						new Colour(shades[0], shades[0], shades[2]),
						new Colour(shades[0], shades[2], shades[0]),
						new Colour(shades[0], shades[2], shades[2]),
						new Colour(shades[2], shades[0], shades[0]),
						new Colour(shades[0], shades[0], shades[0]),
						new Colour(shades[2], shades[1], shades[0]),
						new Colour(shades[2], shades[2], shades[2]),
						new Colour(shades[1], shades[1], shades[1]),
						new Colour(shades[1], shades[1], shades[3]),
						new Colour(shades[1], shades[3], shades[1]),
						new Colour(shades[1], shades[3], shades[3]),
						new Colour(shades[3], shades[1], shades[1]),
						new Colour(shades[3], shades[1], shades[3]),
						new Colour(shades[3], shades[3], shades[1]),
						new Colour(shades[3], shades[3], shades[3]),
					};
				}
				return _palette16;
			}
		}

		private static Palette _palette256;
		public static Palette GetPalette256
		{
			get
			{
				if (_palette256 == null)
				{
					_palette256 = new Palette(256);
					for (int i = 0; i < 256; i++)
					{
						if (i >= 16 && i < 32)
						{
							int ii = (i % 16);
							_palette256[i] = new Colour(254 - (ii * 16), 253 - (ii * 16), 252 - (ii * 16));
							continue;
						}
						if (i >= 32 && i < 40)
						{
							// Greens
							int ii = (i % 8);
							_palette256[i] = new Colour(0, 197 - (ii * 8), 80 - (ii * 7));
							continue;
						}
						if (i >= 40 && i < 48)
						{
							// Yellows
							int ii = (i % 8);
							_palette256[i] = new Colour(254 - (ii * 6), 245 - (ii * 6), 0);
							continue;
						}
						if (i >= 48 && i < 64)
						{
							int r = Convert.ToInt32((float)_palette16[i % 16].R * 0.7F);
							int g = Convert.ToInt32((float)_palette16[i % 16].G * 0.7F);
							int b = Convert.ToInt32((float)_palette16[i % 16].B * 0.7F);
							_palette256[i] = new Colour(r, g, b);
							continue;
						}
						if (i >= 64 && i < 80)
						{
							// Blues
							int ii = (i % 8);
							_palette256[i] = new Colour(0, 67 - (ii * 5), 211 - (ii * 9));
							continue;
						}
						_palette256[i] = GetPalette16[i % 16];
					}
				}
				return _palette256;
			}
		}
	}
}