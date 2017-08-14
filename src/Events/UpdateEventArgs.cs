// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Enums;

namespace CivOne.Events
{
	public delegate void UpdateEventHandler(object sender, UpdateEventArgs args);

	public class UpdateEventArgs : EventArgs
	{
		public bool HasUpdate { get; internal set; }

		public static new UpdateEventArgs Empty => new UpdateEventArgs();
		
		private UpdateEventArgs()
		{
		}
	}
}