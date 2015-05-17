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
		// Set default settings
		private readonly GraphicsMode _graphicsMode = GraphicsMode.Graphics256;
		private readonly byte _framesPerSecond = 15; 
		
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
		
		internal byte FramesPerSecond
		{
			get
			{
				return _framesPerSecond;
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
			int graphicsMode = (int)_graphicsMode;
			byte framesPerSecond = _framesPerSecond;
						
			// Read settings
			Int32.TryParse(GetSetting("GraphicsMode"), out graphicsMode);
			Byte.TryParse(GetSetting("FramesPerSecond"), out framesPerSecond);
						
			// Set settings
			if (graphicsMode > 0 && graphicsMode < 3) _graphicsMode = (GraphicsMode)graphicsMode;
			if (framesPerSecond > 5 && framesPerSecond < 61) _framesPerSecond = framesPerSecond;
		}
	}
}