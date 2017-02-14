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

namespace CivOne.Interfaces
{
	public interface IScreen
	{
		event EventHandler Closed;
		Picture Canvas { get; }
		Color[] Palette { get; }
		MouseCursor Cursor { get; }
		bool HasUpdate(uint gameTick);
		bool KeyDown(KeyboardEventArgs args);
		bool MouseDown(ScreenEventArgs args);
		bool MouseUp(ScreenEventArgs args);
		bool MouseDrag(ScreenEventArgs args);
	}
}