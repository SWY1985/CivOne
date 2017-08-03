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
using CivOne.GFX;
using CivOne.Templates;
using CivOne.Units;
using CivOne.UserInterface;

namespace CivOne.Screens.Dialogs
{
	internal class CaravanChoice : BaseDialog
	{
		private const int FONT_ID = 0;

		private readonly Caravan _unit;
		private readonly City _city;

		private void KeepMoving(object sender, EventArgs args)
		{
			_unit.KeepMoving(_city);
			Cancel();
		}

		private void EstablishTradeRoute(object sender, EventArgs args)
		{
			_unit.EstablishTradeRoute(_city);
			Cancel();
		}

		private void HelpBuildWonder(object sender, EventArgs args)
		{
			_unit.HelpBuildWonder(_city);
			Cancel();
		}

		private bool AllowEstablishTradeRoute => (_unit.Home == null) || (_unit.Home.Tile.DistanceTo(_city.Tile) >= 10);

		protected override void FirstUpdate()
		{
			int choices = 2;
			if (_city.CurrentProduction is IWonder) choices++;

			Menu menu = new Menu(Canvas.Palette, Selection(3, 12, 130, (choices * Resources.Instance.GetFontHeight(FONT_ID)) + 4))
			{
				X = 103,
				Y = 92,
				Width = 130,
				ActiveColour = 11,
				TextColour = 5,
				FontId = FONT_ID
			};

			menu.Items.Add("Keep moving").OnSelect(KeepMoving);
			menu.Items.Add("Establish trade route").OnSelect(EstablishTradeRoute).SetEnabled(AllowEstablishTradeRoute);

			if (_city.CurrentProduction is IWonder)
			{
				menu.Items.Add("Help build WONDER.").OnSelect(HelpBuildWonder);
			}
			
			AddMenu(menu);
		}

		private static int DialogHeight(Caravan unit, City city)
		{
			int choices = 2;
			if (city.CurrentProduction is IWonder) choices++;
			return (choices * Resources.Instance.GetFontHeight(FONT_ID)) + 17;
		}

		internal CaravanChoice(Caravan unit, City city) : base(100, 80, 136, DialogHeight(unit, city))
		{
			_city = city;
			_unit = unit;

			DialogBox.DrawText($"Will you?", 0, 15, 5, 5);
		}
	}
}