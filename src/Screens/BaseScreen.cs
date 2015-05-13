// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.GFX;

namespace CivOne.Screens
{
	internal abstract class BaseScreen : IScreen
	{
		internal int Scale
		{
			get
			{
				return Settings.Instance.Scale;
			}
		}
		
		public abstract MouseCursor Cursor { get; }
		public abstract bool HasUpdate(uint gameTick);
		public virtual void Draw(Graphics gfx)
		{
			gfx.Clear(Color.Black);
		}
		internal void DrawText(Graphics gfx, ColorPalette palette, string text, int font, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			DrawText(gfx, palette, text, font, colour, colour, x, y, align);
		}
		internal void DrawText(Graphics gfx, ColorPalette palette, string text, int font, byte firstLetterColour, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			Bitmap textImage = (Bitmap)Resources.Instance.GetText(text, font, firstLetterColour, colour).Clone();
			textImage.Palette = palette;

			switch (align)
			{
				case TextAlign.Left:
					gfx.DrawImage(textImage, x * Scale, y * Scale, textImage.Width * Scale, textImage.Height * Scale);
					break;
				case TextAlign.Center:
					gfx.DrawImage(textImage, (x - (textImage.Width / 2)) * Scale, y * Scale, textImage.Width * Scale, textImage.Height * Scale);
					break;
				case TextAlign.Right:
					gfx.DrawImage(textImage, (x - textImage.Width) * Scale, y * Scale, textImage.Width * Scale, textImage.Height * Scale);
					break;
			}
		}
		internal void DrawText(Graphics gfx, string text, int font, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			DrawText(gfx, text, font, colour, colour, x, y, align);
		}
		internal void DrawText(Graphics gfx, string text, int font, byte firstLetterColour, byte colour, int x, int y, TextAlign align = TextAlign.Left)
		{
			Bitmap textImage = Resources.Instance.GetText(text, font, firstLetterColour, colour);

			switch (align)
			{
				case TextAlign.Left:
					gfx.DrawImage(textImage, x * Scale, y * Scale, textImage.Width * Scale, textImage.Height * Scale);
					break;
				case TextAlign.Center:
					gfx.DrawImage(textImage, (x - (textImage.Width / 2)) * Scale, y * Scale, textImage.Width * Scale, textImage.Height * Scale);
					break;
				case TextAlign.Right:
					gfx.DrawImage(textImage, (x - textImage.Width) * Scale, y * Scale, textImage.Width * Scale, textImage.Height * Scale);
					break;
			}
		}
		internal TextureBrush ScaleTexture(Bitmap texture)
		{
			Bitmap textureBitmap = new Bitmap(texture.Width * Scale, texture.Height * Scale);
			Graphics gfx = Graphics.FromImage(textureBitmap);
			gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			gfx.PixelOffsetMode = PixelOffsetMode.Half;
			gfx.DrawImage(texture, 0, 0, textureBitmap.Width, textureBitmap.Height);
			return new TextureBrush(textureBitmap);
		}
		public abstract bool KeyDown(KeyEventArgs args);
		public abstract bool MouseDown(MouseEventArgs args);
	}
}