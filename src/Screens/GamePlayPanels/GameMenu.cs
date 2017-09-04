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

namespace CivOne.Screens.GamePlayPanels
{
	[OverlayPanel]
	public class GameMenu : Panel
	{
		private readonly MenuItemCollection<int> _items;
		
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

		private int MaxItemWidth => _items.Select(x => ItemWidth(x)).Max();

		private void MenuItemDraw(Picture picture, MenuItem<int> menuItem, int x, int y)
		{
			if (menuItem == null || menuItem.Text == null) return;
			picture.DrawText(menuItem.Text, 0, (byte)(menuItem.Enabled ? 5 : 3), x, y, TextAlign.Left);
			if (menuItem.Shortcut != null)
			{
				int textWidth = Resources.GetTextSize(0, menuItem.Text).Width;
				picture.DrawText(menuItem.Shortcut, 0, 15, x + textWidth + 8, y, TextAlign.Left);
			}
			Bitmap.AddLayer(picture.Bitmap);
		}
		
		protected override bool HasUpdate(uint gameTick) => _update;
		// {
			// if (!_update) return true;
			
		// 	int ww = MaxItemWidth + 17;
		// 	int hh = (Resources.GetFontHeight(0) * Items.Count) + 9;
			
		// 	// Bitmap = new Bytemap(ww, hh);

		// 	using (Picture picture = new Picture(ww, hh))
		// 	{
		// 		picture.Tile(Pattern.PanelGrey, 1, 1)
		// 			.DrawRectangle()
		// 			.DrawRectangle3D(1, 1, ww - 2, hh - 2)
		// 			.As<Picture>();
				
		// 		int i = 0;
		// 		int yy = 5;
		// 		foreach (MenuItem<int> menuItem in Items)
		// 		{
		// 			if (i == _activeItem)
		// 			{
		// 				picture.ColourReplace(7, 11, 3, yy - 1, MaxItemWidth + 11, Resources.GetFontHeight(0))
		// 					.ColourReplace(22, 3, 3, yy - 1, MaxItemWidth + 11, Resources.GetFontHeight(0));
		// 			}
		// 			MenuItemDraw(menuItem, 11, yy);
		// 			yy += Resources.GetFontHeight(0);
		// 			i++;
		// 		}
		// 		Bitmap.AddLayer(picture.Bitmap);
		// 	}
			
		// 	_update = false;
		// 	return true;
		// }
		
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
					if (_activeItem <= (_items.Count - 1))
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
						_items[_activeItem].Select();
					return;
			}
			args.Handled = true;
		}
		
		private int MouseOverItem(ScreenEventArgs args)
		{
			int fontHeight = Resources.GetFontHeight(0);
			int yy = 5;
			
			for (int i = 0; i < _items.Count; i++)
			{
				if (new Rectangle(3, yy, MaxItemWidth + 8, fontHeight).Contains(args.Location)) return i;
				yy += fontHeight;
			}
			
			return -1;
		}

		private void Draw(object sender, EventArgs args)
		{
			if (Bitmap == null || _items.Count == 0) return;
			using (Picture picture = new Picture(Width, Height))
			{
				picture.Tile(Pattern.PanelGrey, 1, 1)
					.DrawRectangle()
					.DrawRectangle3D(1, 1, Width - 2, Height - 2)
					.As<Picture>();
				
				int i = 0;
				int yy = 5;
				foreach (MenuItem<int> menuItem in _items)
				{
					if (i == _activeItem)
					{
						picture.ColourReplace(7, 11, 3, yy - 1, MaxItemWidth + 11, Resources.GetFontHeight(0))
							.ColourReplace(22, 3, 3, yy - 1, MaxItemWidth + 11, Resources.GetFontHeight(0));
					}
					MenuItemDraw(picture, menuItem, 11, yy);
					yy += Resources.GetFontHeight(0);
					i++;
				}
				Bitmap.AddLayer(picture.Bitmap);
			}
			
			_update = false;
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
			_items[_activeItem]?.Select();
			_keepOpen = false;
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

		private void Redraw(object sender, EventArgs args)
		{
			int ww = MaxItemWidth + 17;
			int hh = (Resources.GetFontHeight(0) * _items.Count) + 9;

			Resize(ww, hh);
		}

		public MenuItem<int> AddItem(string text, int id = default(int)) => AddItem(MenuItem<int>.Create(text, id));
		public MenuItem<int> AddItem(MenuItem<int> item)
		{
			_items.Add(item);
			return item;
		}
		
		public GameMenu(string menuId, int left) : base(left, 8)
		{
			_items = new MenuItemCollection<int>(menuId);
			_items.ItemsChanged += Redraw;

			// OnKeyDown += KeyDown;
			OnDraw += Draw;
			OnMouseDown += MouseDown;
			OnMouseUp += MouseUp;
			OnMouseDrag += MouseDrag;
		}
	}
}