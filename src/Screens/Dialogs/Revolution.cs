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
using CivOne.Players;
using CivOne.Tasks;

namespace CivOne.Screens.Dialogs
{
	internal class Revolution : BaseDialog
	{
		private void MenuRevolution(object sender, EventArgs args)
		{
			Human.Revolt();
			GameTask.Enqueue(Message.Newspaper(null, $"The {Human.Civilization.NamePlural} are", "revolting! Citizens", "demand new govt."));
			Cancel();
		}

		protected override void FirstUpdate()
		{
			Menu menu = new Menu(Palette, Selection(3, 12, 228, 16))
			{
				X = 67,
				Y = 92,
				MenuWidth = 227,
				ActiveColour = 11,
				TextColour = 5,
				FontId = 0,
				Indent = 2
			};
			int i = 0;
			foreach (string choice in new [] { "_No thanks.", "_Yes, we need a new government." })
			{
				menu.Items.Add(choice, i++);
			}
			menu.Items[0].Selected += Cancel;
			menu.Items[1].Selected += MenuRevolution;

			menu.MissClick += Cancel;
			menu.Cancel += Cancel;
			AddMenu(menu);
		}

		public Revolution() : base(64, 80, 231, 31)
		{
			DialogBox.DrawText("Are you sure you want a REVOLUTION?", 0, 15, 5, 5);
		}
	}
}