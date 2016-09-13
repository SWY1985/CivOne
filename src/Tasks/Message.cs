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
	internal class Message : GameTask
	{
		private readonly string[] _message;

		private void ClosedMessage(object sender, EventArgs args)
		{
			EndTask();
		}

		public override void Run()
		{
			TribalHut tribalHut = new TribalHut(_message);
			tribalHut.Closed += ClosedMessage;
			Common.AddScreen(tribalHut);
		}

		public static Message TribalHut(params string[] message)
		{
			return new Message(message);
		}

		private Message(string[] message)
		{
			// Tribal hut message (TODO: Make this more general)
			_message = message;
		}
	}
}