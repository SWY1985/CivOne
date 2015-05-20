// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;

namespace CivOne
{
	internal class Game
	{
		private static Game _instance;
		public static Game Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Game();
				return _instance;
			}
		}
		
		private Game()
		{
			Console.WriteLine("Game instance created");
		}
	}
}