// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CivOne.Attributes;
using CivOne.Enums;
using CivOne.Events;
using CivOne.IO;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne
{
	public class RuntimeHandler
	{
		private static RuntimeHandler _instance;
		internal static RuntimeHandler Instance => _instance;
		internal static IRuntime Runtime { get; private set; }
		
		private IScreen TopScreen => Common.TopScreen;
		private MouseCursor _currentCursor = MouseCursor.None;
		private CursorType _cursorType = CursorType.Native;

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
				return Common.Screens.Last(x => Common.HasAttribute<Modal>(x)).HasUpdate(_gameTick / 4);
			
			bool update = false;
			foreach (IScreen screen in Common.Screens.Reverse())
			{
				if (screen.HasUpdate(_gameTick / 4)) update = true;
				if (Common.HasAttribute<Break>(screen)) return update;
			}
			return update;
		}

		private void OnInitialize(object sender, EventArgs args)
		{
			Common.AddScreen(new Credits());
			if (Runtime.Settings.Setup) Common.AddScreen(new Setup());
			if (Runtime.Settings.Demo) Common.AddScreen(new Demo());
			if (Runtime.Settings.DataCheck && !FileSystem.DataFilesExist()) Common.AddScreen(new MissingFiles());
		}

		private void OnUpdate(object sender, EventArgs args)
		{
			while (_gameTick < TickWatch)
			{
				_gameTick++;
				Update();
			}
		}

		private void OnDraw(object sender, EventArgs args)
		{
			if (TopScreen == null) return;

			Picture bitmap = new Picture(320, 200, Common.TopScreen.Palette);
			bitmap.Palette[0] = Color.Black;
			if (Common.HasAttribute<Modal>(TopScreen))
			{
				bitmap.AddLayer(TopScreen.Canvas);
			}
			else
			{
				foreach (IScreen screen in Common.Screens)
				{
					bitmap.AddLayer(screen.Canvas);
				}
			}
			Runtime.Bitmap = bitmap;

			if (_currentCursor != Common.Cursor || _cursorType != Settings.Instance.CursorType)
			{
				_currentCursor = Common.Cursor;
				_cursorType = Settings.Instance.CursorType;
				Runtime.Cursor = Common.CursorGraphics;
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
				using (CivOne.GFX.ImageFormats.GifFile file = new CivOne.GFX.ImageFormats.GifFile(Runtime.Bitmap))
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

		private void OnMouseUp(object sender, ScreenEventArgs args)
		{
			TopScreen?.MouseUp(args);
		}

		private void OnMouseDown(object sender, ScreenEventArgs args)
		{
			TopScreen?.MouseDown(args);
		}

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
			Task.Run(() => Reflect.PreloadCivilopedia());

			Runtime = runtime;
		
			runtime.Initialize += OnInitialize;
			runtime.Update += OnUpdate;
			runtime.Draw += OnDraw;
			runtime.KeyboardUp += OnKeyboardUp;
			runtime.KeyboardDown += OnKeyboardDown;
			runtime.MouseUp += OnMouseUp;
			runtime.MouseDown += OnMouseDown;
			runtime.MouseMove += OnMouseMove;
		}
	}
}