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
	internal class Menu : BaseScreen
	{
		internal class Item
		{
			public bool Enabled = true;
			public string Text;
			
			public Item(string text)
			{
				Text = text;
			}
		}
		
		public readonly List<Item> Items = new List<Item>(); 
		public int FontId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public byte ActiveColour { get; set; }
		public byte TextColour { get; set; }
		public byte DisabledColour { get; set; }
		
		private bool _change = true;
		private int _selectedItem = 0;
		public int SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			private set
			{
				_change = true;
				_selectedItem = value;
				if (_selectedItem < 0) _selectedItem = 0;
				if (_selectedItem >= Items.Count) _selectedItem = (Items.Count - 1);
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_change)
			{
				int yy = Y + (_selectedItem * 8);
				
				_canvas.FillRectangle(0, 0, 0, 320, 200);
				_canvas.FillRectangle(ActiveColour, X, yy, Width, 8);
				for (int i = 0; i < Items.Count; i++)
				{
					yy = Y + (i * 8);
					_canvas.DrawText(Items[i].Text, FontId, (byte)(Items[i].Enabled ? TextColour : DisabledColour), X + 8, yy + 1);
				}				
				_change = false;
				return true;
			}
			return false;
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			if (args.KeyCode == Keys.Up) { SelectedItem--; return true; }
			if (args.KeyCode == Keys.Down) { SelectedItem++; return true; }
			return false;
		}
		
		public override bool MouseDown(MouseEventArgs args)
		{
			return false;
		}
		
		public Menu(Color[] colours)
		{
			Cursor = MouseCursor.Pointer;
			
			_canvas = new Picture(320, 200, colours);
		}
	}
}