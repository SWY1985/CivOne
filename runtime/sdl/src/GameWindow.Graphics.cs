// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Linq;
using CivOne.Enums;

namespace CivOne
{
	internal partial class GameWindow
	{
		private SDL.Texture CursorTexture = null;
    
		private static Size DefaultCanvasSize
		{
			get
			{
				if (Settings.AspectRatio != AspectRatio.Expand || Settings.ExpandWidth == -1 || Settings.ExpandHeight == -1)
					return new Size(320, 200);
				return new Size(Settings.ExpandWidth, Settings.ExpandHeight);
			}
		}

		private void Render()
		{
			Clear(Color.Black);
			GetBorders(out int x1, out int y1, out int x2, out int y2);
			using (SDL.Texture canvas = CreateTexture(_runtime.Bitmap))
			{
				canvas.Draw(x1, y1, (x2 - x1), (y2 - y1));
				CursorTexture?.Draw(x1 + (_mouseX * ScaleX), y1 + (_mouseY * ScaleY), CursorTexture.Width * ScaleX, CursorTexture.Height * ScaleY);
			}
		}

		private Size SetCanvasSize()
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

			return new Size(cw, ch);
		}

		private static int InitialCanvasWidth => DefaultCanvasSize.Width;
		private static int InitialCanvasHeight => DefaultCanvasSize.Height;

		private static int InitialWidth => InitialCanvasWidth * Settings.Scale;
		private static int InitialHeight => InitialCanvasHeight * Settings.Scale;

		private Size ClientRectangle => new Size(Width, Height);
		
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
			x1 = (ClientRectangle.Width - DrawWidth) / 2;
			y1 = (ClientRectangle.Height - DrawHeight) / 2;
			x2 = x1 + DrawWidth;
			y2 = y1 + DrawHeight;

			switch (Settings.AspectRatio)
			{
				case AspectRatio.Scaled:
					x1 = 0;
					y1 = 0;
					x2 = ClientRectangle.Width;
					y2 = ClientRectangle.Height;
					break;
				case AspectRatio.ScaledFixed:
					float scaleX = (float)ClientRectangle.Width / CanvasWidth;
					float scaleY = (float)ClientRectangle.Height / CanvasHeight;
					if (scaleX > scaleY) scaleX = scaleY;
					else if (scaleY > scaleX) scaleY = scaleX;

					int drawWidth = (int)((float)CanvasWidth * scaleX);
					int drawHeight = (int)((float)CanvasHeight * scaleY);

					x1 = (ClientRectangle.Width - drawWidth) / 2;
					y1 = (ClientRectangle.Height - drawHeight) / 2;
					x2 = x1 + drawWidth;
					y2 = y1 + drawHeight;
					break;
			}
		}
	}
}