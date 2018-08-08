// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Screens;
using CivOne.Units;

namespace CivOne.Tasks
{
	[Fast]
	internal class Turn : GameTask
	{
		private const int TURN_TIME = 10;

		private ITurn _turnObject = null;
		private IUnit _unit = null;
		private bool _endTurn = false;

		private Player _gameOver = null;

		private int _step = 0;

		protected override bool Step()
		{
			if (_unit != null)
			{
				Game.CurrentPlayer.AI.Move(_unit);
				EndTask();
			}
			if (_endTurn && _step-- <= 0)
			{
				Game.EndTurn();
				EndTask();
			}
			return true;
		}

		public override void Run()
		{
			if (_turnObject != null)
			{
				_turnObject.NewTurn();
			}
			else if (_unit != null)
			{
				return;
			}
			else if (_endTurn)
			{
				if (Game.CurrentPlayer.IsHuman)
				{
					_step = TURN_TIME;
					return;
				}
				Game.EndTurn();
				EndTask();
				return;
			}
			else if (_gameOver != null)
			{
				if (_gameOver.IsHuman)
				{
					Common.AddScreen(new GameOver());
				}
				else
				{
					// TODO: Spawn barbarians or respawn civilization
				}
			}
			EndTask();
			return;
		}

		public static Turn New(ITurn turnObject)
		{
			return new Turn()
			{
				_turnObject = turnObject
			};
		}

		public static Turn Move(IUnit unit)
		{
			return new Turn()
			{
				_unit = unit
			};
		}

		public static Turn End()
		{
			return new Turn()
			{
				_endTurn = true
			};
		}

		public static Turn GameOver(Player player)
		{
			return new Turn()
			{
				_gameOver = player
			};
		}

		private Turn()
		{
			
		}
	}
}