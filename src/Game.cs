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
using CivOne.Civilizations;
using CivOne.Interfaces;

namespace CivOne
{
	internal class Game
	{
		private readonly int _difficulty, _competition;
		private readonly Player[] _players;
		private readonly List<City> _cities;
		
		internal ushort GameTurn { get; private set; }
		
		internal string GameYear
		{
			get
			{
				return Common.YearString(GameTurn);
			}
		}
		
		internal Player HumanPlayer
		{
			get;
			private set;
		}
		
		public City GetCity(int x, int y)
		{
			while (x < 0) x += Map.WIDTH;
			while (x >= Map.WIDTH) x-= Map.WIDTH;
			if (y < 0) return null;
			if (y >= Map.HEIGHT) return null; 
			return _cities.Where(c => c.X == x && c.Y == y && c.Size > 0).FirstOrDefault();
		}
		
		public static void CreateGame(int difficulty, int competition, ICivilization tribe, string leaderName = null, string tribeName = null, string tribeNamePlural = null)
		{
			if (_instance != null)
			{
				Console.WriteLine("ERROR: Game instance already exists");
				return;
			}
			_instance = new Game(difficulty, competition, tribe, leaderName, tribeName, tribeNamePlural);
		}
		
		public static void LoadGame(string sveFile, string mapFile)
		{
			if (_instance != null)
			{
				Console.WriteLine("ERROR: Game instance already exists");
				return;
			}
			
			// TODO: Implement full save file configuration
			// - http://forums.civfanatics.com/showthread.php?p=12422448
			// - http://forums.civfanatics.com/showthread.php?t=493581
			using (BinaryReader br = new BinaryReader(File.Open(sveFile, FileMode.Open)))
			{
				ushort humanPlayer = Common.BinaryReadUShort(br, 2);
				ushort randomSeed = Common.BinaryReadUShort(br, 6);
				ushort difficulty = Common.BinaryReadUShort(br, 10);
				string[] leaderNames = Common.BinaryReadStrings(br, 16, 112, 14);
				string[] tribeNamesPlural = Common.BinaryReadStrings(br, 128, 96, 12);
				string[] tribeNames = Common.BinaryReadStrings(br, 224, 88, 11);
				string[] cityNames = Common.BinaryReadStrings(br, 26992, 3328, 13);
				int cc = 0;
				List<City> cities = new List<City>();
				for (int i = 5384; i < 8968; i+= 28)
				{
					byte x = Common.BinaryReadByte(br, i + 4);
					byte y = Common.BinaryReadByte(br, i + 5);
					byte actualSize = Common.BinaryReadByte(br, i + 7);
					byte owner = Common.BinaryReadByte(br, i + 11);
					byte nameId = Common.BinaryReadByte(br, i + 22);
					string name = cityNames[nameId];
					
					if (x == 0 && y == 0 && actualSize == 0 && owner == 0 && nameId == 0) continue;
					
					City city = new City()
					{
						X = x,
						Y = y,
						Owner = owner,
						Name = name,
						Size = actualSize
					};
					cities.Add(city);
				}
				ushort competition = (ushort)(Common.BinaryReadUShort(br, 37820) + 1);
				ushort civIdentity = Common.BinaryReadUShort(br, 37854);
				
				Map.Instance.LoadMap(mapFile, randomSeed);
				_instance = new Game(difficulty, competition);
				Console.WriteLine("Game instance loaded (difficulty: {0}, competition: {1})", difficulty, competition);
				
				for (int i = 0; i <= competition; i++)
				{
					int identity = ((civIdentity >> i) & 0x1);
					ICivilization civ = Common.Civilizations.Where(c => c.PreferredPlayerNumber == i).ToArray()[identity];
					_instance._players[i] = new Player(civ, leaderNames[i], tribeNames[i], tribeNamesPlural[i]);
					_instance._players[i].Gold = (short)Common.BinaryReadUShort(br, 312 + (i * 2));
					
					Console.WriteLine("- Player {0} is {1} of the {2}{3}", i, _instance._players[i].LeaderName, _instance._players[i].TribeNamePlural, (i == humanPlayer) ? " (human)" : "");
				}
				_instance.GameTurn = Common.BinaryReadUShort(br, 0);
				_instance.HumanPlayer = _instance._players[humanPlayer];
				
				foreach (City city in cities)
				{
					_instance._cities.Add(city);
				}
			}
		}
		
		private static Game _instance;
		public static Game Instance
		{
			get
			{
				if (_instance == null)
				{
					Console.WriteLine("ERROR: Game instance does not exist");
				}
				return _instance;
			}
		}
		
		private Game(int difficulty, int competition)
		{
			_difficulty = difficulty;
			_competition = competition;
			_players = new Player[competition + 1];
			_cities = new List<City>();
		}
		
		private Game(int difficulty, int competition, ICivilization tribe, string leaderName, string tribeName, string tribeNamePlural)
		{
			_difficulty = difficulty;
			_competition = competition;
			Console.WriteLine("Game instance created (difficulty: {0}, competition: {1})", _difficulty, _competition);
			
			_players = new Player[competition + 1];
			for (int i = 0; i <= competition; i++)
			{
				if (i == tribe.PreferredPlayerNumber)
				{
					_players[i] = new Player(tribe, leaderName, tribeName, tribeNamePlural);
					HumanPlayer = _players[i];
					Console.WriteLine("- Player {0} is {1} of the {2} (human)", i, _players[i].LeaderName, _players[i].TribeNamePlural);
					continue;
				}
				
				ICivilization[] civs = Common.Civilizations.Where(civ => civ.PreferredPlayerNumber == i).ToArray();
				int r = Common.Random.Next(civs.Length);
				
				_players[i] = new Player(civs[r]);
				Console.WriteLine("- Player {0} is {1} of the {2}", i, _players[i].LeaderName, _players[i].TribeNamePlural);
			}
			
			_cities = new List<City>();
		}
	}
}