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
using System.Linq;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Setup : BaseScreen
	{
		private const int MenuFont = 6;
		
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (!_update) return false;
			_update = false;
			
			if (Menus.Count == 0)
			{
				MainMenu();
			}
			
			return false;
		}
		
		private int GetMenuWidth(string title, string[] items)
		{
			int i = 0;
			Bitmap[] texts = new Bitmap[items.Length + 1];
			texts[i++] = Resources.Instance.GetText(" " + title, MenuFont, 15);
			foreach (string item in items)
				texts[i++] = Resources.Instance.GetText(" " + item, MenuFont, 5);			
			return (texts.Select(t => t.Width).Max()) + 6;
		}
		
		private int GetMenuHeight(string title, string[] items)
		{
			int menuItems = items.Length;
			if (title != null) menuItems++;
			return menuItems * Resources.Instance.GetFontHeight(MenuFont);
		}
		
		private Menu AddMenu(string title, EventHandler setChoice, params string[] menuTexts)
		{
			int width = GetMenuWidth(title, menuTexts);
			int height = GetMenuHeight(title, menuTexts);
			Menu menu = new Menu(Canvas.Image.Palette.Entries)
			{
				Title = title,
				X = (320 - width) / 2,
				Y = (200 - height) / 2,
				Width = width,
				TitleColour = 15,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 8,
				FontId = MenuFont,
				IndentTitle = 2
			};
			
			Menu.Item menuItem;
			for (int i = 0; i < menuTexts.Length; i++)
			{
				menu.Items.Add(menuItem = new Menu.Item(menuTexts[i], i));
				menuItem.Selected += setChoice;
			}
			Menus.Add(menu);
			return menu;
		}
		
		private void MainMenu(int activeItem = 0)
		{
			Menu menu = AddMenu("CIVONE SETUP:", MainChoice, "Settings", "Patches", "Mods", "Launch Game", "Quit");
			menu.Items[1].Enabled = false; // Patches: Not yet implemented
			menu.Items[2].Enabled = false; // Mods: Not yet implemented
			menu.ActiveItem = activeItem;
			Common.AddScreen(menu);
		}
		
		private void SettingsMenu(int activeItem = 0)
		{
			string graphicsMode, fullScreen, sideBar, scale, revealWorld;
			switch (Settings.GraphicsMode)
			{
				case GraphicsMode.Graphics256: graphicsMode = "256 colors"; break;
				case GraphicsMode.Graphics16: graphicsMode = "16 colors"; break;
				default: graphicsMode = "unknown"; break;
			}
			
			graphicsMode = string.Format("Graphics Mode: {0}", graphicsMode);
			fullScreen = string.Format("Full Screen: {0}", Settings.FullScreen ? "yes" : "no");
			sideBar = string.Format("Side bar location: {0}", Settings.RightSideBar ? "right" : "left");
			scale = string.Format("Window scale: {0}x", Settings.Scale);
			revealWorld = string.Format("Reveal World: {0}", Settings.RevealWorld ? "yes" : "no");
			
			Menu menu = AddMenu("SETTINGS:", SettingsChoice, graphicsMode, fullScreen, sideBar, scale, revealWorld, "Back");
			menu.ActiveItem = activeItem;
			Common.AddScreen(menu);
		}
		
		private void GraphicsModeMenu()
		{
			Menu menu = AddMenu("GRAPHICS MODE:", GraphicsModeChoice, "256 colors (default)", "16 colors", "Back");
			switch (Settings.GraphicsMode)
			{
				case GraphicsMode.Graphics256: 
					menu.ActiveItem = 0;
					break;
				case GraphicsMode.Graphics16: 
					menu.ActiveItem = 1;
					break;
			}
			Common.AddScreen(menu);
		}
		
		private void FullScreenMenu()
		{
			Menu menu = AddMenu("FULL SCREEN:", FullScreenChoice, "No (default)", "Yes", "Back");
			menu.ActiveItem = Settings.FullScreen ? 1 : 0;
			Common.AddScreen(menu);
		}
		
		private void SideBarMenu()
		{
			Menu menu = AddMenu("SIDE BAR LOCATION:", SideBarChoice, "Left (default)", "Right", "Back");
			menu.ActiveItem = Settings.RightSideBar ? 1 : 0;
			Common.AddScreen(menu);
		}
		
		private void WindowScaleMenu()
		{
			Menu menu = AddMenu("WINDOW SCALE:", WindowScaleChoice, "1x", "2x", "3x", "4x", "Back");
			menu.ActiveItem = Settings.Scale - 1;
			Common.AddScreen(menu);
		}
		
		private void RevealWorldMenu()
		{
			Menu menu = AddMenu("REVEAL WORLD:", RevealWorldChoice, "No (default)", "Yes", "Back");
			menu.ActiveItem = Settings.RevealWorld ? 1 : 0;
			Common.AddScreen(menu);
		}
		
		private void MainChoice(object sender, EventArgs args)
		{
			int choice = (sender as Menu.Item).Value;
			switch (choice)
			{
				case 0: // Settings
					CloseMenus();
					SettingsMenu();
					break;
				case 1: // Patches: Not yet implemented
					return;
				case 2: // Mods: Not yet implemented
					return;
				case 3:
					Destroy();
					Common.AddScreen(new Credits());
					return;
				case 4:
					Destroy();
					Common.Quit();
					return;
			}
		}
		
		private void SettingsChoice(object sender, EventArgs args)
		{
			CloseMenus();
			int choice = (sender as Menu.Item).Value;
			switch (choice)
			{
				case 0: // Graphics Mode
					GraphicsModeMenu();
					break;
				case 1: // Full Screen
					FullScreenMenu();
					break;
				case 2: // Side bar
					SideBarMenu();
					break;
				case 3: // Scale
					WindowScaleMenu();
					break;
				case 4: // Reveal World
					RevealWorldMenu();
					break;
				case 5: // Back
					MainMenu();
					break;
			}
		}
		
		private void GraphicsModeChoice(object sender, EventArgs args)
		{
			CloseMenus();
			int choice = (sender as Menu.Item).Value;
			switch (choice)
			{
				case 0: // 256 colours
					Settings.GraphicsMode = GraphicsMode.Graphics256;
					break;
				case 1: // 16 colours
					Settings.GraphicsMode = GraphicsMode.Graphics16;
					break;
			}
			SettingsMenu(0);
		}
		
		private void FullScreenChoice(object sender, EventArgs args)
		{
			int choice = (sender as Menu.Item).Value;
			switch (choice)
			{
				case 0: // no
					Settings.FullScreen = false;
					break;
				case 1: // yes
					Settings.FullScreen = true;
					break;
			}
			CloseMenus();
			SettingsMenu(1);
		}
		
		private void SideBarChoice(object sender, EventArgs args)
		{
			int choice = (sender as Menu.Item).Value;
			switch (choice)
			{
				case 0: // left
					Settings.RightSideBar = false;
					break;
				case 1: // right
					Settings.RightSideBar = true;
					break;
			}
			CloseMenus();
			SettingsMenu(2);
		}
		
		private void WindowScaleChoice(object sender, EventArgs args)
		{
			int choice = (sender as Menu.Item).Value;
			if (choice < 4)
			{
				Settings.Scale = (choice + 1);
			}
			CloseMenus();
			SettingsMenu(3);
		}
		
		private void RevealWorldChoice(object sender, EventArgs args)
		{
			int choice = (sender as Menu.Item).Value;
			switch (choice)
			{
				case 0: // no
					Settings.RevealWorld = false;
					break;
				case 1: // yes
					Settings.RevealWorld = true;
					break;
			}
			CloseMenus();
			SettingsMenu(4);
		}
		
		public Setup()
		{
			Cursor = MouseCursor.Pointer;
			
			Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			
			_canvas = new Picture(320, 200, palette);
			_canvas.FillRectangle(3, 0, 0, 320, 200);
		}
	}
}