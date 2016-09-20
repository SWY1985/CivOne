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
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne.Tasks
{
	internal class Show : GameTask
	{
		private readonly IScreen _screen;

		public void Closed(object sender, EventArgs args)
		{
			EndTask();
		}

		public override void Run()
		{
			_screen.Closed += Closed;
			Common.AddScreen(_screen);
		}

		public static Show Empty
		{
			get
			{
				return new Show(Overlay.Empty);
			}
		}

		public static Show Terrain
		{
			get
			{
				GamePlay gamePlay = (GamePlay)Common.Screens.First(s => (s is GamePlay));
				return new Show(Overlay.Terrain(gamePlay.X, gamePlay.Y));
			}
		}

		public static Show Goto
		{
			get
			{
				GamePlay gamePlay = (GamePlay)Common.Screens.First(s => (s is GamePlay));
				return new Show(new Goto(gamePlay.X, gamePlay.Y));
			}
		}

		public static Show Options
		{
			get
			{
				return new Show(new GameOptions());
			}
		}

		public static Show UnitStack(int x, int y)
		{
			return new Show(new UnitStack(x, y));
		}

		private Show(IScreen screen)
		{
			_screen = screen;
		}
	}
}