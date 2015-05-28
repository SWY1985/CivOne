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
using System.Text;
using CivOne.Civilizations;
using CivOne.Interfaces;

namespace CivOne
{
	internal class Common
	{
		public static Random Random = new Random((int)DateTime.Now.Ticks);
		
		public static ICivilization[] Civilizations = new ICivilization[] { new Barbarian(), new Roman(), new Babylonian(), new German(), new Egyptian(), new American(), new Greek(), new Indian(), new Russian(), new Zulu(), new French(), new Aztec(), new Chinese(), new English(), new Mongol() };
		
		private static List<IScreen> _screens = new List<IScreen>();
		internal static IScreen[] Screens
		{
			get
			{
				return _screens.ToArray();
			}
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
				return string.Format("{0} B.C.", -year);
			return string.Format("{0} A.D.", year);
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