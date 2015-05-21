// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Interfaces;

namespace CivOne
{
	internal class Game
	{
		public static void CreateGame(int difficulty, int competition, ICivilization tribe)
		{
			if (_instance == null)
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
			Console.WriteLine("Game instance created");
		}
	}
}