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

namespace CivOne
{
	public abstract class GameTask
	{
		private static GameTask _currentTask = null;
		private static List<GameTask> _tasks = new List<GameTask>();

		public static bool Any()
		{
			return (_tasks.Count > 0);
		}

		public static bool Update()
		{
			if (_currentTask != null)
				return _currentTask.Step();
			else if (_tasks.Count == 0)
				return false;
			
			(_currentTask = _tasks[0]).Run();
			return true;
		}

		public static void Enqueue(GameTask task)
		{
			task.Done += Finish;
			_tasks.Add(task);
		}

		public static void Insert(GameTask task)
		{
			task.Done += Finish;
			_tasks.Insert(0, task);
		}

		private static void Finish(object sender, EventArgs args)
		{
			_tasks.Remove((sender as GameTask));
			_currentTask = null;
		}

		public event EventHandler Done;

		protected virtual bool Step()
		{
			return false;
		}

		public abstract void Run();

		protected void EndTask()
		{
			if (Done == null) return;
			Done(this, null);
		}
	}
}