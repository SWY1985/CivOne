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
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.GFX;
using CivOne.IO;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class NewGame : BaseScreen
	{
		private ICivilization[] _tribesAvailable;
		private string[] _menuItemsDifficulty, _menuItemsCompetition, _menuItemsTribes;
		
		private readonly Picture _background;
		private int _difficulty = -1, _competition = -1, _tribe = -1;
		private string _leaderName = null;
		private bool _done = false, _showIntroText = false;
		
		private Menu AddMenu(string title, EventHandler setChoice, params string[] menuTexts)
		{
			Menu menu = new Menu(Canvas.Image.Palette.Entries)
			{
				Title = title,
				X = 163,
				Y = 40,
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
			Menus.Add(menu);
			return menu;
		}
		
		private void MenuDifficulty()
		{
			Common.AddScreen(AddMenu("Difficulty Level...", SetDifficulty, _menuItemsDifficulty));
		}
		
		private void MenuCompetition()
		{
			Common.AddScreen(AddMenu("Level of Competition...", SetCompetition, _menuItemsCompetition));
		}
		
		private void MenuTribe()
		{
			Common.AddScreen(AddMenu("Pick your tribe...", SetTribe, _menuItemsTribes));
		}
		
		private void InputLeaderName()
		{
			if (Common.HasScreenType(typeof(Input))) return;
			//TODO: Add input box (position 168x105 - 109x10)
			ICivilization civ = _tribesAvailable[_tribe];
			Input input = new Input(_canvas.Image.Palette.Entries, civ.LeaderName, 6, 5, 11, 168, 105, 109, 10, 13);
			input.Accept += LeaderName_Accept;
			input.Cancel += LeaderName_Cancel;
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
			CloseMenus();
			Console.WriteLine("Tribe: {0}", _menuItemsTribes[_tribe]);
		}
		
		private void LeaderName_Accept(object sender, EventArgs args)
		{
			if (sender.GetType() != typeof(Input)) return;
			_leaderName = ((Input)sender).Text;
			((Input)sender).Close();
		}
		
		private void LeaderName_Cancel(object sender, EventArgs args)
		{
			if (sender.GetType() != typeof(Input)) return;
			ICivilization civ = _tribesAvailable[_tribe];
			_leaderName = civ.LeaderName;
			((Input)sender).Close();
		}
		
		private Bitmap DifficultyPicture
		{
			get
			{
				int x = (_difficulty % 2) == 0 ? 21 : 80;
				int y = 6 + (35 * _difficulty);
				return _background.GetPart(x, y, 53, 47);
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (Menus.Count != 0) return false;
			
			if (_difficulty == -1) MenuDifficulty();
			else if (_competition == -1) MenuCompetition();
			else if (_tribe == -1) MenuTribe();
			else if (_leaderName == null) InputLeaderName();
			else if (!_done)
			{
				if (_showIntroText) return false;
				
				ICivilization civ = _tribesAvailable[_tribe];
				Game.CreateGame(_difficulty, _competition, civ, _leaderName);
				
				Bitmap[] borders = new Bitmap[8];
				int index = 0;
				for (int y = 0; y < 2; y++)
				{
					for (int x = 0; x < 4; x++)
					{
						borders[index] = (Bitmap)Resources.Instance.GetPart("SP299", 224 + (8 * x), 120 + (8 * y), 8, 8).Clone();
						Picture.ReplaceColours(borders[index], 15, 16);
						index++;
					}
				}
				
				_canvas.FillRectangle(15, 0, 0, 320, 200);
				for (int x = 8; x < 312; x += 8)
				{
					_canvas.AddLayer(borders[4], x, 0);
					_canvas.AddLayer(borders[6], x, 192);
				}
				for (int y = 8; y < 192; y += 8)
				{
					_canvas.AddLayer(borders[5], 0, y);
					_canvas.AddLayer(borders[7], 312, y);
				}
				_canvas.AddLayer(borders[0], 0, 0);
				_canvas.AddLayer(borders[1], 312, 0);
				_canvas.AddLayer(borders[2], 0, 192);
				_canvas.AddLayer(borders[3], 312, 192);
				
				_canvas.AddLayer(DifficultyPicture, 134, 20);
				
				int yy = 81;
				foreach (string textLine in TextFile.Instance.GetGameText("INIT"))
				{
					string line = textLine.Replace("$RPLC1", Game.Instance.HumanPlayer.LeaderName).Replace("$US", civ.NamePlural).Replace("^", "");
					_canvas.DrawText(line, 0, 5, 88, yy);
					yy += 8;
					Console.WriteLine(line);
				}
				_canvas.DrawText("and Roads.", 0, 5, 88, yy);
				Console.WriteLine("and Roads.");
				
				_showIntroText = true;
				return true;
			}
			else
			{
				Destroy();
				Common.AddScreen(new GamePlay());
				return true;
			}
			
			// Draw background
			_canvas = new Picture(320, 200, _background.Image.Palette.Entries);
			if (_difficulty == -1)
			{
				_canvas.AddLayer(_background.Image);
			}
			else
			{
				if (_tribe == -1)
					_canvas.AddLayer(_background.GetPart(140, 0, 180, 200), 140);
				int pictureStack = (_competition <= 0) ? 0 : _competition;
				for (int i = pictureStack; i >= 0; i--)
				{
					_canvas.AddLayer(DifficultyPicture, 22 + (i * 2), 100 + (i * 3));
				}
				
				if (_tribe != -1 && _leaderName == null)
				{
					ICivilization civ = _tribesAvailable[_tribe];
					
					_canvas.DrawText(civ.NamePlural, 6, 15, 47, 92, TextAlign.Center);
					_canvas.FillRectangle(11, 158, 88, 161, 33);
					_canvas.FillRectangle(15, 159, 89, 159, 31);
					_canvas.DrawText("Your Name...", 6, 5, 166, 90);
					_canvas.FillRectangle(5, 166, 103, 113, 14);
					_canvas.FillRectangle(15, 167, 104, 111, 12);
				}
			}
			
			return true;
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			if (_tribe != -1 && _leaderName == null)
			{
				if (args.KeyCode == Keys.Enter)
				{
					ICivilization civ = _tribesAvailable[_tribe];
					_leaderName = civ.LeaderName;
					return true;
				}
				return false;
			}
			if (_difficulty > -1 && _competition > -1 && _tribe > -1 && !_done)
				_done = true;
			return _done;
		}
		
		public override bool MouseDown(MouseEventArgs args)
		{
			if (_difficulty > -1 && _competition > -1 && _tribe > -1 && !_done)
				_done = true;
			return _done;
		}
		
		public NewGame()
		{
			Cursor = MouseCursor.Pointer;
			
			_background = Resources.Instance.LoadPIC("DIFFS");
			
			_canvas = new Picture(320, 200, _background.Image.Palette.Entries);
			_canvas.AddLayer(_background.Image);
			
			_menuItemsDifficulty = new[] { "Chieftain (easiest)", "Warlock", "Prince", "King", "Emperor (toughest)" };
			_menuItemsCompetition = Enumerable.Range(3, 5).Reverse().Select(i => string.Format("{0} Civilizations", i)).ToArray();
		}
	}
}