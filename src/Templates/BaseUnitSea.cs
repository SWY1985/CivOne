// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Linq;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.Screens;
using CivOne.UserInterface;
using CivOne.Wonders;

namespace CivOne.Templates
{
	internal abstract class BaseUnitSea : BaseUnit
	{
		private readonly int _range;

		public int Range
		{
			get
			{
				return _range;
			}
		}

		protected override void MovementDone(ITile previousTile)
		{
			if ((this is IBoardable) && previousTile.Units.Any(u => u.Class == UnitClass.Land))
			{
				IUnit[] moveUnits = previousTile.Units.Where(u => u.Class == UnitClass.Land).ToArray();
				if (previousTile.City != null)
					moveUnits = moveUnits.Where(u => u.Sentry).ToArray();
				moveUnits = moveUnits.Take((this as IBoardable).Cargo).ToArray();
				foreach (IUnit unit in moveUnits)
				{
					unit.X = X;
					unit.Y = Y;
					unit.Sentry = true;
					unit.Fortify = false;
				}
			}

			base.MovementDone(previousTile);
			
			if (Map[X, Y].City != null)
			{
				// End turn when entering city
				MovesLeft = 0;
				return;
			}
		}

		public override void Explore()
		{
			Explore(_range, sea: true);
		}

		public bool Unload()
		{
			if (!(this is IBoardable))
				return false;
			IUnit[] units = Map[X, Y].Units.Where(u => u.Class == UnitClass.Land).Take((this as IBoardable).Cargo).ToArray();
			if (units.Length == 0)
				return false;
			
			foreach (IUnit unit in units)
			{
				unit.Sentry = false;
			}
			MovesLeft = 0;
			PartMoves = 0;
			return true;
		}

		private MenuItem<int> MenuUnload() => MenuItem<int>.Create("Unload").SetShortcut("u").OnSelect((s, a) => Unload());
		
		public override IEnumerable<MenuItem<int>> MenuItems
		{
			get
			{
				yield return MenuNoOrders();
				yield return MenuWait();
				yield return MenuSentry();
				yield return MenuGoTo();
				if (Map[X, Y].City != null)
				{
					yield return MenuHomeCity();
				}
				else if (Map[X, Y].Units.Any(u => u.Class == UnitClass.Land))
				{
					yield return MenuUnload();
				}
				yield return null;
				yield return MenuDisbandUnit();
			}
		}
		
		protected override bool ValidMoveTarget(ITile tile)
		{
			// Check whether the tile exists, is an ocean tile or contains a city.
			return (tile != null && (tile.Type == Terrain.Ocean || tile.City != null));
		}

		public override void NewTurn()
		{
			base.NewTurn();

			Player player = Game.GetPlayer(Owner);
			if (player.HasWonder<MagellansExpedition>() || (!Game.WonderObsolete<Lighthouse>() && player.HasWonder<Lighthouse>())) MovesLeft++;
		}
		
		protected BaseUnitSea(byte price = 1, byte attack = 1, byte defense = 1, byte move = 1, int range = 1) : base(price, attack, defense, move)
		{
			Class = UnitClass.Water;
			_range = range;
		}
	}
}