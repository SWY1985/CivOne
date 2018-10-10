// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.IO;
using CivOne.Screens;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.UserInterface;

namespace CivOne.Units
{
	internal abstract class BaseUnitAir : BaseUnit
	{
		public int TotalFuel { get; protected set; }
		public int FuelLeft { get; set; }

		private void HandleFuel()
		{
            // If landing
			if (Map[X, Y].City != null || Map[X, Y].Units.Any(u => u is Carrier))
			{
				MovesLeft = 0;
				FuelLeft = TotalFuel;
				return;
			}
            // if still in air with fuel
			if (MovesLeft > 0 || FuelLeft > 0) return;
			
			// Air unit is out of fuel
			Game.DisbandUnit(this);
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
			if (FuelLeft == Move)
				FuelLeft -= Move;
			FuelLeft -= (FuelLeft % Move);
			HandleFuel();
		}
		
		public override IEnumerable<MenuItem<int>> MenuItems
		{
			get
			{
				ITile tile = Map[X, Y];

				yield return MenuNoOrders();
				yield return MenuWait();
				yield return MenuSentry();
				yield return MenuGoTo();
				if ((TotalFuel > Move) && (tile.Irrigation || tile.Mine || tile.Road || tile.RailRoad))
				{
					yield return MenuPillage();
				}
				if (tile.City != null)
				{
					yield return MenuHomeCity();
				}
				yield return null;
				yield return MenuDisbandUnit();
			}
		}

		protected override bool ValidMoveTarget(ITile tile)
		{
			return (tile != null);
		}

        public override bool MoveTo(int relX, int relY)
        {
            BaseUnitAir unit = this;
            int fuel = unit.FuelLeft;
            bool _o = base.MoveTo( relX, relY );
            return _o;
        }

        /*  ******************************************************************************************************** */
        public override void SetHome()
        {
            if( Map[ X, Y ].City == null )
            {
                Point point;
                int _CarrierDistance = 100;
                IUnit _nearestCarrier = null;

                City nearestCity = Game.GetCities().Where( c => c.Owner == Owner ).OrderBy( c => Common.Distance( c.X, c.Y, _x, _y ) ).First();
                int _Citydistance = Common.Distance( nearestCity.X, nearestCity.Y, _x, _y );

                // Check for carriers
                IUnit[] _OwnCarriers = Game.GetUnits().Where( u => u.Owner == Owner && u.Type == UnitType.Carrier ).ToArray();
                if( _OwnCarriers.Length > 0 )
                {
                    _nearestCarrier = _OwnCarriers.OrderBy( c => Common.Distance( c.X, c.Y, _x, _y ) ).First();
                    _CarrierDistance = Common.Distance( _nearestCarrier.X, _nearestCarrier.Y, _x, _y );
                }
                if( _Citydistance < _CarrierDistance || _nearestCarrier == null )
                {
                    point.X = nearestCity.X;
                    point.Y = nearestCity.Y;
                }
                else
                {
                    point.X = _nearestCarrier.X;
                    point.Y = _nearestCarrier.Y;
                }
                this.Goto = point;
            }
            else
            {
                Home = Map[ X, Y ].City;
            }
        }

        private bool _sentry;
        public override bool Sentry
        {
            get
            {
                return _sentry;
            }
            set
            {
                // No sentry for air unit unless in city or on carrier
                ITile Tile = Map[ X, Y ];
                if( Tile.Units.Any( u => u.Type == UnitType.Carrier ) || Tile.City != null )
                {
                    if( _sentry == value ) return;
                    if( !( _sentry = value ) || !Game.Started ) return;
                    MovesLeft = 0;
                    PartMoves = 0;
                    MovementDone( Map[ X, Y ] );
                    return;
                }
                _sentry = false;
            }
        }

        protected BaseUnitAir( byte price = 1, byte attack = 1, byte defense = 1, byte move = 1 ) : base( price, attack, defense, move )
        {
            Class = UnitClass.Air;
            TotalFuel = move;
            FuelLeft = move;
        }

    }
}
