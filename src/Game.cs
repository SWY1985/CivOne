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
using CivOne.Civilizations;
using CivOne.Interfaces;

namespace CivOne
{
	internal class Game
	{
		private readonly int _difficulty, _competition;
		private readonly Player[] _players;
		
		internal Player HumanPlayer
		{
			get;
			private set;
		}
		
		public static void CreateGame(int difficulty, int competition, ICivilization tribe)
		{
			if (_instance != null)
			{
				Console.WriteLine("ERROR: Game instance already exists");
				return;
			}
			_instance = new Game(difficulty, competition, tribe);
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
		
		private Game(int difficulty, int competition, ICivilization tribe)
		{
			_difficulty = difficulty;
			_competition = competition;
			Console.WriteLine("Game instance created (difficulty: {0}, competition: {1})", _difficulty, _competition);
			
			_players = new Player[competition + 1];
			for (int i = 0; i <= competition; i++)
			{
				if (i == tribe.PreferredPlayerNumber)
				{
					_players[i] = new Player(tribe);
					HumanPlayer = _players[i];
					Console.WriteLine("- Player {0} is {1} of the {2} (human)", i, _players[i].Civilization.LeaderName, _players[i].Civilization.NamePlural);
					continue;
				}
				
				ICivilization[] civs = Common.Civilizations.Where(civ => civ.PreferredPlayerNumber == i).ToArray();
				int r = Common.Random.Next(civs.Length);
				
				_players[i] = new Player(civs[r]);
				Console.WriteLine("- Player {0} is {1} of the {2}", i, _players[i].Civilization.LeaderName, _players[i].Civilization.NamePlural);
			}
		}
	}
}