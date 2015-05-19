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
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class NewGame : BaseScreen
	{
		private string[] MenuItemsDifficulty, MenuItemsCompetition, MenuItemsTribes;
		
		private readonly Picture _background;
		private int _difficulty = -1, _competition = -1, _tribe = -1;
		
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
			Common.AddScreen(AddMenu("Difficulty Level...", SetDifficulty, MenuItemsDifficulty));
		}
		
		private void MenuCompetition()
		{
			Common.AddScreen(AddMenu("Level of Competition...", SetCompetition, MenuItemsCompetition));
		}
		
		private void MenuTribe()
		{
			Common.AddScreen(AddMenu("Pick your tribe...", SetTribe, MenuItemsTribes));
		}
		
		private void SetDifficulty(object sender, EventArgs args)
		{
			_difficulty = (sender as Menu.Item).Value;
			CloseMenus();
		}
		
		private void SetCompetition(object sender, EventArgs args)
		{
			_competition = (7 - (sender as Menu.Item).Value);
			CloseMenus();
			
			MenuItemsTribes = Common.Civilizations.Where(c => c.PreferredPlayerNumber <= _competition).Select(civ => civ.Name).ToArray();
		}
		
		private void SetTribe(object sender, EventArgs args)
		{
			_tribe = (sender as Menu.Item).Value;
			CloseMenus();
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
			if (Menus.Count == 0)
			{
				if (_difficulty == -1) MenuDifficulty();
				else if (_competition == -1) MenuCompetition();
				else if (_tribe == -1) MenuTribe();
				else
				{
					Destroy();
					Common.AddScreen(new Demo());
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
					_canvas.AddLayer(_background.GetPart(140, 0, 180, 200), 140);
					int pictureStack = (_competition <= 0) ? 1 : _competition;
					for (int i = pictureStack; i > 0; i--)
					{
						_canvas.AddLayer(DifficultyPicture, 20 + (i * 2), 100 + (i * 3));
					}
				}
				
				return true;
			}
			return false;
		}
		
		public NewGame()
		{
			Cursor = MouseCursor.Pointer;
			
			_background = Resources.Instance.LoadPIC("DIFFS");
			
			_canvas = new Picture(320, 200, _background.Image.Palette.Entries);
			_canvas.AddLayer(_background.Image);
			
			MenuItemsDifficulty = new[] { "Chieftain (easiest)", "Warlock", "Prince", "King", "Emperor (toughest)" };
			MenuItemsCompetition = Enumerable.Range(3, 5).Reverse().Select(i => string.Format("{0} Civilizations", i)).ToArray();
		}
	}
}