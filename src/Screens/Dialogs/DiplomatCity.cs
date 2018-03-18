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
using CivOne.Graphics;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.UserInterface;
using CivOne.Buildings;

namespace CivOne.Screens.Dialogs
{
	internal class DiplomatCity : BaseDialog
	{
		private const int FONT_ID = 0;

		private readonly City _enemyCity;
		private readonly Diplomat _diplomat;

		private void EstablishEmbassy(object sender, EventArgs args)
		{
			Cancel();
		}

		private void InvestigateCity(object sender, EventArgs args)
		{
			Cancel();
		}

		private void InciteRevolt(object sender, EventArgs args)
		{
			GameTask.Enqueue(Tasks.Show.DiplomatIncite(_enemyCity, _diplomat));
			Cancel();
		}

		private void IndustrialSabotage(object sender, EventArgs args)
		{
			Cancel();
		}

		private void MeetWithKing(object sender, EventArgs args)
		{
			Cancel();
		}

		private void StealTechnology(object sender, EventArgs args)
		{
			Cancel();
		}

		protected override void FirstUpdate()
		{
			int choices = 6;

			Menu menu = new Menu(Palette, Selection(3, 5 + (2 * Resources.GetFontHeight(FONT_ID)), 130, ((2 * Resources.GetFontHeight(FONT_ID)) + (choices * Resources.GetFontHeight(FONT_ID)) + 9)))
			{
				X = 103,
				Y = 100,
				MenuWidth = 130,
				ActiveColour = 11,
				TextColour = 5,
				FontId = FONT_ID
			};

			menu.Items.Add("Establish Embassy").OnSelect(EstablishEmbassy).Disable();
			menu.Items.Add("InvestigateCity").OnSelect(InvestigateCity).Disable();
			menu.Items.Add("Steal Technology").OnSelect(StealTechnology).Disable();
			menu.Items.Add("Industrial Sabotage").OnSelect(IndustrialSabotage).Disable();
			menu.Items.Add("Incite a Revolt").OnSelect(InciteRevolt);
			menu.Items.Add("Meet with King").OnSelect(MeetWithKing).Disable();
			
			AddMenu(menu);
		}

		private static int DialogHeight()
		{
			int choices = 6;

			return (choices * Resources.GetFontHeight(FONT_ID)) + 30;
		}

		internal DiplomatCity(City enemyCity, Diplomat diplomat) : base(100, 80, 155, DialogHeight())
		{
			_enemyCity = enemyCity ?? throw new ArgumentNullException(nameof(enemyCity));
			_diplomat = diplomat ?? throw new ArgumentNullException(nameof(diplomat));

			DialogBox.DrawText($"{_enemyCity.Player.TribeName} diplomat arrives", 0, 15, 5, 5);
			DialogBox.DrawText($"in {_enemyCity.Name}", 0, 15, 5, 5 + Resources.GetFontHeight(FONT_ID));
		}
	}
}