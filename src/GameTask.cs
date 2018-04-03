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
using System.Linq;
using CivOne.Events;
using CivOne.Tasks;

namespace CivOne
{
	public abstract class GameTask : BaseInstance
	{
		private static GameTask _currentTask = null;
		private static List<GameTask> _tasks = new List<GameTask>();

		public static bool Any() => (_tasks.Count > 0);
		public static bool Any<T>() where T : GameTask => _tasks.Any(x => x is T);
		public static bool Is<T>() where T : GameTask => (_currentTask != null && _currentTask is T);
		public static bool Fast => Common.HasAttribute<Fast>(_currentTask);
		public static int Count<T>() where T : GameTask => _tasks.Count(t => t is T);

		private static void NextTask()
		{
			_currentTask = _tasks[0];
			TaskEventArgs eventArgs = new TaskEventArgs();
			Started?.Invoke(_currentTask, eventArgs);
			if (eventArgs.Aborted)
				_currentTask.EndTask();
			else
				_currentTask.Run();
		}
		
		public static bool Update()
		{
			if (_currentTask != null)
				return _currentTask.Step();
			else if (_tasks.Count == 0)
				return false;
			
			NextTask();
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

		public static void Remove<T>() where T : GameTask => _tasks.RemoveAll(x => x is T);

		private static void Finish(object sender, EventArgs args)
		{
			_tasks.Remove((sender as GameTask));
			if (!_tasks.Any())
			{
				_currentTask = null;
				return;
			}

			NextTask();
		}

		public static event TaskEventHandler Started;
		public event EventHandler Done;

		protected virtual bool Step() => false;

		public abstract void Run();

		protected void EndTask()
		{
			if (Done == null) return;
			Done(this, null);
			Done = null;
		}
	}
}