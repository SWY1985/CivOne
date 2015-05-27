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
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class LoadGame : BaseScreen
	{
		private readonly Color[] _palette;
		private char _driveLetter = 'C';
		private bool _update = true;
		private Menu _menu;
		
		public bool Cancel { get; private set; }
		
		private void DrawDriveQuestion()
		{
			_canvas = new Picture(320, 200, _palette);
			_canvas.FillRectangle(15, 0, 0, 320, 200);
			_canvas.DrawText("Which drive contains your", 0, 5, 92, 72, TextAlign.Left);
			_canvas.DrawText("saved game files?", 0, 5, 104, 80, TextAlign.Left);
			
			_canvas.DrawText(string.Format("{0}:", _driveLetter), 0, 5, 146, 96, TextAlign.Left);
			
			_canvas.DrawText("Press drive letter and", 0, 5, 104, 112, TextAlign.Left);
			_canvas.DrawText("Return when disk is inserted", 0, 5, 80, 120, TextAlign.Left);
			_canvas.DrawText("Press Escape to cancel", 0, 5, 104, 128, TextAlign.Left);
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_menu != null)
			{
				if (_menu.HasUpdate(gameTick))
				{
					_canvas = new Picture(320, 200, _palette);
					_canvas.FillRectangle(15, 0, 0, 320, 200);
					_canvas.AddLayer(_menu.Canvas.Image);
					_canvas.DrawText("Work in progress, press ESCAPE to return to menu", 0, 4, 160, 192, TextAlign.Center);
					return true;
				}
				return Cancel;
			}
			else if (_update)
			{
				DrawDriveQuestion();
				_update = false;
				return true;
			}
			return Cancel;
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			if (Cancel) return false;
						
			char c = Char.ToUpper((char)args.KeyCode);
			if (args.KeyCode == Keys.Escape)
			{
				Console.WriteLine("Cancel");
				Cancel = true;
				_update = true;
				return true;
			}
			else if (_menu != null)
			{
				return _menu.KeyDown(args);
			}
			else if (args.KeyCode == Keys.Enter)
			{
				_menu = new Menu(Canvas.Image.Palette.Entries)
				{
					Title = "Select Load File...",
					X = 51,
					Y = 70,
					Width = 217,
					TitleColour = 12,
					ActiveColour = 11,
					TextColour = 5,
					FontId = 0,
					IndentTitle = 2,
					RowHeight = 8
				};
				
				Menu.Item menuItem;
				for (int i = 0; i < 10; i++)
				{
					_menu.Items.Add(menuItem = new Menu.Item("(EMPTY)", i));
				}
			}
			else if (c >= 'A' && c <= 'Z')
			{
				_driveLetter = c;
				_update = true;
				return true;
			}
			return false;
		}
		
		public LoadGame(Color[] palette)
		{
			_palette = palette;
		}
	}
}