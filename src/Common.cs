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
using CivOne.Civilizations;
using CivOne.Interfaces;

namespace CivOne
{
	internal class Common
	{
		public static Random Random = new Random((int)DateTime.Now.Ticks);
		
		public static IAdvance[] Advances = Reflect.GetAdvances().ToArray();
		public static ICivilization[] Civilizations = new ICivilization[] { new Roman(), new Babylonian(), new German(), new Egyptian(), new American(), new Greek(), new Indian(), new Russian(), new Zulu(), new French(), new Aztec(), new Chinese(), new English(), new Mongol(), new Barbarian() };
		public static byte[] ColourLight = new byte[] { 12, 15, 10, 9, 14, 11, 13, 7 };
		public static byte[] ColourDark = new byte[] { 4, 7, 2, 1, 10, 3, 4, 8 };
		
		internal static IEnumerable<string> AllCityNames
		{
			get
			{
				foreach (ICivilization civilization in Civilizations)
				{
					foreach (string cityName in civilization.CityNames)
					{
						yield return cityName;
					}
				}
			}
		}

		private static List<IScreen> _screens = new List<IScreen>();
		internal static IScreen[] Screens
		{
			get
			{
				return _screens.ToArray();
			}
		}
		
		internal static void SetRandomSeed(short seed)
		{
			Random = new Random(seed);
		}
		
		internal static void AddScreen(IScreen screen)
		{
			_screens.Add(screen);
		}
		
		internal static void DestroyScreen(IScreen screen)
		{
			_screens.Remove(screen);
		}
		
		internal static bool HasScreenType(Type type)
		{
			foreach (IScreen screen in _screens)
			{
				if (screen.GetType() == type) return true;
			}
			return false;
		}
		
		internal static string CaptureFilename
		{
			get
			{
				for (int i = 1; i < 99999; i++)
				{
					string filename = Path.Combine(Settings.Instance.CaptureDirectory, string.Format("capture{0:00000}.png", i));
					if (File.Exists(filename)) continue;
					return filename;
				}
				
				Console.WriteLine("Error: Capture folder is full.");
				return null;
			}
		}
		
		internal static bool EndGame
		{
			get; private set;
		}
		internal static void Quit()
		{
			EndGame = true;
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
		
		public static bool InCityRange(int x1, int y1, int x2, int y2)
		{
			return new Rectangle(x2 - 2, y2 - 2, 5, 5).IntersectsWith(new Rectangle(x1, y1, 1, 1));
		}
		
		public static int DistanceToTile(int x1, int y1, int x2, int y2)
		{
			return Math.Min(Math.Abs(x1 - x2), Math.Abs(y1 - y2));
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
		
		private static Color[] _palette16;
		public static Color[] GetPalette16
		{
			get
			{
				if (_palette16 == null)
				{
					byte[] shades = new byte[] { 0, 104, 183, 255 };
					_palette16 = new Color[]
					{
						Color.Transparent,
						Color.FromArgb(shades[0], shades[0], shades[2]),
						Color.FromArgb(shades[0], shades[2], shades[0]),
						Color.FromArgb(shades[0], shades[2], shades[2]),
						Color.FromArgb(shades[2], shades[0], shades[0]),
						Color.FromArgb(shades[0], shades[0], shades[0]),
						Color.FromArgb(shades[2], shades[1], shades[0]),
						Color.FromArgb(shades[2], shades[2], shades[2]),
						Color.FromArgb(shades[1], shades[1], shades[1]),
						Color.FromArgb(shades[1], shades[1], shades[3]),
						Color.FromArgb(shades[1], shades[3], shades[1]),
						Color.FromArgb(shades[1], shades[3], shades[3]),
						Color.FromArgb(shades[3], shades[1], shades[1]),
						Color.FromArgb(shades[3], shades[1], shades[3]),
						Color.FromArgb(shades[3], shades[3], shades[1]),
						Color.FromArgb(shades[3], shades[3], shades[3]),
					};
				}
				return _palette16;
			}
		}
	}
}