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
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.IO;

namespace CivOne.Screens
{
	public abstract partial class BaseScreen : IDefaultTextSettings
	{
		public TextSettings DefaultTextSettings { get; set; }

		protected int Width => Bitmap.Width;
		protected int Height => Bitmap.Height;

		private Bytemap _bitmap;
		public Bytemap Bitmap
		{
			get
			{
				return _bitmap;
			}
			protected set
			{
				_bitmap?.Dispose();
				_bitmap = value;
			}
		}
		private Palette _palette, _originalColours;
		public Palette Palette
		{
			get
			{
				return _palette;
			}
			set
			{
				_palette = value.Copy();
				if (_originalColours == null)
					_originalColours = value.Copy();
			}
		}
		public Palette OriginalColours => _originalColours;
		
		protected void DrawPanel(int x, int y, int width, int height, bool border = true)
		{
			int xx = x, yy = y, ww = width, hh = height;
			if (border)
			{
				xx++;
				yy++;
				ww -= 2;
				hh -= 2;
				this.DrawRectangle(x, y, width, height);
			}
			this.AddLayer(new Picture(ww, hh)
				.Tile(Patterns.PanelGrey)
				.DrawRectangle3D(), xx, yy, dispose: true);
		}

		protected void DrawBorder(int border)
		{
			border = (border % 2);
			Picture[] borders = new Picture[8];
			int index = 0;
			for (int yy = 0; yy < 2; yy++)
			for (int xx = 0; xx < 4; xx++)
			{
				borders[index] = Resources["SP299"].GetPart(((border == 0) ? 192 : 224) + (8 * xx), 120 + (8 * yy), 8, 8);
				index++;
			}
			
			for (int x = 8; x < Width - 8; x += 8)
			{
				this.AddLayer(borders[4], x, 0)
					.AddLayer(borders[6], x, Height - 8);
	}
			for (int y = 8; y < Height - 8; y += 8)
			{
				this.AddLayer(borders[7], 0, y)
					.AddLayer(borders[5], Width - 8, y);
			}
			this.AddLayer(borders[0], 0, 0)
				.AddLayer(borders[1], Width - 8, 0)
				.AddLayer(borders[2], 0, Height - 8)
				.AddLayer(borders[3], Width - 8, Height - 8);
		}

		protected void DrawButton(string text, byte colour, byte colourDark, int x, int y, int width)
		{
			this.FillRectangle(x, y, width, 1, 7)
				.FillRectangle(x, y + 1, 1, 8, 7)
				.FillRectangle(x + 1, y + 8, width - 1, 1, colourDark)
				.FillRectangle(x + width - 1, y, 1, 8, colourDark)
				.FillRectangle(x + 1, y + 1, width - 2, 7, colour)
				.DrawText(text, 1, colourDark, x + (int)Math.Ceiling((double)width / 2), y + 2, TextAlign.Center);
		}

		//
		public void ResetPalette()
		{
			for (int i = 0; i < 256; i++)
				Palette[i] = OriginalColours[i];
		}

		public void SetPalette(Palette palette)
		{
			for (int i = 1; i < palette.Length && i < 256; i++)
				Palette[i] = palette[i];
		}
		//

		public virtual void Dispose()
		{
			Bitmap?.Dispose();
			Palette?.Dispose();
			OriginalColours?.Dispose();
		}
	}
}