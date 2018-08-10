// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Governments;
using CivOne.Players;
using CivOne.Screens;
using CivOne.Units;
using CivOne.Wonders;

namespace CivOne.Tasks
{
	[Fast]
	internal class Turn : GameTask
	{
		private const int TURN_TIME = 10;

		private Action _action = null;
		private ITurn _turnObject = null;
		private IUnit _unit = null;
		private bool _endTurn = false;

		private IPlayer _gameOver = null;

		private int _step = 0;

		protected override bool Step()
		{
			if (_unit != null)
			{
				AI.Instance(Game.CurrentPlayer).Move(_unit);
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
				if (Game.CurrentPlayer is HumanPlayer)
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
				if (_gameOver is HumanPlayer)
				{
					Common.AddScreen(new GameOver());
				}
				else
				{
					// TODO: Spawn barbarians or respawn civilization
				}
			}
			else if (_action != null)
			{
				_action();
			}
			EndTask();
			return;
		}

		public static Turn New(ITurn turnObject) => new Turn()
		{
			_turnObject = turnObject
		};

		public static Turn Move(IUnit unit) => new Turn()
		{
			_unit = unit
		};

		public static Turn End() => new Turn()
		{
			_endTurn = true
		};

		public static Turn GameOver(IPlayer player) => new Turn()
		{
			_gameOver = player
		};

		public static Turn HandleAnarchy(IPlayer player) => new Turn(() =>
		{
			// When anarchy is over (GameTurn is dividable by 4 or Pyramids are in effect), choose a new government
			if (!player.HasGovernment<Anarchy>()) return;
			if (Game.GameTurn % 4 != 0 && !player.HasWonder<Pyramids>(true)) return;
			
			player.ChooseGovernment();
		});

		private Turn(Action action = null)
		{
			_action = action;
		}
	}
}