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
using CivOne.Events;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class MenuBar : BaseScreen
	{
		private const int FONT_ID = 0;
		
		public event EventHandler GameSelected;
		public event EventHandler OrdersSelected;
		public event EventHandler AdvisorsSelected;
		public event EventHandler WorldSelected;
		public event EventHandler CivilopediaSelected;

		public bool MenuDrag { get; private set; }
		
		private readonly Rectangle[] _rectMenus;
		
		private bool _update = true;
		private int _mouseX, _mouseY;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas.FillRectangle(5, 0, 0, 320, 8);
				_canvas.DrawText("GAME", FONT_ID, 15, 7, 8, 1, TextAlign.Left);
				_canvas.DrawText("ORDERS", FONT_ID, 15, 7, 64, 1, TextAlign.Left);
				_canvas.DrawText("ADVISORS", FONT_ID, 15, 7, 128, 1, TextAlign.Left);
				_canvas.DrawText("WORLD", FONT_ID, 15, 7, 192, 1, TextAlign.Left);
				_canvas.DrawText("CIVILOPEDIA", FONT_ID, 15, 7, 240, 1, TextAlign.Left);

				_update = false;
				return true;
			}
			return false;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (!args.Alt) return false;
			
			switch (args.KeyChar)
			{
				case 'G':
					GameSelected?.Invoke(this, null);
					break;
				case 'O':
					OrdersSelected?.Invoke(this, null);
					break;
				case 'A':
					AdvisorsSelected?.Invoke(this, null);
					break;
				case 'W':
					WorldSelected?.Invoke(this, null);
					break;
				case 'C':
					CivilopediaSelected?.Invoke(this, null);
					break;
				default:
					return false;
			}
			MenuDrag = false;
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			_mouseX = args.X;
			_mouseY = args.Y;

			if (_rectMenus[0].Contains(args.Location) && GameSelected != null) GameSelected(this, null);
			if (_rectMenus[1].Contains(args.Location) && OrdersSelected != null) OrdersSelected(this, null);
			if (_rectMenus[2].Contains(args.Location) && AdvisorsSelected != null) AdvisorsSelected(this, null);
			if (_rectMenus[3].Contains(args.Location) && WorldSelected != null) WorldSelected(this, null);
			if (_rectMenus[4].Contains(args.Location) && CivilopediaSelected != null) CivilopediaSelected(this, null);
			
			return false;
		}
		
		public override bool MouseUp(ScreenEventArgs args)
		{
			MenuDrag = !(args.X == _mouseX && args.Y == _mouseY);

			return false;
		}

		public void Resize()
		{
			_update = true;
		}
		
		public MenuBar(Color[] palette)
		{
			_canvas = new Picture(320, 8, palette);
			_canvas.FillRectangle(5, 0, 0, 320, 8);
			_update = true;
			
			_rectMenus = new Rectangle[5];
			_rectMenus[0] = new Rectangle(0, 0, 56, 8);
			_rectMenus[1] = new Rectangle(56, 0, 64, 8);
			_rectMenus[2] = new Rectangle(120, 0, 64, 8);
			_rectMenus[3] = new Rectangle(184, 0, 48, 8);
			_rectMenus[4] = new Rectangle(232, 0, 88, 8);
		}
	}
}