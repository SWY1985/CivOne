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
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Setup : BaseScreen
	{
		public override bool HasUpdate(uint gameTick)
		{
			return false;
		}
		
		private Menu AddMenu(int x, int y, string title, EventHandler setChoice, params string[] menuTexts)
		{
			Menu menu = new Menu(Canvas.Image.Palette.Entries)
			{
				Title = title,
				X = x,
				Y = y,
				Width = 80,
				TitleColour = 15,
				ActiveColour = 11,
				TextColour = 79,
				DisabledColour = 8,
				FontId = 0
			};
			
			Menu.Item menuItem;
			for (int i = 0; i < menuTexts.Length; i++)
			{
				menu.Items.Add(menuItem = new Menu.Item(menuTexts[i], i));
				menuItem.Selected += setChoice;
			}
			menu.ActiveItem = 1;
			Menus.Add(menu);
			return menu;
		}
		
		private void MainChoice(object sender, EventArgs args)
		{
		}
		
		public Setup()
		{
			Cursor = MouseCursor.Pointer;
			
            Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			
			_canvas = new Picture(320, 200, palette);
			_canvas.FillRectangle(3, 0, 0, 320, 200);
			_canvas.DrawText("CivOne Settings", 3, 5, 160, 2, TextAlign.Center);
			
			Common.AddScreen(AddMenu(2, 16, "MAKE A CHOICE:", MainChoice, "Settings", "Patches", "Launch Game", "Quit"));
		}
	}
}