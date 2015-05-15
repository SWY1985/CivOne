// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.IO;
using CivOne.Enums;

namespace CivOne
{
	internal class Settings
	{
		private readonly GraphicsMode _graphicsMode;
		
		internal string BinDirectory
		{
			get
			{
				return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
			}
		}
		
		internal string DataDirectory
		{
			get
			{
				return Path.Combine(BinDirectory, "data");
			}
		}
		
		private string SettingsDirectory
		{
			get
			{
				return Path.Combine(BinDirectory, "settings");
			}
		}
		
		internal GraphicsMode GraphicsMode
		{
			get
			{
				return _graphicsMode;
			}
		}
		
		internal int ScaleX
		{
			get
			{
				return 2;
			}
		}
		
		internal int ScaleY
		{
			get
			{
				return 2;
			}
		}
		
		private static Settings _instance;
		internal static Settings Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Settings();
				return _instance;
			}
		}
		
		private string GetSetting(string settingName)
		{
			string filename = Path.Combine(SettingsDirectory, settingName);
			if (!File.Exists(filename)) return null;
			using (StreamReader sr = new StreamReader(filename))
			{
				string value = sr.ReadToEnd();
				
				Console.WriteLine("[SETTING] {0}: {1}", settingName, value);
				
				return value;
			}
		}
		
		private Settings()
		{
			// Set default settings
			_graphicsMode = GraphicsMode.Graphics256;
			
			// Read settings
			string graphicsMode = GetSetting("GraphicsMode");
			
			// Override settings
			if (graphicsMode != null && graphicsMode == "2") _graphicsMode = GraphicsMode.Graphics16;
		}
	}
}