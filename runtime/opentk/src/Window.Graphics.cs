// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Enums;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using DSize = System.Drawing.Size;

namespace CivOne
{
	internal partial class Window
	{
		private const int TEXTURE_CANVAS = 0, TEXTURE_CURSOR = 1;

		private int ScaleX
		{
			get
			{
				int cw = CanvasWidth, ch = CanvasHeight;
				if (cw == 0) cw = DefaultCanvasSize.Width;
				if (ch == 0) ch = DefaultCanvasSize.Height;
				
				switch (Settings.AspectRatio)
				{
					case AspectRatio.Fixed:
					case AspectRatio.ScaledFixed:
					case AspectRatio.Expand:
						int scaleX = (ClientRectangle.Width - (ClientRectangle.Width % cw)) / cw;
						int scaleY = (ClientRectangle.Height - (ClientRectangle.Height % ch)) / ch;
						if (scaleX > scaleY)
							return scaleY;
						return scaleX;
					default:
						return (ClientRectangle.Width - (ClientRectangle.Width % cw)) / cw;
				}
			}
		}

		private int ScaleY
		{
			get
			{
				int cw = CanvasWidth, ch = CanvasHeight;
				if (cw == 0) cw = DefaultCanvasSize.Width;
				if (ch == 0) ch = DefaultCanvasSize.Height;

				switch (Settings.Instance.AspectRatio)
				{
					case AspectRatio.Fixed:
					case AspectRatio.ScaledFixed:
					case AspectRatio.Expand:
						int scaleX = (ClientRectangle.Width - (ClientRectangle.Width % cw)) / cw;
						int scaleY = (ClientRectangle.Height - (ClientRectangle.Height % ch)) / ch;
						if (scaleY > scaleX)
							return scaleX;
						return scaleY;
					default:
						return (ClientRectangle.Height - (ClientRectangle.Height % ch)) / ch;
				}
			}
		}

		private int CanvasWidth => Runtime.CanvasSize.Width;
		private int CanvasHeight => Runtime.CanvasSize.Height;

		private int DrawWidth => CanvasWidth * ScaleX;
		private int DrawHeight => CanvasHeight * ScaleY;

		private void GetBorders(out int x1, out int y1, out int x2, out int y2)
		{
			x1 = (ClientSize.Width - DrawWidth) / 2;
			y1 = (ClientSize.Height - DrawHeight) / 2;
			x2 = x1 + DrawWidth;
			y2 = y1 + DrawHeight;

			switch (Settings.AspectRatio)
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

		private static DSize DefaultCanvasSize
		{
			get
			{
				if (Settings.AspectRatio != AspectRatio.Expand || Settings.ExpandWidth == -1 || Settings.ExpandHeight == -1)
					return new DSize(320, 200);
				return new DSize(Settings.ExpandWidth, Settings.ExpandHeight);
			}
		}

		private DSize SetCanvasSize()
		{
			if (Settings.AspectRatio != AspectRatio.Expand || (ScaleX < 1 || ScaleY < 1))
			{
				return DefaultCanvasSize;
			}

			int cw = ClientRectangle.Width, ch = ClientRectangle.Height;
			int scale = new int[] { (cw - (cw % 320)) / 320, (ch - (ch % 200)) / 200 }.Min();

			if (Settings.ExpandWidth != -1 && Settings.ExpandHeight != -1)
			{
				cw = Settings.ExpandWidth;
				ch = Settings.ExpandHeight;
			}
			else
			{
				cw /= scale;
				ch /= scale;
			}

			// Make sure the canvas resolution is a multiple of 8
			cw -= (cw % 8);
			ch -= (ch % 8);

			// Set maximum bounds to 512x384, the maximum logical boundaries 
			// according this this table: https://github.com/SWY1985/CivOne/wiki/Settings#expand-experimental
			if (cw > 512) cw = 512;
			if (ch > 384) ch = 384;

			return new DSize(cw, ch);
		}

		private void BitmapToTexture(int textureId, Canvas bitmap)
		{
			int[] canvas = bitmap.ColourMap;

			GL.BindTexture(TextureTarget.Texture2D, textureId);
			GL.TexImage2D<int>(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, canvas);

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
		}

		private void DrawQuad(int textureId, int x1, int y1, int x2, int y2)
		{
			GL.BindTexture(TextureTarget.Texture2D, textureId);
			
			GL.Begin(PrimitiveType.Quads);
			GL.TexCoord2(0, 1); GL.Vertex2(x1, y1);
			GL.TexCoord2(1, 1); GL.Vertex2(x2, y1);
			GL.TexCoord2(1, 0); GL.Vertex2(x2, y2);
			GL.TexCoord2(0, 0); GL.Vertex2(x1, y2);
			GL.End();
		}

		private void InitializeGraphics()
		{
			SetCanvasSize();

			GL.Enable(EnableCap.Texture2D);
			GL.Disable(EnableCap.DepthTest);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		private void DrawCursor(int left, int top, int width, int height)
		{
			int x1 = left + (_mouseX * ScaleX);
			int y1 = top + (_mouseY * ScaleY);
			int x2 = x1 + (width * ScaleX);
			int y2 = y1 + (height * ScaleY);

			GL.Enable(EnableCap.Blend);
			DrawQuad(TEXTURE_CURSOR, x1, y1, x2, y2);
			GL.Disable(EnableCap.Blend);
		}

		private void Render(Canvas canvas, Canvas cursor)
		{
			bool update = HasUpdate;

			int x1, y1, x2, y2;
			GetBorders(out x1, out y1, out x2, out y2);

			if (update)
			{
				Runtime.CanvasSize = SetCanvasSize();
				BitmapToTexture(TEXTURE_CANVAS, canvas);
			}

			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, ClientSize.Width, ClientSize.Height, 0, -1, 1);

			DrawQuad(TEXTURE_CANVAS, x1, y1, x2, y2);

			if (cursor != null)
			{
				if (update) BitmapToTexture(TEXTURE_CURSOR, cursor);
				DrawCursor(x1, y1, cursor.Width, cursor.Height);
			}

			SwapBuffers();
		}
	}
}