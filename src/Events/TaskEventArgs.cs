// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Tasks;

namespace CivOne.Events
{
	public delegate void TaskEventHandler(object sender, TaskEventArgs args);

	public class TaskEventArgs : EventArgs
	{
		public bool Aborted { get; private set; }

		public void Abort()
		{
			Aborted = true;
		}
		
		public TaskEventArgs()
		{
		}
	}
}