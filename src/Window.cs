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
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.IO;
using CivOne.Screens;

namespace CivOne
{
	internal partial class Window : GameWindow
	{
		private uint _gameTick = 0;

		private bool _update = false;

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
				if (!GameTask.Update() && (!GameTask.Fast && (_gameTick % 4) > 0)) return false;
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

		private Picture _canvas = new Picture(320, 200);
		internal Picture Canvas
		{
			get
			{
				if (ScaleX < 1 || ScaleY < 1)
				{
					CanvasWidth = 320;
					CanvasHeight = 200;
				}
				else if (Settings.Instance.AspectRatio == AspectRatio.Expand)
				{
					int scale = 1;
					int scaleX = (ClientRectangle.Width - (ClientRectangle.Width % 320)) / 320;
					int scaleY = (ClientRectangle.Height - (ClientRectangle.Height % 200)) / 200;
					if (scaleX > scaleY) scale = scaleY;
					else scale = scaleX;

					CanvasWidth = (int)(ClientSize.Width / scale);
					CanvasHeight = (int)(ClientSize.Height / scale);

					// Make sure the canvas resolution is a multiple of 4
					CanvasWidth -= (CanvasWidth % 4);
					CanvasHeight -= (CanvasHeight % 4);

					// Set maximum bounds to 512x384, the maximum logical boundaries 
					// according this this table: https://github.com/SWY1985/CivOne/wiki/Settings#expand-experimental
					if (CanvasWidth > 512) CanvasWidth = 512;
					if (CanvasHeight > 384) CanvasHeight = 384;
				}
				else
				{
					CanvasWidth = 320;
					CanvasHeight = 200;
				}

				if (_canvas.Width != CanvasWidth  || _canvas.Height != CanvasHeight)
				{
					_canvas = new Picture(CanvasWidth, CanvasHeight);
				}

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
						if (screen is IExpand && (screen.Canvas.Width != CanvasWidth || screen.Canvas.Height != CanvasHeight))
						{
							(screen as IExpand).Resize(CanvasWidth, CanvasHeight);
						}
						_canvas.AddLayer(screen.Canvas, 0, 0);
					}
				}

				DrawMouseCursor(_canvas);

				return _canvas;
			}
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

		protected override void OnResize(EventArgs args)
		{
			if (WindowState == WindowState.Minimized) return;
			GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
		}

		protected override void OnLoad(EventArgs args)
		{
			GL.Enable(EnableCap.Texture2D);
			GL.Disable(EnableCap.DepthTest);

			// Set full screen
			if (Settings.Instance.FullScreen)
			{
				WindowState = WindowState.Fullscreen;
			}
			
			LoadCursorGraphics();
			LoadResources();
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			if (Common.EndGame)
			{
				Exit();
				return;
			}

			if (Common.ReloadSettings)
			{
				LoadCursorGraphics();
			}

			float scaleX = (float)ClientSize.Width / CanvasWidth;
			float scaleY = (float)ClientSize.Height / CanvasHeight;

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

				OnMouseMove(scaleX, scaleY);
			}
			else if (!CursorVisible)
			{
				CursorVisible = true;
			}

			_gameTick += (uint)(GameTask.Fast ? 4 : 1);
			_update = HasUpdate;
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			int ww = ClientSize.Width;
			int hh = ClientSize.Height;
			int x1, y1, x2, y2;
			GetBorders(out x1, out y1, out x2, out y2);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, ClientSize.Width, ClientSize.Height, 0, -1, 1);
			
			int textureId = 0;
			int[] canvas = GetCanvas().ToArray();

			GL.BindTexture(TextureTarget.Texture2D, textureId);
			GL.TexImage2D<int>(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, CanvasWidth, CanvasHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, canvas);

			switch (Settings.Instance.AspectRatio)
			{
				case AspectRatio.Scaled:
				case AspectRatio.ScaledFixed:
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
					break;
				default:
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
					break;
			}
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.BindTexture(TextureTarget.Texture2D, textureId);

			GL.Begin(PrimitiveType.Quads);
			GL.TexCoord2(0, 1); GL.Vertex2(x1, y1);
			GL.TexCoord2(1, 1); GL.Vertex2(x2, y1);
			GL.TexCoord2(1, 0); GL.Vertex2(x2, y2);
			GL.TexCoord2(0, 0); GL.Vertex2(x1, y2);
			GL.End();

			GL.Flush();

			SwapBuffers();
		}

		public Window(string screen) : base(320 * Settings.Instance.ScaleX, 200 * Settings.Instance.ScaleX, OpenTK.Graphics.GraphicsMode.Default, "CivOne", GameWindowFlags.Default, DisplayDevice.Default, 1, 0, GraphicsContextFlags.ForwardCompatible)
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
			if (!FileSystem.DataFilesExist())
			{
				MissingFiles missingFiles = new MissingFiles();
				missingFiles.Closed += (s, a) => Common.AddScreen(startScreen);
				Common.AddScreen(missingFiles);
			}
			else
			{
				Common.AddScreen(startScreen);
			}

			KeyDown += OnKeyDown;
			KeyUp += OnKeyUp;
			MouseDown += OnMouseDown;
			MouseUp += OnMouseUp;
		}
	}
}