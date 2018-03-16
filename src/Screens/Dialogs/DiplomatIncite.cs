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
	internal class DiplomatIncite : BaseDialog
	{
		private const int FONT_ID = 0;

		private readonly City _cityToIncite;
		private readonly Diplomat _diplomat;

		private int _inciteCost;

		private bool _canIncite;

		private void DontIncite(object sender, EventArgs args)
		{
			Cancel();
		}

		private void Incite(object sender, EventArgs args)
		{
			Player previousOwner = Game.GetPlayer(_cityToIncite.Owner);

			Game.DisbandUnit(_diplomat);
			_cityToIncite.Owner = _diplomat.Owner;

			previousOwner.IsDestroyed();

			if (Human == _cityToIncite.Owner || Human == _diplomat.Owner)
			{
				GameTask.Insert(Tasks.Show.CityManager(_cityToIncite));
			}

			Cancel();
		}

		private static int InciteCost(City cityToIncite)
		{
			City capital = cityToIncite.Player.Cities.Where(c => c.HasBuilding(new Palace())).FirstOrDefault();

			int distance = capital == null ? 16 : cityToIncite.Tile.DistanceTo(capital);
			
			int cost = (cityToIncite.Player.Gold + 1000) / (distance + 3);

			// todo: if city is in disorder need to halve the cost
			return cost;
		}

		private static bool CanIncite(City cityToIncice, short gold)
		{
			return gold >= InciteCost(cityToIncice);
		}

		protected override void FirstUpdate()
		{
			int choices = _canIncite ? 2 : 0;

			if (_canIncite)
			{
				Menu menu = new Menu(Palette, Selection(3, 12, 130, ((2 * Resources.GetFontHeight(FONT_ID)) + (choices * Resources.GetFontHeight(FONT_ID)) + 9)))
				{
					X = 143,
					Y = 110,
					MenuWidth = 130,
					ActiveColour = 11,
					TextColour = 5,
					FontId = FONT_ID
				};

				menu.Items.Add("Forget It.").OnSelect(DontIncite);
				menu.Items.Add("Incite revolt").OnSelect(Incite);
				
				AddMenu(menu);
			}
		}

		internal DiplomatIncite(City cityToIncite, Diplomat diplomat) : base(100, 80, 180, 56)
		{
			_cityToIncite = cityToIncite ?? throw new ArgumentNullException(nameof(cityToIncite));
			_diplomat = diplomat ?? throw new ArgumentNullException(nameof(diplomat));

			IBitmap spyPortrait = Icons.Spy;

			Palette palette = Common.DefaultPalette;
			for (int i = 144; i < 256; i++)
			{
				palette[i] = spyPortrait.Palette[i];
			}
			this.SetPalette(palette);

			DialogBox.AddLayer(spyPortrait, 2, 2);

			_inciteCost = InciteCost(cityToIncite);
			_canIncite = CanIncite(cityToIncite, diplomat.Player.Gold);

			DialogBox.DrawText($"Spies Report", 0, 15, 45, 5);
			DialogBox.DrawText($"Dissidents in {_cityToIncite.Name}", 0, 15, 45, 5 + Resources.GetFontHeight(FONT_ID));
			DialogBox.DrawText($"will revolt for ${_inciteCost}", 0, 15, 45, 5 + (2 * Resources.GetFontHeight(FONT_ID)));
		}
	}
}