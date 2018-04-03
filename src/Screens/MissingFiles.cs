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
using CivOne.Graphics;
using CivOne.IO;
using CivOne.Tasks;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	[Break]
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

		private bool _success = false;

		private int _y;

		private Menu _menu = null;

		private void Menu_Continue(object sender, EventArgs args)
		{
			Destroy();
		}

		private void Menu_Copy(object sender, EventArgs args)
		{
			string path = Runtime.BrowseFolder("Location of Civilization data files");
			if (path == null)
			{
				// User pressed cancel
				return;
			}

			this.Clear(8)
				.FillRectangle(40, 50, 240, 100, 15);

			if (FileSystem.CopyDataFiles(path))
			{
				_success = true;
				Resources.ClearInstance();
				if (GameTask.Any<CreditsScreen>())
				{
					GameTask.Remove<CreditsScreen>();
					GameTask.Enqueue(CreditsScreen.Show());
				}

				this.FillRectangle(0, 0, 320, 200, 8)
					.FillRectangle(40, 50, 240, 100, 15);

				this.DrawText("Succes!", 1, 2, 160, 54, TextAlign.Center);

				string[] text = new string[] { "Done copying the data files.", " ", "Press any key to start the game..." };

				for (int i = 0; i < text.Length; i++)
				{
					this.DrawText(text[i], 1, 5, 44, 66 + (i * 9), TextAlign.Left);
				}
			}
			else
			{
				this.DrawText("Failed!", 1, 4, 160, 54, TextAlign.Center);

				string[] text = new string[] { "Copying the data files has failed.", "Please make sure you pointed to the correct", "data folder and try again.", " ", "Press any key to close the game..." };

				for (int i = 0; i < text.Length; i++)
				{
					this.DrawText(text[i], 1, 5, 44, 66 + (i * 9), TextAlign.Left);
				}
			}

			_update = true;

			CloseMenus();
		}

		private void Menu_Quit(object sender, EventArgs args)
		{
			Runtime.Quit();
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_menu == null)
			{
				_menu = new Menu(Palette)
				{
					X = 44,
					Y = _y,
					MenuWidth = 232,
					ActiveColour = 11,
					TextColour = 5,
					FontId = 1,
					Indent = 4
				};

				_menu.Items.AddRange(
					MenuItem.Create("Continue without data files (not recommended)").OnSelect(Menu_Continue),
					MenuItem.Create("Browse for data files").OnSelect(Menu_Copy),
					MenuItem.Create("Quit").OnSelect(Menu_Quit)
				);
				
				AddMenu(_menu);
				return true;
			}

			if (!_update) return false;
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (!_success) Runtime.Quit();
			Destroy();
			return true;
		}
		
		public MissingFiles()
		{
			Palette = Common.GetPalette256;
			this.Clear(8)
				.FillRectangle(40, 50, 240, 100, 15)
				.DrawText("Warning!", 1, 4, 160, 54, TextAlign.Center);

			for (int i = 0; i < _text.Length; i++)
			{
				this.DrawText(_text[i], 1, 5, 44, 66 + (i * 9), TextAlign.Left);
			}

			_y = 75 + (9 * _text.Length);
		}
	}
}