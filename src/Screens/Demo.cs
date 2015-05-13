// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.GFX;

namespace CivOne.Screens
{
	internal class Demo : BaseScreen
	{
		private readonly Picture _background;
		private readonly Picture _logo;
		private readonly ColorPalette _palette;
		private readonly byte[] _textColours;
		
		public override MouseCursor Cursor
		{
			get
			{
				return MouseCursor.Pointer;
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			return false;
		}
		
		public override void Draw(Graphics gfx)
		{
			base.Draw(gfx);

			gfx.DrawImage(_background.Image, 0, 0, 320 * Scale, 200 * Scale);
			gfx.DrawImage(_logo.Image, 0, 0, 320 * Scale, 200 * Scale);
			
			DrawText(gfx, _palette, "One more turn...", 3, _textColours[0], 160, 160, TextAlign.Center);
			DrawText(gfx, _palette, "One more turn...", 3, _textColours[2], 160, 162, TextAlign.Center);
			DrawText(gfx, _palette, "One more turn...", 3, _textColours[1], 160, 161, TextAlign.Center);
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			return false;
		}
		
		public override bool MouseDown(MouseEventArgs args)
		{
			return false;
		}
		
		public Demo()
		{
			_background = Resources.Instance.LoadPIC("BIRTH1");
			_logo = Resources.Instance.LoadPIC("LOGO");
			_palette = _logo.Image.Palette;
			switch (Settings.Instance.GraphicsMode)
			{
				case GraphicsMode.Graphics256:
					_textColours = new byte[] { 239, 236, 233, 5, 229 };
					break;
				case GraphicsMode.Graphics16:
					_textColours = new byte[] { 15, 15, 7, 5, 8 };
					break;
			}
		}
	}
}