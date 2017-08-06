// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CivOne
{
	internal partial class Window
	{
		private const int TEXTURE_CANVAS = 0, TEXTURE_CURSOR = 1;

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
			GL.Enable(EnableCap.Texture2D);
			GL.Disable(EnableCap.DepthTest);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		private void DrawCursor(Canvas cursor, int ox, int oy)
		{
			if (cursor == null) return;
			BitmapToTexture(TEXTURE_CURSOR, cursor);

			int x1 = ox + (_mouseX * ScaleX);
			int y1 = oy + (_mouseY * ScaleY);
			int x2 = x1 + (cursor.Width * ScaleX);
			int y2 = y1 + (cursor.Height * ScaleY);

			GL.Enable(EnableCap.Blend);
			DrawQuad(TEXTURE_CURSOR, x1, y1, x2, y2);
			GL.Disable(EnableCap.Blend);
		}

		private void Render(Canvas canvas, Canvas cursor)
		{
			int x1, y1, x2, y2;
			GetBorders(out x1, out y1, out x2, out y2);

			BitmapToTexture(TEXTURE_CANVAS, canvas);

			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, ClientSize.Width, ClientSize.Height, 0, -1, 1);

			DrawQuad(TEXTURE_CANVAS, x1, y1, x2, y2);
			DrawCursor(cursor, x1, y1);

			SwapBuffers();
		}
	}
}