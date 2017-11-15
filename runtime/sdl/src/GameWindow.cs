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
	internal class GameWindow : SDL.Window
	{
		private readonly Runtime _runtime;

		private Settings Settings => Settings.Instance;

		private int _mouseX = 10, _mouseY = 10;

		private bool _mouseCursorVisible = true;
		private bool _hasUpdate = true;

		private void Load(object sender, EventArgs args)
		{
			_runtime.InvokeInitialize();
		}

		private void Update(object sender, EventArgs args)
		{
			UpdateEventArgs updateArgs = UpdateEventArgs.Empty;
			_runtime.InvokeUpdate(ref updateArgs);
			_hasUpdate = (_hasUpdate || updateArgs.HasUpdate);

			CursorVisible = !(Settings.CursorType != CursorType.Native || _runtime.CurrentCursor == MouseCursor.None);
		}

		private void Draw(object sender, EventArgs args)
		{
			if (!_hasUpdate) return;
			_runtime.InvokeDraw();
			_hasUpdate = false;

			Clear(Color.Black);
			DrawBitmap(_runtime.Bitmap, 0, 0, 2, 2);
			DrawBitmap(_runtime.Cursor, _mouseX * 2, _mouseY * 2, 2, 2);
		}

		private ScreenEventArgs Transform(ScreenEventArgs args)
		{
			int x = args.X - (args.X % 2);
			int y = args.Y - (args.Y % 2);
			if (args.Buttons == MouseButton.None)
				return new ScreenEventArgs(x / 2, y / 2);
			return new ScreenEventArgs(x / 2, y / 2, args.Buttons);
		}

		private void KeyDown(object sender, KeyboardEventArgs args) => _runtime.InvokeKeyboardDown(args);

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

		public GameWindow(Runtime runtime) : base("CivOne")
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