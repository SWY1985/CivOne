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
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Units;
using CivOne.Tasks;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class GamePlay : BaseScreen
	{
		private readonly MenuBar _menuBar;
		private readonly SideBar _sideBar;
		private readonly GameMap _gameMap;
		
		private readonly Bitmap _menuBackground = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
		
		private GameMenu _gameMenu = null;
		private int _menuX, _menuY;
		private uint _lastGameTick;
		private bool _update = true;
		private bool _redraw = false;
		private bool _rightSideBar = Settings.Instance.RightSideBar;
		
		private Point _menuLocation = Point.Empty;
		private Picture _menuGraphics = null;

		private bool _shift5 = false;

		internal int X
		{
			get
			{
				return _gameMap.X;
			}
		}

		internal int Y
		{
			get
			{
				return _gameMap.Y;
			}
		}
		
		private void MenuCancel(object sender, EventArgs args)
		{
			CloseMenus();
			_menuLocation = Point.Empty;
			_menuGraphics = null;
			_update = true;
			_redraw = true;
		}
		
		private void MenuQuitChoice(object sender, EventArgs args)
		{
			switch ((sender as Menu.Item).Value)
			{
				case 0:
					break;
				case 1:
					Common.Quit();
					break;
			}
			MenuCancel(sender, args);
		}
		
		private void MenuRevolutionChoice(object sender, EventArgs args)
		{
			switch ((sender as Menu.Item).Value)
			{
				case 0:
					break;
				case 1:
					Game.Instance.HumanPlayer.Revolt();
					break;
			}
			MenuCancel(sender, args);
		}
		
		private void MenuRevolution()
		{
			_menuLocation = new Point(64, 80);
			_menuGraphics = new Picture(232, 31);
			_menuGraphics.FillLayerTile(_menuBackground);
			_menuGraphics.FillRectangle(0, 231, 0, 1, 31);
			_menuGraphics.AddBorder(15, 8, 0, 0, 231, 31);
			_menuGraphics.DrawText("Are you sure you want a REVOLUTION?", 0, 15, 4, 4);
			
			Bitmap background = (Bitmap)_menuGraphics.GetPart(2, 11, 228, 16).Clone();
			Picture.ReplaceColours(background, new byte[] { 7, 22 }, new byte[] { 11, 3 });

			Menu menu = new Menu(Canvas.Image.Palette.Entries, background)
			{
				X = 66,
				Y = 91,
				Width = 227,
				ActiveColour = 11,
				TextColour = 5,
				FontId = 0,
				Indent = 2
			};
			Menu.Item menuItem;
			int i = 0;
			foreach (string choice in new [] { "_No, thanks.", "_Yes, we need a new government." })
			{
				menu.Items.Add(menuItem = new Menu.Item(choice, i++));
				menuItem.Selected += MenuRevolutionChoice;
			}
			menu.MissClick += MenuCancel;
			menu.Cancel += MenuCancel;
			Menus.Add(menu);
			Common.AddScreen(menu);
		}
		
		private void MenuQuit()
		{
			_menuLocation = new Point(101, 81);
			_menuGraphics = new Picture(104, 39);
			_menuGraphics.FillLayerTile(_menuBackground);
			_menuGraphics.AddBorder(15, 8, 0, 0, 104, 39);
			_menuGraphics.DrawText("Are you sure you", 0, 15, 4, 4);
			_menuGraphics.DrawText("want to Quit?", 0, 15, 4, 12);
			
			Bitmap background = (Bitmap)_menuGraphics.GetPart(2, 19, 100, 16).Clone();
			Picture.ReplaceColours(background, new byte[] { 7, 22 }, new byte[] { 11, 3 });

			Menu menu = new Menu(Canvas.Image.Palette.Entries, background)
			{
				X = 103,
				Y = 100,
				Width = 100,
				ActiveColour = 11,
				TextColour = 5,
				FontId = 0
			};
			Menu.Item menuItem;
			int i = 0;
			foreach (string choice in new [] { "Keep Playing", "Yes, Quit" })
			{
				menu.Items.Add(menuItem = new Menu.Item(choice, i++));
				menuItem.Selected += MenuQuitChoice;
			}
			menu.MissClick += MenuCancel;
			menu.Cancel += MenuCancel;
			Menus.Add(menu);
			Common.AddScreen(menu);
		}
		
		private void MenuBarGame(object sender, EventArgs args)
		{
			_gameMenu = new GameMenu(_canvas.Image.Palette.Entries);
			_gameMenu.Items.Add(new GameMenu.Item("Tax Rate"));
			_gameMenu.Items.Add(new GameMenu.Item("Luxuries Rate"));
			_gameMenu.Items.Add(new GameMenu.Item("FindCity"));
			_gameMenu.Items.Add(new GameMenu.Item("Options"));
			_gameMenu.Items.Add(new GameMenu.Item("Save Game") { Enabled = false });
			_gameMenu.Items.Add(new GameMenu.Item("REVOLUTION!"));
			_gameMenu.Items.Add(new GameMenu.Item(null));
			_gameMenu.Items.Add(new GameMenu.Item("Retire"));
			_gameMenu.Items.Add(new GameMenu.Item("QUIT to DOS"));
			
			_gameMenu.Items[0].Selected += (s, a) => Common.AddScreen(SetRate.Taxes);
			_gameMenu.Items[1].Selected += (s, a) => Common.AddScreen(SetRate.Luxuries);
			_gameMenu.Items[3].Selected += (s, a) => Common.AddScreen(new GameOptions());
			_gameMenu.Items[5].Selected += (s, a) => MenuRevolution();
			_gameMenu.Items[8].Selected += (s, a) => MenuQuit();
			
			_menuX = 16;
			_menuY = 8;
			
			_update = true;
		}
		
		private void MenuBarOrders(object sender, EventArgs args)
		{
			if (Game.Instance.ActiveUnit == null) return;

			_gameMenu = new GameMenu(_canvas.Image.Palette.Entries);
			_gameMenu.Items.Add(new GameMenu.Item("No Orders", "space"));
			_gameMenu.Items.Add(new GameMenu.Item("Found New City", "b"));
			_gameMenu.Items.Add(new GameMenu.Item("Build Road", "r"));
			_gameMenu.Items.Add(new GameMenu.Item("Build Irrigation", "i"));
			_gameMenu.Items.Add(new GameMenu.Item("Change to Forest", "m"));
			_gameMenu.Items.Add(new GameMenu.Item("Build Fortress", "f") { Enabled = false });
			_gameMenu.Items.Add(new GameMenu.Item("Wait", "w"));
			_gameMenu.Items.Add(new GameMenu.Item("Sentry", "s"));
			_gameMenu.Items.Add(new GameMenu.Item("GoTo"));
			_gameMenu.Items.Add(new GameMenu.Item(null));
			_gameMenu.Items.Add(new GameMenu.Item("Disband Unit", "D"));
			
			_gameMenu.Items[1].Selected += (s, a) => { if (Game.Instance.ActiveUnit is Settlers) GameTask.Enqueue(Orders.NewCity(Game.Instance.ActiveUnit as Settlers)); };
			_gameMenu.Items[2].Selected += (s, a) => { if (Game.Instance.ActiveUnit is Settlers) (Game.Instance.ActiveUnit as Settlers).BuildRoad(); };
			_gameMenu.Items[3].Selected += (s, a) => { if (Game.Instance.ActiveUnit is Settlers) (Game.Instance.ActiveUnit as Settlers).BuildIrrigation(); };
			_gameMenu.Items[10].Selected += (s, a) => Game.Instance.DisbandUnit(Game.Instance.ActiveUnit);
			
			_menuX = 72;
			_menuY = 8;
			
			_update = true;
		}
		
		private void MenuBarAdvisors(object sender, EventArgs args)
		{
			_gameMenu = new GameMenu(_canvas.Image.Palette.Entries);
			_gameMenu.Items.Add(new GameMenu.Item("City Status (F1)"));
			_gameMenu.Items.Add(new GameMenu.Item("Military Advisor (F2)"));
			_gameMenu.Items.Add(new GameMenu.Item("Intelligence Advisor (F3)"));
			_gameMenu.Items.Add(new GameMenu.Item("Attitude Advisor (F4)"));
			_gameMenu.Items.Add(new GameMenu.Item("Trade Advisor (F5)"));
			_gameMenu.Items.Add(new GameMenu.Item("Science Advisor (F6)"));
			
			_gameMenu.Items[0].Selected += (s, a) => Common.AddScreen(new CityStatus());
			_gameMenu.Items[1].Selected += (s, a) => { Common.AddScreen(new MilitaryLosses()); Common.AddScreen(new MilitaryStatus()); };
			_gameMenu.Items[2].Selected += (s, a) => Common.AddScreen(new IntelligenceReport());
			_gameMenu.Items[3].Selected += (s, a) => Common.AddScreen(new AttitudeSurvey());
			_gameMenu.Items[4].Selected += (s, a) => Common.AddScreen(new TradeReport());
			_gameMenu.Items[5].Selected += (s, a) => Common.AddScreen(new ScienceReport());
			
			_menuX = 112;
			_menuY = 8;
			
			_update = true;
		}
		
		private void MenuBarWorld(object sender, EventArgs args)
		{
			_gameMenu = new GameMenu(_canvas.Image.Palette.Entries);
			_gameMenu.Items.Add(new GameMenu.Item("Wonders of the World (F7)"));
			_gameMenu.Items.Add(new GameMenu.Item("Top 5 Cities (F8)"));
			_gameMenu.Items.Add(new GameMenu.Item("Civilization Score (F9)"));
			_gameMenu.Items.Add(new GameMenu.Item("World Map (F10)"));
			_gameMenu.Items.Add(new GameMenu.Item("Demographics"));
			_gameMenu.Items.Add(new GameMenu.Item("SpaceShips") { Enabled = false });
			
			_gameMenu.Items[2].Selected += (s, a) => Common.AddScreen(new CivilizationScore());
			_gameMenu.Items[3].Selected += (s, a) => Common.AddScreen(new WorldMap());
			
			_menuX = 144;
			_menuY = 8;
			
			_update = true;
		}
		
		private void MenuBarCivilopedia(object sender, EventArgs args)
		{
			_gameMenu = new GameMenu(_canvas.Image.Palette.Entries);
			_gameMenu.Items.Add(new GameMenu.Item("Complete"));
			_gameMenu.Items.Add(new GameMenu.Item("Civilization Advances"));
			_gameMenu.Items.Add(new GameMenu.Item("City Improvements"));
			_gameMenu.Items.Add(new GameMenu.Item("Military Units"));
			_gameMenu.Items.Add(new GameMenu.Item("Terrain Types"));
			_gameMenu.Items.Add(new GameMenu.Item("Miscellaneous"));
			
			_gameMenu.Items[0].Selected += (s, a) => Common.AddScreen(new Civilopedia(Civilopedia.Complete));
			_gameMenu.Items[1].Selected += (s, a) => Common.AddScreen(new Civilopedia(Civilopedia.Advances));
			_gameMenu.Items[2].Selected += (s, a) => Common.AddScreen(new Civilopedia(Civilopedia.Improvements));
			_gameMenu.Items[3].Selected += (s, a) => Common.AddScreen(new Civilopedia(Civilopedia.Units));
			_gameMenu.Items[4].Selected += (s, a) => Common.AddScreen(new Civilopedia(Civilopedia.TerrainType));
			_gameMenu.Items[5].Selected += (s, a) => Common.AddScreen(new Civilopedia(Civilopedia.Misc));
			
			_menuX = 182;
			_menuY = 8;
			
			_update = true;
		}
		
		private void DrawLayer(IScreen layer, uint gameTick, int x, int y)
		{
			if (layer == null) return;
			if (!layer.HasUpdate(gameTick) && !_redraw) return;
			AddLayer(layer, x, y);
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_gameMap.MustUpdate(gameTick)) _update = true;
			if (_sideBar.HasUpdate(gameTick)) _update = true;
			if (gameTick != _lastGameTick && gameTick % 3 == 0) _canvas.Cycle(96, 103).Cycle(104, 111);//_canvas.Cycle(96, 111);
			if (!_update && !_redraw) return (gameTick % 3 == 0);
			
			DrawLayer(_menuBar, gameTick, 0, 0);
			DrawLayer(_sideBar, gameTick, _rightSideBar ? 240 : 0, 8);
			DrawLayer(_gameMap, gameTick, _rightSideBar ? 0 : 80, 8);
			DrawLayer(_gameMenu, gameTick, _menuX, _menuY);
			
			if (_menuLocation != Point.Empty && _menuGraphics != null)
			{
				_canvas.FillRectangle(5, _menuLocation.X - 1, _menuLocation.Y - 1, _menuGraphics.Image.Width + 2, _menuGraphics.Image.Height + 2);
				AddLayer(_menuGraphics, _menuLocation.X, _menuLocation.Y);
			}
			
			_redraw = false;
			_update = false;
			_lastGameTick = gameTick;
			return true;
		}

		private bool CheckShift56(KeyboardEventArgs args)
		{
			if (!_shift5 && args.Modifier == KeyModifier.Shift && args.KeyChar == '5')
			{
				_shift5 = true;
				return true;
			}
			else if (_shift5 && args.Modifier == KeyModifier.Shift && args.KeyChar == '6')
			{
				_shift5 = false;
				Settings.Instance.RevealWorldCheat();
				return true;
			}
			else if (_shift5)
			{
				_shift5 = false;
			}
			return false;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (CheckShift56(args))
				return true;
			
			if (_gameMenu != null)
			{
				if (!_gameMenu.KeyDown(args))
				{
					_gameMenu = null;
					_redraw = true;
				}
				return true;
			}

			if (_menuBar.KeyDown(args) && _gameMenu != null)
			{
				_gameMenu.KeepOpen = true;
				return true;
			}

			switch (args.Key)
			{
				case Key.F1:
					Common.AddScreen(new CityStatus());
					return true;
				case Key.F2:
					Common.AddScreen(new MilitaryLosses());
					Common.AddScreen(new MilitaryStatus());
					return true;
				case Key.F3:
					Common.AddScreen(new IntelligenceReport());
					return true;
				case Key.F4:
					Common.AddScreen(new AttitudeSurvey());
					return true;
				case Key.F5:
					Common.AddScreen(new TradeReport());
					return true;
				case Key.F6:
					Common.AddScreen(new ScienceReport());
					return true;
				case Key.F9:
					Common.AddScreen(new CivilizationScore());
					return true;
				case Key.F10:
					Common.AddScreen(new WorldMap());
					return true;
				case Key.Plus:
					Common.AddScreen(SetRate.Taxes);
					return true;
				case Key.Minus:
					Common.AddScreen(SetRate.Luxuries);
					return true;
			}
			return _gameMap.KeyDown(args);
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (_gameMenu != null && _gameMenu.KeepOpen)
			{
				MouseArgsOffset(ref args, _menuX, _menuY);
				_update |= _gameMenu.MouseDown(args);
				return _update;
			}

			if (args.Y < 8)
			{
				return _menuBar.MouseDown(args);
			}
			if (_rightSideBar)
			{
				if (args.X > 240)
				{
					MouseArgsOffset(ref args, 240, 8);
					return _sideBar.MouseDown(args);
				}
				else
				{
					MouseArgsOffset(ref args, 0, 8);
					return (_update = _gameMap.MouseDown(args));
				}
			}
			else
			{
				if (args.X < 80)
				{
					MouseArgsOffset(ref args, 0, 8);
					return _sideBar.MouseDown(args);
				}
				else
				{
					MouseArgsOffset(ref args, 80, 8);
					return (_update = _gameMap.MouseDown(args));
				}
			}
		}
		
		public override bool MouseUp(ScreenEventArgs args)
		{
			if (_gameMenu == null) return false;
			if (args.Y < 8)
			{
				_menuBar.MouseDown(args);
				if (!_menuBar.MenuDrag)
				{
					_gameMenu.KeepOpen = true;
					return true;
				}
			}
			
			_gameMenu.MouseUp(args);
			_gameMenu = null;
			_redraw = true;
			return true;
		}
		
		public override bool MouseDrag(ScreenEventArgs args)
		{
			if (_gameMenu == null) return false;
			
			MouseArgsOffset(ref args, _menuX, _menuY);
			_update |= _gameMenu.MouseDrag(args);
			return _update;
		}
		
		public GamePlay()
		{
			Cursor = MouseCursor.Pointer;
			
			Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			
			_canvas = new Picture(320, 200, palette);
			_canvas.FillRectangle(5, 0, 0, 320, 200);
			
			_menuBar = new MenuBar(palette);
			_sideBar = new SideBar(palette);
			_gameMap = new GameMap();
			
			_menuBar.GameSelected += MenuBarGame;
			_menuBar.OrdersSelected += MenuBarOrders;
			_menuBar.AdvisorsSelected += MenuBarAdvisors;
			_menuBar.WorldSelected += MenuBarWorld;
			_menuBar.CivilopediaSelected += MenuBarCivilopedia;

			GameTask.Enqueue(Turn.End());
		}
	}
}