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
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne.Tasks
{
	internal class Message : GameTask
	{
		private readonly IScreen _screen;

		private void ClosedMessage(object sender, EventArgs args)
		{
			EndTask();
		}

		public override void Run()
		{
			_screen.Closed += ClosedMessage;
			Common.AddScreen(_screen);
		}

		public static Message Advisor(Advisor advisor, params string[] message)
		{
			return new Message(new AdvisorMessage(advisor, message));
		}

		public static Message NewGoverment(City city, params string[] message)
		{
			return new Message(new Newspaper(city, message, showGovernment: true));
		}

		public static Message Newspaper(City city, params string[] message)
		{
			return new Message(new Newspaper(city, message, showGovernment: false));
		}

		public static Message TribalHut(params string[] message)
		{
			return new Message(new TribalHut(message));
		}

		public static Message Help(string title, params string[] message)
		{
			return new Message(new PopupMessage(2, title, message));
		}

		public static Message Error(string title, params string[] message)
		{
			return new Message(new PopupMessage(4, title, message));
		}

		private Message(IScreen screen)
		{
			_screen = screen;
		}
	}
}