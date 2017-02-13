// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;

using TkKey = OpenTK.Input.Key;
using TkMouseButton = OpenTK.Input.MouseButton;
using CivKey = CivOne.Enums.Key;
using CivMouseButton = CivOne.Enums.MouseButton;
using CivMouseCursor = CivOne.Enums.MouseCursor;

namespace CivOne
{
	internal class Window : GameWindow
	{
		private uint _gameTick = 0;

		private bool _update = false;

		private Picture _cursorPointer, _cursorGoto;

		private int _mouseX, _mouseY;
		
		private static void LoadResources()
		{
			ThreadStart civilopediaDelegate = new ThreadStart(Reflect.PreloadCivilopedia);
			Thread civilopedia = new Thread(new ThreadStart(Reflect.PreloadCivilopedia))
			{
				IsBackground = true
			};
			
			civilopedia.Start();
		}
		
		// Returns whether any changes have been made to the screen.
		private bool HasUpdate
		{
			get
			{
				if (!GameTask.Update() && (_gameTick % 4) > 0) return false;
				if (Common.Screens.Any(x => x is IModal))
					return Common.Screens.Last(x => x is IModal).HasUpdate(_gameTick / 4);
				return (Common.Screens.Count(x => x.HasUpdate(_gameTick / 4)) > 0);
			}
		}

		protected IScreen TopScreen
		{
			get
			{
				return Common.TopScreen;
			}
		}

		private readonly Picture _canvas = new Picture(320, 200);
		internal Picture Canvas
		{
			get
			{
				if (Common.Screens.Length == 0) return _canvas;

				CivOne.GFX.Color[] palette = TopScreen.Canvas.Palette;
				palette[0] = CivOne.GFX.Color.Black;
				_canvas.FillRectangle(0, 0, 0, _canvas.Width, _canvas.Height);
				_canvas.SetPalette(palette);

				if (TopScreen is IModal)
				{
					_canvas.AddLayer(TopScreen.Canvas, 0, 0);
				}
				else
				{
					foreach (IScreen screen in Common.Screens)
					{
						_canvas.AddLayer(screen.Canvas, 0, 0);
					}
				}

				// Draw the mouse cursor
				switch (TopScreen.Cursor)
				{
					case CivMouseCursor.Pointer:
						_canvas.AddLayer(_cursorPointer, _mouseX, _mouseY);
						break;
					case CivMouseCursor.Goto:
						_canvas.AddLayer(_cursorGoto, _mouseX, _mouseY);
						break;
				}

				return _canvas;
			}
		}

		private IEnumerable<int> GetCanvas()
		{
			Picture canvas = Canvas;
			byte[,] bitmap = canvas.ScaleBitmap(2, 2);
			for (int yy = bitmap.GetLength(1) - 1; yy >= 0; yy--)
			for (int xx = 0; xx < bitmap.GetLength(0); xx++)
			{
				yield return canvas.Palette[bitmap[xx, yy]].GetHashCode();
			}
		}

		private KeyboardEventArgs ConvertKeyboardEvents(KeyboardKeyEventArgs args)
		{
			KeyModifier modifier = KeyModifier.None;
			if (args.Control) modifier |= KeyModifier.Control;
			if (args.Shift) modifier |= KeyModifier.Shift;
			if (args.Alt) modifier |= KeyModifier.Alt;

			switch (args.Key)
			{
				case TkKey.F1: return new KeyboardEventArgs(CivKey.F1, modifier);
				case TkKey.F2: return new KeyboardEventArgs(CivKey.F2, modifier);
				case TkKey.F3: return new KeyboardEventArgs(CivKey.F3, modifier);
				case TkKey.F4: return new KeyboardEventArgs(CivKey.F4, modifier);
				case TkKey.F5: return new KeyboardEventArgs(CivKey.F5, modifier);
				case TkKey.F6: return new KeyboardEventArgs(CivKey.F6, modifier);
				case TkKey.F7: return new KeyboardEventArgs(CivKey.F7, modifier);
				case TkKey.F8: return new KeyboardEventArgs(CivKey.F8, modifier);
				case TkKey.F9: return new KeyboardEventArgs(CivKey.F9, modifier);
				case TkKey.F10: return new KeyboardEventArgs(CivKey.F10, modifier);
				case TkKey.F11: return new KeyboardEventArgs(CivKey.F11, modifier);
				case TkKey.F12: return new KeyboardEventArgs(CivKey.F12, modifier);
				case TkKey.Keypad0: return new KeyboardEventArgs(CivKey.NumPad0, modifier);
				case TkKey.Keypad1: return new KeyboardEventArgs(CivKey.NumPad1, modifier);
				case TkKey.Keypad2: return new KeyboardEventArgs(CivKey.NumPad2, modifier);
				case TkKey.Keypad3: return new KeyboardEventArgs(CivKey.NumPad3, modifier);
				case TkKey.Keypad4: return new KeyboardEventArgs(CivKey.NumPad4, modifier);
				case TkKey.Keypad5: return new KeyboardEventArgs(CivKey.NumPad5, modifier);
				case TkKey.Keypad6: return new KeyboardEventArgs(CivKey.NumPad6, modifier);
				case TkKey.Keypad7: return new KeyboardEventArgs(CivKey.NumPad7, modifier);
				case TkKey.Keypad8: return new KeyboardEventArgs(CivKey.NumPad8, modifier);
				case TkKey.Keypad9: return new KeyboardEventArgs(CivKey.NumPad9, modifier);
				case TkKey.Up: return new KeyboardEventArgs(CivKey.Up, modifier);
				case TkKey.Left: return new KeyboardEventArgs(CivKey.Left, modifier);
				case TkKey.Right: return new KeyboardEventArgs(CivKey.Right, modifier);
				case TkKey.Down: return new KeyboardEventArgs(CivKey.Down, modifier);
				case TkKey.Enter: return new KeyboardEventArgs(CivKey.Enter, modifier);
				case TkKey.Space: return new KeyboardEventArgs(CivKey.Space, modifier);
				case TkKey.Escape: return new KeyboardEventArgs(CivKey.Escape, modifier);
				case TkKey.Delete: return new KeyboardEventArgs(CivKey.Delete, modifier);
				case TkKey.Back: return new KeyboardEventArgs(CivKey.Backspace, modifier);
				case TkKey.Period: return new KeyboardEventArgs('.', modifier);
				case TkKey.Comma: return new KeyboardEventArgs(',', modifier);
				case TkKey.Plus: return new KeyboardEventArgs(CivKey.Plus, modifier);
				case TkKey.Minus: return new KeyboardEventArgs(CivKey.Minus, modifier);
			}

			return null;
		}

		private void OnKeyDown(object sender, KeyboardKeyEventArgs args)
		{
			if (TopScreen == null) return;
			KeyboardEventArgs keyArgs = ConvertKeyboardEvents(args);

			if (keyArgs == null) return;
			TopScreen.KeyDown(keyArgs);
		}

		private void OnMouseDown(object sender, MouseEventArgs args)
		{
			if (TopScreen == null) return;

			CivMouseButton buttons = CivMouseButton.None;
			if (args.Mouse.IsButtonDown(TkMouseButton.Left)) buttons = CivMouseButton.Left;
			else if (args.Mouse.IsButtonDown(TkMouseButton.Right)) buttons = CivMouseButton.Right;
			TopScreen.MouseDown(new ScreenEventArgs(_mouseX, _mouseY, buttons));
		}

		private void OnMouseUp(object sender, MouseEventArgs args)
		{
			if (TopScreen == null) return;

			CivMouseButton buttons = CivMouseButton.None;
			if (args.Mouse.IsButtonUp(TkMouseButton.Left)) buttons = CivMouseButton.Left;
			else if (args.Mouse.IsButtonUp(TkMouseButton.Right)) buttons = CivMouseButton.Right;
			TopScreen.MouseUp(new ScreenEventArgs(_mouseX, _mouseY, buttons));
		}

		protected override void OnResize(EventArgs args)
		{
			GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
		}

		protected override void OnLoad(EventArgs args)
		{
			GL.Disable(EnableCap.DepthTest);

			// Load cursor graphics
			_cursorPointer = Resources.Instance.GetPart("SP257", 112, 32, 16, 16);
			_cursorGoto = Resources.Instance.GetPart("SP257", 32, 32, 16, 16);
			
			// Load the first screen
			IScreen startScreen = new Credits();
			
			Common.AddScreen(startScreen);
			
			LoadResources();
		}

		protected override void OnKeyPress(KeyPressEventArgs args)
		{
			if (TopScreen == null) return;
			TopScreen.KeyDown(new KeyboardEventArgs(char.ToUpper((char)args.KeyChar), KeyModifier.None));
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			_mouseX = Mouse.X / 2;
			_mouseY = Mouse.Y / 2;

			_gameTick += (uint)(GameTask.Fast ? 4 : 1);
			if (_gameTick % 4 != 0) return;
			_update = HasUpdate;
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.DrawPixels<int>(640, 400, PixelFormat.Rgba, PixelType.UnsignedInt8888Reversed, GetCanvas().ToArray());
			SwapBuffers();
		}

		public Window() : base(640, 400, OpenTK.Graphics.GraphicsMode.Default, "CivOne", GameWindowFlags.FixedWindow, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible)
		{
			KeyDown += OnKeyDown;
			MouseDown += OnMouseDown;
			MouseUp += OnMouseUp;
		}
	}
}