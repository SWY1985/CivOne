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

		private KeyboardEventArgs ConvertSdlKeyArgs(SdlKeyEventArgs args)
		{
			Key key = Key.None;
			switch(args.ScanCode)
			{
				case SDL.SDL_Scancode.SDL_SCANCODE_RETURN:
				case SDL.SDL_Scancode.SDL_SCANCODE_KP_ENTER:
					key = Key.Enter;
					break;
				case SDL.SDL_Scancode.SDL_SCANCODE_UP:
					key = Key.Up;
					break;
				case SDL.SDL_Scancode.SDL_SCANCODE_DOWN:
					key = Key.Down;
					break;
				case SDL.SDL_Scancode.SDL_SCANCODE_LEFT:
					key = Key.Left;
					break;
				case SDL.SDL_Scancode.SDL_SCANCODE_RIGHT:
					key = Key.Right;
					break;
				case SDL.SDL_Scancode.SDL_SCANCODE_SPACE:
					key = Key.Space;
					break;
				default:
					return null;
			}
			return new KeyboardEventArgs(key, KeyModifier.None);
		}

		private void KeyDown(object sender, SdlKeyEventArgs args)
		{
			if (args.Repeat) return;
			KeyboardEventArgs keyArgs = ConvertSdlKeyArgs(args);
			if (keyArgs == null) return;
			_runtime.InvokeKeyboardDown(keyArgs);
		}

		private void KeyUp(object sender, SdlKeyEventArgs args)
		{
			if (args.Repeat) return;
			KeyboardEventArgs keyArgs = ConvertSdlKeyArgs(args);
			if (keyArgs == null) return;
			_runtime.InvokeKeyboardUp(keyArgs);
		}

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