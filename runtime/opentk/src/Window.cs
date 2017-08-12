// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using CivOne.Enums;
using CivOne.Events;

namespace CivOne
{
	internal partial class Window : GameWindow
	{
		private static Settings Settings => Settings.Instance;

		private readonly Runtime _runtime;
		
		private void Log(string value, params object[] formatArgs) => _runtime.Log(value, formatArgs);

		private WindowState _previousState = WindowState.Normal;

		private bool _hasUpdate = false;
		private bool HasUpdate
		{
			get
			{
				if (_hasUpdate)
				{
					_hasUpdate = false;
					return true;
				}
				return false;
			}
			set
			{
				if (!value) return;
				_hasUpdate = true;
			}
		}

		private void ToggleFullscreen()
		{
			if (WindowState == WindowState.Fullscreen)
			{
				Log("Windowed mode");
				WindowState = _previousState;
				return;
			}
			Log("Fullscreen mode");
			_previousState = WindowState;
			WindowState = WindowState.Fullscreen;
		}

		private void WindowResize(object sender, EventArgs args)
		{
			if (WindowState == WindowState.Minimized) return;
			GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
		}

		private void WindowChangeFocus(object sender, EventArgs args) => WindowResize(sender, args);

		private void WindowLoad(object sender, EventArgs args)
		{
			InitializeGraphics();
			InitializeKeyboard();
			InitializeMouse();

			_runtime.InvokeInitialize();
		}

		private void WindowUpdate(object sender, FrameEventArgs args)
		{
			if (_runtime.SignalQuit)
			{
				Close();
				return;
			}
			UpdateEventArgs updateArgs = UpdateEventArgs.Empty;
			_runtime.InvokeUpdate(ref updateArgs);
			HasUpdate = updateArgs.HasUpdate;
		}

		private void WindowRender(object sender, FrameEventArgs args)
		{
			_runtime.InvokeDraw();

			Canvas canvas = new Canvas(_runtime.Bitmap);
			Canvas cursor = null;
			if (canvas == null) return;
			if (_runtime.Cursor != null) cursor = new Canvas(_runtime.Cursor);

			Render(canvas, cursor);
		}

		private static int InitialCanvasWidth => DefaultCanvasSize.Width;
		private static int InitialCanvasHeight => DefaultCanvasSize.Height;

		private static int InitialWidth => InitialCanvasWidth * Settings.Scale;
		private static int InitialHeight => InitialCanvasHeight * Settings.Scale;

		internal Window(Runtime runtime) : base(InitialWidth, InitialHeight, GraphicsMode.Default, "CivOne", GameWindowFlags.Default, DisplayDevice.Default, 1, 0, GraphicsContextFlags.ForwardCompatible)
		{
			_runtime = runtime;

			// Run OS native functions for initialization
			Native.Init(WindowInfo.Handle);

			// Bind events
			Load += WindowLoad;
			Resize += WindowResize;
			FocusedChanged += WindowChangeFocus;
			UpdateFrame += WindowUpdate;
			RenderFrame += WindowRender;
		}
	}
}