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

		private readonly Action _action = null;
		private readonly Func<bool> _stepAction = null;

		protected override bool Step()
		{
			if (_stepAction != null && _stepAction())
			{
				EndTask();
			}
			return true;
		}

		public override void Run()
		{
			if (_action != null)
			{
				_action();
				EndTask();
			}
			return;
		}

		public static Turn New(ITurn turnObject) => new Turn(() => turnObject.NewTurn());

		public static Turn Move(IUnit unit) => new Turn(() =>
		{
			AI.Instance(Game.CurrentPlayer).Move(unit);
			return true;
		});

		public static Turn End()
		{
			int step = (Game.CurrentPlayer is HumanPlayer) ? TURN_TIME : 0;
			return new Turn(() =>
			{
				if (step-- > 0) return false;
				Game.EndTurn();
				return true;
			});
		}

		public static Turn GameOver(IPlayer player) => new Turn(() =>
		{
			switch (player)
			{
				case HumanPlayer _:
					Common.AddScreen(new GameOver());
					return;
				default:
					// TODO: Respawn civilization
					return;
			};
		});

		public static Turn HandleAnarchy(IPlayer player) => new Turn(() =>
		{
			// When anarchy is over (GameTurn is dividable by 4 or Pyramids are in effect), choose a new government
			if (!player.HasGovernment<Anarchy>()) return;
			if (Game.GameTurn % 4 != 0 && !player.HasWonder<Pyramids>(true)) return;
			
			player.ChooseGovernment();
		});

		private Turn(Action action = null) => _action = action;

		private Turn(Func<bool> stepAction) => _stepAction = stepAction;
	}
}