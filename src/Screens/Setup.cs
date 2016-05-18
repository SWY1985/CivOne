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
			Settings settings = Settings.Instance; 
			string graphicsMode, fullScreen, framesPerSecond, sideBar;
			switch (settings.GraphicsMode)
			{
				case GraphicsMode.Graphics256: graphicsMode = "256 colors"; break;
				case GraphicsMode.Graphics16: graphicsMode = "16 colors"; break;
				default: graphicsMode = "unknown"; break;
			}
			
			graphicsMode = string.Format("Graphics Mode: {0}", graphicsMode);
			fullScreen = string.Format("Full Screen: {0}", settings.FullScreen ? "yes" : "no");
			framesPerSecond = string.Format("Frames per second: {0}", settings.FramesPerSecond);
			sideBar = string.Format("Side bar location: {0}", settings.RightSideBar ? "right" : "left");
			
			Menu menu = AddMenu("SETTINGS:", SettingsChoice, graphicsMode, fullScreen, framesPerSecond, sideBar, "Back");
			menu.ActiveItem = activeItem;
			Common.AddScreen(menu);
		}
		
		private void GraphicsModeMenu()
		{
			Menu menu = AddMenu("GRAPHICS MODE:", GraphicsModeChoice, "256 colors (default)", "16 colors", "Back");
			switch (Settings.Instance.GraphicsMode)
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
			menu.ActiveItem = Settings.Instance.FullScreen ? 1 : 0;
			Common.AddScreen(menu);
		}
		
		private void FramesPerSecondMenu()
		{
			Menu menu = AddMenu("FRAMES PER SECOND:", FramesPerSecondChoice, "5 (not recommended)", "10", "15 (default)", "20", "25", "30", "35", "40", "45", "50", "55", "60", "Custom", "Back");
			menu.Items[12].Enabled = false; // Custom FPS: Not yet implemented
			if (Settings.Instance.FramesPerSecond % 5 == 0)
			{
				menu.ActiveItem = (Settings.Instance.FramesPerSecond / 5) - 1;
			}
			else
			{
				menu.ActiveItem = 12;
			}
			Common.AddScreen(menu);
		}
		
		private void SideBarMenu()
		{
			Menu menu = AddMenu("SIDE BAR LOCATION:", SideBarChoice, "Left (default)", "Right", "Back");
			menu.ActiveItem = Settings.Instance.RightSideBar ? 1 : 0;
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
				case 2: // Frames per second
					FramesPerSecondMenu();
					break;
				case 3: // Side bar
					SideBarMenu();
					break;
				case 4: // Back
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
					Settings.Instance.GraphicsMode = GraphicsMode.Graphics256;
					break;
				case 1: // 16 colours
					Settings.Instance.GraphicsMode = GraphicsMode.Graphics16;
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
					Settings.Instance.FullScreen = false;
					break;
				case 1: // yes
					Settings.Instance.FullScreen = true;
					break;
			}
			CloseMenus();
			SettingsMenu(1);
		}
		
		private void FramesPerSecondChoice(object sender, EventArgs args)
		{
			int choice = (sender as Menu.Item).Value;
			if (choice < 12)
			{
				byte framesPerSecond = (byte)((choice * 5) + 5);
				Settings.Instance.FramesPerSecond = framesPerSecond;
			}
			CloseMenus();
			SettingsMenu(2);
		}
		
		private void SideBarChoice(object sender, EventArgs args)
		{
			int choice = (sender as Menu.Item).Value;
			switch (choice)
			{
				case 0: // left
					Settings.Instance.RightSideBar = false;
					break;
				case 1: // right
					Settings.Instance.RightSideBar = true;
					break;
			}
			CloseMenus();
			SettingsMenu(3);
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