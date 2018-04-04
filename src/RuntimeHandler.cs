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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CivOne.Enums;
using CivOne.Events;
using CivOne.IO;
using CivOne.Graphics;
using CivOne.Graphics.ImageFormats;
using CivOne.Screens;
using CivOne.Graphics.Sprites;
using CivOne.Tasks;
using CivOne.Tiles;

namespace CivOne
{
	public class RuntimeHandler
	{
		private static RuntimeHandler _instance;
		internal static RuntimeHandler Instance => _instance;
		internal static IRuntime Runtime { get; private set; }
		
		private Settings Settings => Settings.Instance;
		private IScreen TopScreen => Common.TopScreen;
		private MouseCursor _currentCursor = MouseCursor.None;
		private CursorType _cursorType = CursorType.Native;

		internal int CanvasWidth => Math.Max(320, Math.Min(512, Runtime.CanvasWidth));
		internal int CanvasHeight => Math.Max(200, Math.Min(384, Runtime.CanvasHeight));

		private Stopwatch _tickWatch = new Stopwatch();
		private uint TickWatch
		{
			get
			{
				if (!_tickWatch.IsRunning)
				{
					_tickWatch.Start();
				}
				return Convert.ToUInt32(((double)_tickWatch.ElapsedMilliseconds / 1000) * 60);
			}
		}
		private uint _gameTick = 0;

		private bool Update()
		{
			if (!GameTask.Update() && (!GameTask.Fast && (_gameTick % 4) > 0)) return false;
			if (Common.Screens.Any(x => Common.HasAttribute<Modal>(x)))
				return Common.Screens.Last(x => Common.HasAttribute<Modal>(x)).Update(_gameTick / 4);
			
			bool update = false;
			foreach (IScreen screen in Common.Screens.Reverse())
			{
				if (screen.Update(_gameTick / 4)) update = true;
				if (Common.HasAttribute<Break>(screen)) return update;
			}
			return update;
		}

		private IEnumerable<Type> StartupScreens
		{
			get
			{
				if (Runtime.Settings.DataCheck && !FileSystem.DataFilesExist()) yield return typeof(MissingFiles);
				if (Runtime.Settings.Demo) yield return typeof(Demo);
				if (Runtime.Settings.Setup) yield return typeof(Setup);
				yield return typeof(Credits);
			}
		}

		private void OnInitialize(object sender, EventArgs args) => GameTask.Enqueue(Show.Screens(StartupScreens));

		private void OnUpdate(object sender, UpdateEventArgs args)
		{
			while (_gameTick < TickWatch)
			{
				_gameTick++;
				if (!Update()) continue;
				args.HasUpdate = true;
			}
		}

		private void OnDraw(object sender, EventArgs args)
		{
			if (TopScreen == null) return;

			if (Runtime.Bitmap == null || Runtime.Bitmap.Width() != CanvasWidth || Runtime.Bitmap.Height() != CanvasHeight)
			{
				Runtime.Bitmap?.Dispose();
				Runtime.Bitmap = new Picture(CanvasWidth, CanvasHeight, Common.TopScreen.Palette.Copy());
				Runtime.Bitmap.Palette[0] = Colour.Black;
			}
			else
			{
				Runtime.Bitmap.Palette.MergePalette(Common.TopScreen.Palette.Copy(), 1);
				Runtime.Bitmap.Bitmap.Clear();
			}
			
			if (Common.HasAttribute<Modal>(TopScreen))
			{
				Runtime.Bitmap.AddLayer(TopScreen);
			}
			else
			{
				foreach (IScreen screen in Common.Screens)
				{
					Runtime.Bitmap.AddLayer(screen);
				}
			}

			if (_currentCursor != Common.MouseCursor || _cursorType != Settings.Instance.CursorType)
			{
				_currentCursor = Common.MouseCursor;
				_cursorType = Settings.Instance.CursorType;
				Runtime.CurrentCursor = _currentCursor;
				if (Cursor.Current?.Bitmap != null)
				{
					Runtime.Cursor = Cursor.Current.ToBitmap();
				}
				else
				{
					Runtime.Cursor = null;
				}
			}
		}

		private void OnKeyboardUp(object sender, KeyboardEventArgs args)
		{
		}

		private void OnKeyboardDown(object sender, KeyboardEventArgs args)
		{
			if (args[KeyModifier.Control, Key.F5])
			{
				string filename = Common.CaptureFilename;
				using (GifFile file = new GifFile(Runtime.Bitmap))
				using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
				{
					byte[] output = file.GetBytes();
					fs.Write(output, 0, output.Length);
					Runtime.Log($"Screenshot saved: {filename}");
				}
				return;
			}

			if (args[KeyModifier.Control, Key.F6] && Game.Started)
			{
				string filename = Common.CaptureFilename;
				using (IBitmap tilesPicture = Map.Instance[0, 0, Map.WIDTH, Map.HEIGHT].ToBitmap())
				using (GifFile file = new GifFile(tilesPicture))
				using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
				{
					byte[] output = file.GetBytes();
					fs.Write(output, 0, output.Length);
					Runtime.Log($"Screenshot saved: {filename}");
				}
				return;
			}

			TopScreen?.KeyDown(args);
		}

		private void OnMouseUp(object sender, ScreenEventArgs args) => TopScreen?.MouseUp(args);

		private void OnMouseDown(object sender, ScreenEventArgs args) => TopScreen?.MouseDown(args);

		private void OnMouseMove(object sender, ScreenEventArgs args)
		{
			if (args.Buttons != MouseButton.None)
			{
				TopScreen?.MouseDrag(args);
			}
			TopScreen?.MouseMove(args);
		}

		public static void Register(IRuntime runtime)
		{
			if (_instance != null)
			{
				throw new Exception("Only one runtime can be registered.");
			}

			_instance = new RuntimeHandler(runtime);
		}

		private RuntimeHandler(IRuntime runtime)
		{
			Runtime = runtime;

			runtime.Initialize += OnInitialize;
			runtime.Update += OnUpdate;
			runtime.Draw += OnDraw;
			runtime.KeyboardUp += OnKeyboardUp;
			runtime.KeyboardDown += OnKeyboardDown;
			runtime.MouseUp += OnMouseUp;
			runtime.MouseDown += OnMouseDown;
			runtime.MouseMove += OnMouseMove;

			foreach (Plugin plugin in Reflect.Plugins())
			{
				runtime.Log($"Plugin loaded: {plugin.Name} version {plugin.Version} by {plugin.Author}");
			}

			Task.Run(() => Reflect.PreloadCivilopedia());
		}
	}
}