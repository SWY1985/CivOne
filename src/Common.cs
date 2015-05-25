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