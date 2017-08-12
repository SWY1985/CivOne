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
using System.Text;
using CivOne.Advances;
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.IO;
using CivOne.Tasks;
using CivOne.Units;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	[Expand]
	internal class NewGame : BaseScreen
	{
		private ICivilization[] _tribesAvailable;
		private string[] _menuItemsDifficulty, _menuItemsCompetition, _menuItemsTribes;
		
		private readonly Picture _background;

		private int OffsetX => ((_canvas.Width - 320) / 2);
		private int OffsetY => ((_canvas.Height - 200) / 2);

		private int _difficulty = -1, _competition = -1, _tribe = -1;
		private string _leaderName = null, _tribeName = null, _tribeNamePlural = null;
		private bool _done = false, _showIntroText = false;
		
		private Menu CreateMenu(string title, MenuItemEventHandler<int> setChoice, params string[] menuTexts)
		{
			Menu menu = new Menu("NewGameMenu", Palette)
			{
				Title = title,
				X = OffsetX + 163,
				Y = OffsetY + 39,
				Width = 114,
				TitleColour = 3,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 8,
				FontId = 6,
				IndentTitle = 2,
				RowHeight = 8
			};
			
			for (int i = 0; i < menuTexts.Length; i++)
			{
				menu.Items.Add(menuTexts[i], i).OnSelect(setChoice);
			}
			return menu;
		}
		
		private void MenuDifficulty()
		{
			AddMenu(CreateMenu("Difficulty Level...", SetDifficulty, _menuItemsDifficulty));
		}
		
		private void MenuCompetition()
		{
			AddMenu(CreateMenu("Level of Competition...", SetCompetition, _menuItemsCompetition));
		}
		
		private void MenuTribe()
		{
			Menu menu = CreateMenu("Pick your tribe...", SetTribe, _menuItemsTribes);
			if (_menuItemsTribes.Length > 14)
			{
				menu.FontId = 1;
				menu.RowHeight -= 2;
			}
			menu.Cancel += SetTribe_Cancel;
			AddMenu(menu);
		}
		
		private void InputLeaderName()
		{
			if (Common.HasScreenType<Input>()) return;
			
			ICivilization civ = _tribesAvailable[_tribe];
			Input input = new Input(_canvas.Palette, civ.Leader.Name, 6, 5, 11, OffsetX + 168, OffsetY + 105, 109, 10, 13);
			input.Accept += LeaderName_Accept;
			input.Cancel += LeaderName_Accept;
			Common.AddScreen(input);
		}
		
		private void SetDifficulty(object sender, MenuItemEventArgs<int> args)
		{
			_difficulty = args.Value;
			CloseMenus();
			Log("Difficulty: {0}", _menuItemsDifficulty[_difficulty]);
		}
		
		private void SetCompetition(object sender, MenuItemEventArgs<int> args)
		{
			_competition = (7 - args.Value);
			CloseMenus();
			Log("Competition: {0} Civilizations", _competition);
			
			_tribesAvailable = Common.Civilizations.Where(c => c.PreferredPlayerNumber > 0 && c.PreferredPlayerNumber <= _competition).ToArray();
			_menuItemsTribes = _tribesAvailable.Select(c => c.Name).ToArray();
		}
		
		private void SetTribe(object sender, MenuItemEventArgs<int> args)
		{
			_tribe = args.Value;
			
			ICivilization civ = _tribesAvailable[_tribe];
			_tribeName = civ.Name;
			_tribeNamePlural = civ.NamePlural;
			CloseMenus();
			Log("Tribe: {0}", _menuItemsTribes[_tribe]);
		}
		
		private void SetTribe_Cancel(object sender, EventArgs args)
		{
			if (sender.GetType() != typeof(Menu)) return;
			
			_tribe = Common.Random.Next(_competition);
			AddLayer((Menu)sender);
			CloseMenus();
			
			ICivilization civ = _tribesAvailable[_tribe];
			Input input = new Input(_canvas.Palette, civ.NamePlural, 6, 5, 11, OffsetX + 168, OffsetY + 105, 109, 10, 11);
			input.Accept += TribeName_Accept;
			input.Cancel += TribeName_Accept;
			Common.AddScreen(input);
		}
		
		private void LeaderName_Accept(object sender, EventArgs args)
		{
			if (sender.GetType() != typeof(Input)) return;
			
			_leaderName = ((Input)sender).Text;
			((Input)sender).Close();
		}
		
		private void TribeName_Accept(object sender, EventArgs args)
		{
			if (sender.GetType() != typeof(Input)) return;
			
			_tribeNamePlural = ((Input)sender).Text;
			_tribeName = ((Input)sender).Text;
			((Input)sender).Close();
		}
		
		private Picture DifficultyPicture
		{
			get
			{
				int pictureId = _difficulty;
				if (pictureId > 4) pictureId = 4;

				int x = (pictureId % 2) == 0 ? 21 : 80;
				int y = 6 + (35 * pictureId);
				return _background.GetPart(x, y, 53, 47);
			}
		}
		
		private void DrawInputBox(string text)
		{
			_canvas.FillRectangle(11, OffsetX + 158, OffsetY + 88, 161, 33);
			_canvas.FillRectangle(15, OffsetX + 159, OffsetY + 89, 159, 31);
			_canvas.DrawText(text, 6, 5, OffsetX + 166, OffsetY + 90);
			_canvas.FillRectangle(5, OffsetX + 166, OffsetY + 103, 113, 14);
			_canvas.FillRectangle(15, OffsetX + 167, OffsetY + 104, 111, 12);
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (HasMenu) return false;

			if (_difficulty == -1) MenuDifficulty();
			else if (_competition == -1) MenuCompetition();
			else if (_tribe == -1) MenuTribe();
			else if (_leaderName == null) InputLeaderName();
			else if (!_done)
			{
				if (_showIntroText) return false;
				
				ICivilization civ = _tribesAvailable[_tribe];
				Game.CreateGame(_difficulty, _competition, civ, _leaderName, _tribeName, _tribeNamePlural);
				
				_canvas.FillRectangle(15, 0, 0, _canvas.Width, _canvas.Height);
				DrawBorder(Common.Random.Next(2));
				
				AddLayer(DifficultyPicture, OffsetX + 134, OffsetY + 20);
				
				int yy = OffsetY + 81;
				foreach (string textLine in TextFile.Instance.GetGameText("KING/INIT"))
				{
					string line = textLine.Replace("$RPLC1", Human.LeaderName).Replace("$US", Human.TribeNamePlural).Replace("^", "");
					_canvas.DrawText(line, 0, 5, OffsetX + 88, yy);
					yy += 8;
					Log(line);
				}
				StringBuilder sb = new StringBuilder();
				int i = 0;
				foreach (IAdvance advance in Human.Advances.OrderBy(a => a.Id))
				{
					sb.Append($"{advance.Name}, ");
					i++;
					if (i % 2 == 0) sb.Append("|");
				}
				sb.Append("and Roads.");

				foreach (string line in sb.ToString().Split('|'))
				{
					_canvas.DrawText(line, 0, 5, OffsetX + 88, yy);
					Log(line);
					yy += 8;
				}

				Runtime.PlaySound(Human.Civilization.Tune);
				
				_showIntroText = true;
				return true;
			}
			else if (HandleScreenFadeOut())
			{
				return true;
			}
			else
			{
				Destroy();

				GamePlay gamePlay = new GamePlay();
				Common.AddScreen(gamePlay);
				IUnit startUnit = Game.GetUnits().First(x => Game.Human == x.Owner);
				gamePlay.CenterOnPoint(startUnit.X, startUnit.Y);
				
				if (Game.Difficulty == 0)
				{
					GameTask.Enqueue(Show.InterfaceHelp);
					GameTask.Enqueue(Message.Help("--- Civilization Note ---", TextFile.Instance.GetGameText("HELP/FIRSTMOVE")));
				}
				return true;
			}
			
			if (_tribe != -1 && _tribeName == null)
			{
				DrawInputBox("Name of your Tribe...");
				return true;
			}
			
			// Draw background
			_canvas = new Picture(_canvas.Width, _canvas.Height, _background.Palette);
			if (_difficulty == -1)
			{
				AddLayer(_background, OffsetX, OffsetY);
			}
			else
			{
				if (_tribe == -1)
					AddLayer(_background.GetPart(140, 0, 180, 200), OffsetX + 140, OffsetY);
				int pictureStack = (_competition <= 0) ? 1 : _competition;
				for (int i = pictureStack; i > 0; i--)
				{
					AddLayer(DifficultyPicture, OffsetX + 22 + (i * 2), OffsetY + 100 + (i * 3));
				}
				
				if (_tribe != -1 && _leaderName == null)
				{
					_canvas.DrawText(_tribeNamePlural, 6, 15, OffsetX + 47, OffsetY + 92, TextAlign.Center);
					DrawInputBox("Your Name...");
				}
			}
			
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (_tribe != -1 && _leaderName == null)
			{
				if (args.Key == Key.Enter)
				{
					ICivilization civ = _tribesAvailable[_tribe];
					_leaderName = civ.Leader.Name;
					return true;
				}
				return false;
			}
			if (_difficulty > -1 && _competition > -1 && _tribe > -1 && !_done)
				_done = true;
			return _done;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (_difficulty > -1 && _competition > -1 && _tribe > -1 && !_done)
				_done = true;
			return _done;
		}

		private void Resize(object sender, ResizeEventArgs args)
		{
			_canvas.FillRectangle(5, 0, 0, args.Width, args.Height);
			if (_leaderName == null)
			{
				CloseMenus();
			}
			foreach (Input input in Common.Screens.Where(x => x is Input))
			{
				input.X = OffsetX + 168;
				input.Y = OffsetY + 105;
			}
			_showIntroText = false;
		}
		
		public NewGame() : base(MouseCursor.Pointer)
		{
			OnResize += Resize;
			
			_background = Resources.Instance.LoadPIC("DIFFS");
			
			_canvas = new Picture(320, 200, _background.Palette);
			AddLayer(_background);
			
			if (Settings.Instance.DeityEnabled)
				_menuItemsDifficulty = new[] { "Chieftain (easiest)", "Warlord", "Prince", "King", "Emperor", "Deity (toughest)" };
			else
				_menuItemsDifficulty = new[] { "Chieftain (easiest)", "Warlord", "Prince", "King", "Emperor (toughest)" };
			_menuItemsCompetition = Enumerable.Range(3, 5).Reverse().Select(i => string.Format("{0} Civilizations", i)).ToArray();
		}
	}
}