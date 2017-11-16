// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.IO;

namespace CivOne
{
	internal partial class GameWindow : SDL.Window
	{
		private readonly Runtime _runtime;

		private static Settings Settings => Settings.Instance;

		private int _mouseX = -1, _mouseY = -1;

		private bool _mouseCursorVisible = true;
		private bool _hasUpdate = true;
		private bool _settingsFullscreen = Settings.FullScreen;

		private void Load(object sender, EventArgs args)
		{
			Runtime.CanvasSize = SetCanvasSize();
			_runtime.InvokeInitialize();
		}

		private void Update(object sender, EventArgs args)
		{
			UpdateEventArgs updateArgs = UpdateEventArgs.Empty;
			_runtime.InvokeUpdate(ref updateArgs);
			_hasUpdate = (_hasUpdate || updateArgs.HasUpdate);

			if (_settingsFullscreen != Settings.FullScreen)
			{
				_settingsFullscreen = Settings.FullScreen;
				Fullscreen = _settingsFullscreen;
			}
			CursorVisible = !(Settings.CursorType != CursorType.Native || _runtime.CurrentCursor == MouseCursor.None);
			Runtime.CanvasSize = SetCanvasSize();
			if (_runtime.SignalQuit) StopRunning();
		}

		private void Draw(object sender, EventArgs args)
		{
			if (!_hasUpdate) return;
			_runtime.InvokeDraw();
			_hasUpdate = false;

			Clear(Color.Black);
			GetBorders(out int offsetX, out int offsetY, out _, out _);
			DrawBitmap(_runtime.Bitmap, offsetX, offsetY, ScaleX, ScaleY);
			DrawBitmap(_runtime.Cursor, offsetX + (_mouseX * ScaleX), offsetY + (_mouseY * ScaleY), ScaleX, ScaleY);
		}

		private ScreenEventArgs Transform(ScreenEventArgs args)
		{
			GetBorders(out int offsetX, out int offsetY, out _, out _);
			int x = args.X - offsetX - (args.X % ScaleX);
			int y = args.Y - offsetY - (args.Y % ScaleY);
			if (args.Buttons == MouseButton.None)
				return new ScreenEventArgs(x / ScaleX, y / ScaleY);
			return new ScreenEventArgs(x / ScaleX, y / ScaleY, args.Buttons);
		}

		private void KeyDown(object sender, KeyboardEventArgs args)
		{
			if (args.Modifier == KeyModifier.Alt && args.Key == Key.Enter)
			{
				Fullscreen = !Fullscreen;
				return;
			}
			_runtime.InvokeKeyboardDown(args);
		}

		private void KeyUp(object sender, KeyboardEventArgs args) => _runtime.InvokeKeyboardUp(args);

		private void MouseMove(object sender, ScreenEventArgs args)
		{
			args = Transform(args);
			if (args.X == _mouseX && args.Y == _mouseY) return;
			_hasUpdate = true;
			_mouseX = args.X;
			_mouseY = args.Y;
			_runtime.InvokeMouseMove(args);
		}

		private void MouseDown(object sender, ScreenEventArgs args) => _runtime.InvokeMouseDown(Transform(args));
		private void MouseUp(object sender, ScreenEventArgs args) => _runtime.InvokeMouseUp(Transform(args));

		public GameWindow(Runtime runtime) : base("CivOne", InitialWidth, InitialHeight, Settings.FullScreen)
		{
			_runtime = runtime;

			OnLoad += Load;
			OnUpdate += Update;
			OnDraw += Draw;
			OnKeyDown += KeyDown;
			OnKeyUp += KeyUp;
			OnMouseMove += MouseMove;
			OnMouseDown += MouseDown;
			OnMouseUp += MouseUp;
		}
	}
}