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
using System.Threading;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne.Tasks
{
	internal class Turn : GameTask, IFast
	{
		private ITurn _turnObject = null;
		private IUnit _unit = null;
		private bool _endTurn = false;

		public override void Run()
		{
			if (_turnObject != null)
			{
				_turnObject.NewTurn();
			}
			else if (_unit != null)
			{
				AI.Move(_unit);
			}
			else if (_endTurn)
			{
				Game.Instance.EndTurn();
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

		private Turn()
		{
			
		}
	}
}