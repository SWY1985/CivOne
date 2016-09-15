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
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class SetRate : BaseScreen
	{
		private readonly bool _luxuries;

		private readonly Bitmap _background;
		private readonly int _fontId = 0;
		
		private bool _update = true;

		private IEnumerable<string> MenuOptions
		{
			get
			{
				if (_luxuries)
				{
					for (int i = 0; i <= (10 - Game.Instance.HumanPlayer.TaxesRate); i++)
					{
						int science = 10 - Game.Instance.HumanPlayer.TaxesRate - i;
						yield return $"{i * 10}% Luxuries, ({science * 10}% Science)";
					}
					yield break;
				}

				for (int i = 0; i <= (10 - Game.Instance.HumanPlayer.LuxuriesRate); i++)
				{
					int science = 10 - Game.Instance.HumanPlayer.LuxuriesRate - i;
					yield return $"{i * 10}% Tax, ({science * 10}% Science)";
				}
			}
		}

		private string ScreenName
		{
			get
			{
				if (_luxuries)
					return "Luxuries";
				return "Tax";
			}
		}
		
		private void MenuCancel(object sender, EventArgs args)
		{
			CloseMenus();
			Close();
		}

		private void TaxesChoice(object sender, EventArgs args)
		{
			Game.Instance.HumanPlayer.TaxesRate = (sender as Menu.Item).Value;
			MenuCancel(sender, args);
		}

		private void LuxuriesChoice(object sender, EventArgs args)
		{
			Game.Instance.HumanPlayer.LuxuriesRate = (sender as Menu.Item).Value;
			MenuCancel(sender, args);
		}

		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				string[] menuItems = MenuOptions.ToArray();
				int itemWidth = menuItems.Max(x => Resources.Instance.GetTextSize(0, x).Width) + 12;

				int actualWidth = itemWidth + 14;
				int width = actualWidth;
				if (width % 4 > 0)
					width += (4 - (width % 4));
				int height = (menuItems.Length * 8) + 15;

				Picture menuGfx = new Picture(width, height);
				menuGfx.FillLayerTile(_background);
				menuGfx.AddBorder(15, 8, 0, 0, actualWidth, height);
				menuGfx.DrawText($"Select new {ScreenName} rate...", 0, 15, 4, 4); 
				if (width > actualWidth)
					menuGfx.FillRectangle(0, actualWidth, 0, (width - actualWidth), height);
				
				_canvas.FillRectangle(5, 100, 80, actualWidth + 2, height + 2);
				AddLayer(menuGfx, 101, 81);
				
				Bitmap background = (Bitmap)menuGfx.GetPart(2, 11, itemWidth, 8 * menuItems.Count() + 4).Clone();
				Picture.ReplaceColours(background, new byte[] { 7, 22 }, new byte[] { 11, 3 });

				Menu menu = new Menu(Canvas.Image.Palette.Entries, background)
				{
					X = 103,
					Y = 92,
					Width = itemWidth,
					ActiveColour = 11,
					TextColour = 5,
					FontId = _fontId
				};
				Menu.Item menuItem;
				int i = 0;
				
				foreach (string item in menuItems)
				{
					menu.Items.Add(menuItem = new Menu.Item(item, i++));
					if (_luxuries)
						menuItem.Selected += LuxuriesChoice;
					else
						menuItem.Selected += TaxesChoice;
				}
				menu.Width += 10;
				menu.MissClick += MenuCancel;
				menu.Cancel += MenuCancel;
				if (_luxuries)
					menu.ActiveItem = Game.Instance.HumanPlayer.LuxuriesRate;
				else
					menu.ActiveItem = Game.Instance.HumanPlayer.TaxesRate;
				Menus.Add(menu);
				
				Common.AddScreen(menu);
				
				_update = false;
			}
			return true;
		}

		public void Close()
		{
			HandleClose();
			Destroy();
		}

		public static SetRate Taxes
		{
			get
			{
				return new SetRate(luxuries: false);
			}
		}

		public static SetRate Luxuries
		{
			get
			{
				return new SetRate(luxuries: true);
			}
		}

		private SetRate(bool luxuries)
		{
			_luxuries = luxuries;
			_background = (Bitmap)Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
			
			Cursor = MouseCursor.Pointer;

			Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			
			_canvas = new Picture(320, 200, palette);
		}
	}
}