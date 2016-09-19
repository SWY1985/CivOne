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
	internal class Show : GameTask
	{
		private readonly IScreen _screen;

		public void Closed(object sender, EventArgs args)
		{
			EndTask();
		}

		public override void Run()
		{
			_screen.Closed += Closed;
			Common.AddScreen(_screen);
		}

		public static Show Empty
		{
			get
			{
				return new Show(Overlay.Empty);
			}
		}

		private Show(IScreen screen)
		{
			_screen = screen;
		}
	}
}