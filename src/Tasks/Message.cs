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
		private readonly IScreen _screen;
		/*
		private readonly string _title;
		private readonly string[] _message;
		private readonly bool _newspaper;
		private bool _showGovernment;*/

		private void ClosedMessage(object sender, EventArgs args)
		{
			EndTask();
		}

		public override void Run()
		{
			//TribalHut tribalHut = new TribalHut(_message);
			//tribalHut.Closed += ClosedMessage;
			//Common.AddScreen(tribalHut);
			_screen.Closed += ClosedMessage;
			Common.AddScreen(_screen);
		}
/*
		private static Message Newspaper(bool goverment, string city, string[] message)
		{
			return new Message(city, message, newspaper: true)
			{
				_showGovernment = 
			};
		}*/

		public static Message NewGoverment(City city, params string[] message)
		{
			return new Message(new Newspaper(city, message, showGovernment: true));
			//return Newspaper(true, city, message);
		}

		public static Message Newspaper(City city, params string[] message)
		{
			return new Message(new Newspaper(city, message, showGovernment: false));
			//return Newspaper(false, city, message);
		}

		public static Message TribalHut(params string[] message)
		{
			//return new Message(null, message);
			return new Message(new TribalHut(message));
		}

		private Message(IScreen screen)
		{
			_screen = screen;
		}
/*
		private Message(string title, string[] message, bool newspaper = false)
		{
			_title = title;
			_message = message;
		}*/
	}
}