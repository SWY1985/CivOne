// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.IO;

using TkMouseButton = OpenTK.Input.MouseButton;
using TkMouseArgs = OpenTK.Input.MouseEventArgs;
using TkMouseState = OpenTK.Input.MouseState;

namespace CivOne
{
	internal partial class Window
	{
		private MouseButton _mouseButtons = MouseButton.None;

		private Picture _cursorPointer, _cursorGoto;

		private CursorType _cursorType = CursorType.Native;

		private int _mouseX, _mouseY;

		private void LoadCursorGraphics()
		{
			if (_cursorType == Settings.Instance.CursorType) return;

			_cursorType = Settings.Instance.CursorType;
			
			if (_cursorType == CursorType.Default && !FileSystem.DataFilesExist(FileSystem.MouseCursorFiles))
			{
				_cursorType = CursorType.Builtin;
			}

			_cursorPointer = Icons.Cursor(MouseCursor.Pointer, (_cursorType == CursorType.Builtin));
			_cursorGoto = Icons.Cursor(MouseCursor.Goto, (_cursorType == CursorType.Builtin));
		}

		private void DrawMouseCursor(Picture canvas)
		{
			// Draw the mouse cursor
			if (_cursorType != CursorType.Native && _mouseX >= 0 && _mouseX < DrawWidth && _mouseY >= 0 && _mouseY < DrawHeight)
			{
				switch (TopScreen.Cursor)
				{
					case MouseCursor.Pointer:
						canvas.AddLayer(_cursorPointer, _mouseX, _mouseY);
						break;
					case MouseCursor.Goto:
						canvas.AddLayer(_cursorGoto, _mouseX, _mouseY);
						break;
				}
			}
		}

		private void OnMouseDown(object sender, TkMouseArgs args)
		{
			if (TopScreen == null) return;

			MouseButton buttons = MouseButton.None;
			if (args.Mouse.IsButtonDown(TkMouseButton.Left)) buttons |= MouseButton.Left;
			if (args.Mouse.IsButtonDown(TkMouseButton.Right)) buttons = MouseButton.Right;
			_mouseButtons = buttons;
			TopScreen.MouseDown(new ScreenEventArgs(_mouseX, _mouseY, buttons));
		}

		private void OnMouseUp(object sender, TkMouseArgs args)
		{
			if (TopScreen == null) return;

			MouseButton buttons = MouseButton.None;
			if (args.Mouse.IsButtonUp(TkMouseButton.Left) && (_mouseButtons & MouseButton.Left) > 0) buttons |= MouseButton.Left;
			if (args.Mouse.IsButtonUp(TkMouseButton.Right) && (_mouseButtons & MouseButton.Right) > 0) buttons |= MouseButton.Right;
			TopScreen.MouseUp(new ScreenEventArgs(_mouseX, _mouseY, buttons));
		}

		private void OnMouseMove(float scaleX, float scaleY)
		{
			int x1, y1, x2, y2;
			GetBorders(out x1, out y1, out x2, out y2);

			bool mouseMove =
				(_mouseX != (_mouseX = (int)((Mouse.X - x1) / scaleX))) |
				(_mouseY != (_mouseY = (int)((Mouse.Y - y1) / scaleY)));
			
			if (mouseMove)
			{
				MouseButton buttons = MouseButton.None;
				TkMouseState mouse = Mouse.GetState();
				
				if (mouse.IsButtonDown(TkMouseButton.Left)) buttons |= MouseButton.Left;
				if (mouse.IsButtonDown(TkMouseButton.Right)) buttons |= MouseButton.Right;
				if (buttons != MouseButton.None)
				{
					TopScreen?.MouseDrag(new ScreenEventArgs(_mouseX, _mouseY, buttons));
				}

				TopScreen?.MouseMove(new ScreenEventArgs(_mouseX, _mouseY, buttons));
			}

			CursorVisible = (_cursorType == CursorType.Native) || (_mouseX <= 0 || _mouseX >= (CanvasWidth - 1) || _mouseY <= 0 || _mouseY >= (CanvasHeight - 1));
		}
	}
}