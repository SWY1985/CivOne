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
using CivOne.Events;
using CivOne.IO;
using CivOne.Graphics;
using CivOne.UserInterface;

namespace CivOne.Screens.GamePlayPanels
{
	internal class MenuBar : Panel
	{
		private const int FONT_ID = 0;

		private readonly TextSettings _textSettings;
		
		public event EventHandler GameSelected;
		public event EventHandler OrdersSelected;
		public event EventHandler AdvisorsSelected;
		public event EventHandler WorldSelected;
		public event EventHandler CivilopediaSelected;
		
		private readonly Rectangle[] _rectMenus = new Rectangle[5];

		private void Draw(object sender, EventArgs args)
		{
			Bitmap.AddLayer(new Picture(Width, Height)
				.Clear(5)
				.DrawText("GAME", 8, 1, _textSettings)
				.DrawText("ORDERS", 64, 1, _textSettings)
				.DrawText("ADVISORS", 128, 1, _textSettings)
				.DrawText("WORLD", 192, 1, _textSettings)
				.DrawText("CIVILOPEDIA", 240, 1, _textSettings)
				.Bitmap);
		}

		private void MouseDown(object sender, ScreenEventArgs args)
		{
			args.Handled = true;

			if (_rectMenus[0].Contains(args.Location) && GameSelected != null) GameSelected(this, null);
			if (_rectMenus[1].Contains(args.Location) && OrdersSelected != null) OrdersSelected(this, null);
			if (_rectMenus[2].Contains(args.Location) && AdvisorsSelected != null) AdvisorsSelected(this, null);
			if (_rectMenus[3].Contains(args.Location) && WorldSelected != null) WorldSelected(this, null);
			if (_rectMenus[4].Contains(args.Location) && CivilopediaSelected != null) CivilopediaSelected(this, null);
		}

		public void Resize(int width) => base.Resize(width, 8);

		public MenuBar() : base(0, 0)
		{
			OnDraw += Draw;
			OnMouseDown += MouseDown;

			_textSettings = TextSettings.DifferentFirstLetter(15, 7);
			
			_rectMenus[0] = new Rectangle(0, 0, 56, 8);
			_rectMenus[1] = new Rectangle(56, 0, 64, 8);
			_rectMenus[2] = new Rectangle(120, 0, 64, 8);
			_rectMenus[3] = new Rectangle(184, 0, 48, 8);
			_rectMenus[4] = new Rectangle(232, 0, 88, 8);

			Resize(320);
		}
	}
}