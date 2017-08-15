// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Graphics;

namespace CivOne.Screens.Dialogs
{
	internal class ConfirmQuit : BaseDialog
	{
		private void MenuQuit(object sender, EventArgs args)
		{
			Runtime.Quit();
			Cancel();
		}

		protected override void FirstUpdate()
		{
			Menu menu = new Menu(Palette, Selection(3, 20, 100, 16))
			{
				X = 103,
				Y = 100,
				MenuWidth = 100,
				ActiveColour = 11,
				TextColour = 5,
				FontId = 0
			};
			foreach (string choice in new [] { "Keep Playing", "Yes, Quit" })
			{
				menu.Items.Add(choice);
			}
			menu.Items[0].Selected += Cancel;
			menu.Items[1].Selected += MenuQuit;

			menu.MissClick += Cancel;
			menu.Cancel += Cancel;
			AddMenu(menu);
		}

		public ConfirmQuit() : base(100, 80, 104, 39)
		{
			DialogBox.DrawText("Are you sure you", 0, 15, 5, 5);
			DialogBox.DrawText("want to Quit?", 0, 15, 5, 13);
		}
	}
}