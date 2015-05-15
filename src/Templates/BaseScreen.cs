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

namespace CivOne.Templates
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
		
		private Picture _canvas;
		public abstract Picture Canvas { get; }
		public abstract MouseCursor Cursor { get; }
		public abstract bool HasUpdate(uint gameTick);
		public virtual void Draw(Graphics gfx)
		{
			gfx.Clear(Color.Black);
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