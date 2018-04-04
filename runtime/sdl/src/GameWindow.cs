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
			
			Runtime.CanvasSize = SetCanvasSize();
			if (_runtime.SignalQuit) StopRunning();
		}

		private void Draw(object sender, EventArgs args)
		{
			if (!_hasUpdate) return;
			_runtime.InvokeDraw();
			_hasUpdate = false;
			
			Render();
		}

		private void CursorChanged(object sender, EventArgs args)
		{
			CursorVisible = !(Settings.CursorType != CursorType.Native || _runtime.CurrentCursor == MouseCursor.None);
			CursorTexture?.Dispose();
			CursorTexture = CreateTexture(_runtime.Cursor);
		}

		private PointF GetScaleF()
		{
			GetBorders(out int x1, out int y1, out int x2, out int y2);
			float scaleX = (float)x2 / CanvasWidth;
			float scaleY = (float)y2 / CanvasHeight;
			if (Settings.AspectRatio == AspectRatio.ScaledFixed)
			{
				if (scaleX > scaleY) scaleX = scaleY;
				else scaleY = scaleX;
			}
			return new PointF(scaleX, scaleY);
		}

		private ScreenEventArgs Transform(ScreenEventArgs args)
		{
			GetBorders(out int offsetX, out int offsetY, out _, out _);
			int x = args.X - offsetX - (args.X % ScaleX);
			int y = args.Y - offsetY - (args.Y % ScaleY);

			switch (Settings.AspectRatio)
			{
				case AspectRatio.Scaled:
				case AspectRatio.ScaledFixed:
					PointF scaleF = GetScaleF();
					x = (int)((float)args.X / scaleF.X);
					y = (int)((float)args.Y / scaleF.Y);
					break;
				default:
					x /= ScaleX;
					y /= ScaleY;
					break;
			}

			if (args.Buttons == MouseButton.None)
				return new ScreenEventArgs(x, y);
			return new ScreenEventArgs(x, y, args.Buttons);
		}

		private void KeyDown(object sender, KeyboardEventArgs args)
		{
			if (args.Key == Key.None) return;
			if (args.Modifier == KeyModifier.Alt && args.Key == Key.Enter)
			{
				Fullscreen = !Fullscreen;
				return;
			}
			_runtime.InvokeKeyboardDown(args);
		}

		private void KeyUp(object sender, KeyboardEventArgs args)
		{
			if (args.Key == Key.None) return;
			_runtime.InvokeKeyboardUp(args);
		}

		private void MouseMove(object sender, ScreenEventArgs args)
		{
			args = Transform(args);
			if (args.X == _mouseX && args.Y == _mouseY) return;
			_hasUpdate = true;
			_mouseX = args.X;
			_mouseY = args.Y;
			if (Settings.AspectRatio == AspectRatio.ScaledFixed)
			{
				PointF scaleF = GetScaleF();
				GetBorders(out int offsetX, out int offsetY, out _, out _);
				args = new ScreenEventArgs(args.X - (int)((float)offsetX / scaleF.X), args.Y - (int)((float)offsetY / scaleF.Y), args.Buttons);
			}
			_runtime.InvokeMouseMove(args);
		}

		private void MouseDown(object sender, ScreenEventArgs args)
		{
			args = Transform(args);
			if (Settings.AspectRatio == AspectRatio.ScaledFixed)
			{
				PointF scaleF = GetScaleF();
				GetBorders(out int offsetX, out int offsetY, out _, out _);
				args = new ScreenEventArgs(args.X - (int)((float)offsetX / scaleF.X), args.Y - (int)((float)offsetY / scaleF.Y), args.Buttons);
			}
			_runtime.InvokeMouseDown(args);
		}
		private void MouseUp(object sender, ScreenEventArgs args)
		{
			args = Transform(args);
			if (Settings.AspectRatio == AspectRatio.ScaledFixed)
			{
				PointF scaleF = GetScaleF();
				GetBorders(out int offsetX, out int offsetY, out _, out _);
				args = new ScreenEventArgs(args.X - (int)((float)offsetX / scaleF.X), args.Y - (int)((float)offsetY / scaleF.Y), args.Buttons);
			}
			_runtime.InvokeMouseUp(args);
		}

		public GameWindow(Runtime runtime, bool softwareRender) : base("CivOne", InitialWidth, InitialHeight, Settings.FullScreen, softwareRender)
		{
			_runtime = runtime;
			_runtime.CursorChanged += CursorChanged;
			_runtime.SetWindowTitle += (string title) => Title = title;

			OnLoad += Load;
			OnUpdate += Update;
			OnDraw += Draw;
			OnKeyDown += KeyDown;
			OnKeyUp += KeyUp;
			OnMouseMove += MouseMove;
			OnMouseDown += MouseDown;
			OnMouseUp += MouseUp;

			if (!_runtime.Settings.Get<bool>("no-sound"))
			{
				runtime.PlaySound += (string filename) => PlaySound(filename);
				runtime.StopSound += () => StopSound();
			}
		}
	}
}