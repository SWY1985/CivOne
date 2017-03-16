// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.IO;
using CivOne.Templates;

namespace CivOne.Screens
{
	public class MissingFiles : BaseScreen
	{
		private readonly string[] _text = new string[]
		{
			"One or more data files are missing from the",
			"data folder. CivOne works best with the",
			"original Civilization for DOS data files.",
			" ",
			"What do you want to do?"
		};
		private bool _update = true;

		private int _y;

		private Menu _menu = null;

		private void Menu_Continue(object sender, EventArgs args)
		{
			Destroy();
		}

		private void Menu_Copy(object sender, EventArgs args)
		{
			string path = Native.FolderBrowser("Location of Civilization data files");
			if (path == null)
			{
				// User pressed cancel
				return;
			}

			_canvas.FillRectangle(8, 0, 0, 320, 200);
			_canvas.FillRectangle(15, 40, 50, 240, 100);

			if (FileSystem.CopyDataFiles(path))
			{
				_canvas.FillRectangle(8, 0, 0, 320, 200);
				_canvas.FillRectangle(15, 40, 50, 240, 100);

				_canvas.DrawText("Succes!", 1, 2, 160, 54, TextAlign.Center);

				string[] text = new string[] { "Done copying the data files.", "Please close the window and restart the game.", " ", "Press any key to close the game..." };

				for (int i = 0; i < text.Length; i++)
				{
					_canvas.DrawText(text[i], 1, 5, 44, 66 + (i * 9), TextAlign.Left);
				}
			}
			else
			{
				_canvas.DrawText("Failed!", 1, 4, 160, 54, TextAlign.Center);

				string[] text = new string[] { "Copying the data files has failed.", "Please make sure you pointed to the correct", "data folder and try again.", " ", "Press any key to close the game..." };

				for (int i = 0; i < text.Length; i++)
				{
					_canvas.DrawText(text[i], 1, 5, 44, 66 + (i * 9), TextAlign.Left);
				}
			}

			_update = true;

			CloseMenus();
		}

		private void Menu_Quit(object sender, EventArgs args)
		{
			Common.Quit();
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_menu == null)
			{
				_menu = new Menu(Canvas.Palette)
				{
					X = 44,
					Y = _y,
					Width = 232,
					ActiveColour = 11,
					TextColour = 5,
					FontId = 1,
					Indent = 4
				};
				int i = 0;
				foreach (string choice in new [] { "Continue without data files (not recommended)", "Browse for data files (requires manual restart)", "Quit" })
				{
					_menu.Items.Add(new Menu.Item(choice, i++));
				}
				_menu.Items[0].Selected += Menu_Continue;
				_menu.Items[1].Selected += Menu_Copy;
				_menu.Items[2].Selected += Menu_Quit;
				
				AddMenu(_menu);
				return true;
			}

			if (!_update) return false;
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			Common.Quit();
			Destroy();
			return true;
		}
		
		public MissingFiles()
		{
			Cursor = MouseCursor.None;

			_canvas = new Picture(320, 200, Common.GetPalette256);
			_canvas.FillRectangle(8, 0, 0, 320, 200);
			_canvas.FillRectangle(15, 40, 50, 240, 100);

			_canvas.DrawText("Warning!", 1, 4, 160, 54, TextAlign.Center);

			for (int i = 0; i < _text.Length; i++)
			{
				_canvas.DrawText(_text[i], 1, 5, 44, 66 + (i * 9), TextAlign.Left);
			}

			_y = 75 + (9 * _text.Length);
		}
	}
}