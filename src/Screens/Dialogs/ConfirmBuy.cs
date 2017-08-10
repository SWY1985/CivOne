// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;

namespace CivOne.Screens.Dialogs
{
	internal class ConfirmBuy : BaseDialog
	{
		public event EventHandler Buy;

		private void MenuYes(object sender, EventArgs args)
		{
			if (Buy != null)
				Buy(this, args);
			Cancel();
		}

		protected override void FirstUpdate()
		{
			Menu menu = new Menu(Palette, Selection(3, 28, TextWidth + 5, 20))
			{
				X = 103,
				Y = 108,
				Width = TextWidth + 5,
				ActiveColour = 11,
				TextColour = 5,
				FontId = 0
			};
			int i = 0;
			foreach (string choice in new [] { "Yes", "No" })
			{
				menu.Items.Add(choice, i++);
			}
			menu.Items[0].Selected += MenuYes;
			menu.Items[1].Selected += Cancel;

			menu.MissClick += Cancel;
			menu.Cancel += Cancel;
			AddMenu(menu);
		}

		public ConfirmBuy(string name, short price, short treasury) : base(100, 80, 9, 23, new string[] { "Cost to complete", $"{name}: ${price}", $"Treasury: ${treasury}" })
		{
			for (int i = 0; i < TextLines.Length; i++)
			{
				DialogBox.AddLayer(TextLines[i], 5, (TextLines[i].Height * i) + 5);
			}
		}
	}
}