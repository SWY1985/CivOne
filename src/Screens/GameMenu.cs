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
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class GameMenu : BaseScreen
	{
		internal class Item
		{
			public event EventHandler Selected;
			public bool Enabled = true;
			public string Text;
			public string Shortcut;
			
			internal void Select()
			{
				if (Selected == null) return;
				Selected(this, null);
			}
			
			internal int ItemWidth
			{
				get
				{
					if (Shortcut == null)
						return TextWidth;
					return Resources.Instance.GetTextSize(0, Shortcut).Width + TextWidth + 8;
				}
			}
			
			internal int TextWidth
			{
				get
				{
					if (Text == null)
						return 0;
					return Resources.Instance.GetTextSize(0, Text).Width;
				}
			}
			
			internal void Draw(Picture picture, int x, int y)
			{
				if (Text == null) return;
				picture.DrawText(Text, 0, (byte)(Enabled ? 5 : 3), x, y, TextAlign.Left);
				if (Shortcut == null) return;
				picture.DrawText(Shortcut, 0, 15, x + TextWidth + 8, y, TextAlign.Left);
			}
			
			public Item(string text, string shortcut = null)
			{
				Text = text;
				Shortcut = shortcut;
			}
		}
		
		private readonly Color[] _palette;
		public readonly List<Item> Items = new List<Item>();
		
		private int _activeItem = -1;
		private bool _update = true;
		
		private int MaxItemWidth
		{
			get
			{
				int ww = 0;
				foreach (Item item in Items)
				{
					if (item.ItemWidth <= ww) continue;
					ww = item.ItemWidth;
				}
				return ww;
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (!_update) return false;
			
			int ww = MaxItemWidth + 17;
			int hh = (Resources.Instance.GetFontHeight(0) * Items.Count) + 9;
			
			Bitmap background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
			
			// This is a workaround, until I figure out how to make bitmaps where Width doesn't divide by 4
			int ow = ww;
			if (ww % 4 > 0)
				ww += (4 - (ww % 4));
			
			_canvas = new Picture(ww, hh, _palette);
			_canvas.FillLayerTile(background, 1, 1);
			if (ow != ww)
				_canvas.FillRectangle(0, ow, 0, 4 - (ow % 4), hh);
			
			_canvas.AddBorder(5, 5, 0, 0, ow, hh);
			_canvas.AddBorder(15, 8, 0, 0, ow, hh, 1);
			
			int i = 0;
			int yy = 5;
			foreach (Item item in Items)
			{
				if (i == _activeItem)
				{
					_canvas.ColourReplace(7, 11, 3, yy - 1, MaxItemWidth + 11, Resources.Instance.GetFontHeight(0));
					_canvas.ColourReplace(22, 3, 3, yy - 1, MaxItemWidth + 11, Resources.Instance.GetFontHeight(0));
				}
				item.Draw(_canvas, 11, yy);
				yy += Resources.Instance.GetFontHeight(0);
				i++;
			}
			
			_update = false;
			return true;
		}
		
		private int MouseOverItem(MouseEventArgs args)
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
		
		public override bool MouseDrag(MouseEventArgs args)
		{
			int index = MouseOverItem(args);
			if (index < 0 || index == _activeItem) return false;
			
			_activeItem = index;
			
			_update = true;
			return true;
		}
		
		public GameMenu(Color[] palette)
		{
			_palette = palette;
			
			_canvas = new Picture(8, 8, _palette);
			_canvas.FillRectangle(2, 0, 0, 8, 8);
		}
	}
}