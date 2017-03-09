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
using CivOne.Enums;
using CivOne.Events;
using CivOne.Interfaces;
using CivOne.GFX;
using CivOne.IO;
using CivOne.Tasks;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class NewGame : BaseScreen
	{
		private ICivilization[] _tribesAvailable;
		private string[] _menuItemsDifficulty, _menuItemsCompetition, _menuItemsTribes;
		
		private readonly Picture _background;
		private int _difficulty = -1, _competition = -1, _tribe = -1;
		private string _leaderName = null, _tribeName = null, _tribeNamePlural = null;
		private bool _done = false, _showIntroText = false;
		
		private Menu CreateMenu(string title, EventHandler setChoice, params string[] menuTexts)
		{
			Menu menu = new Menu(Canvas.Palette)
			{
				Title = title,
				X = 163,
				Y = 39,
				Width = 114,
				TitleColour = 3,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 8,
				FontId = 6,
				IndentTitle = 2,
				RowHeight = 8
			};
			
			Menu.Item menuItem;
			for (int i = 0; i < menuTexts.Length; i++)
			{
				menu.Items.Add(menuItem = new Menu.Item(menuTexts[i], i));
				menuItem.Selected += setChoice;
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
			Input input = new Input(_canvas.Palette, civ.Leader.Name, 6, 5, 11, 168, 105, 109, 10, 13);
			input.Accept += LeaderName_Accept;
			input.Cancel += LeaderName_Accept;
			Common.AddScreen(input);
		}
		
		private void SetDifficulty(object sender, EventArgs args)
		{
			_difficulty = (sender as Menu.Item).Value;
			CloseMenus();
			Console.WriteLine("Difficulty: {0}", _menuItemsDifficulty[_difficulty]);
		}
		
		private void SetCompetition(object sender, EventArgs args)
		{
			_competition = (7 - (sender as Menu.Item).Value);
			CloseMenus();
			Console.WriteLine("Competition: {0} Civilizations", _competition);
			
			_tribesAvailable = Common.Civilizations.Where(c => c.PreferredPlayerNumber > 0 && c.PreferredPlayerNumber <= _competition).ToArray();
			_menuItemsTribes = _tribesAvailable.Select(c => c.Name).ToArray();
		}
		
		private void SetTribe(object sender, EventArgs args)
		{
			_tribe = (sender as Menu.Item).Value;
			
			ICivilization civ = _tribesAvailable[_tribe];
			_tribeName = civ.Name;
			_tribeNamePlural = civ.NamePlural;
			CloseMenus();
			Console.WriteLine("Tribe: {0}", _menuItemsTribes[_tribe]);
		}
		
		private void SetTribe_Cancel(object sender, EventArgs args)
		{
			if (sender.GetType() != typeof(Menu)) return;
			
			_tribe = Common.Random.Next(_competition);
			AddLayer((Menu)sender);
			CloseMenus();
			
			ICivilization civ = _tribesAvailable[_tribe];
			Input input = new Input(_canvas.Palette, civ.NamePlural, 6, 5, 11, 168, 105, 109, 10, 11);
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
				int x = (_difficulty % 2) == 0 ? 21 : 80;
				int y = 6 + (35 * _difficulty);
				return _background.GetPart(x, y, 53, 47);
			}
		}
		
		private void DrawInputBox(string text)
		{
			_canvas.FillRectangle(11, 158, 88, 161, 33);
			_canvas.FillRectangle(15, 159, 89, 159, 31);
			_canvas.DrawText(text, 6, 5, 166, 90);
			_canvas.FillRectangle(5, 166, 103, 113, 14);
			_canvas.FillRectangle(15, 167, 104, 111, 12);
		}
		
		public override bool HasUpdate(uint gameTick)
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
				
				Picture[] borders = new Picture[8];
				int index = 0;
				for (int y = 0; y < 2; y++)
				{
					for (int x = 0; x < 4; x++)
					{
						borders[index] = Resources.Instance.GetPart("SP299", 224 + (8 * x), 120 + (8 * y), 8, 8);
						index++;
					}
				}
				
				_canvas.FillRectangle(15, 0, 0, 320, 200);
				for (int x = 8; x < 312; x += 8)
				{
					AddLayer(borders[4], x, 0);
					AddLayer(borders[6], x, 192);
				}
				for (int y = 8; y < 192; y += 8)
				{
					AddLayer(borders[7], 0, y);
					AddLayer(borders[5], 312, y);
				}
				AddLayer(borders[0], 0, 0);
				AddLayer(borders[1], 312, 0);
				AddLayer(borders[2], 0, 192);
				AddLayer(borders[3], 312, 192);
				
				AddLayer(DifficultyPicture, 134, 20);
				
				int yy = 81;
				foreach (string textLine in TextFile.Instance.GetGameText("KING/INIT"))
				{
					string line = textLine.Replace("$RPLC1", Human.LeaderName).Replace("$US", Human.TribeNamePlural).Replace("^", "");
					_canvas.DrawText(line, 0, 5, 88, yy);
					yy += 8;
					Console.WriteLine(line);
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
					_canvas.DrawText(line, 0, 5, 88, yy);
					Console.WriteLine(line);
					yy += 8;
				}
				/*
				_canvas.DrawText("and Roads.", 0, 5, 88, yy);
				Console.WriteLine("and Roads.");*/
				
				_showIntroText = true;
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
			_canvas = new Picture(320, 200, _background.Palette);
			if (_difficulty == -1)
			{
				AddLayer(_background);
			}
			else
			{
				if (_tribe == -1)
					AddLayer(_background.GetPart(140, 0, 180, 200), 140);
				int pictureStack = (_competition <= 0) ? 0 : _competition;
				for (int i = pictureStack; i > 0; i--)
				{
					AddLayer(DifficultyPicture, 22 + (i * 2), 100 + (i * 3));
				}
				
				if (_tribe != -1 && _leaderName == null)
				{
					_canvas.DrawText(_tribeNamePlural, 6, 15, 47, 92, TextAlign.Center);
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
		
		public NewGame()
		{
			Cursor = MouseCursor.Pointer;
			
			_background = Resources.Instance.LoadPIC("DIFFS");
			
			_canvas = new Picture(320, 200, _background.Palette);
			AddLayer(_background);
			
			_menuItemsDifficulty = new[] { "Chieftain (easiest)", "Warlord", "Prince", "King", "Emperor (toughest)" };
			_menuItemsCompetition = Enumerable.Range(3, 5).Reverse().Select(i => string.Format("{0} Civilizations", i)).ToArray();
		}
	}
}