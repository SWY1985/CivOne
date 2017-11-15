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
using CivOne.GameSave;
using CivOne.Graphics.Sprites;

namespace CivOne
{
	public class Settings
	{
		private static IRuntime Runtime => RuntimeHandler.Runtime;
		private static void Log(string text, params object[] parameters) => RuntimeHandler.Runtime.Log(text, parameters);

		// Set default settings
		private GraphicsMode _graphicsMode = GraphicsMode.Graphics256;
		private bool _fullScreen = false;
		private bool _rightSideBar = false;
		private int _scale = 2;
		private AspectRatio _aspectRatio = AspectRatio.Auto;
		private int _expandWidth, _expandHeight;
		private bool _revealWorld = false;
		private bool _debugMenu = false;
		private bool _deityEnabled = false;
		private bool _arrowHelper = false;
		private CursorType _cursorType = CursorType.Default;
		private DestroyAnimation _destroyAnimation = DestroyAnimation.Sprites;
		
		internal string StorageDirectory => Directory.GetCurrentDirectory();
		internal string CaptureDirectory => Path.Combine(StorageDirectory, "capture");
		internal string DataDirectory => Path.Combine(StorageDirectory, "data");
		internal string PluginsDirectory => Path.Combine(StorageDirectory, "plugins");

		internal string SavesDirectory
		{
		    get
		    {
		        if (GameSavesMode == GameSavesMode.ORIGINAL)
		            return Path.Combine(StorageDirectory, "saves", "original");

		        return Path.Combine(StorageDirectory, "saves", "json");
            }
		}

	    private string SettingsDirectory => Path.Combine(StorageDirectory, "settings");
		internal string SoundsDirectory => Path.Combine(StorageDirectory, "sounds");

		// Settings
		
		internal GraphicsMode GraphicsMode
		{
			get
			{
				return _graphicsMode;
			}
			set
			{
				_graphicsMode = value;
				string saveValue = _graphicsMode == GraphicsMode.Graphics256 ? "1" : "2";
				SetSetting("GraphicsMode", saveValue);
				Common.ReloadSettings = true;
			}
		}

	    public GameSavesMode GameSavesMode { get; set; }

		public bool FullScreen
		{
			get
			{
				return _fullScreen;
			}
			set
			{
				_fullScreen = value;
				SetSetting("FullScreen", _fullScreen ? "1" : "0");
				Common.ReloadSettings = true;
			}
		}
		
		public int Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				if (value < 1 || value > 4) return;
				_scale = value;
				SetSetting("Scale", _scale.ToString());
				Common.ReloadSettings = true;
			}
		}

		public AspectRatio AspectRatio
		{
			get
			{
				return _aspectRatio;
			}
			set
			{
				_aspectRatio = value;
				string saveValue = ((int)_aspectRatio).ToString();
				SetSetting("AspectRatio", saveValue);
				Common.ReloadSettings = true;
			}
		}

		public int ExpandWidth
		{
			get
			{
				return _expandWidth;
			}
			set
			{
				_expandWidth = value;
				string saveValue = ((int)_expandWidth).ToString();
				SetSetting("ExpandWidth", saveValue);
				Common.ReloadSettings = true;
			}
		}

		public int ExpandHeight
		{
			get
			{
				return _expandHeight;
			}
			set
			{
				_expandHeight = value;
				string saveValue = ((int)_expandHeight).ToString();
				SetSetting("ExpandHeight", saveValue);
				Common.ReloadSettings = true;
			}
		}

		// Patches
		
		internal bool RevealWorld
		{
			get
			{
				return _revealWorld;
			}
			set
			{
				_revealWorld = value;
				SetSetting("RevealWorld", _revealWorld ? "1" : "0");
				Common.ReloadSettings = true;
			}
		}
		
		internal bool RightSideBar
		{
			get
			{
				return _rightSideBar;
			}
			set
			{
				_rightSideBar = value;
				SetSetting("SideBar", _rightSideBar ? "1" : "0");
				Common.ReloadSettings = true;
			}
		}
		
		internal bool DebugMenu
		{
			get
			{
				return _debugMenu;
			}
			set
			{
				_debugMenu = value;
				SetSetting("DebugMenu", _debugMenu ? "1" : "0");
				Common.ReloadSettings = true;
			}
		}
		
		internal bool DeityEnabled
		{
			get
			{
				return _deityEnabled;
			}
			set
			{
				_deityEnabled = value;
				SetSetting("DeityEnabled", _deityEnabled ? "1" : "0");
				Common.ReloadSettings = true;
			}
		}

		internal bool ArrowHelper
		{
			get
			{
				return _arrowHelper;
			}
			set
			{
				_arrowHelper = value;
				SetSetting("ArrowHelper", _arrowHelper ? "1" : "0");
				Common.ReloadSettings = true;
			}
		}
		
		public CursorType CursorType
		{
			get
			{
				if (Runtime.Settings.Free && _cursorType == CursorType.Default)
					return CursorType.Builtin;
				return _cursorType;
			}
			internal set
			{
				_cursorType = value;
				string saveValue = ((int)_cursorType).ToString();
				SetSetting("CursorType", saveValue);
				Cursor.ClearCache();
				Common.ReloadSettings = true;
			}
		}

		internal DestroyAnimation DestroyAnimation
		{
			get
			{
				return _destroyAnimation;
			}
			set
			{
				_destroyAnimation = value;
				string saveValue = ((int)_destroyAnimation).ToString();
				SetSetting("DestroyAnimation", saveValue);
				Common.ReloadSettings = true;
			}
		}

		// In game settings

		internal bool Animations { get; set; }
		internal bool Sound { get; set; }
		internal bool CivilopediaText { get; set; }
		internal bool EndOfTurn { get; set; }
		internal bool InstantAdvice { get; set; }
		internal bool AutoSave { get; set; }
		internal bool EnemyMoves { get; set; }

		internal void RevealWorldCheat()
		{
			_revealWorld = !_revealWorld;
		}
		
		internal int ScaleX
		{
			get
			{
				return _scale;
			}
		}
		
		internal int ScaleY
		{
			get
			{
				return _scale;
			}
		}
		
		private static Settings _instance;
		public static Settings Instance
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
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			using (StreamReader sr = new StreamReader(fs))
			{
				string value = sr.ReadToEnd();
				
				Log("Setting Loaded - {0}: {1}", settingName, value);
				
				return value.Trim();
			}
		}
		
		private void SetSetting(string settingName, string value)
		{
			string filename = Path.Combine(SettingsDirectory, settingName);
			using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
			using (StreamWriter sw = new StreamWriter(fs))
			{
				sw.Write(value);
				
				Log("Setting Saved - {0}: {1}", settingName, value);
			}
		}
		
		private void CreateDirectories()
		{
			foreach (string dir in new[] { CaptureDirectory, DataDirectory, PluginsDirectory, SavesDirectory, SettingsDirectory, SoundsDirectory })
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			
			for (char c = 'a'; c <= 'z'; c++)
			{
				string dir = Path.Combine(SavesDirectory, c.ToString());
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
			}
		}
		
		private Settings()
		{
			CreateDirectories();
			
			int graphicsMode = (int)_graphicsMode;
			bool fullScreen = _fullScreen;
			bool rightSideBar = _rightSideBar;
			int scale = _scale;
			int aspectRatio = (int)_aspectRatio;
			int expandWidth = (int)_expandWidth;
			int expandHeight = (int)_expandHeight;
			bool revealWorld = false;
			bool debugMenu = false;
			bool deityEnabled = false;
			bool arrowHelper = false;
			int cursorType = (int)_cursorType;
			int destroyAnimation = (int)_destroyAnimation;
			
			// Read settings
			Int32.TryParse(GetSetting("GraphicsMode"), out graphicsMode);
			fullScreen = (GetSetting("FullScreen") == "1");
			rightSideBar = (GetSetting("SideBar") == "1");
			Int32.TryParse(GetSetting("Scale"), out scale);
			Int32.TryParse(GetSetting("AspectRatio"), out aspectRatio);
			Int32.TryParse(GetSetting("ExpandWidth"), out expandWidth);
			Int32.TryParse(GetSetting("ExpandHeight"), out expandHeight);
			revealWorld = (GetSetting("RevealWorld") == "1");
			debugMenu = (GetSetting("DebugMenu") == "1");
			deityEnabled = (GetSetting("DeityEnabled") == "1");
			arrowHelper = (GetSetting("ArrowHelper") == "1");
			Int32.TryParse(GetSetting("CursorType"), out cursorType);
			Int32.TryParse(GetSetting("DestroyAnimation"), out destroyAnimation);
			
			// Set settings
			if (graphicsMode > 0 && graphicsMode < 3) _graphicsMode = (GraphicsMode)graphicsMode;
			_fullScreen = fullScreen;
			_rightSideBar = rightSideBar;
			if (scale < 1 || scale > 4) scale = 2;
			_scale = scale;
			_aspectRatio = (AspectRatio)aspectRatio;
			if (expandWidth < 320 || expandWidth > 512 || expandHeight < 200 || expandHeight > 384)
			{
				_expandWidth = -1;
				_expandHeight = -1;
			}
			else
			{
				_expandWidth = expandWidth;
				_expandHeight = expandHeight;
			}
			_revealWorld = revealWorld;
			_debugMenu = debugMenu;
			_deityEnabled = deityEnabled;
			_arrowHelper = arrowHelper;
			_cursorType = (CursorType)cursorType;
			_destroyAnimation = (DestroyAnimation)destroyAnimation;

			// Set game options
			EndOfTurn = false;
			AutoSave = true;
		}
	}
}