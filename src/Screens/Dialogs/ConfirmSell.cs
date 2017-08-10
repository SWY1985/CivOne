// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Buildings;
using CivOne.Graphics;

namespace CivOne.Screens.Dialogs
{
	internal class ConfirmSell : BaseDialog
	{
		public IBuilding Building { get; private set; }

		public event EventHandler Sell;

		private void MenuYes(object sender, EventArgs args)
		{
			if (Sell != null)
				Sell(this, args);
			Cancel();
		}

		protected override void FirstUpdate()
		{
			Menu menu = new Menu(Palette, Selection(3, 20, TextWidth + 5, 20))
			{
				X = 131,
				Y = 100,
				Width = TextWidth + 5,
				ActiveColour = 11,
				TextColour = 5,
				FontId = 0
			};
			int i = 0;
			foreach (string choice in new [] { "No.", "Yes." })
			{
				menu.Items.Add(choice, i++);
			}
			menu.Items[0].Selected += Cancel;
			menu.Items[1].Selected += MenuYes;

			menu.MissClick += Cancel;
			menu.Cancel += Cancel;
			AddMenu(menu);
		}

		public ConfirmSell(IBuilding building) : base(128, 80, 9, 23, new string[] { "Do you want to sell", $"your {building.Name} for {building.SellPrice}$?" })
		{
			Building = building;
			
			for (int i = 0; i < TextLines.Length; i++)
			{
				DialogBox.AddLayer(TextLines[i], 5, (TextLines[i].Height * i) + 5);
			}
		}
	}
}