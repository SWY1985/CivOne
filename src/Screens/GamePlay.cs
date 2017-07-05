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
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens.Dialogs;
using CivOne.Screens.Reports;
using CivOne.Tasks;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class GamePlay : BaseScreen, IExpand
	{
		private readonly MenuBar _menuBar;
		private readonly SideBar _sideBar;
		private readonly GameMap _gameMap;
		
		private readonly Picture _menuBackground = Resources["SP299"].GetPart(288, 120, 32, 16);
		
		private GameMenu _gameMenu = null;
		private int _menuX, _menuY;
		private uint _lastGameTick;
		private bool _update = true;
		private bool _redraw = false;
		private bool _rightSideBar;

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

		internal void CenterOnPoint(int x, int y)
		{
			_gameMap.CenterOnPoint(x, y);
		}

		internal void RefreshMap()
		{
			_gameMap.ForceRefresh();
		}

		internal Color[] MainPalette
		{
			get
			{
				return _canvas.OriginalColours.ToArray();
			}
		}
		
		private void MenuBarGame(object sender, EventArgs args)
		{
			bool debugMenu = Settings.DebugMenu;
			int quitItem = 8;

			_gameMenu = new GameMenu(_canvas.Palette);
			_gameMenu.Items.Add(new GameMenu.Item("Tax Rate"));
			_gameMenu.Items.Add(new GameMenu.Item("Luxuries Rate"));
			_gameMenu.Items.Add(new GameMenu.Item("FindCity"));
			_gameMenu.Items.Add(new GameMenu.Item("Options"));
			_gameMenu.Items.Add(new GameMenu.Item("Save Game") { Enabled = (Game.GameTurn > 0) });
			_gameMenu.Items.Add(new GameMenu.Item("REVOLUTION!"));
			_gameMenu.Items.Add(new GameMenu.Item(null));
			if (debugMenu)
			{
				quitItem += 2;
				_gameMenu.Items.Add(new GameMenu.Item("Debug Options"));
				_gameMenu.Items.Add(new GameMenu.Item(null));
				_gameMenu.Items[7].Selected += (s, a) => GameTask.Enqueue(Show.Screen<DebugOptions>());
			}
			_gameMenu.Items.Add(new GameMenu.Item("Retire") { Enabled = false });
			_gameMenu.Items.Add(new GameMenu.Item("QUIT to DOS"));
			
			_gameMenu.Items[0].Selected += (s, a) => GameTask.Enqueue(Show.TaxRate);
			_gameMenu.Items[1].Selected += (s, a) => GameTask.Enqueue(Show.LuxuryRate);
			_gameMenu.Items[2].Selected += (s, a) => GameTask.Enqueue(Show.Search);
			_gameMenu.Items[3].Selected += (s, a) => GameTask.Enqueue(Show.Screen<GameOptions>());
			_gameMenu.Items[4].Selected += (s, a) => GameTask.Enqueue(Show.Screen<SaveGame>());
			_gameMenu.Items[5].Selected += (s, a) => GameTask.Enqueue(Show.Screen<Revolution>());
			_gameMenu.Items[quitItem].Selected += (s, a) => GameTask.Enqueue(Show.Screen<ConfirmQuit>());
			
			_menuX = 16;
			_menuY = 8;
			
			_update = true;
		}
		
		private void MenuBarOrders(object sender, EventArgs args)
		{
			if (Game.ActiveUnit == null) return;

			_gameMenu = new GameMenu(_canvas.Palette);
			_gameMenu.Items.AddRange(Game.ActiveUnit.MenuItems);
			
			_menuX = 72;
			_menuY = 8;
			
			_update = true;
		}
		
		private void MenuBarAdvisors(object sender, EventArgs args)
		{
			_gameMenu = new GameMenu(_canvas.Palette);
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
			_gameMenu = new GameMenu(_canvas.Palette);
			_gameMenu.Items.Add(new GameMenu.Item("Wonders of the World (F7)"));
			_gameMenu.Items.Add(new GameMenu.Item("Top 5 Cities (F8)"));
			_gameMenu.Items.Add(new GameMenu.Item("Civilization Score (F9)"));
			_gameMenu.Items.Add(new GameMenu.Item("World Map (F10)"));
			_gameMenu.Items.Add(new GameMenu.Item("Demographics"));
			_gameMenu.Items.Add(new GameMenu.Item("SpaceShips") { Enabled = false });
			
			_gameMenu.Items[0].Selected += (s, a) =>
			{
				if (Game.BuiltWonders.Length == 0)
					GameTask.Enqueue(Show.Empty);
				else
					Common.AddScreen(new WorldWonders());
			};
			_gameMenu.Items[1].Selected += (s, a) => Common.AddScreen(new TopCities());
			_gameMenu.Items[2].Selected += (s, a) => Common.AddScreen(new CivilizationScore());
			_gameMenu.Items[3].Selected += (s, a) => Common.AddScreen(new WorldMap());
			
			_menuX = 144;
			_menuY = 8;
			
			_update = true;
		}
		
		private void MenuBarCivilopedia(object sender, EventArgs args)
		{
			_gameMenu = new GameMenu(_canvas.Palette);
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
			if (Common.TopScreen is GamePlay && !GameTask.Any())
			{
				Game.Update();
			}

			if (Game.MovingUnit != null || Human != Game.CurrentPlayer || GameTask.Any())
			{
				Cursor = MouseCursor.None;
			}
			else
			{
				Cursor = MouseCursor.Pointer;
			}

			if (gameTick == _lastGameTick)
			{
				_gameMap.MustUpdate(gameTick);
				DrawLayer(_gameMap, gameTick, _rightSideBar ? 0 : 80, 8);
				return true;
			}

			if (_gameMap.MustUpdate(gameTick)) _update = true;
			if (_sideBar.HasUpdate(gameTick)) _update = true;
			if (gameTick % 3 == 0) _canvas.Cycle(96, 103).Cycle(104, 111);//_canvas.Cycle(96, 111);
			if (!_update && !_redraw) return (gameTick % 3 == 0);
			
			DrawLayer(_menuBar, gameTick, 0, 0);
			DrawLayer(_sideBar, gameTick, _rightSideBar ? (_canvas.Width - 80) : 0, 8);
			DrawLayer(_gameMap, gameTick, _rightSideBar ? 0 : 80, 8);
			DrawLayer(_gameMenu, gameTick, _menuX, _menuY);
			
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
				Settings.RevealWorldCheat();
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
			if (GameTask.Any()) return true;

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
				case Key.F7:
					if (Game.BuiltWonders.Length == 0)
						GameTask.Enqueue(Show.Empty);
					else
						Common.AddScreen(new WorldWonders());
					return true;
				case Key.F8:
					Common.AddScreen(new TopCities());
					return true;
				case Key.F9:
					Common.AddScreen(new CivilizationScore());
					return true;
				case Key.F10:
					Common.AddScreen(new WorldMap());
					return true;
				case Key.Plus:
					GameTask.Enqueue(Show.TaxRate);
					return true;
				case Key.Minus:
					GameTask.Enqueue(Show.LuxuryRate);
					return true;
				case Key.Slash:
					GameTask.Enqueue(Show.Search);
					return true;
			}
			return _gameMap.KeyDown(args);
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (Cursor == MouseCursor.None) return true;
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
				if (args.X > (_canvas.Width - 80))
				{
					MouseArgsOffset(ref args, (_canvas.Width - 80), 8);
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
			if (Cursor == MouseCursor.None) return true;
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
			if (Cursor == MouseCursor.None) return true;
			if (_gameMenu == null) return false;
			
			MouseArgsOffset(ref args, _menuX, _menuY);
			_update |= _gameMenu.MouseDrag(args);
			return _update;
		}
		
		public void Resize(int width, int height)
		{
			_canvas = new Picture(width, height, _canvas.Palette);
			_canvas.FillRectangle(5, 0, 0, width, height);

			_menuBar.Resize();
			_sideBar.Resize(height - 8);
			_gameMap.Resize(width - 80, height - 8);

			_update = true;
			HasUpdate(0);
		}
		
		public GamePlay()
		{
			Cursor = MouseCursor.Pointer;
			
			Color[] palette = Resources.Instance.LoadPIC("SP257").Palette;
			
			_canvas = new Picture(320, 200, palette);
			_canvas.FillRectangle(5, 0, 0, 320, 200);
			
			_rightSideBar = Settings.RightSideBar;

			_menuBar = new MenuBar(palette);
			_sideBar = new SideBar(palette);
			_gameMap = new GameMap();
			
			_menuBar.GameSelected += MenuBarGame;
			_menuBar.OrdersSelected += MenuBarOrders;
			_menuBar.AdvisorsSelected += MenuBarAdvisors;
			_menuBar.WorldSelected += MenuBarWorld;
			_menuBar.CivilopediaSelected += MenuBarCivilopedia;

			while (Game.CurrentPlayer != Game.HumanPlayer)
			{
				Game.Instance.Update();
				GameTask.Update();
			}
		}
	}
}