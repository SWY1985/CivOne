// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.IO;
using CivOne.Screens;
using CivOne.Tasks;

namespace CivOne.Templates
{
	internal abstract class BaseUnitAir : BaseUnit
	{
		public int TotalFuel { get; protected set; }
		public int FuelLeft { get; protected set; }

		private void HandleFuel()
		{
			if (Map[X, Y].City != null)
			{
				MovesLeft = 0;
				FuelLeft = TotalFuel;
				return;
			}
			if (MovesLeft > 0 || FuelLeft > 0) return;
			
			// Air unit is out of fuel
			Game.Instance.DisbandUnit(this);
			GameTask.Enqueue(Message.Error("-- Civilization Note --", TextFile.Instance.GetGameText("ERROR/FUEL")));
		}

		protected override void MovementDone(ITile previousTile)
		{
			base.MovementDone(previousTile);
			
			FuelLeft--;
			HandleFuel();
		}

		public override void SkipTurn()
		{
			MovesLeft = 0;
			FuelLeft -= (FuelLeft % Move);
			HandleFuel();
		}
		
		public override IEnumerable<GameMenu.Item> MenuItems
		{
			get
			{
				yield return MenuNoOrders();
				yield return MenuWait();
				yield return MenuSentry();
				yield return MenuGoTo();
				if (Map[X, Y].Irrigation || Map[X, Y].Mine || Map[X, Y].Road || Map[X, Y].RailRoad)
				{
					yield return MenuPillage();
				}
				if (Map[X, Y].City != null)
				{
					yield return MenuHomeCity();
				}
				yield return new GameMenu.Item(null);
				yield return MenuDisbandUnit();
			}
		}

		protected override bool ValidMoveTarget(ITile tile)
		{
			return (tile != null);
		}

		protected BaseUnitAir(byte price = 1, byte attack = 1, byte defense = 1, byte move = 1) : base(price, attack, defense, move)
		{
			Class = UnitClass.Air;
			TotalFuel = move;
			FuelLeft = move;
		}
	}
}