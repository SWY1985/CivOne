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
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens.Dialogs
{
	internal class ChooseGovernment : BaseDialog
	{
		private readonly IGovernment[] _availableGovernments;

		public IGovernment Result { get; private set; }

		private void GovernmentChoice(object sender, EventArgs args)
		{
			Result = _availableGovernments[(sender as Menu.Item).Value];
			CloseMenus();
			Cancel();
		}

		protected override void FirstUpdate()
		{
			Menu menu = new Menu(Canvas.Palette, Selection(3, 20, 84, (_availableGovernments.Length * Resources.Instance.GetFontHeight(0))))
			{
				X = 103,
				Y = 84,
				Width = 82,
				ActiveColour = 11,
				TextColour = 5,
				FontId = 0
			};
			Menu.Item menuItem;
			for (int i = 0; i < _availableGovernments.Length; i++)
			{
				menu.Items.Add(menuItem = new Menu.Item($"{_availableGovernments[i].NameAdjective}", i));
				menuItem.Selected += GovernmentChoice;
			}
			AddMenu(menu);
		}

		private static int DialogHeight
		{
			get
			{
				return (Game.HumanPlayer.AvailableGovernments.Count() * Resources.Instance.GetFontHeight(0)) + 23;
			}
		}

		public ChooseGovernment() : base(100, 64, 86, DialogHeight)
		{
			_availableGovernments = Game.HumanPlayer.AvailableGovernments.ToArray();

			DialogBox.DrawText("Select type of", 0, 15, 5, 5);
			DialogBox.DrawText("Government...", 0, 15, 5, 13);
		}
	}
}