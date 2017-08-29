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
using CivOne.Graphics;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	[Modal]
	internal class SaveGame : BaseScreen
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
				
				try
				{
					using (FileStream fs = new FileStream(SveFile, FileMode.Open))
					using (BinaryReader br = new BinaryReader(fs))
					{
						if (fs.Length != 37856)
						{
							Name = "(INCORRECT FILE SIZE)";
							return;
						}

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
				}
				catch(Exception ex)
				{
					Log($"Could not open .SVE file: {ex.InnerException}");
					Name = "(COULD NOT READ SAVE FILE HEADER)";
				}
			}
		}
		
		internal static int SelectedGame = 0;
		
		private char _driveLetter = 'C';
		private readonly int _border = Common.Random.Next(2);
		private int _gameId;
		private bool _update = true;
		private bool _saving = false;
		private Menu _menu;

		public override MouseCursor Cursor => (_menu == null ? MouseCursor.Pointer : MouseCursor.None);
		
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
			int item = (sender as MenuItem<int>).Value;
			_gameId = item;
			SelectedGame = item;
			_saving = true;
			_update = true;

			SaveGameFile file = GetSaveGames().ToArray()[item];
			Game.Save(file.SveFile, file.MapFile);
		}
		
		private void DrawDriveQuestion()
		{
			Bitmap.Clear();
			this.Clear(15);
			DrawBorder(_border);

			this.DrawText("Which drive contains your", 0, 5, 92, 72, TextAlign.Left)
				.DrawText("Save Game disk?", 0, 5, 104, 80, TextAlign.Left)
				.DrawText(string.Format("{0}:", _driveLetter), 0, 5, 146, 96, TextAlign.Left)
				.DrawText("Press drive letter and", 0, 5, 104, 112, TextAlign.Left)
				.DrawText("Return when disk is inserted", 0, 5, 80, 120, TextAlign.Left)
				.DrawText("Press Escape to cancel", 0, 5, 104, 128, TextAlign.Left);
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_saving)
			{
				if (!_update) return false;
				_update = false;
				Bitmap.Clear();
				this.Clear(15);
				DrawBorder(_border);

				if (_menu != null)
				{
					this.AddLayer(_menu);
					_menu.Close();
					_menu = null;
				}

				DrawPanel(64, 86, 124, 41);
				this.DrawText($"{char.ToLower(_driveLetter)}:CIVIL{_gameId}.SVE", 0, 5, 75, 91)
					.DrawText($"{Common.DifficultyName(Game.Difficulty)} {Game.HumanPlayer.LeaderName}", 0, 5, 75, 99)
					.DrawText($"{Game.HumanPlayer.TribeNamePlural}/{Game.GameYear}", 0, 5, 75, 107)
					.DrawText("... save in progress.", 0, 5, 75, 115);
				
				this.DrawText("Game has been saved.", 0, 5, 75, 132)
					.DrawText("Press key to continue.", 0, 5, 75, 140);
				return true;
			}
			else if (_menu != null)
			{
				if (_menu.Update(gameTick))
				{
					this.Clear(15);
					DrawBorder(_border);
					this.AddLayer(_menu);
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
				Log("Cancel");
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

				_menu = new Menu(Palette)
				{
					Title = "Select Save File...",
					X = 51,
					Y = 38,
					MenuWidth = 217,
					TitleColour = 12,
					ActiveColour = 11,
					TextColour = 5,
					FontId = 0,
					IndentTitle = 2,
					RowHeight = 8
				};
				
				int i = 0;
				foreach (SaveGameFile file in GetSaveGames().Take(4))
				{
					_menu.Items.Add(file.Name, i++).OnSelect(SaveFile);
				}
				
				_menu.ActiveItem = SelectedGame;
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

		public SaveGame() : this(-1)
		{
		}
		
		public SaveGame(int gameId)
		{
			Palette = Resources["SP257"].Palette;
			_gameId = gameId;
		}
	}
}