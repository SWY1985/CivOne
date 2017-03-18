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
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class SaveGame : BaseScreen, IModal
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
		
		internal static int SelectedGame = 0;
		
		private readonly Color[] _palette;
		private char _driveLetter = 'C';
		private readonly int _border = Common.Random.Next(2);
		private int _gameId;
		private bool _update = true;
		private bool _saving = false;
		private Menu _menu;
		
		private IEnumerable<SaveGameFile> GetSaveGames()
		{
			string path = Path.Combine(Settings.SavesDirectory, char.ToLower(_driveLetter).ToString());
			for (int i = 0; i < 10; i++)
			{
				string filename = Path.Combine(path, string.Format("CIVIL{0}", i));
				yield return new SaveGameFile(filename);
			}
		}
		
		private void SaveFile(object sender, EventArgs args)
		{
			int item = (sender as Menu.Item).Value;

			SaveGameFile file = GetSaveGames().ToArray()[item];
			Game.Save(file.SveFile, file.MapFile);
			_gameId = item;
			SelectedGame = item;
			_saving = true;
			_update = true;
		}
		
		private void DrawDriveQuestion()
		{
			_canvas = new Picture(320, 200, _palette);
			_canvas.FillRectangle(15, 0, 0, 320, 200);
			DrawBorder(_border);

			_canvas.DrawText("Which drive contains your", 0, 5, 92, 72, TextAlign.Left);
			_canvas.DrawText("Save Game disk?", 0, 5, 104, 80, TextAlign.Left);
			
			_canvas.DrawText(string.Format("{0}:", _driveLetter), 0, 5, 146, 96, TextAlign.Left);
			
			_canvas.DrawText("Press drive letter and", 0, 5, 104, 112, TextAlign.Left);
			_canvas.DrawText("Return when disk is inserted", 0, 5, 80, 120, TextAlign.Left);
			_canvas.DrawText("Press Escape to cancel", 0, 5, 104, 128, TextAlign.Left);
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_saving)
			{
				if (!_update) return false;
				Cursor = MouseCursor.None;
				_update = false;
				_canvas = new Picture(320, 200, _palette);
				_canvas.FillRectangle(15, 0, 0, 320, 200);
				DrawBorder(_border);

				if (_menu != null)
				{
					AddLayer(_menu);
					_menu.Close();
					_menu = null;
				}

				DrawPanel(64, 86, 124, 41);
				_canvas.DrawText($"{char.ToLower(_driveLetter)}:CIVIL{_gameId}.SVE", 0, 5, 75, 91);
				_canvas.DrawText($"{Common.DifficultyName(Game.Difficulty)} {Game.HumanPlayer.LeaderName}", 0, 5, 75, 99);
				_canvas.DrawText($"{Game.HumanPlayer.TribeNamePlural}/{Game.GameYear}", 0, 5, 75, 107);
				_canvas.DrawText("... save in progress.", 0, 5, 75, 115);
				
				_canvas.DrawText("Game has been saved.", 0, 5, 75, 132);
				_canvas.DrawText("Press key to continue.", 0, 5, 75, 140);
				return true;
			}
			else if (_menu != null)
			{
				if (_menu.HasUpdate(gameTick))
				{
					_canvas = new Picture(320, 200, _palette);
					_canvas.FillRectangle(15, 0, 0, 320, 200);
					DrawBorder(_border);
					AddLayer(_menu);
					return true;
				}
				return false;
			}
			else if (_update)
			{
				DrawDriveQuestion();
				_update = false;
				return true;
			}
			return false;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (_saving)
			{
				Destroy();
				return true;
			}
			
			char c = Char.ToUpper(args.KeyChar);
			if (args.Key == Key.Escape)
			{
				Console.WriteLine("Cancel");
				Destroy();
				return true;
			}
			else if (_menu != null)
			{
				return _menu.KeyDown(args);
			}
			else if (args.Key == Key.Enter)
			{
				if (_gameId >= 0)
				{
					SaveGameFile file = GetSaveGames().ToArray()[_gameId];
					Game.Save(file.SveFile, file.MapFile);
					_saving = true;
					_update = true;
					return true;
				}

				_menu = new Menu(Canvas.Palette)
				{
					Title = "Select Save File...",
					X = 51,
					Y = 38,
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
				foreach (SaveGameFile file in GetSaveGames().Take(4))
				{
					_menu.Items.Add(menuItem = new Menu.Item(file.Name, i++));
					menuItem.Selected += SaveFile;
				}
				
				_menu.ActiveItem = SelectedGame;
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
		
		public SaveGame()
		{
			_palette = Resources.Instance.LoadPIC("SP257").Palette;
			_gameId = -1;
		}
		
		public SaveGame(int gameId)
		{
			_palette = Resources.Instance.LoadPIC("SP257").Palette;
			_gameId = gameId;
		}
	}
}