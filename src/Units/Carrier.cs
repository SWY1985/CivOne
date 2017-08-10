// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.Tiles;

namespace CivOne.Units
{
	internal class Carrier : BaseUnitSea
	{
		protected override void MovementDone(ITile previousTile)
		{
			if (previousTile.Units.Any(u => u.Class == UnitClass.Air))
			{
				IUnit[] moveUnits = previousTile.Units.Where(u => u.Class == UnitClass.Air).ToArray();
				moveUnits = moveUnits.Take(8).ToArray();
				foreach (IUnit unit in moveUnits)
				{
					unit.X = X;
					unit.Y = Y;
					unit.Sentry = false;
				}
			}

			base.MovementDone(previousTile);
		}

		public Carrier() : base(16, 1, 12, 5, 2)
		{
			Type = Unit.Carrier;
			Name = "Carrier";
			RequiredTech = new AdvancedFlight();
			ObsoleteTech = null;
			SetIcon('D', 1, 0);
		}
	}
}