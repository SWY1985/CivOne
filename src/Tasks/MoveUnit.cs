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
using CivOne.Screens;

namespace CivOne.Tasks
{
	public class MoveUnit : GameTask
	{
		private const int STEP_SIZE = 2;

		public readonly int RelX, RelY;

		private int _step = 0;

		//public event EventHandler Moved;

		public int X { get; private set; }
		public int Y { get; private set; }

		protected override void Step()
		{
			_step += STEP_SIZE;
			X = (RelX * _step);
			Y = (RelY * _step);
			if (_step < 16)
				return;
			EndTask();
		}

		public override void Run()
		{
			// Off screen? End task immediately
			//EndTask();
		}

		public MoveUnit(int relX, int relY)
		{
			RelX = relX;
			RelY = relY;
		}
	}
}