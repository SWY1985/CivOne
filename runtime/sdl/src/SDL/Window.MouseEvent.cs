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

namespace CivOne
{
	internal static partial class SDL
	{
		internal abstract partial class Window
		{
			protected ScreenEventHandler OnMouseMove, OnMouseUp, OnMouseDown;

			private readonly bool[] _mouseButtonState = new bool[3];
			protected int MouseX { get; private set; }
			protected int MouseY { get; private set; }

			private bool _cursorVisible = true;
			protected bool CursorVisible
			{
				get => _cursorVisible;
				set
				{
					if (value == _cursorVisible) return;
					_cursorVisible = value;
					SDL_ShowCursor(_cursorVisible ? 1 : 0);
				}
			}

			private void CheckMouseButton(MouseButton button, uint buttonMask, int mask)
			{
				bool state = (buttonMask & mask) > 0;
				if (_mouseButtonState[(int)button] == state) return;
				_mouseButtonState[(int)button] = state;
				if (state)
				{
					OnMouseDown?.Invoke(this, new ScreenEventArgs(MouseX, MouseY, button));
				}
				else
				{
					OnMouseUp?.Invoke(this, new ScreenEventArgs(MouseX, MouseY, button));
				}
			}

			private void HandleMouse()
			{
				uint buttonMask = SDL_GetMouseState(out int x, out int y);
				if (MouseX != x || MouseY != y)
				{
					MouseX = x;
					MouseY = y;
					OnMouseMove?.Invoke(null, new ScreenEventArgs(x, y));
				}
				
				CheckMouseButton(MouseButton.Left, buttonMask, 1);
				CheckMouseButton(MouseButton.Right, buttonMask, 4);
			}
		}
	}
}