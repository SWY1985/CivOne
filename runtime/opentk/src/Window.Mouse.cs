// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using OpenTK.Input;
using CivOne.Events;

using CButton = CivOne.Enums.MouseButton;

namespace CivOne
{
	internal partial class Window
	{
		private int _mouseX, _mouseY;
		private CButton _mouseButtons;

		private void InitializeMouse()
		{
			MouseUp += MouseButtonUp;
			MouseDown += MouseButtonDown;
			MouseMove += MouseMoved;
		}

		private void MouseButtonUp(object sender, MouseButtonEventArgs args)
		{
			CButton buttons = CButton.None;
			if (args.Mouse.IsButtonUp(MouseButton.Left) && (_mouseButtons & CButton.Left) > 0) buttons |= CButton.Left;
			if (args.Mouse.IsButtonUp(MouseButton.Right) && (_mouseButtons & CButton.Right) > 0) buttons |= CButton.Right;

			_runtime.InvokeMouseUp(new ScreenEventArgs(_mouseX, _mouseY, buttons));
		}

		private void MouseButtonDown(object sender, MouseButtonEventArgs args)
		{
			CButton buttons = CButton.None;
			if (args.Mouse.IsButtonDown(MouseButton.Left)) buttons |= CButton.Left;
			if (args.Mouse.IsButtonDown(MouseButton.Right)) buttons |= CButton.Right;
			_mouseButtons = buttons;

			_runtime.InvokeMouseDown(new ScreenEventArgs(_mouseX, _mouseY, buttons));
		}

		private void MouseMoved(object sender, MouseMoveEventArgs args)
		{
			GetBorders(out int x1, out int y1, out int x2, out int y2);

			bool mouseMove =
				(_mouseX != (_mouseX = (int)((Mouse.X - x1) / ScaleX))) |
				(_mouseY != (_mouseY = (int)((Mouse.Y - y1) / ScaleY)));
			
			if (!mouseMove) return;

			MouseState mouse = Mouse.GetState();
			CButton buttons = CButton.None;
			if (mouse.IsButtonDown(MouseButton.Left)) buttons |= CButton.Left;
			if (mouse.IsButtonDown(MouseButton.Right)) buttons |= CButton.Right;

			_runtime.InvokeMouseMove(new ScreenEventArgs(_mouseX, _mouseY, buttons));
		}
	}
}