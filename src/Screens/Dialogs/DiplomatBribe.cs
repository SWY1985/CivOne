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
using CivOne.Players;

namespace CivOne.Screens.Dialogs
{
	internal class DiplomatBribe : BaseDialog
	{
		private const int FONT_ID = 0;

		private readonly BaseUnitLand _unitToBribe;
		private readonly Diplomat _diplomat;

		private int _bribeCost;

		private bool _canBribe;

		private void DontBrbe(object sender, EventArgs args)
		{
			Cancel();
		}

		private void Bribe(object sender, EventArgs args)
		{
			IUnit newUnit = Game.CreateUnit(_unitToBribe.Type, _unitToBribe.X, _unitToBribe.Y, _diplomat.Owner);
			Game.DisbandUnit(_unitToBribe);
			_diplomat.KeepMoving(newUnit);

			_diplomat.Player.Gold -= (short)_bribeCost;

			Cancel();
		}

		private static int BribeCost(BaseUnitLand unitToBribe)
		{
			City capital = unitToBribe.Player.GetCapital();

			int distance = capital == null ? 16 : unitToBribe.Tile.DistanceTo(capital);
			
			return ((unitToBribe.Player.Gold + 750) / (distance + 2)) * unitToBribe.Price;
		}

		private static bool CanBribe(BaseUnitLand unitToBribe, short gold)
		{
			return gold >= BribeCost(unitToBribe);
		}

		protected override void FirstUpdate()
		{
			int choices = _canBribe ? 2 : 0;

			if (_canBribe)
			{
				Menu menu = new Menu(Palette, Selection(3, 5 + (3 * Resources.GetFontHeight(FONT_ID)), 130, ((2 * Resources.GetFontHeight(FONT_ID)) + (choices * Resources.GetFontHeight(FONT_ID)) + 9)))
				{
					X = 103,
					Y = 110,
					MenuWidth = 130,
					ActiveColour = 11,
					TextColour = 5,
					FontId = FONT_ID
				};

				menu.Items.Add("Forget It.").OnSelect(DontBrbe);
				menu.Items.Add("Pay").OnSelect(Bribe);
				
				AddMenu(menu);
			}
		}

		private static int DialogHeight(BaseUnitLand unitToBribe, short gold)
		{
			int choices = 0;

			if (CanBribe(unitToBribe, gold))
				choices = 2;

			return (choices * Resources.GetFontHeight(FONT_ID)) + 30;
		}

		internal DiplomatBribe(BaseUnitLand unitToBribe, Diplomat diplomat) : base(100, 80, 135, DialogHeight(unitToBribe, diplomat.Player.Gold))
		{
			_unitToBribe = unitToBribe ?? throw new ArgumentNullException(nameof(unitToBribe));
			_diplomat = diplomat ?? throw new ArgumentNullException(nameof(diplomat));

			_bribeCost = BribeCost(unitToBribe);
			_canBribe = CanBribe(unitToBribe, diplomat.Player.Gold);

			DialogBox.DrawText($"{unitToBribe.Player.Civilization.Name} {unitToBribe.Name}", 0, 15, 5, 5);
			DialogBox.DrawText($"will desert for ${_bribeCost}", 0, 15, 5, 5 + Resources.GetFontHeight(FONT_ID));
			DialogBox.DrawText($"Treasury ${diplomat.Player.Gold}", 0, 15, 5, 5 + (2 * Resources.GetFontHeight(FONT_ID)));
		}
	}
}