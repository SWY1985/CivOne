// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Interfaces;
using CivOne.IO;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	[Break]
	internal class Setup : BaseScreen, IExpand
	{
		private const int MenuFont = 6;
		
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (!_update) return false;
			_update = false;
			
			if (!HasMenu)
			{
				MainMenu();
			}
			
			return false;
		}
		
		private int GetMenuWidth(string title, string[] items)
		{
			int i = 0;
			Picture[] texts = new Picture[items.Length + 1];
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
		
		private Menu CreateMenu(string title, MenuItemEventHandler<int> setChoice, params string[] menuTexts)
		{
			int width = GetMenuWidth(title, menuTexts);
			int height = GetMenuHeight(title, menuTexts);
			Menu menu = new Menu("Setup", Palette)
			{
				Title = title,
				X = (_canvas.Width - width) / 2,
				Y = (_canvas.Height - height) / 2,
				Width = width,
				TitleColour = 15,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 8,
				FontId = MenuFont,
				IndentTitle = 2
			};
			
			for (int i = 0; i < menuTexts.Length; i++)
			{
				menu.Items.Add(menuTexts[i], i).OnSelect(setChoice);
			}
			return menu;
		}
		
		private void MainMenu(int activeItem = 0)
		{
			Menu menu = CreateMenu("CIVONE SETUP:", MainChoice, "Settings", "Patches", "Mods", "Launch Game", "Quit");
			menu.Items[2].Enabled = false; // Mods: Not yet implemented
			menu.ActiveItem = activeItem;
			AddMenu(menu);
		}
		
		private void SettingsMenu(int activeItem = 0)
		{
			string graphicsMode, fullScreen, scale, aspectRatio;
			switch (Settings.GraphicsMode)
			{
				case GraphicsMode.Graphics256: graphicsMode = "256 colors"; break;
				case GraphicsMode.Graphics16: graphicsMode = "16 colors"; break;
				default: graphicsMode = "unknown"; break;
			}

			switch (Settings.AspectRatio)
			{
				case AspectRatio.Fixed: aspectRatio = "Fixed"; break;
				case AspectRatio.Scaled: aspectRatio = "Scaled (blurry)"; break;
				case AspectRatio.ScaledFixed: aspectRatio = "Scaled and fixed (blurry)"; break;
				case AspectRatio.Expand: aspectRatio = "Expand (experimental)"; break;
				default: aspectRatio = "Automatic"; break;
			}
			
			graphicsMode = string.Format("Graphics Mode: {0}", graphicsMode);
			fullScreen = string.Format("Full Screen: {0}", Settings.FullScreen ? "yes" : "no");
			scale = string.Format("Window scale: {0}x", Settings.Scale);
			aspectRatio = string.Format("Aspect ratio: {0}", aspectRatio);
			
			Menu menu = CreateMenu("SETTINGS:", SettingsChoice, graphicsMode, fullScreen, scale, aspectRatio, "Back");
			menu.ActiveItem = activeItem;
			AddMenu(menu);
		}
		
		private void GraphicsModeMenu()
		{
			Menu menu = CreateMenu("GRAPHICS MODE:", GraphicsModeChoice, "256 colors (default)", "16 colors", "Back");
			switch (Settings.GraphicsMode)
			{
				case GraphicsMode.Graphics256: 
					menu.ActiveItem = 0;
					break;
				case GraphicsMode.Graphics16: 
					menu.ActiveItem = 1;
					break;
			}
			AddMenu(menu);
		}
		
		private void FullScreenMenu()
		{
			Menu menu = CreateMenu("FULL SCREEN:", FullScreenChoice, "No (default)", "Yes", "Back");
			menu.ActiveItem = Settings.FullScreen ? 1 : 0;
			AddMenu(menu);
		}
		
		private void WindowScaleMenu()
		{
			Menu menu = CreateMenu("WINDOW SCALE:", WindowScaleChoice, "1x", "2x", "3x", "4x", "Back");
			menu.ActiveItem = Settings.Scale - 1;
			AddMenu(menu);
		}
		
		private void AspectRatioMenu()
		{
			Menu menu = CreateMenu("ASPECT RATIO:", AspectRatioChoice, "Automatic", "Fixed", "Scaled (blurry)", "Scaled and fixed (blurry)", "Expand (experimental)", "Back");
			menu.ActiveItem = (int)Settings.AspectRatio;
			AddMenu(menu);
		}
		
		private void PatchesMenu(int activeItem = 0)
		{
			string revealWorld, sideBar, debugMenu, cursorType, destroyAnimation, deityMenu, arrowHelperMenu;
			switch (Settings.CursorType)
			{
				case CursorType.Builtin: cursorType = "Built-in"; break;
				case CursorType.Native: cursorType = "Native"; break;
				default: cursorType = "Default"; break;
			}

			switch (Settings.DestroyAnimation)
			{
				case DestroyAnimation.Noise: destroyAnimation = "Noise"; break;
				default: destroyAnimation = "Sprites (original)"; break;
			}

			revealWorld = $"Reveal World: {(Settings.RevealWorld ? "yes" : "no")}";
			sideBar = $"Side bar location: {(Settings.RightSideBar ? "right" : "left")}";
			debugMenu = $"Show debug menu: {(Settings.DebugMenu ? "yes" : "no")}";
			cursorType = $"Mouse cursor type: {cursorType}";
			destroyAnimation = $"Destroy animation: {destroyAnimation}";
			deityMenu = $"Enable Deity difficulty: {(Settings.DeityEnabled ? "yes" : "no")}";
			arrowHelperMenu = $"Enable (no keypad) arrow helper: {(Settings.ArrowHelper ? "yes" : "no")}";
			
			Menu menu = CreateMenu("PATCHES:", PatchesChoice, revealWorld, sideBar, debugMenu, cursorType, destroyAnimation, deityMenu, arrowHelperMenu, "Back");
			menu.ActiveItem = activeItem;
			AddMenu(menu);
		}
		
		private void RevealWorldMenu()
		{
			Menu menu = CreateMenu("REVEAL WORLD:", RevealWorldChoice, "No (default)", "Yes", "Back");
			menu.ActiveItem = Settings.RevealWorld ? 1 : 0;
			AddMenu(menu);
		}
		
		private void SideBarMenu()
		{
			Menu menu = CreateMenu("SIDE BAR LOCATION:", SideBarChoice, "Left (default)", "Right", "Back");
			menu.ActiveItem = Settings.RightSideBar ? 1 : 0;
			AddMenu(menu);
		}
		
		private void DebugMenuMenu()
		{
			Menu menu = CreateMenu("SHOW DEBUG MENU:", DebugMenuChoice, "No (default)", "Yes", "Back");
			menu.ActiveItem = Settings.DebugMenu ? 1 : 0;
			AddMenu(menu);
		}

		private void CursorTypeMenu()
		{
			Menu menu = CreateMenu("MOUSE CURSOR TYPE:", CursorTypeChoice, "Default", "Built-in", "Native", "Back");
			menu.ActiveItem = (int)Settings.CursorType;
			if (menu.ActiveItem == (int)CursorType.Default && !FileSystem.DataFilesExist(FileSystem.MouseCursorFiles))
			{
				menu.ActiveItem = (int)CursorType.Builtin;
			}
			menu.Items[0].Enabled = (FileSystem.DataFilesExist(FileSystem.MouseCursorFiles));
			AddMenu(menu);
		}

		private void DestroyAnimationMenu()
		{
			Menu menu = CreateMenu("DESTROY ANIMATION:", DestroyAnimationChoice, "Sprites (original)", "Noise", "Back");
			menu.ActiveItem = (int)Settings.DestroyAnimation;
			AddMenu(menu);
		}
		
		private void DeityEnabledMenu()
		{
			Menu menu = CreateMenu("ENABLE DEITY DIFFICULTY:", DeityEnabledChoice, "No", "Yes", "Back");
			menu.ActiveItem = Settings.DeityEnabled ? 1 : 0;
			AddMenu(menu);
		}
		
		private void ArrowHelperMenu()
		{
			Menu menu = CreateMenu("ENABLE (NO KEYPAD) ARROW HELPER:", ArrowHelperChoice, "No", "Yes", "Back");
			menu.ActiveItem = Settings.ArrowHelper ? 1 : 0;
			AddMenu(menu);
		}
		
		private void MainChoice(object sender, MenuItemEventArgs<int> args)
		{
			switch (args.Value)
			{
				case 0: // Settings
					CloseMenus();
					SettingsMenu();
					break;
				case 1: // Patches
					CloseMenus();
					PatchesMenu();
					return;
				case 2: // Mods: Not yet implemented
					return;
				case 3:
					Destroy();
					return;
				case 4:
					Destroy();
					Runtime.Quit();
					return;
			}
		}
		
		private void SettingsChoice(object sender, MenuItemEventArgs<int> args)
		{
			CloseMenus();
			switch (args.Value)
			{
				case 0: // Graphics Mode
					GraphicsModeMenu();
					break;
				case 1: // Full Screen
					FullScreenMenu();
					break;
				case 2: // Scale
					WindowScaleMenu();
					break;
				case 3: // Aspect Ratio
					AspectRatioMenu();
					break;
				case 4: // Back
					MainMenu();
					break;
			}
		}
		
		private void GraphicsModeChoice(object sender, MenuItemEventArgs<int> args)
		{
			CloseMenus();
			switch (args.Value)
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
		
		private void FullScreenChoice(object sender, MenuItemEventArgs<int> args)
		{
			switch (args.Value)
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
		
		private void WindowScaleChoice(object sender, MenuItemEventArgs<int> args)
		{
			int choice = args.Value;
			if (choice < 4)
			{
				Settings.Scale = (choice + 1);
			}
			CloseMenus();
			SettingsMenu(2);
		}

		private void AspectRatioChoice(object sender, MenuItemEventArgs<int> args)
		{
			int choice = args.Value;
			if (choice < 5)
			{
				Settings.AspectRatio = (AspectRatio)(choice);
			}
			CloseMenus();
			SettingsMenu(3);
		}

		private void PatchesChoice(object sender, MenuItemEventArgs<int> args)
		{
			CloseMenus();
			switch (args.Value)
			{
				case 0: // Reveal World
					RevealWorldMenu();
					break;
				case 1: // Side bar
					SideBarMenu();
					break;
				case 2: // Debug Menu
					DebugMenuMenu();
					break;
				case 3: // Cursor Type
					CursorTypeMenu();
					break;
				case 4: // Destroy Animation
					DestroyAnimationMenu();
					break;
				case 5: // Enable Deity difficulty
					DeityEnabledMenu();
					break;
				case 6: // Enable (no keypad) arrow helper
					ArrowHelperMenu();
					break;
				case 7: // Back
					MainMenu(1);
					break;
			}
		}
		
		private void RevealWorldChoice(object sender, MenuItemEventArgs<int> args)
		{
			switch (args.Value)
			{
				case 0: // no
					Settings.RevealWorld = false;
					break;
				case 1: // yes
					Settings.RevealWorld = true;
					break;
			}
			CloseMenus();
			PatchesMenu(0);
		}
		
		private void SideBarChoice(object sender, MenuItemEventArgs<int> args)
		{
			switch (args.Value)
			{
				case 0: // left
					Settings.RightSideBar = false;
					break;
				case 1: // right
					Settings.RightSideBar = true;
					break;
			}
			CloseMenus();
			PatchesMenu(1);
		}
		
		private void DebugMenuChoice(object sender, MenuItemEventArgs<int> args)
		{
			switch (args.Value)
			{
				case 0: // no
					Settings.DebugMenu = false;
					break;
				case 1: // yes
					Settings.DebugMenu = true;
					break;
			}
			CloseMenus();
			PatchesMenu(2);
		}
		
		private void CursorTypeChoice(object sender, MenuItemEventArgs<int> args)
		{
			switch (args.Value)
			{
				case 0:
					Settings.CursorType = CursorType.Default;
					break;
				case 1:
					Settings.CursorType = CursorType.Builtin;
					break;
				case 2:
					Settings.CursorType = CursorType.Native;
					break;
			}
			CloseMenus();
			PatchesMenu(3);
		}

		private void DestroyAnimationChoice(object sender, MenuItemEventArgs<int> args)
		{
			switch (args.Value)
			{
				case 0:
					Settings.DestroyAnimation = DestroyAnimation.Sprites;
					break;
				case 1:
					Settings.DestroyAnimation = DestroyAnimation.Noise;
					break;
			}
			CloseMenus();
			PatchesMenu(4);
		}
		
		private void DeityEnabledChoice(object sender, MenuItemEventArgs<int> args)
		{
			switch (args.Value)
			{
				case 0: // no
					Settings.DeityEnabled = false;
					break;
				case 1: // yes
					Settings.DeityEnabled = true;
					break;
			}
			CloseMenus();
			PatchesMenu(5);
		}

		private void ArrowHelperChoice(object sender, MenuItemEventArgs<int> args)
		{
			switch (args.Value)
			{
				case 0: // no
					Settings.ArrowHelper = false;
					break;
				case 1: // yes
					Settings.ArrowHelper = true;
					break;
			}
			CloseMenus();
			PatchesMenu(6);
		}

		public void Resize(int width, int height)
		{
			_canvas = new Picture(width, height, _canvas.Palette);
			_canvas.FillRectangle(3, 0, 0, width, height);

			foreach (Menu menu in Common.Screens.Where(x => x is Menu && (x as Menu).Id == "Setup"))
			{
				int menuHeight = GetMenuHeight(menu.Title, menu.Items.Select(x => x.Text).ToArray());

				menu.X = (width - menu.Width) / 2;
				menu.Y = (height - menuHeight) / 2;
			}
		}
		
		public Setup()
		{
			Cursor = MouseCursor.Pointer;
			
			_canvas = new Picture(320, 200, Common.GetPalette256);
			_canvas.FillRectangle(3, 0, 0, 320, 200);
		}
	}
}