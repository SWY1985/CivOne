// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class LoadGame : BaseScreen
	{
		private class SaveGameFile
		{
			public bool ValidFile { get; private set; }
			public string SveFile { get; private set; }
			public string MapFile { get; private set; }
			public int Difficulty { get; private set; }
			
			public string Name { get; private set; }
			
			private ushort ReadUShort(BinaryReader reader, int position)
			{
				return Common.BinaryReadUShort(reader, position);
			}
			
			private string[] ReadStrings(BinaryReader reader, int position, int length, int itemLength)
			{
				return Common.BinaryReadStrings(reader, position, length, itemLength);
			}
			
			public SaveGameFile(string filename)
			{
				ValidFile = false;
				Name = "(EMPTY)";
				SveFile = string.Format("{0}.SVE", filename);
				MapFile = string.Format("{0}.MAP", filename);
				if (!File.Exists(SveFile) || !File.Exists(MapFile)) return;
				
				using (BinaryReader br = new BinaryReader(File.Open(SveFile, FileMode.Open)))
				{
					string turn = Common.YearString(ReadUShort(br, 0));
					ushort humanPlayer = ReadUShort(br, 2);
					ushort difficultyLevel = ReadUShort(br, 10);
					string leaderName = ReadStrings(br, 16, 112, 14)[humanPlayer];
					string civName = ReadStrings(br, 128, 96, 12)[humanPlayer];
					string tribeName = ReadStrings(br, 224, 88, 11)[humanPlayer];
					string title = Common.DifficultyName(difficultyLevel);
					
					Name = string.Format("{0} {1}, {2}/{3}", title, leaderName, civName, turn);
					Difficulty = (int)difficultyLevel;
				}
				ValidFile = true;
				//TODO: Handle invalid save files
			}
		}
		
		private readonly Color[] _palette;
		private char _driveLetter = 'C';
		private bool _update = true;
		private Menu _menu;
		
		public bool Cancel { get; private set; }
		
		private IEnumerable<SaveGameFile> GetSaveGames()
		{
			string path = Path.Combine(Settings.SavesDirectory, _driveLetter.ToString());
			for (int i = 0; i < 10; i++)
			{
				string filename = Path.Combine(path, string.Format("CIVIL{0}", i));
				yield return new SaveGameFile(filename);
			}
		}
		
		private void LoadSaveFile(object sender, EventArgs args)
		{
			int item = (sender as Menu.Item).Value;
			
			SaveGameFile file = GetSaveGames().ToArray()[item];

			SaveGame.SelectedGame = (item > 3 ? 3 : item);
			
			Console.WriteLine("Load game: {0}", file.Name);
			
			Destroy();
			
			Game.LoadGame(file.SveFile, file.MapFile);
			Common.AddScreen(new GamePlay());
		}
		
		private void LoadEmptyFile(object sender, EventArgs args)
		{
			Console.WriteLine("Empty save file, cancel");
			Cancel = true;
			_update = true;
		}
		
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
					AddLayer(_menu);
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
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (Cancel) return false;
			
			char c = Char.ToUpper(args.KeyChar);
			if (args.Key == Key.Escape)
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
			else if (args.Key == Key.Enter)
			{
				_menu = new Menu(Canvas.Palette)
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
				
				int i = 0;
				foreach (SaveGameFile file in GetSaveGames())
				{
					_menu.Items.Add(menuItem = new Menu.Item(file.Name, i++));
					if (file.ValidFile)
					{
						menuItem.Selected += LoadSaveFile;
					}
					else
					{
						menuItem.Selected += LoadEmptyFile;
					}
				}
				Cursor = MouseCursor.Pointer;
			}
			else if (c >= 'A' && c <= 'Z')
			{
				_driveLetter = c;
				_update = true;
				return true;
			}
			return false;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (_menu != null)
				return _menu.MouseDown(args);
			return false;
		}
		
		public override bool MouseUp(ScreenEventArgs args)
		{
			if (_menu != null)
				return _menu.MouseUp(args);
			return false;
		}
		
		public override bool MouseDrag(ScreenEventArgs args)
		{
			if (_menu != null)
				return _menu.MouseDrag(args);
			return false;
		}
		
		public LoadGame(Color[] palette)
		{
			_palette = palette;
		}
	}
}