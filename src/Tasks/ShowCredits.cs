// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Screens;

namespace CivOne.Tasks
{
	internal class CreditsScreen : GameTask
	{
		public void Closed(object sender, EventArgs args) => EndTask();

		public override void Run()
		{
			IScreen screen = new Credits();
			screen.Closed += Closed;
			Common.AddScreen(screen);
		}

		public static CreditsScreen Show() => new CreditsScreen();

		private CreditsScreen()
		{
		}
	}
}