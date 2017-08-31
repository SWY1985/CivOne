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
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.IO;
using CivOne.Graphics.Sprites;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	public class GameMenu : BaseScreen
	{
		public readonly MenuItemCollection<int> Items;
		
		private int _activeItem = -1;
		private bool _update = true;

		private bool _keepOpen = false;
		public bool KeepOpen
		{
			get
			{
				return _keepOpen;
			}
			set
			{
				_keepOpen = true;
				_activeItem = 0;
			}
		}

		private int ItemWidth(MenuItem<int> menuItem)
		{
			int width = 0;
			if (menuItem != null)
			{
				if (menuItem.Text != null) width += Resources.GetTextSize(0, menuItem.Text).Width;
				if (menuItem.Shortcut != null) width += Resources.GetTextSize(0, menuItem.Shortcut).Width + 8;
			}
			return width;
		}

		private int MaxItemWidth => Items.Select(x => ItemWidth(x)).Max();

		private void MenuItemDraw(MenuItem<int> menuItem, int x, int y)
		{
			if (menuItem == null || menuItem.Text == null) return;
			this.DrawText(menuItem.Text, 0, (byte)(menuItem.Enabled ? 5 : 3), x, y, TextAlign.Left);
			if (menuItem.Shortcut == null) return;
			int textWidth = Resources.GetTextSize(0, menuItem.Text).Width;
			this.DrawText(menuItem.Shortcut, 0, 15, x + textWidth + 8, y, TextAlign.Left);
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (!_update) return true;
			
			int ww = MaxItemWidth + 17;
			int hh = (Resources.GetFontHeight(0) * Items.Count) + 9;
			
			Bitmap = new Bytemap(ww, hh);
			this.Tile(Pattern.PanelGrey, 1, 1)
				.DrawRectangle()
				.DrawRectangle3D(1, 1, ww - 2, hh - 2)
				.As<Picture>();
			
			int i = 0;
			int yy = 5;
			foreach (MenuItem<int> menuItem in Items)
			{
				if (i == _activeItem)
				{
					this.ColourReplace(7, 11, 3, yy - 1, MaxItemWidth + 11, Resources.GetFontHeight(0))
						.ColourReplace(22, 3, 3, yy - 1, MaxItemWidth + 11, Resources.GetFontHeight(0));
				}
				MenuItemDraw(menuItem, 11, yy);
				yy += Resources.GetFontHeight(0);
				i++;
			}
			
			_update = false;
			return true;
		}
		
		private void KeyDown(object sender, KeyboardEventArgs args)
		{
			switch (args.Key)
			{
				case Key.NumPad8:
				case Key.Up:
					if (_activeItem > 0)
					{
						_activeItem--;
						_update = true;
					}
					break;
				case Key.NumPad2:
				case Key.Down:
					if (_activeItem <= (Items.Count - 1))
					{
						_activeItem++;
						_update = true;
					}
					break;
				case Key.Escape:
					KeepOpen = false;
					return;
				case Key.Enter:
					if (_activeItem >= 0)
						Items[_activeItem].Select();
					return;
			}
			args.Handled = true;
		}
		
		private int MouseOverItem(ScreenEventArgs args)
		{
			int fontHeight = Resources.GetFontHeight(0);
			int yy = 5;
			
			for (int i = 0; i < Items.Count; i++)
			{
				if (new Rectangle(3, yy, MaxItemWidth + 8, fontHeight).Contains(args.Location)) return i;
				yy += fontHeight;
			}
			
			return -1;
		}
		
		private void MouseDown(object sender, ScreenEventArgs args)
		{
			if (!KeepOpen) return;
			int index = MouseOverItem(args);
			if (index == _activeItem) return;
						
			_activeItem = index;
			
			_update = true;
			args.Handled = true;
		}
		
		private void MouseUp(object sender, ScreenEventArgs args)
		{
			if (_activeItem < 0 && !KeepOpen) return;
			if (_activeItem < 0 && KeepOpen)
			{
				KeepOpen = false;
				return;
			}
			Items[_activeItem]?.Select();
			args.Handled = true;
		}
		
		private void MouseDrag(object sender, ScreenEventArgs args)
		{
			if (KeepOpen) return;
			int index = MouseOverItem(args);
			if (index == _activeItem) return;
						
			_activeItem = index;
			
			_update = true;
			args.Handled = true;
		}
		
		public GameMenu(string menuId, Palette palette) : base(8, 8)
		{
			Items = new MenuItemCollection<int>(menuId);
			
			Palette = palette.Copy();

			OnKeyDown += KeyDown;
			OnMouseDown += MouseDown;
			OnMouseUp += MouseUp;
			OnMouseDrag += MouseDrag;
		}
	}
}