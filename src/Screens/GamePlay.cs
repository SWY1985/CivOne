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
using CivOne.Screens.Dialogs;
using CivOne.Screens.GamePlayPanels;
using CivOne.Screens.Reports;
using CivOne.Tasks;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	[Expand]
	internal class GamePlay : BaseScreen
	{
		private readonly MenuBar _menuBar;
		private readonly SideBar _sideBar;
		private readonly GameMap _gameMap;

		private bool Busy => (Game.MovingUnit != null || Human != Game.CurrentPlayer || GameTask.Any());
		
		private GameMenu _gameMenu = null;
		private uint _lastGameTick;
		private bool _update = true;
		private bool _redraw = false;
		private bool _rightSideBar;

		private bool _shift5 = false;

		public override MouseCursor Cursor => Busy ? MouseCursor.None : MouseCursor.Pointer;

		internal int X => _gameMap.X;
		internal int Y => _gameMap.Y;

		internal void CenterOnPoint(int x, int y) => _gameMap.CenterOnPoint(x, y);

		internal void RefreshMap() => _gameMap.ForceRefresh();

		internal Palette MainPalette => OriginalColours.Copy();

		private void SetMenuItem(GameMenu gameMenu)
		{
			Elements.RemoveAll(x => x is GameMenu);
			Elements.Add(gameMenu);
		}
		
		private void MenuBarGame(object sender, EventArgs args)
		{
			_gameMenu = new GameMenu("MenuBarGame", 16);
			_gameMenu.AddItem("Tax Rate").OnSelect((s, a) => GameTask.Enqueue(Show.TaxRate));
			_gameMenu.AddItem("Luxuries Rate").OnSelect((s, a) => GameTask.Enqueue(Show.LuxuryRate));
			_gameMenu.AddItem("FindCity").OnSelect((s, a) => GameTask.Enqueue(Show.Search));
			_gameMenu.AddItem("Options").OnSelect((s, a) => GameTask.Enqueue(Show.Screen<GameOptions>()));
			_gameMenu.AddItem("Save Game").SetEnabled(Game.GameTurn > 0).OnSelect((s, a) => GameTask.Enqueue(Show.Screen<SaveGame>()));
			_gameMenu.AddItem("REVOLUTION!").OnSelect((s, a) => GameTask.Enqueue(Show.Screen<Revolution>()));
			_gameMenu.AddItem(null);
			if (Settings.DebugMenu)
			{
				_gameMenu.AddItem("Debug Options").OnSelect((s, a) => GameTask.Enqueue(Show.Screen<DebugOptions>()));
				_gameMenu.AddItem(null);
			}
			_gameMenu.AddItem("Retire").Disable();
			_gameMenu.AddItem("QUIT to DOS").OnSelect((s, a) => GameTask.Enqueue(Show.Screen<ConfirmQuit>()));

			SetMenuItem(_gameMenu);
		}
		
		private void MenuBarOrders(object sender, EventArgs args)
		{
			if (Game.ActiveUnit == null) return;

			_gameMenu = new GameMenu("MenuBarOrders", 72);
			foreach (MenuItem<int> item in Game.ActiveUnit.MenuItems)
			{
				_gameMenu.AddItem(item);
			}
			
			SetMenuItem(_gameMenu);
		}
		
		private void MenuBarAdvisors(object sender, EventArgs args)
		{
			_gameMenu = new GameMenu("MenuBarAdvisors", 112);
			_gameMenu.AddItem("City Status (F1)").OnSelect((s, a) => Common.AddScreen(new CityStatus()));
			_gameMenu.AddItem("Military Advisor (F2)").OnSelect((s, a) => { Common.AddScreen(new MilitaryLosses()); Common.AddScreen(new MilitaryStatus()); });
			_gameMenu.AddItem("Intelligence Advisor (F3)").OnSelect((s, a) => Common.AddScreen(new IntelligenceReport()));
			_gameMenu.AddItem("Attitude Advisor (F4)").OnSelect((s, a) => Common.AddScreen(new AttitudeSurvey()));
			_gameMenu.AddItem("Trade Advisor (F5)").OnSelect((s, a) => Common.AddScreen(new TradeReport()));
			_gameMenu.AddItem("Science Advisor (F6)").OnSelect((s, a) => Common.AddScreen(new ScienceReport()));
			
			SetMenuItem(_gameMenu);
		}
		
		private void MenuBarWorld(object sender, EventArgs args)
		{
			_gameMenu = new GameMenu("MenuBarWorld", 144);
			_gameMenu.AddItem("Wonders of the World (F7)").OnSelect((s, a) => {
				if (Game.BuiltWonders.Length == 0)
					GameTask.Enqueue(Show.Empty);
				else
					Common.AddScreen(new WorldWonders());
			});
			_gameMenu.AddItem("Top 5 Cities (F8)").OnSelect((s, a) => Common.AddScreen(new TopCities()));;
			_gameMenu.AddItem("Civilization Score (F9)").OnSelect((s, a) => Common.AddScreen(new CivilizationScore()));
			_gameMenu.AddItem("World Map (F10)").OnSelect((s, a) => Common.AddScreen(new WorldMap()));
			_gameMenu.AddItem("Demographics").OnSelect((s, a) => Common.AddScreen(new Demographics()));
			_gameMenu.AddItem("SpaceShips").Disable();
			
			SetMenuItem(_gameMenu);
		}
		
		private void MenuBarCivilopedia(object sender, EventArgs args)
		{
			_gameMenu = new GameMenu("MenuBarCivilopedia", 182);
			_gameMenu.AddItem("Complete").OnSelect((s, a) => Common.AddScreen(new Civilopedia(Civilopedia.Complete)));
			_gameMenu.AddItem("Civilization Advances").OnSelect((s, a) => Common.AddScreen(new Civilopedia(Civilopedia.Advances)));
			_gameMenu.AddItem("City Improvements").OnSelect((s, a) => Common.AddScreen(new Civilopedia(Civilopedia.Improvements)));
			_gameMenu.AddItem("Military Units").OnSelect((s, a) => Common.AddScreen(new Civilopedia(Civilopedia.Units)));
			_gameMenu.AddItem("Terrain Types").OnSelect((s, a) => Common.AddScreen(new Civilopedia(Civilopedia.TerrainType)));
			_gameMenu.AddItem("Miscellaneous").OnSelect((s, a) => Common.AddScreen(new Civilopedia(Civilopedia.Misc)));
			
			SetMenuItem(_gameMenu);
		}
		
		private void DrawLayer(IScreen layer, uint gameTick, int x, int y)
		{
			if (layer == null) return;
			if (!layer.Update(gameTick) && !_redraw) return;
			this.AddLayer(layer, x, y);
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (Common.TopScreen is GamePlay && !GameTask.Any())
			{
				Game.Update();
			}

			if (gameTick == _lastGameTick)
			{
				_gameMap.MustUpdate(gameTick);
				DrawLayer(_gameMap, gameTick, _rightSideBar ? 0 : 80, 8);
				return true;
			}

			if (_gameMap.MustUpdate(gameTick)) _update = true;
			if (gameTick % 3 == 0) this.Cycle(96, 103).Cycle(104, 111);
			if (!_update && !_redraw) return (gameTick % 3 == 0);
			
			DrawLayer(_gameMap, gameTick, _rightSideBar ? 0 : 80, 8);
			// DrawLayer(_gameMenu, gameTick, _menuX, _menuY);
			
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
		
		private void KeyDown(object sender, KeyboardEventArgs args)
		{
			if (GameTask.Any())
			{
				args.Handled = true;
				return;
			}

			if (CheckShift56(args))
			{
				args.Handled = true;
				return;
			}
			
			if (_gameMenu != null)
			{
				// if (!_gameMenu.KeyDown(args))
				// {
				// 	_gameMenu = null;
				// 	_redraw = true;
				// }
				args.Handled = true;
				return;
			}

			switch (args.Key)
			{
				case Key.F1:
					Common.AddScreen(new CityStatus());
					args.Handled = true;
					return;
				case Key.F2:
					Common.AddScreen(new MilitaryLosses());
					Common.AddScreen(new MilitaryStatus());
					args.Handled = true;
					return;
				case Key.F3:
					Common.AddScreen(new IntelligenceReport());
					args.Handled = true;
					return;
				case Key.F4:
					Common.AddScreen(new AttitudeSurvey());
					args.Handled = true;
					return;
				case Key.F5:
					Common.AddScreen(new TradeReport());
					args.Handled = true;
					return;
				case Key.F6:
					Common.AddScreen(new ScienceReport());
					args.Handled = true;
					return;
				case Key.F7:
					if (Game.BuiltWonders.Length == 0)
						GameTask.Enqueue(Show.Empty);
					else
						Common.AddScreen(new WorldWonders());
					args.Handled = true;
					return;
				case Key.F8:
					Common.AddScreen(new TopCities());
					args.Handled = true;
					return;
				case Key.F9:
					Common.AddScreen(new CivilizationScore());
					args.Handled = true;
					return;
				case Key.F10:
					Common.AddScreen(new WorldMap());
					args.Handled = true;
					return;
				case Key.Plus:
					GameTask.Enqueue(Show.TaxRate);
					args.Handled = true;
					return;
				case Key.Minus:
					GameTask.Enqueue(Show.LuxuryRate);
					args.Handled = true;
					return;
				case Key.Slash:
					GameTask.Enqueue(Show.Search);
					args.Handled = true;
					return;
			}
			args.Handled = _gameMap.KeyDown(args);
		}
		
		private void MouseDown(object sender, ScreenEventArgs args)
		{
			if (Cursor == MouseCursor.None)
			{
				args.Handled = true;
				return;
			}

			if (_rightSideBar)
			{
				MouseArgsOffset(ref args, 0, 8);
				args.Handled = (_update = _gameMap.MouseDown(args));
			}
			else
			{
				MouseArgsOffset(ref args, 80, 8);
				args.Handled = (_update = _gameMap.MouseDown(args));
			}
		}
		
		private void MouseUp(object sender, ScreenEventArgs args)
		{
			Elements.RemoveAll(x => x == _gameMenu);
			_gameMenu?.Dispose();
			_gameMenu = null;

			if (Cursor == MouseCursor.None)
			{
				args.Handled = true;
			}
		}
		
		private void MouseDrag(object sender, ScreenEventArgs args)
		{
			if (Cursor == MouseCursor.None)
			{
				args.Handled = true;
			}
		}
		
		private void Resize(object sender, ResizeEventArgs args)
		{
			this.Clear(5);

			_menuBar.Resize(args.Width);
			_sideBar.Resize(args.Height - 8);
			_gameMap.Resize(args.Width - 80, args.Height - 8);

			_update = true;
			HasUpdate(0);
		}
		
		public GamePlay()
		{
			Palette = Resources["SP257"].Palette;
			
			OnKeyDown += KeyDown;
			OnMouseDown += MouseDown;
			OnMouseUp += MouseUp;
			OnMouseDrag += MouseDrag;
			OnResize += Resize;
			
			_rightSideBar = Settings.RightSideBar;

			Elements.AddRange(new Element [] {
				_menuBar = new MenuBar(),
				_sideBar = new SideBar()
			});
			_gameMap = new GameMap();

			if (Width != 320 || Height != 200)
			{
				Resize(null, new ResizeEventArgs(Width, Height));
			}
			else
			{
				this.Clear(5);
			}
			
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