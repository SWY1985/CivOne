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
using CivOne.UserInterface;

namespace CivOne.Screens
{
	public class GameMenu : BaseScreen
	{
		private readonly Palette _palette;
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

		private void MenuItemDraw(MenuItem<int> menuItem, Picture picture, int x, int y)
		{
			if (menuItem == null || menuItem.Text == null) return;
			picture.DrawText(menuItem.Text, 0, (byte)(menuItem.Enabled ? 5 : 3), x, y, TextAlign.Left);
			if (menuItem.Shortcut == null) return;
			int textWidth = Resources.GetTextSize(0, menuItem.Text).Width;
			picture.DrawText(menuItem.Shortcut, 0, 15, x + textWidth + 8, y, TextAlign.Left);
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (!_update) return true;
			
			int ww = MaxItemWidth + 17;
			int hh = (Resources.Instance.GetFontHeight(0) * Items.Count) + 9;
			
			_canvas = new Picture(ww, hh, _palette)
				.Tile(Patterns.PanelGrey, 1, 1)
				.DrawRectangle()
				.DrawRectangle3D(1, 1, ww - 2, hh - 2)
				.As<Picture>();
			
			int i = 0;
			int yy = 5;
			foreach (MenuItem<int> menuItem in Items)
			{
				if (i == _activeItem)
				{
					_canvas.ColourReplace(7, 11, 3, yy - 1, MaxItemWidth + 11, Resources.Instance.GetFontHeight(0));
					_canvas.ColourReplace(22, 3, 3, yy - 1, MaxItemWidth + 11, Resources.Instance.GetFontHeight(0));
				}
				MenuItemDraw(menuItem, _canvas, 11, yy);
				yy += Resources.Instance.GetFontHeight(0);
				i++;
			}
			
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
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
					return true;
				case Key.NumPad2:
				case Key.Down:
					if (_activeItem <= (Items.Count - 1))
					{
						_activeItem++;
						_update = true;
					}
					return true;
				case Key.Escape:
					KeepOpen = false;
					return false;
				case Key.Enter:
					if (_activeItem >= 0)
						Items[_activeItem].Select();
					return false;
			}
			return true;
		}
		
		private int MouseOverItem(ScreenEventArgs args)
		{
			int fontHeight = Resources.Instance.GetFontHeight(0);
			int yy = 5;
			
			for (int i = 0; i < Items.Count; i++)
			{
				if (new Rectangle(3, yy, MaxItemWidth + 8, fontHeight).Contains(args.Location)) return i;
				yy += fontHeight;
			}
			
			return -1;
		}
		
		public override bool MouseDrag(ScreenEventArgs args)
		{
			if (KeepOpen) return false;
			int index = MouseOverItem(args);
			if (index == _activeItem) return false;
						
			_activeItem = index;
			
			_update = true;
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (!KeepOpen) return false;
			int index = MouseOverItem(args);
			if (index == _activeItem) return false;
						
			_activeItem = index;
			
			_update = true;
			return true;
		}
		
		public override bool MouseUp(ScreenEventArgs args)
		{
			if (_activeItem < 0 && !KeepOpen) return false;
			if (_activeItem < 0 && KeepOpen)
			{
				KeepOpen = false;
				return false;
			}
			Items[_activeItem]?.Select();
			
			return true;
		}
		
		public GameMenu(string menuId, Palette palette)
		{
			Items = new MenuItemCollection<int>(menuId);
			
			_palette = palette;
			
			_canvas = new Picture(8, 8, _palette);
			this.FillRectangle(2, 0, 0, 8, 8);
		}
	}
}