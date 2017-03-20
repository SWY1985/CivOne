// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
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

		private bool _showCursor = false;
		private MouseCursor _mouseCursor = MouseCursor.None;

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
			_mouseCursor = MouseCursor.None;

			_cursorPointer.Palette[0] = new Color(0, 0, 0, 0);
			_cursorGoto.Palette[0] = new Color(0, 0, 0, 0);

			OnMouseEnter(this, EventArgs.Empty);
		}

		private void UpdateMousePosition()
		{
			float scaleX = (float)ClientSize.Width / CanvasWidth;
			float scaleY = (float)ClientSize.Height / CanvasHeight;
			
			switch (Settings.Instance.AspectRatio)
			{
				case AspectRatio.Scaled:

					break;
				case AspectRatio.ScaledFixed:
					if (scaleX > scaleY) scaleX = scaleY;
					else if (scaleY > scaleX) scaleY = scaleX;
					break;
				default:
					scaleX = ScaleX;
					scaleY = ScaleY;
					break;
			}

			OnMouseMove(scaleX, scaleY);
		}

		private void UpdateMouseTexture()
		{
			if (_mouseCursor != (_mouseCursor = TopScreen.Cursor)) return;
			if (CursorHidden) return;

			switch (_mouseCursor)
			{
				case MouseCursor.Pointer:
					PictureToTexture(TEXTURE_CURSOR, _cursorPointer);
					break;
				case MouseCursor.Goto:
					PictureToTexture(TEXTURE_CURSOR, _cursorGoto);
					break;
			}
		}

		private bool CursorHidden
		{
			get
			{
				return (_cursorType == CursorType.Native || _mouseCursor == MouseCursor.None || !_showCursor);
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

		private void OnMouseEnter(object sender, EventArgs args)
		{
			if (_cursorType != CursorType.Native)
			{
				if (!_showCursor) Native.HideCursor();
				_showCursor = true;
				return;
			}

			if (_showCursor) Native.ShowCursor();
			_showCursor = false;
		}

		private void OnMouseLeave(object sender, EventArgs args)
		{
			int x1, y1, x2, y2;
			GetBorders(out x1, out y1, out x2, out y2);
			if (_mouseButtons != MouseButton.None) return;
			if (_showCursor) Native.ShowCursor();
			_showCursor = false;
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
		}
	}
}