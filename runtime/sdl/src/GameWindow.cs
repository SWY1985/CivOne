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

			if (_runtime.Bitmap == null) return;
			Bytemap bitmap = _runtime.Bitmap.Bitmap;
			Colour[] palette = _runtime.Bitmap.Palette.Entries.ToArray();
			for (int y = 0; y < 200; y++)
			for (int x = 0; x < 320; x++)
			{
				int entry = bitmap[x, y];
				Colour c = entry == 0 ? Colour.Black : palette[entry];
				Rectangle rect = new Rectangle(x * 2, y * 2, 2, 2);
				Color color = Color.FromArgb(255, c.R, c.G, c.B);

				FillRectangle(rect, color);
			}
		}

		private void KeyDown(object sender, KeyboardEventArgs args) => _runtime.InvokeKeyboardDown(args);

		private void KeyUp(object sender, KeyboardEventArgs args) => _runtime.InvokeKeyboardUp(args);

		public GameWindow(Runtime runtime) : base("CivOne")
		{
			_runtime = runtime;

			OnLoad += Load;
			OnUpdate += Update;
			OnDraw += Draw;
			OnKeyDown += KeyDown;
			OnKeyUp += KeyUp;
		}
	}
}