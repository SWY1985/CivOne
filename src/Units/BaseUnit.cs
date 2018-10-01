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
using CivOne.Advances;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.IO;
using CivOne.Screens;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.UserInterface;
using CivOne.Wonders;

namespace CivOne.Units
{
    internal abstract class BaseUnit : BaseInstance, IUnit
    {
        protected int _x, _y;

        public virtual bool Busy
        {
            get
            {
                return (Sentry || Fortify);
            }
            set
            {
                Sentry = false;
                Fortify = false;
                FortifyActive = false;
            }
        }
        public bool Veteran { get; set; }
        public bool FortifyActive { get; private set; }
        private bool _fortify = false;
        public bool Fortify
        {
            get
            {
                return (_fortify || FortifyActive);
            }
            set
            {
                if (Class != UnitClass.Land) return;
                if (this is Settlers) return;
                if (!value)
                    _fortify = false;
                else if (Fortify)
                    return;
                else
                    FortifyActive = true;
            }
        }

        private bool _sentry;
        public bool Sentry
        {
            get
            {
                return _sentry;
            }
            set
            {
                if (_sentry == value) return;
                if (!(_sentry = value) || !Game.Started) return;
                MovesLeft = 0;
                PartMoves = 0;
                MovementDone(Map[X, Y]);
            }
        }

        public bool Moving => (Movement != null);
        public MoveUnit Movement { get; protected set; }

        private int AttackStrength(IUnit defendUnit)
        {
            // Step 1: Determine the nominal attack value of the attacking unit and multiply it by 8.
            int attackStrength = ((int)Attack * 8);

            if (Owner == 0)
            {
                // Step 2: If the attacking unit is a Barbarian unit and the defending unit is player-controlled, multiply the attack strength by the Difficulty Modifier, then divide it by 4.
                if (Human == defendUnit.Owner)
                {
                    attackStrength *= (Game.Difficulty + 1);
                    attackStrength /= 4;
                }

                // Step 3: If the attacking unit is a Barbarian unit and the defensing unit is AI-controlled, divide the attack strength by 2.
                if (Human != defendUnit.Owner)
                {
                    attackStrength /= 2;
                }

                // Step 4: If the attacking unit is a Barbarian unit and the defending unit is inside a city and the defending civilization does not control any other cities, set the attack strength to zero.
                // This actually makes the defending unit invincible in this special case. Might well save you from being obliterated by that unlucky hut at 3600BC.
                if (defendUnit.Tile.City != null && Game.GetPlayer(defendUnit.Owner).Cities.Length == 1)
                {
                    attackStrength = 0;
                }

                // Step 5: If the attacking unit is a Barbarian unit and the defending unit is inside a city with a Palace, divide the attack strength by 2.
                if (defendUnit.Tile.City != null && defendUnit.Tile.City.HasBuilding<Palace>())
                {
                    attackStrength /= 2;
                }
            }

            // Step 6: If the attacking unit is a veteran unit, increase the attack strength by 50%.
            if (Veteran)
            {
                attackStrength += (attackStrength / 2);
            }

            // Step 7: If the attacking unit has only 0.2 movement points left, multiply the attack strength by 2, then divide it by 3. If the attacking unit has only 0.1 movement points left, then just divide by 3 instead.
            if (MovesLeft == 0)
            {
                attackStrength *= PartMoves;
                attackStrength /= 3;
            }

            // Step 8: If the attacking unit is a Barbarian unit and the defending unit is player-controlled, check the difficulty level. On Chieftain and Warlord levels, divide the attack strength by 2.
            if (Owner == 0 && Human == defendUnit.Owner)
            {
                if (Game.Difficulty < 2)
                {
                    attackStrength /= 2;
                }
            }

            // Step 9: If the attacking unit is player-controlled, check the difficulty level. On Chieftain level, multiply the attack strength by 2.
            // So on Chieftain difficulty, it is often better to attack than be attacked, even with a defensive unit.
            if (Human == Owner && Game.Difficulty == 0)
            {
                attackStrength *= 2;
            }

            return attackStrength;
        }

        private int DefendStrength(IUnit defendUnit, IUnit attackUnit)
        {
            // Check City Walls for step 5
            bool cityWalls = (defendUnit.Tile.City != null && defendUnit.Tile.City.HasBuilding<CityWalls>());

            // Step 1: Determine the nominal defense value of defending unit.
            int defendStrength = (int)defendUnit.Defense;

            if (defendUnit.Class == UnitClass.Land || (defendUnit.Class == UnitClass.Water && cityWalls && attackUnit.Attack != 12))
            {
                int fortificationModifier = 4;
                if (defendUnit.Tile.Fortress)
                    fortificationModifier = 8;
                else if (defendUnit.Fortify || defendUnit.FortifyActive)
                    fortificationModifier = 6;

                // Step 2: If the defending unit is a ground unit, multiply the defense strength by the Terrain Modifier.
                // This modifier effectively includes a factor of 2.
                defendStrength *= defendUnit.Tile.Defense;

                if (!cityWalls || attackUnit.Attack == 12)
                {
                    // Step 3: If the defending unit is a ground unit, multiply the defense strength by the Fortification Modifier.
                    // This modifier effectively includes a factor of 4, resulting in a combined factor of 8.
                    defendStrength *= fortificationModifier;
                }
            }

            // Step 4: If the defending unit is a sea or air unit, multiply the defense strength by 8.
            // This effectively treats the Terrain Modifier as 2, regardless of the actual terrain type. It also means that these units will never benefit from the Fortification Modifier.
            if (defendUnit.Class != UnitClass.Land && (!cityWalls || attackUnit.Attack == 12))
            {
                defendStrength *= 8;
            }

            // Step 5: If the defending unit is inside a city with City Walls and the nominal attack value of the attacking unit is NOT equal to 12, 
            // check the domain of the defending unit. If the domain is NOT air, re-calculate steps 1 and 2 (ignore steps 3 and 4) and multiply the result by 12.
            // When determining if the attacking unit ignores City Walls, the game just checks for attack value, not unit type. So if you change any unit's attack rating to 12, 
            // the game will have it ignore City Walls as well.
            if (cityWalls && attackUnit.Attack != 12)
            {
                defendStrength *= 12;
            }

            // Step 6: If the defending unit is a veteran unit, increase the defense strength by 50%.
            if (defendUnit.Veteran)
            {
                defendStrength += (defendStrength / 2);
            }

            return defendStrength;
        }

        private bool AttackOutcome(IUnit attackUnit, ITile defendTile)
        {
            IUnit defendUnit = defendTile.Units.OrderByDescending(x => x.Attack * (x.Veteran ? 1.5 : 1)).ThenBy(x => (int)x.Type).First();

            int attackStrength = AttackStrength(defendUnit);
            int defenseStrength = DefendStrength(defendUnit, attackUnit);
            int randomAttack = Common.Random.Next(attackStrength);
            int randomDefense = Common.Random.Next(defenseStrength);
            bool win = (randomAttack > randomDefense);
            if (win && attackUnit.Owner == 0 && defendUnit.Tile.City != null)
            {
                // If the attacking unit is a Barbarian unit and the defending unit is inside a city, then, if the attacking unit won, the procedure will be repeated once
                // This time, the attacking unit wins on a tie.
                randomAttack = Common.Random.Next(attackStrength);
                randomDefense = Common.Random.Next(defenseStrength);
                win = (randomAttack >= randomDefense);
            }

            // 50% chance to award veteran status to the winner
            if (Common.Random.Next(100) < 50)
            {
                if (win && !attackUnit.Veteran) attackUnit.Veteran = true;
                if (!win && !defendUnit.Veteran) defendUnit.Veteran = true;
            }

            return win;
        }

        protected virtual bool Confront(int relX, int relY)
        {
            Goto = Point.Empty;             // Cancel any goto mode when Confronting

            if( Class == UnitClass.Land && (this is Diplomat || this is Caravan))
            {
                // TODO: Perform other unit action (confront)
                return false;
            }

            Movement = new MoveUnit(relX, relY);

            ITile moveTarget = Map[X, Y][relX, relY];
            if (moveTarget == null) return false;
            if (!moveTarget.Units.Any(u => u.Owner != Owner) && moveTarget.City != null && moveTarget.City.Owner != Owner)
            {
                if (Class != UnitClass.Land)
                {
                    GameTask.Enqueue(Message.Error("-- Civilization Note --", TextFile.Instance.GetGameText($"ERROR/OCCUPY")));
                    Movement = null;
                    return false;
                }

                City capturedCity = moveTarget.City;
                Movement.Done += (s, a) =>
                {
                    Action changeOwner = delegate ()
                    {
                        Player previousOwner = Game.GetPlayer(capturedCity.Owner);

                        if (capturedCity.HasBuilding<Palace>())
                            capturedCity.RemoveBuilding<Palace>();
                        capturedCity.Food = 0;
                        capturedCity.Shields = 0;
                        while (capturedCity.Units.Length > 0)
                            Game.DisbandUnit(capturedCity.Units[0]);
                        capturedCity.Owner = Owner;

                        if (!capturedCity.HasBuilding<CityWalls>())
                        {
                            capturedCity.Size--;
                        }

                        previousOwner.IsDestroyed();
                    };

                    IList<IAdvance> advancesToSteal = GetAdvancesToSteal(capturedCity.Player);

                    if (Human == capturedCity.Owner || Human == Owner)
                    {
                        Show captureCity = Show.CaptureCity(capturedCity);
                        captureCity.Done += (s1, a1) =>
                        {
                            changeOwner();

                            if (capturedCity.Size == 0 || Human != Owner) return;
                            GameTask.Insert(Show.CityManager(capturedCity));
                        };
                        GameTask.Insert(captureCity);

                        if (advancesToSteal.Any())
                            GameTask.Enqueue(Tasks.Show.SelectAdvanceAfterCityCapture(Player, advancesToSteal));
                    }
                    else
                    {
                        changeOwner();
                        if (advancesToSteal.Any())
                            Player.AddAdvance(advancesToSteal.First());
                    }
                    MoveEnd(s, a);
                };
            }
            else if (this is Nuclear)
            {
                int xx = (X - Common.GamePlay.X + relX) * 16;
                int yy = (Y - Common.GamePlay.Y + relY) * 16;
                Show nuke = Show.Nuke(xx, yy);

                if (Map[X, Y][relX, relY].City != null)
                    PlaySound("airnuke");
                else
                    PlaySound("s_nuke");

                nuke.Done += (s, a) =>
                {
                    foreach (ITile tile in Map.QueryMapPart(X + relX - 1, Y + relY - 1, 3, 3))
                    {
                        while (tile.Units.Length > 0)
                        {
                            Game.DisbandUnit(tile.Units[0]);
                        }
                    }
                };
                GameTask.Enqueue(nuke);
            }
            else if (AttackOutcome(this, Map[X, Y][relX, relY]))
            {
                Movement.Done += (s, a) =>
                {
                    if (this is Cannon)
                    {
                        PlaySound("cannon");
                    }
                    else if (this is Musketeers || this is Riflemen || this is Armor || this is Artillery || this is MechInf)
                    {
                        PlaySound("s_land");
                    }
                    else
                    {
                        PlaySound("they_die");
                    }

                    IUnit unit = Map[X, Y][relX, relY].Units.FirstOrDefault();
                    if (unit != null)
                    {
                        GameTask.Insert(Show.DestroyUnit(unit, true));
                    }

                    if (MovesLeft == 0)
                    {
                        PartMoves = 0;
                    }
                    else if (MovesLeft > 0)
                    {
                        if (this is Bomber)
                        {
                            SkipTurn();
                        }
                        else
                        {
                            MovesLeft--;
                        }
                    }
                    Movement = null;
                    if (Map[X, Y][relX, relY].City != null)
                    {
                        if (!Map[X, Y][relX, relY].City.HasBuilding<CityWalls>())
                        {
                            Map[X, Y][relX, relY].City.Size--;
                        }
                    }
                };
            }
            else
            {
                Movement.Done += (s, a) =>
                {
                    if (this is Cannon)
                    {
                        PlaySound("cannon");
                    }
                    else if (this is Musketeers || this is Riflemen || this is Armor || this is Artillery || this is MechInf)
                    {
                        PlaySound("s_land");
                    }
                    else
                    {
                        PlaySound("we_die");
                    }
                    GameTask.Insert(Show.DestroyUnit(this, false));
                    Movement = null;
                };
            }
            GameTask.Insert(Movement);
            return false;
        }

        private IList<IAdvance> GetAdvancesToSteal(Player victim)
        {
            return victim.Advances
            .Where(p => !Player.Advances.Any(p2 => p2.Id == p.Id))
            .OrderBy(a => Common.Random.Next(0, 1000))
            .Take(3)
            .ToList();
        }



        public virtual bool MoveTo(int relX, int relY)
        {
            if (Movement != null) return false;

            // Check for nmy nearby, if so exit goto


            ITile moveTarget = Map[X, Y][relX, relY];
            if (moveTarget == null) return false;
            if (moveTarget.Units.Any(u => u.Owner != Owner))
            {
                Goto = Point.Empty;             // Cancel any goto mode

                if (Class == UnitClass.Land && Tile.IsOcean)
                {
                    if (Human == Owner) GameTask.Enqueue(Message.Error("-- Civilization Note --", TextFile.Instance.GetGameText($"ERROR/AMPHIB")));
                    return false;
                }
                return Confront(relX, relY);
            }
            if (Class == UnitClass.Land && !(this is Diplomat || this is Caravan) && !new ITile[] { Map[X, Y], moveTarget }.Any(t => t.IsOcean || t.City != null) 
                && moveTarget.GetBorderTiles().SelectMany(t => t.Units).Any(u => u.Owner != Owner))
            {
                if (!moveTarget.Units.Any(x => x.Owner == Owner))
                {
                    IUnit[] targetUnits = moveTarget.GetBorderTiles().SelectMany(t => t.Units).Where(u => u.Owner != Owner).ToArray();
                    IUnit[] borderUnits = Map[X, Y].GetBorderTiles().SelectMany(t => t.Units).Where(u => u.Owner != Owner).ToArray();

                    if (borderUnits.Any(u => targetUnits.Any(t => t.X == u.X && t.Y == u.Y)))
                    {
                        if (Human == Owner)
                            GameTask.Enqueue(Message.Error("-- Civilization Note --", TextFile.Instance.GetGameText($"ERROR/ZOC")));
                        return false;
                    }
                }
            }
            if (moveTarget.City != null && moveTarget.City.Owner != Owner)
            {
                return Confront(relX, relY);

            }

            if (!MoveTargets.Any(t => t.X == moveTarget.X && t.Y == moveTarget.Y))
            {
                // Target tile is invalid
                // TODO: For some tiles, display a message detailing why the move is illegal
                return false;
            }

            // TODO: This implementation was done by observation, may need a revision
            if ((moveTarget.Road || moveTarget.RailRoad) && (Tile.Road || Tile.RailRoad))
            {
                // Handle movement in MovementDone
            }
            else if (MovesLeft == 0 && !moveTarget.Road && moveTarget.Movement > 1)
            {
                bool success;
                if (PartMoves >= 2)
                {
                    // 2/3 moves left? 50% chance of success
                    success = (Common.Random.Next(0, 2) == 0);
                }
                else
                {
                    // 2/3 moves left? 33% chance of success
                    success = (Common.Random.Next(0, 3) == 0);
                }

                if (!success)
                {
                    PartMoves = 0;
                    return false;
                }
            }

            MovementTo(relX, relY);
            return true;
        }

        private void MoveEnd(object sender, EventArgs args)
        {
            ITile previousTile = Map[_x, _y];
            X += Movement.RelX;
            Y += Movement.RelY;
            if (X == Goto.X && Y == Goto.Y)
            {
                Goto = Point.Empty;
            }
            Movement = null;

            Explore();
            MovementDone(previousTile);
        }

        protected void MovementTo(int relX, int relY)
        {
            MovementStart(Tile);
            Movement = new MoveUnit(relX, relY);
            Movement.Done += MoveEnd;
            GameTask.Insert(Movement);
        }

        protected virtual void MovementStart(ITile previousTile)
        {
        }

        protected virtual void MovementDone(ITile previousTile)
        {
            if (MovesLeft > 0) MovesLeft--;

            Tile.Visit(Owner);

            if (Tile.Hut)
            {
                Tile.Hut = false;
            }
        }

        private static IBitmap[] _iconCache = new IBitmap[28];
        public virtual IBitmap Icon { get; private set; }
        private string _name;
        public string Name
        {
            get => Modifications.LastOrDefault(x => x.Name.HasValue)?.Name.Value ?? _name;
            protected set => _name = value;
        }
        public byte PageCount => 2;
        public Picture DrawPage(byte pageNumber)
        {
            string[] text = new string[0];
            switch (pageNumber)
            {
                case 1:
                    text = Resources.GetCivilopediaText("BLURB2/" + _name.ToUpper());
                    break;
                case 2:
                    text = Resources.GetCivilopediaText("BLURB2/" + _name.ToUpper() + "2");
                    break;
                default:
                    Log("Invalid page number: {0}", pageNumber);
                    break;
            }

            Picture output = new Picture(320, 200);

            output.AddLayer(this.ToBitmap(1), 215, 47);

            int yy = 76;
            foreach (string line in text)
            {
                Log(line);
                output.DrawText(line, 6, 1, 12, yy);
                yy += 9;
            }

            if (pageNumber == 2)
            {
                yy += 8;
                string requiredTech = "";
                if (RequiredTech != null) requiredTech = RequiredTech.Name;
                output.DrawText(string.Format("Requires {0}", requiredTech), 6, 9, 100, yy); yy += 8;
                output.DrawText(string.Format("Cost: {0}0 resources.", Price), 6, 9, 100, yy); yy += 8;
                output.DrawText(string.Format("Attack Strength: {0}", Attack), 6, 12, 100, yy); yy += 8;
                output.DrawText(string.Format("Defense Strength: {0}", Defense), 6, 12, 100, yy); yy += 8;
                output.DrawText(string.Format("Movement Rate: {0}", Move), 6, 5, 100, yy);
            }

            return output;
        }

        private IAdvance _requiredTech;
        public IAdvance RequiredTech
        {
            get => Modifications.LastOrDefault(x => x.Requires.HasValue)?.Requires.Value.ToInstance() ?? _requiredTech;
            protected set => _requiredTech = value;
        }

        public IWonder RequiredWonder { get; protected set; }

        private IAdvance _obsoleteTech;
        public IAdvance ObsoleteTech
        {
            get => Modifications.LastOrDefault(x => x.Obsolete.HasValue)?.Obsolete.Value.ToInstance() ?? _obsoleteTech;
            protected set => _obsoleteTech = value;
        }

        public UnitClass Class { get; protected set; }
        public UnitType Type { get; protected set; }
        public City Home { get; protected set; }
        public short _buyPrice;
        public short BuyPrice
        {
            get => Modifications.LastOrDefault(x => x.BuyPrice.HasValue)?.BuyPrice.Value ?? _buyPrice;
            private set => _buyPrice = value;
        }
        public byte ProductionId => (byte)Type;
        private byte _price;
        public byte Price
        {
            get => Modifications.LastOrDefault(x => x.Price.HasValue)?.Price.Value ?? _price;
            protected set => _price = value;
        }
        public virtual UnitRole Role
        {
            get
            {
                UnitRole output = UnitRole.LandAttack;
                if (this is Settlers) output = UnitRole.Settler;
                else if (this is Caravan || this is Diplomat) output = UnitRole.Civilian;
                else if (this is BaseUnitSea)
                {
                    if (this is IBoardable) output = UnitRole.Transport;
                    else output = UnitRole.SeaAttack;
                }
                else if (this is Fighter) output = UnitRole.AirAttack;
                else if (this.Defense >= this.Attack) output = UnitRole.Defense;
                return output;
            }
        }

        private byte _attack;
        public byte Attack
        {
            get => Modifications.LastOrDefault(x => x.Attack.HasValue)?.Attack.Value ?? _attack;
            protected set => _attack = value;
        }

        private byte _defense;
        public byte Defense
        {
            get => Modifications.LastOrDefault(x => x.Defense.HasValue)?.Defense.Value ?? _defense;
            protected set => _defense = value;
        }

        private byte _move;
        public byte Move
        {
            get => Modifications.LastOrDefault(x => x.Moves.HasValue)?.Moves.Value ?? _move;
            protected set => _move = value;
        }

        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                int val = value;
                while (val < 0) val += Map.WIDTH;
                while (val >= Map.WIDTH) val -= Map.WIDTH;
                if (_x == -1 && _y != -1) Explore();
                _x = val;
            }
        }
        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (value < 0 || value >= Map.HEIGHT) return;
                if (_y == -1 && _x != -1 && value != -1) Explore();
                _y = value;
            }
        }

        public Point Goto { get; set; }

        public ITile Tile => Map[_x, _y];

        private byte _owner;
        public byte Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                if (Game.Started) Tile.Visit(_owner);
            }
        }

        public Player Player => Game.GetPlayer(Owner);

        public byte Status
        {
            get
            {
                return 0;
            }
            set
            {
                bool[] bits = new bool[8];
                for (int i = 0; i < 8; i++)
                    bits[i] = (((value >> i) & 1) > 0);
                if (bits[0]) Sentry = true;
                else if (bits[2]) FortifyActive = true;
                else if (bits[3]) _fortify = true;

                if (this is Settlers)
                {
                    (this as Settlers).SetStatus(bits);
                }

                Veteran = bits[5];
            }
        }
        public byte MovesLeft { get; set; }
        public byte PartMoves { get; set; }

        public virtual void NewTurn()
        {
            if (FortifyActive)
            {
                FortifyActive = false;
                _fortify = true;
            }
            MovesLeft = Move;
            Explore();
        }

// ****************************************************************************************************
        public void SetHome()
        {
            if (Map[X, Y].City == null)
            {
                if (this is Bomber || this is Fighter )     // Maybe move this code to BaseUnitAir.cs
                {
                    Point point;
                    int _CarrierDistance = 100;
                    IUnit _nearestCarrier = null;

                    City nearestCity = Game.GetCities().Where(c => c.Owner == Owner).OrderBy(c => Common.DistanceToTile( c.X, c.Y, _x, _y)).First();
                    int _Citydistance = Common.DistanceToTile( nearestCity.X, nearestCity.Y, _x, _y );

                    // Check for carriers
                    IUnit[] _OwnCarriers = Game.GetUnits().Where( u => u.Owner == Owner && u.Type == UnitType.Carrier ).ToArray();
                    if( _OwnCarriers.Length > 0 )
                    {
                        _nearestCarrier = _OwnCarriers.OrderBy( c => Common.DistanceToTile( c.X, c.Y, _x, _y ) ).First();
                        _CarrierDistance = Common.DistanceToTile( _nearestCarrier.X, _nearestCarrier.Y, _x, _y );
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
                else return;
            }
            else
            {
                if (Map[X, Y].City == null) return;
                Home = Map[X, Y].City;
            }
		}

		public void SetHome(City city) => Home = city;
		
		public void Pillage()
		{
			if (!(Tile.Irrigation || Tile.Mine || Tile.Road || Tile.RailRoad))
				return;
			
			if (Tile.Irrigation)
				Tile.Irrigation = false;
			else if (Tile.Mine)
				Tile.Mine = false;
			else if (Tile.Road)
				Tile.Road = false;
			else if (Tile.RailRoad)
			{
				Tile.RailRoad = false;
				Tile.Road = true;
			}
			
			MovesLeft = 0;
			PartMoves = 0;
		}

		public virtual void SkipTurn()
		{
			MovesLeft = 0;
			PartMoves = 0;
		}
		
		protected void SetIcon(char page, int col, int row)
		{
			if (_iconCache[(int)Type] == null)
			{
				_iconCache[(int)Type] = Resources[$"ICONPG{page}"][col * 160, row * 62, 160, 60]
					.ColourReplace((byte)(GFX256 ? 253 : 15), 0);
			}
			Icon = _iconCache[(int)Type];
		}

		protected MenuItem<int> MenuNoOrders() => MenuItem<int>.Create("No Orders").SetShortcut("space").OnSelect((s, a) => SkipTurn());
		
		protected MenuItem<int> MenuFortify() => MenuItem<int>.Create("Fortify").SetShortcut("f").OnSelect((s, a) => Fortify = true);
		
		protected MenuItem<int> MenuWait() => MenuItem<int>.Create("Wait").SetShortcut("w").OnSelect((s, a) => Game.UnitWait());
		
		protected MenuItem<int> MenuSentry() => MenuItem<int>.Create("Sentry").SetShortcut("s").OnSelect((s, a) => Sentry = true);
		
		protected MenuItem<int> MenuGoTo() => MenuItem<int>.Create("GoTo").OnSelect((s, a) => GameTask.Enqueue(Show.Goto));
		
		protected MenuItem<int> MenuPillage() => MenuItem<int>.Create("Pillage").SetShortcut("P").OnSelect((s, a) => Pillage());
		
		protected MenuItem<int> MenuHomeCity() => MenuItem<int>.Create("Home City").SetShortcut("h").OnSelect((s, a) => SetHome());
		
		protected MenuItem<int> MenuDisbandUnit() => MenuItem<int>.Create("Disband Unit").SetShortcut("D").OnSelect((s, a) => Game.DisbandUnit(this));

		public abstract IEnumerable<MenuItem<int>> MenuItems { get; }

		protected abstract bool ValidMoveTarget(ITile tile);

		public IEnumerable<ITile> MoveTargets => Map[X, Y].GetBorderTiles().Where(t => ValidMoveTarget(t));

		protected void Explore(int range, bool sea = false)
		{
			if (Game == null) return;
			Player player = Game.GetPlayer(Owner);
			if (player == null) return;
			player.Explore(X, Y, range, sea);
			if (player.IsHuman) Common.GamePlay?.RefreshMap();
		}

		public virtual void Explore() => Explore(1);

		internal static IBitmap GetBaseSprite(UnitType type)
		{
			if (!_modifications.ContainsKey(type)) return null;
			return _modifications[type].LastOrDefault(x => x.Sprite != null && x.Sprite.GifToBitmap() != null)?.Sprite.GifToBitmap();
		}

		private static Dictionary<UnitType, List<UnitModification>> _modifications = new Dictionary<UnitType, List<UnitModification>>();
		internal static void LoadModifications()
		{
			_modifications.Clear();

			UnitModification[] unitModifications = Reflect.GetModifications<UnitModification>().ToArray();
			if (unitModifications.Length == 0) return;

			Log("Applying unit modifications");

			foreach (UnitModification modification in Reflect.GetModifications<UnitModification>())
			{
				if (!_modifications.ContainsKey(modification.UnitType))
					_modifications.Add(modification.UnitType, new List<UnitModification>());
				_modifications[modification.UnitType].Add(modification);
			}

			Log("Finished applying unit modifications");
		}
		public IEnumerable<UnitModification> Modifications => _modifications.ContainsKey(Type) ? _modifications[Type].ToArray() : new UnitModification[0];
		
		protected BaseUnit(byte price = 1, byte attack = 1, byte defense = 1, byte move = 1)
		{
			Price = price;
			BuyPrice = (short)((Price + 4) * 10 * Price);
			Attack = attack;
			Defense = defense;
			Move = move;
			X = -1;
			Y = -1;
			Goto = Point.Empty;
			Owner = 0;
			Status = 0;
			RequiredWonder = null;
		}
	}
}
