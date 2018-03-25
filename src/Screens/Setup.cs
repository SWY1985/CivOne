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
using CivOne.IO;
using CivOne.UserInterface;

using static CivOne.Enums.AspectRatio;
using static CivOne.Enums.CursorType;
using static CivOne.Enums.DestroyAnimation;
using static CivOne.Enums.GraphicsMode;

namespace CivOne.Screens
{
	[Break, Expand]
	internal class Setup : BaseScreen
	{
		private const int MenuFont = 6;
		
		private bool _update = true;
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (!_update) return false;
			_update = false;
			
			if (!HasMenu)
			{
				MainMenu();
			}
			
			return false;
		}

		private void BrowseForSoundFiles(object sender, MenuItemEventArgs<int> args)
		{
			string path = Runtime.BrowseFolder("Location of Civilization for Windows sound files");
			if (path == null)
			{
				// User pressed cancel
				return;
			}

			FileSystem.CopySoundFiles(path);
		}

		private void CreateMenu(string title, int activeItem, MenuItemEventHandler<int> always, params MenuItem<int>[] items) =>
			AddMenu(new Menu("Setup", Palette)
			{
				Title = $"{title.ToUpper()}:",
				TitleColour = 15,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 8,
				FontId = MenuFont,
				IndentTitle = 2
			}
			.Items(items)
			.Always(always)
			.Center(this)
			.SetActiveItem(activeItem)
		);
		private void CreateMenu(string title, MenuItemEventHandler<int> always, params MenuItem<int>[] items) => CreateMenu(title, -1, always, items);
		private void CreateMenu(string title, int activeItem, params MenuItem<int>[] items) => CreateMenu(title, activeItem, null, items);
		private void CreateMenu(string title, params MenuItem<int>[] items) => CreateMenu(title, -1, null, items);
		
		private MenuItemEventHandler<int> GotoMenu(Action<int> action, int selectedItem = 0) => (s, a) =>
		{
			CloseMenus();
			action(selectedItem);
		};

		private MenuItemEventHandler<int> GotoMenu(Action action) => (s, a) =>
		{
			CloseMenus();
			action();
		};

		private MenuItemEventHandler<int> CloseScreen(Action action = null) => (s, a) =>
		{
			Destroy();
			if (action != null) action();
		};
		
		private void MainMenu(int activeItem = 0) => CreateMenu("CivOne Setup", activeItem,
			MenuItem.Create("Settings").OnSelect(GotoMenu(SettingsMenu)),
			MenuItem.Create("Patches").OnSelect(GotoMenu(PatchesMenu)),
			MenuItem.Create("Plugins").Disable(),
			MenuItem.Create("Launch Game").OnSelect(CloseScreen()),
			MenuItem.Create("Quit").OnSelect(CloseScreen(Runtime.Quit))
		);

		private void SettingsMenu(int activeItem = 0) => CreateMenu("Settings", activeItem,
			MenuItem.Create($"Graphics Mode: {Settings.GraphicsMode.ToText()}").OnSelect(GotoMenu(GraphicsModeMenu)),
			MenuItem.Create($"Aspect Ratio: {Settings.AspectRatio.ToText()}").OnSelect(GotoMenu(AspectRatioMenu)),
			MenuItem.Create($"Full Screen: {Settings.FullScreen.YesNo()}").OnSelect(GotoMenu(FullScreenMenu)),
			MenuItem.Create($"Window Scale: {Settings.Scale}x").OnSelect(GotoMenu(WindowScaleMenu)),
			MenuItem.Create($"In-game sound: {Settings.GameSound.OnOff()}").OnSelect(GotoMenu(SoundMenu)),
			MenuItem.Create($"Back").OnSelect(GotoMenu(MainMenu, 0))
		);

		private void GraphicsModeMenu() => CreateMenu("Graphics Mode", GotoMenu(SettingsMenu, 0),
			MenuItem.Create($"{Graphics256.ToText()} (default)").OnSelect((s, a) => Settings.GraphicsMode = Graphics256).SetActive(() => Settings.GraphicsMode == Graphics256),
			MenuItem.Create(Graphics16.ToText()).OnSelect((s, a) => Settings.GraphicsMode = Graphics16).SetActive(() => Settings.GraphicsMode == Graphics16),
			MenuItem.Create("Back")
		);

		private void AspectRatioMenu() => CreateMenu("Aspect Ratio", GotoMenu(SettingsMenu, 1),
			MenuItem.Create($"{Auto.ToText()} (default)").OnSelect((s, a) => Settings.AspectRatio = Auto).SetActive(() => Settings.AspectRatio == Auto),
			MenuItem.Create(Fixed.ToText()).OnSelect((s, a) => Settings.AspectRatio = Fixed).SetActive(() => Settings.AspectRatio == Fixed),
			MenuItem.Create(Scaled.ToText()).OnSelect((s, a) => Settings.AspectRatio = Scaled).SetActive(() => Settings.AspectRatio == Scaled),
			MenuItem.Create(ScaledFixed.ToText()).OnSelect((s, a) => Settings.AspectRatio = ScaledFixed).SetActive(() => Settings.AspectRatio == ScaledFixed),
			MenuItem.Create(AspectRatio.Expand.ToText()).OnSelect((s, a) => Settings.AspectRatio = AspectRatio.Expand).SetActive(() => Settings.AspectRatio == AspectRatio.Expand),
			MenuItem.Create("Back")
		);

		private void FullScreenMenu() => CreateMenu("Full Screen", GotoMenu(SettingsMenu, 2),
			MenuItem.Create($"{false.YesNo()} (default)").OnSelect((s, a) => Settings.FullScreen = false).SetActive(() => !Settings.FullScreen),
			MenuItem.Create(true.YesNo()).OnSelect((s, a) => Settings.FullScreen = true).SetActive(() => Settings.FullScreen),
			MenuItem.Create("Back")
		);

		private void WindowScaleMenu() => CreateMenu("Window Scale", GotoMenu(SettingsMenu, 3),
			MenuItem.Create("1x").OnSelect((s, a) => Settings.Scale = 1).SetActive(() => Settings.Scale == 1),
			MenuItem.Create("2x (default)").OnSelect((s, a) => Settings.Scale = 2).SetActive(() => Settings.Scale == 2),
			MenuItem.Create("3x").OnSelect((s, a) => Settings.Scale = 3).SetActive(() => Settings.Scale == 3),
			MenuItem.Create("4x").OnSelect((s, a) => Settings.Scale = 4).SetActive(() => Settings.Scale == 4),
			MenuItem.Create("Back")
		);

		private void SoundMenu() => CreateMenu("In-game sound", GotoMenu(SettingsMenu, 4),
			MenuItem.Create($"{true.OnOff()} (default)").OnSelect((s, a) => Settings.GameSound = true).SetActive(() => Settings.GameSound),
			MenuItem.Create(false.OnOff()).OnSelect((s, a) => Settings.GameSound = false).SetActive(() => !Settings.GameSound),
			MenuItem.Create("Browse for files...").OnSelect(BrowseForSoundFiles).SetEnabled(!FileSystem.SoundFilesExist()),
			MenuItem.Create("Back")
		);

		private void PatchesMenu(int activeItem = 0) => CreateMenu("Patches", activeItem,
			MenuItem.Create($"Reveal world: {Settings.RevealWorld.YesNo()}").OnSelect(GotoMenu(RevealWorldMenu)),
			MenuItem.Create($"Side bar location: {(Settings.RightSideBar ? "right" : "left")}").OnSelect(GotoMenu(SideBarMenu)),
			MenuItem.Create($"Debug menu: {Settings.DebugMenu.YesNo()}").OnSelect(GotoMenu(DebugMenuMenu)),
			MenuItem.Create($"Cursor type: {Settings.CursorType.ToText()}").OnSelect(GotoMenu(CursorTypeMenu)),
			MenuItem.Create($"Destroy animation: {Settings.DestroyAnimation.ToText()}").OnSelect(GotoMenu(DestroyAnimationMenu)),
			MenuItem.Create($"Enable Deity difficulty: {Settings.DeityEnabled.YesNo()}").OnSelect(GotoMenu(DeityEnabledMenu)),
			MenuItem.Create($"Enable (no keypad) arrow helper: {Settings.ArrowHelper.YesNo()}").OnSelect(GotoMenu(ArrowHelperMenu)),
			MenuItem.Create($"Custom map sizes (experimental): {Settings.CustomMapSize.YesNo()}").OnSelect(GotoMenu(CustomMapSizeMenu)),
			MenuItem.Create("Back").OnSelect(GotoMenu(MainMenu, 1))
		);

		private void RevealWorldMenu() => CreateMenu("Reveal world", GotoMenu(PatchesMenu, 0),
			MenuItem.Create($"{false.YesNo()} (default)").OnSelect((s, a) => Settings.RevealWorld = false).SetActive(() => !Settings.RevealWorld),
			MenuItem.Create(true.YesNo()).OnSelect((s, a) => Settings.RevealWorld = true).SetActive(() => Settings.RevealWorld),
			MenuItem.Create("Back")
		);

		private void SideBarMenu() => CreateMenu("Side bar location", GotoMenu(PatchesMenu, 1),
			MenuItem.Create("Left (default)").OnSelect((s, a) => Settings.RightSideBar = false).SetActive(() => !Settings.RightSideBar),
			MenuItem.Create("Right").OnSelect((s, a) => Settings.RightSideBar = true).SetActive(() => Settings.RightSideBar),
			MenuItem.Create("Back")
		);

		private void DebugMenuMenu() => CreateMenu("Show debug menu", GotoMenu(PatchesMenu, 2),
			MenuItem.Create($"{false.YesNo()} (default)").OnSelect((s, a) => Settings.DebugMenu = false).SetActive(() => !Settings.DebugMenu),
			MenuItem.Create(true.YesNo()).OnSelect((s, a) => Settings.DebugMenu = true).SetActive(() => Settings.DebugMenu),
			MenuItem.Create("Back")
		);

		private void CursorTypeMenu() => CreateMenu("Mouse cursor type", GotoMenu(PatchesMenu, 3),
			MenuItem.Create(Default.ToText()).OnSelect((s, a) => Settings.CursorType = Default).SetActive(() => Settings.CursorType == Default && FileSystem.DataFilesExist(FileSystem.MouseCursorFiles)).SetEnabled(FileSystem.DataFilesExist(FileSystem.MouseCursorFiles)),
			MenuItem.Create(Builtin.ToText()).OnSelect((s, a) => Settings.CursorType = Builtin).SetActive(() => Settings.CursorType == Builtin || (Settings.CursorType == Default && !FileSystem.DataFilesExist(FileSystem.MouseCursorFiles))),
			MenuItem.Create(Native.ToText()).OnSelect((s, a) => Settings.CursorType = Native).SetActive(() => Settings.CursorType == Native),
			MenuItem.Create("Back")
		);

		private void DestroyAnimationMenu() => CreateMenu("Destroy animation", GotoMenu(PatchesMenu, 4),
			MenuItem.Create(Sprites.ToText()).OnSelect((s, a) => Settings.DestroyAnimation = Sprites).SetActive(() => Settings.DestroyAnimation == Sprites),
			MenuItem.Create(Noise.ToText()).OnSelect((s, a) => Settings.DestroyAnimation = Noise).SetActive(() => Settings.DestroyAnimation == Noise),
			MenuItem.Create("Back")
		);

		private void DeityEnabledMenu() => CreateMenu("Enable Deity difficulty", GotoMenu(PatchesMenu, 5),
			MenuItem.Create($"{false.YesNo()} (default)").OnSelect((s, a) => Settings.DeityEnabled = false).SetActive(() => !Settings.DeityEnabled),
			MenuItem.Create(true.YesNo()).OnSelect((s, a) => Settings.DeityEnabled = true).SetActive(() => Settings.DeityEnabled),
			MenuItem.Create("Back")
		);

		private void ArrowHelperMenu() => CreateMenu("Enable (no keypad) arrow helper", GotoMenu(PatchesMenu, 6),
			MenuItem.Create($"{false.YesNo()} (default)").OnSelect((s, a) => Settings.ArrowHelper = false).SetActive(() => !Settings.ArrowHelper),
			MenuItem.Create(true.YesNo()).OnSelect((s, a) => Settings.ArrowHelper = true).SetActive(() => Settings.ArrowHelper),
			MenuItem.Create("Back")
		);

		private void CustomMapSizeMenu() => CreateMenu("Custom map sizes (experimental)", GotoMenu(PatchesMenu, 7),
			MenuItem.Create($"{false.YesNo()} (default)").OnSelect((s, a) => Settings.CustomMapSize = false).SetActive(() => !Settings.CustomMapSize),
			MenuItem.Create(true.YesNo()).OnSelect((s, a) => Settings.CustomMapSize = true).SetActive(() => Settings.CustomMapSize),
			MenuItem.Create("Back")
		);

		private void Resize(object sender, ResizeEventArgs args)
		{
			this.Clear(3);

			foreach (Menu menu in Menus["Setup"])
			{
				menu.Center(this).ForceUpdate();
			}

			_update = true;
		}
		
		public Setup() : base(MouseCursor.Pointer)
		{
			OnResize += Resize;
			
			Palette = Common.GetPalette256;
			this.Clear(3);
		}
	}
}