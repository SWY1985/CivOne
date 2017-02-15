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
using System.Threading.Tasks;
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

		private KeyModifier _keyModifier = KeyModifier.None;

		private WindowState _previousState = WindowState.Normal;

		private int ScaleX
		{
			get
			{
				switch (Settings.Instance.AspectRatio)
				{
					case AspectRatio.Fixed:
					case AspectRatio.ScaledFixed:
					case AspectRatio.Expand:
						int scaleX = (ClientRectangle.Width - (ClientRectangle.Width % CanvasWidth)) / CanvasWidth;
						int scaleY = (ClientRectangle.Height - (ClientRectangle.Height % CanvasHeight)) / CanvasHeight;
						if (scaleX > scaleY)
							return scaleY;
						return scaleX;
					default:
						return (ClientRectangle.Width - (ClientRectangle.Width % CanvasWidth)) / CanvasWidth;
				}
			}
		}

		private int ScaleY
		{
			get
			{
				switch (Settings.Instance.AspectRatio)
				{
					case AspectRatio.Fixed:
					case AspectRatio.ScaledFixed:
					case AspectRatio.Expand:
						int scaleX = (ClientRectangle.Width - (ClientRectangle.Width % CanvasWidth)) / CanvasWidth;
						int scaleY = (ClientRectangle.Height - (ClientRectangle.Height % CanvasHeight)) / CanvasHeight;
						if (scaleY > scaleX)
							return scaleX;
						return scaleY;
					default:
						return (ClientRectangle.Height - (ClientRectangle.Height % CanvasHeight)) / CanvasHeight;
				}
			}
		}

		private int CanvasWidth { get; set; }
		private int CanvasHeight { get; set; }

		private int DrawWidth
		{
			get
			{
				return CanvasWidth * ScaleX;
			}
		}

		private int DrawHeight
		{
			get
			{
				return CanvasHeight * ScaleY;
			}
		}
		
		private static void LoadResources()
		{
			Task.Run(() => Reflect.PreloadCivilopedia());
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
				if (_mouseX >= 0 && _mouseX < DrawWidth && _mouseY >= 0 && _mouseY < DrawHeight)
				{
					switch (TopScreen.Cursor)
					{
						case CivMouseCursor.Pointer:
							_canvas.AddLayer(_cursorPointer, _mouseX, _mouseY);
							break;
						case CivMouseCursor.Goto:
							_canvas.AddLayer(_cursorGoto, _mouseX, _mouseY);
							break;
					}
				}

				return _canvas;
			}
		}

		private void ScreenBorder(int x1, int y1, int x2, int y2)
		{
			int ww = ClientSize.Width;
			int hh = ClientSize.Height;

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, ClientSize.Width, ClientSize.Height, 0, -1, 1);
			GL.Begin(PrimitiveType.Quads);

			GL.Color3(0, 0, 0); GL.Vertex2(0, 0);
			GL.Color3(0, 0, 0); GL.Vertex2(x1, 0);
			GL.Color3(0, 0, 0); GL.Vertex2(x1, hh);
			GL.Color3(0, 0, 0); GL.Vertex2(0, hh);

			GL.Color3(0, 0, 0); GL.Vertex2(x2, 0);
			GL.Color3(0, 0, 0); GL.Vertex2(ww, 0);
			GL.Color3(0, 0, 0); GL.Vertex2(ww, ClientSize.Height);
			GL.Color3(0, 0, 0); GL.Vertex2(x2, ClientSize.Height);

			GL.Color3(0, 0, 0); GL.Vertex2(x1, 0);
			GL.Color3(0, 0, 0); GL.Vertex2(x2, 0);
			GL.Color3(0, 0, 0); GL.Vertex2(x2, y1);
			GL.Color3(0, 0, 0); GL.Vertex2(x1, y1);

			GL.Color3(0, 0, 0); GL.Vertex2(x1, y2);
			GL.Color3(0, 0, 0); GL.Vertex2(x2, y2);
			GL.Color3(0, 0, 0); GL.Vertex2(x2, hh);
			GL.Color3(0, 0, 0); GL.Vertex2(x1, hh);

			GL.End();
		}

		private IEnumerable<int> GetCanvas()
		{
			Picture canvas = Canvas;
			CanvasWidth = canvas.Width;
			CanvasHeight = canvas.Height;
			int[] colors = canvas.Palette.Select(x => x.GetHashCode()).ToArray();
			byte[,] bitmap = canvas.GetBitmap;
			for (int yy = bitmap.GetLength(1) - 1; yy >= 0; yy--)
			for (int xx = 0; xx < bitmap.GetLength(0); xx++)
			{
				yield return colors[bitmap[xx, yy]];
			}
		}

		private void GetBorders(out int x1, out int y1, out int x2, out int y2)
		{
			x1 = (ClientSize.Width - DrawWidth) / 2;
			y1 = (ClientSize.Height - DrawHeight) / 2;
			x2 = x1 + DrawWidth;
			y2 = y1 + DrawHeight;

			switch (Settings.Instance.AspectRatio)
			{
				case AspectRatio.Scaled:
					x1 = 0;
					y1 = 0;
					x2 = ClientSize.Width;
					y2 = ClientSize.Height;
					break;
				case AspectRatio.ScaledFixed:
					float scaleX = (float)ClientSize.Width / CanvasWidth;
					float scaleY = (float)ClientSize.Height / CanvasHeight;
					if (scaleX > scaleY) scaleX = scaleY;
					else if (scaleY > scaleX) scaleY = scaleX;

					int drawWidth = (int)((float)CanvasWidth * scaleX);
					int drawHeight = (int)((float)CanvasHeight * scaleY);

					x1 = (ClientSize.Width - drawWidth) / 2;
					y1 = (ClientSize.Height - drawHeight) / 2;
					x2 = x1 + drawWidth;
					y2 = y1 + drawHeight;
					break;
			}
		}

		private KeyboardEventArgs ConvertKeyboardEvents(KeyboardKeyEventArgs args)
		{
			switch (args.Key)
			{
				case TkKey.F1: return new KeyboardEventArgs(CivKey.F1, _keyModifier);
				case TkKey.F2: return new KeyboardEventArgs(CivKey.F2, _keyModifier);
				case TkKey.F3: return new KeyboardEventArgs(CivKey.F3, _keyModifier);
				case TkKey.F4: return new KeyboardEventArgs(CivKey.F4, _keyModifier);
				case TkKey.F5: return new KeyboardEventArgs(CivKey.F5, _keyModifier);
				case TkKey.F6: return new KeyboardEventArgs(CivKey.F6, _keyModifier);
				case TkKey.F7: return new KeyboardEventArgs(CivKey.F7, _keyModifier);
				case TkKey.F8: return new KeyboardEventArgs(CivKey.F8, _keyModifier);
				case TkKey.F9: return new KeyboardEventArgs(CivKey.F9, _keyModifier);
				case TkKey.F10: return new KeyboardEventArgs(CivKey.F10, _keyModifier);
				case TkKey.F11: return new KeyboardEventArgs(CivKey.F11, _keyModifier);
				case TkKey.F12: return new KeyboardEventArgs(CivKey.F12, _keyModifier);
				case TkKey.Keypad0: return new KeyboardEventArgs(CivKey.NumPad0, _keyModifier);
				case TkKey.Keypad1: return new KeyboardEventArgs(CivKey.NumPad1, _keyModifier);
				case TkKey.Keypad2: return new KeyboardEventArgs(CivKey.NumPad2, _keyModifier);
				case TkKey.Keypad3: return new KeyboardEventArgs(CivKey.NumPad3, _keyModifier);
				case TkKey.Keypad4: return new KeyboardEventArgs(CivKey.NumPad4, _keyModifier);
				case TkKey.Keypad5: return new KeyboardEventArgs(CivKey.NumPad5, _keyModifier);
				case TkKey.Keypad6: return new KeyboardEventArgs(CivKey.NumPad6, _keyModifier);
				case TkKey.Keypad7: return new KeyboardEventArgs(CivKey.NumPad7, _keyModifier);
				case TkKey.Keypad8: return new KeyboardEventArgs(CivKey.NumPad8, _keyModifier);
				case TkKey.Keypad9: return new KeyboardEventArgs(CivKey.NumPad9, _keyModifier);
				case TkKey.Up: return new KeyboardEventArgs(CivKey.Up, _keyModifier);
				case TkKey.Left: return new KeyboardEventArgs(CivKey.Left, _keyModifier);
				case TkKey.Right: return new KeyboardEventArgs(CivKey.Right, _keyModifier);
				case TkKey.Down: return new KeyboardEventArgs(CivKey.Down, _keyModifier);
				case TkKey.KeypadEnter:
				case TkKey.Enter: return new KeyboardEventArgs(CivKey.Enter, _keyModifier);
				case TkKey.Space: return new KeyboardEventArgs(CivKey.Space, _keyModifier);
				case TkKey.Escape: return new KeyboardEventArgs(CivKey.Escape, _keyModifier);
				case TkKey.Delete: return new KeyboardEventArgs(CivKey.Delete, _keyModifier);
				case TkKey.Back: return new KeyboardEventArgs(CivKey.Backspace, _keyModifier);
				case TkKey.Period: return new KeyboardEventArgs('.', _keyModifier);
				case TkKey.Comma: return new KeyboardEventArgs(',', _keyModifier);
				case TkKey.KeypadPlus:
				case TkKey.Plus: return new KeyboardEventArgs(CivKey.Plus, _keyModifier);
				case TkKey.KeypadMinus:
				case TkKey.Minus: return new KeyboardEventArgs(CivKey.Minus, _keyModifier);
				case TkKey.Number0: return new KeyboardEventArgs('0', _keyModifier);
				case TkKey.Number1: return new KeyboardEventArgs('1', _keyModifier);
				case TkKey.Number2: return new KeyboardEventArgs('2', _keyModifier);
				case TkKey.Number3: return new KeyboardEventArgs('3', _keyModifier);
				case TkKey.Number4: return new KeyboardEventArgs('4', _keyModifier);
				case TkKey.Number5: return new KeyboardEventArgs('5', _keyModifier);
				case TkKey.Number6: return new KeyboardEventArgs('6', _keyModifier);
				case TkKey.Number7: return new KeyboardEventArgs('7', _keyModifier);
				case TkKey.Number8: return new KeyboardEventArgs('8', _keyModifier);
				case TkKey.Number9: return new KeyboardEventArgs('9', _keyModifier);
			}

			return null;
		}

		private void OnKeyDown(object sender, KeyboardKeyEventArgs args)
		{
			_keyModifier = KeyModifier.None;
			if (args.Control) _keyModifier |= KeyModifier.Control;
			if (args.Shift) _keyModifier |= KeyModifier.Shift;
			if (args.Alt) _keyModifier |= KeyModifier.Alt;

			if (_keyModifier == KeyModifier.Alt && args.Key == TkKey.Enter)
			{
				if (WindowState == WindowState.Fullscreen)
				{
					Console.WriteLine("Windowed mode");
					WindowState = _previousState;
					return;
				}
				Console.WriteLine("Fullscreen mode");
				_previousState = WindowState;
				WindowState = WindowState.Fullscreen;
				return;
			}

			if (TopScreen == null) return;
			KeyboardEventArgs keyArgs = ConvertKeyboardEvents(args);

			if (keyArgs == null) return;
			TopScreen.KeyDown(keyArgs);
		}

		private void OnKeyUp(object sender, KeyboardKeyEventArgs args)
		{
			_keyModifier = KeyModifier.None;
			if (args.Control) _keyModifier |= KeyModifier.Control;
			if (args.Shift) _keyModifier |= KeyModifier.Shift;
			if (args.Alt) _keyModifier |= KeyModifier.Alt;
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
			if (WindowState == WindowState.Minimized) return;
			GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
		}

		protected override void OnLoad(EventArgs args)
		{
			GL.Disable(EnableCap.DepthTest);

			// Set full screen
			if (Settings.Instance.FullScreen)
			{
				WindowState = WindowState.Fullscreen;
			} 

			// Load cursor graphics
			_cursorPointer = Resources.Instance.GetPart("SP257", 112, 32, 16, 16);
			_cursorGoto = Resources.Instance.GetPart("SP257", 32, 32, 16, 16);
			
			LoadResources();
		}

		protected override void OnKeyPress(KeyPressEventArgs args)
		{
			if (TopScreen == null) return;
			char keyChar = (char)args.KeyChar;
			if (char.IsLetter(keyChar))
			{
				TopScreen.KeyDown(new KeyboardEventArgs(char.ToUpper((char)args.KeyChar), _keyModifier));
			}
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			if (Common.EndGame)
			{
				Close();
				return;
			}

			float scaleX = (float)ClientSize.Width / CanvasWidth;
			float scaleY = (float)ClientSize.Height / CanvasHeight;
			int x1, y1, x2, y2;
			GetBorders(out x1, out y1, out x2, out y2);

			if (WindowState != WindowState.Minimized && this.Focused)
			{
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
				_mouseX = (int)((Mouse.X - x1) / scaleX);
				_mouseY = (int)((Mouse.Y - y1) / scaleY);

				CursorVisible = (_mouseX <= 0 || _mouseX >= (CanvasWidth - 1) || _mouseY <= 0 || _mouseY >= (CanvasHeight - 1));
			}
			else if (!CursorVisible)
			{
				CursorVisible = true;
			}

			_gameTick += (uint)(GameTask.Fast ? 4 : 1);
			if (_gameTick % 4 != 0) return;
			_update = HasUpdate;
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			int[] canvas = GetCanvas().ToArray();
			// int x1 = (ClientSize.Width - DrawWidth) / 2;
			// int y1 = (ClientSize.Height - DrawHeight) / 2;
			// int x2 = x1 + DrawWidth;
			// int y2 = y1 + DrawHeight;

			// BlitFramebufferFilter fbFilter;
			// switch (Settings.Instance.AspectRatio)
			// {
			// 	case AspectRatio.Scaled:
			// 		x1 = 0;
			// 		y1 = 0;
			// 		x2 = ClientSize.Width;
			// 		y2 = ClientSize.Height;
			// 		fbFilter = BlitFramebufferFilter.Linear;
			// 		break;
			// 	case AspectRatio.ScaledFixed:
			// 		float scaleX = (float)ClientSize.Width / CanvasWidth;
			// 		float scaleY = (float)ClientSize.Height / CanvasHeight;
			// 		if (scaleX > scaleY) scaleX = scaleY;
			// 		else if (scaleY > scaleX) scaleY = scaleX;

			// 		int drawWidth = (int)((float)CanvasWidth * scaleX);
			// 		int drawHeight = (int)((float)CanvasHeight * scaleY);

			// 		x1 = (ClientSize.Width - drawWidth) / 2;
			// 		y1 = (ClientSize.Height - drawHeight) / 2;
			// 		x2 = x1 + drawWidth;
			// 		y2 = y1 + drawHeight;
			// 		fbFilter = BlitFramebufferFilter.Linear;
			// 		break;
			// 	default:
			// 		fbFilter = BlitFramebufferFilter.Nearest;
			// 		break;
			// }

			BlitFramebufferFilter fbFilter;
			int x1, y1, x2, y2;
			GetBorders(out x1, out y1, out x2, out y2);
			switch (Settings.Instance.AspectRatio)
			{
				case AspectRatio.Scaled:
				case AspectRatio.ScaledFixed:
					fbFilter = BlitFramebufferFilter.Linear;
					break;
				default:
					fbFilter = BlitFramebufferFilter.Nearest;
					break;
			}

			GL.DrawPixels<int>(CanvasWidth, CanvasHeight, PixelFormat.Rgba, PixelType.UnsignedInt8888Reversed, canvas);
			GL.BlitFramebuffer(0, 0, CanvasWidth, CanvasHeight, x1, y1, x2, y2, ClearBufferMask.ColorBufferBit, fbFilter);

			switch (Settings.Instance.AspectRatio)
			{
				case AspectRatio.Scaled:
				case AspectRatio.Expand:
					break;
				default:
					ScreenBorder(x1, y1, x2, y2);
					break;
			}
			
			SwapBuffers();
		}

		public Window(string screen) : base(320 * Settings.Instance.ScaleX, 200 * Settings.Instance.ScaleX, OpenTK.Graphics.GraphicsMode.Default, "CivOne", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible)
		{
			CanvasWidth = 320;
			CanvasHeight = 200;

			// Load the first screen
			IScreen startScreen;
			switch (screen)
			{
				case "demo":
					startScreen = new Demo();
					break;
				case "setup":
					startScreen = new Setup();
					break;
				default:
					startScreen = new Credits();
					break;
			}
			Common.AddScreen(startScreen);

			KeyDown += OnKeyDown;
			KeyUp += OnKeyUp;
			MouseDown += OnMouseDown;
			MouseUp += OnMouseUp;
		}
	}
}