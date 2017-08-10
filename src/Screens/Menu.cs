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
using CivOne.Graphics;
using CivOne.Interfaces;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	public class Menu<T> : BaseScreen, IMenu, IExpand
	{
		private readonly Picture _background;
		
		public event EventHandler Cancel;
		public event EventHandler MissClick;
		
		public readonly MenuItemCollection<T> Items;

		public string Id => Items.Id;

		public string Title { get; set; }
		public int FontId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int Indent { get; set; }
		public byte TitleColour { get; set; }
		public byte ActiveColour { get; set; }
		public byte TextColour { get; set; }
		public byte DisabledColour { get; set; }
		public int IndentTitle { get; set; }
		public int RowHeight { get; set; }
		
		private bool _mouseDown = false;
		private bool _change = true;
		private int _activeItem = 0;
		public int ActiveItem
		{
			get
			{
				return _activeItem;
			}
			set
			{
				_change = true;
				_activeItem = value;
				if (_activeItem < 0) _activeItem = 0;
				if (_activeItem >= Items.Count) _activeItem = (Items.Count - 1);
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			int fontHeight = Resources.Instance.GetFontHeight(FontId);
			if (RowHeight != 0) fontHeight = RowHeight;
			if (_change)
			{
				int yy = Y + (_activeItem * fontHeight);
				int offsetY = 0;
				
				_canvas.FillRectangle(0, 0, 0, 320, 200);
				if (Title != null)
				{
					_canvas.DrawText(Title, FontId, TitleColour, X + IndentTitle, Y + 1);
					offsetY = fontHeight;
				}
				if (_activeItem >= 0)
				{
					if (_background == null)
					{
						_canvas.FillRectangle(ActiveColour, X, yy + offsetY, Width, fontHeight);
					}
					else
					{
						_canvas.AddLayer(_background.GetPart(0, (_activeItem * fontHeight) + offsetY, Width, fontHeight), X, yy + offsetY);
					}
				}
				for (int i = 0; i < Items.Count; i++)
				{
					yy = Y + (i * fontHeight) + offsetY;
					_canvas.DrawText(Items[i].Text, FontId, (byte)(Items[i].Enabled ? TextColour : DisabledColour), X + Indent, yy + 1);
				}
				_change = false;
				return true;
			}
			return false;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			switch (args.Key)
			{
				case Key.NumPad8:
				case Key.Up:
					ActiveItem--;
					return true;
				case Key.NumPad2:
				case Key.Down:
					ActiveItem++;
					return true;
				case Key.Enter:
					if (!Items[_activeItem].Enabled) return false;
					Items[_activeItem].Select();
					return true;
				case Key.Escape:
					if (Cancel != null)
					{
						_activeItem = -1;
						_change = true;
						HasUpdate(0);
						Cancel(this, null);
					}
					break;
			}
			return false;
		}
		
		private int MouseOverItem(ScreenEventArgs args)
		{
			int fontHeight = Resources.Instance.GetFontHeight(FontId);
			if (RowHeight != 0) fontHeight = RowHeight;
			int yy = Y;
			
			if (Title != null) yy += fontHeight;
			for (int i = 0; i < Items.Count; i++)
			{
				if (new Rectangle(X, yy, Width, fontHeight).Contains(args.Location)) return i;
				yy += fontHeight;
			}
			
			return -1;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			_mouseDown = true;

			int index = MouseOverItem(args);
			if (index < 0 && MissClick != null)
			{
				MissClick(this, null);
				return true;
			}
			if (index < 0 || index == _activeItem) return false;
			ActiveItem = index;
			_change = true;
			return true;
		}
		
		public override bool MouseUp(ScreenEventArgs args)
		{
			if (!_mouseDown) return true;

			int index = MouseOverItem(args);
			if (index < 0) return false;
			ActiveItem = index;
			if (Items[_activeItem].Enabled)
			{
				if ((args.Buttons & MouseButton.Right) > 0)
				{
					_mouseDown = false;
					Items[_activeItem].Context();
				}
				else
				{
					Items[_activeItem].Select();
				}
			}
			_change = true;
			return true;
		}
		
		public override bool MouseDrag(ScreenEventArgs args)
		{
			int index = MouseOverItem(args);
			if (index < 0 || index == _activeItem) return false;
			ActiveItem = index;
			_change = true;
			return true;
		}

		public void Resize(int width, int height)
		{
			_canvas = new Picture(width, height, _canvas.Palette);
			_change = true;
			HasUpdate(0);
		}
		
		public void Close()
		{
			Destroy();
		}
		
		public Menu(string menuId, Color[] colours, Picture background = null)
		{
			Items = new MenuItemCollection<T>(menuId);

			if (background != null)
			{
				_background = new Picture(background);
			}

			Cursor = MouseCursor.Pointer;
			IndentTitle = 8;
			Indent = 8;
			
			_canvas = new Picture(320, 200, colours);
		}
	}

	public class Menu : Menu<int>
	{
		public Menu(Color[] colours, Picture background = null) : base(null, colours, background)
		{
		}

		public Menu(string menuId, Color[] colours, Picture background = null) : base(menuId, colours, background)
		{
		}
	}
}