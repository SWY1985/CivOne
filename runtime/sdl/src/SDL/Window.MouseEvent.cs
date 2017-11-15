// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne
{
	internal static partial class SDL
	{
		internal abstract partial class Window
		{
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

			private void HandleMouse()
			{
				uint buttonMask = SDL_GetMouseState(out int x, out int y);
				MouseX = x;
				MouseY = y;
			}
		}
	}
}