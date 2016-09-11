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
using CivOne.Screens;
using CivOne.Units;

namespace CivOne.Templates
{
	internal abstract class BaseUnit : IUnit
	{
		private int _x, _y;

		public virtual bool Busy
		{
			get
			{
				return (Sentry || Fortify);
			}
		}
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
				else
					FortifyActive = true;
			}
		}
		public bool Sentry { get; set; }
		public bool Moving { get; private set; }
		public int MoveFrame { get; private set; }
		public int FromX { get; private set; }
		public int FromY { get; private set; }

		protected Map Map
		{
			get
			{
				return Map.Instance;
			}
		}

		private int NearestCity
		{
			get
			{
				int output = 0;
				if (Game.Instance.GetCities().Any())
					output = Game.Instance.GetCities().Select(c => Common.DistanceToTile(_x, _y, c.X, c.Y)).Min();
				return output;
			}
		}

		private void TribalHutMessage(EventHandler method, params string[] message)
		{
			if (Player.Human)
			{
				TribalHut tribalHut = new TribalHut(message);
				tribalHut.Closed += method;
				Common.AddScreen(tribalHut);
				return;
			}
			method(this, null);
		}

		protected void TribalHut(HutResult result = HutResult.Random)
		{
			switch(result)
			{
				case HutResult.MetalDeposits:
					TribalHutMessage((s, e) => {
						 Player.Gold += 50;
					}, "You have discovered", "valuable metal deposits", "worth 50$");
					return;
				case HutResult.FriendlyTribe:
					TribalHutMessage((s, e) => {
						Game.Instance.CreateUnit(Common.Random.Next(0, 100) < 50 ? Unit.Cavalry : Unit.Legion, X, Y, Owner, true);
					}, "You have discovered", "a friendly tribe of", "skilled mercenaries.");
					return;
				case HutResult.AdvancedTribe:
					TribalHutMessage((s, e) => {
						Game.Instance.FoundCity(_x, _y, discardSettlers: false);
					}, "You have discovered", "an advanced tribe.");
					return;
				case HutResult.AncientScrolls:
					TribalHutMessage((s, e) => {
						// TODO: Actually give the ancient scroll of wisdom to the player.
					}, "You have discovered", "scrolls of ancient wisdom.");
					return;
				case HutResult.Barbarians:
					TribalHutMessage((s, e) => {
						// TODO: Actually unleash the horde of barbarians.
					}, "You have unleashed", "a horde of barbarians!");
					return;
			}

			// Tribal hut outcome, as described here: http://forums.civfanatics.com/showthread.php?t=510312
			switch (Common.Random.Next(0, 4))
			{
				case 0:
					if (NearestCity > 3)
					{
						if (Map[_x, _y].LandValue > 12)
						{
							TribalHut(HutResult.AdvancedTribe);
							break;
						}
						TribalHut(HutResult.MetalDeposits);
						break;
					}
					TribalHut(HutResult.FriendlyTribe);
					break;
				case 1:
					if (Game.Instance.GameTurn == 0 || Common.TurnToYear(Game.Instance.GameTurn) >= 1000)
					{
						TribalHut(HutResult.MetalDeposits);
						break;
					}
					TribalHut(HutResult.AncientScrolls);
					break;
				case 2:
					TribalHut(HutResult.MetalDeposits);
					break;
				case 3:
					if (NearestCity < 4 || !Game.Instance.GetCities().Any(c => c.Owner == Game.Instance.PlayerNumber(Player)))
					{
						TribalHut(HutResult.FriendlyTribe);
						break;
					}
					TribalHut(HutResult.Barbarians);
					break;
				default:
					TribalHut(HutResult.FriendlyTribe);
					break;
			}
		}

		public void MoveUpdate()
		{
			MoveFrame++;
			if (!(Moving = (MoveFrame < 8)))
			{
				Explore();
				if (Class == UnitClass.Land && Map[FromX, FromY].Road && Map[X, Y].Road)
				{
					if (Map[X, Y].RailRoad && Map[FromX, FromY].RailRoad)
					{
						// No moves lost
					}
					else if (PartMoves > 0)
					{
						PartMoves--;
					}
					else
					{
						if (MovesLeft > 0)
							MovesLeft--;
						PartMoves = 2;
					}
				}
				else if (Class == UnitClass.Land && Map[X, Y].Type == Terrain.Ocean)
				{
					MovesLeft = 0;
					PartMoves = 0;
					Sentry = true;
				}
				else if (Class == UnitClass.Water && (this is IBoardable) && Map[FromX, FromY].Units.Any(u => u.Class == UnitClass.Land))
				{
					IUnit[] moveUnits = Map[FromX, FromY].Units.Where(u => u.Class == UnitClass.Land).Take((this as IBoardable).Cargo).ToArray();
					foreach (IUnit unit in moveUnits)
					{
						unit.X = X;
						unit.Y = Y;
						unit.Sentry = true;
					}
				}
				else
				{
					if (MovesLeft > 0)
						MovesLeft--;
					else if (PartMoves > 0)
						PartMoves = 0;
				}
				if (Map[_x, _y].Hut)
				{
					Map[_x, _y].Hut = false;
					if (Class == UnitClass.Land)
					{
						TribalHut();
					}
				}
			}
		}

		private static Picture[,] _unitCache = new Picture[28,8];
		private static Picture[] _iconCache = new Picture[28];
		public virtual Picture Icon
		{
			get;
			private set;
		}
		public string Name { get; protected set; }
		public byte PageCount
		{
			get
			{
				return 2;
			}
		}
		public Picture DrawPage(byte pageNumber)
		{
			string[] text = new string[0];
			switch (pageNumber)
			{
				case 1:
					text = Resources.Instance.GetCivilopediaText("BLURB2/" + Name.ToUpper());
					break;
				case 2:
					text = Resources.Instance.GetCivilopediaText("BLURB2/" + Name.ToUpper() + "2");
					break;
				default:
					Console.WriteLine("Invalid page number: {0}", pageNumber);
					break;
			}
			
			Picture output = new Picture(320, 200);
			
			output.AddLayer(GetUnit(1).Image, 215, 47);
			
			int yy = 76;
			foreach (string line in text)
			{
				Console.WriteLine(line);
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
		
		public IAdvance RequiredTech { get; protected set; }
		public IWonder RequiredWonder { get; protected set; }
		public IAdvance ObsoleteTech { get; protected set; }
		public UnitClass Class { get; protected set; }
		public Unit Type { get; protected set; }
		public City Home { get; protected set; }
		public byte Price { get; protected set; }
		public byte Attack { get; protected set; }
		public byte Defense { get; protected set; }
		public byte Move { get; protected set; }
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
				if (_x == -1 && _y != -1 && value != -1) Explore();
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
		public byte Owner { get; set; }
		public Player Player
		{
			get
			{
				return Game.Instance.GetPlayer(Owner);
			}
		}
		public byte Status { get; set; }
		public byte MovesLeft { get; protected set; }
		public byte PartMoves { get; protected set; }
		
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

		public virtual bool MoveTo(int relX, int relY)
		{
			if (Moving) return false;
			if (Map[X, Y][relX, relY].Units.Any(u => u.Owner != Owner))
			{
				// TODO: Attack, or perform other unit action (confront)
				return false;
			}
			if (Map[X, Y][relX, relY].City != null && Map[X, Y][relX, relY].City.Owner != Owner)
			{
				// TODO: Attack, take city or perform other unit action (confront)
				return false;
			}

			int toX = (X + relX);
			int toY = (Y + relY);
			
			while (toX < 0) toX += Map.WIDTH;
			while (toX >= Map.WIDTH) toX -= Map.WIDTH;

			if (!MoveTargets.Any(t => t.X == toX && t.Y == toY)) return false;
			
			Moving = true;
			MoveFrame = 0;
			FromX = X;
			FromY = Y;
			
			X += relX;
			Y += relY;
			return true;
		}

		public void SetHome(City home)
		{
			Home = home;
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
				Bitmap icon = Resources.Instance.LoadPIC(string.Format("ICONPG{0}", page), true).GetPart(col * 160, row * 62, 160, 60);
				Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
				_iconCache[(int)Type] = new Picture(icon);
			}
			Icon = _iconCache[(int)Type];
		}
		
		public virtual Picture GetUnit(byte colour, bool showState = true)
		{
			int unitId = (int)Type;
			if (_unitCache[unitId, colour] == null)
			{
				int xx = (unitId % 20) * 16;
				int yy = unitId < 20 ? 160 : 176;
				
				Bitmap image = (Bitmap)Resources.Instance.GetPart((Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? "SP257" : "SPRITES"), xx, yy, 16, 16).Clone();
				if (Common.ColourLight[colour] == 15) Picture.ReplaceColours(image, new byte[] { 3, 15, 10, 2 }, new byte[] { 0, 11, Common.ColourLight[colour], Common.ColourDark[colour] });
				else if (Common.ColourDark[colour] == 8) Picture.ReplaceColours(image, new byte[] { 3, 7, 10, 2 }, new byte[] { 0, 3, Common.ColourLight[colour], Common.ColourDark[colour] });
				else Picture.ReplaceColours(image, new byte[] { 3, 10, 2 }, new byte[] { 0, Common.ColourLight[colour], Common.ColourDark[colour] });
				
				_unitCache[unitId, colour] = new Picture(image);
			}
			if (!showState || (!Sentry && !Fortify))
				return _unitCache[unitId, colour];
			
			if (Sentry)
			{
				Bitmap output = (Bitmap)_unitCache[unitId, colour].Image.Clone();
				Picture.ReplaceColours(output, new byte[] { 5, 8, }, new byte[] { 7, 7 });
				return new Picture(output);
			}
			if (FortifyActive)
			{
				Picture unit = new Picture(_unitCache[unitId, colour].Image);
				unit.DrawText("F", 0, 5, 8, 9, TextAlign.Center);
				unit.DrawText("F", 0, (byte)(colour == 1 ? 9 : 15), 8, 8, TextAlign.Center);
				return unit; 
			}
			else if (_fortify)
			{
				Picture unit = new Picture(_unitCache[unitId, colour].Image);
				unit.AddLayer(Icons.Fortify, 0, 0);
				return unit; 
			}
			return _unitCache[unitId, colour];
		}

		private bool ValidMoveTarget(ITile tile)
		{
			if (tile == null) return false;
			switch (Class)
			{
				case UnitClass.Water:
					return (tile.Type == Terrain.Ocean || tile.City != null);
				case UnitClass.Land:
					{
						if (tile.Type == Terrain.Ocean)
						{
							return (tile.Units.Any(u => (u is IBoardable)) && tile.Units.Where(u => u is IBoardable).Sum(u => (u as IBoardable).Cargo) > tile.Units.Count(u => u.Class == UnitClass.Land));
						}
						return true;
					}
			}
			return true;
		}

		public IEnumerable<ITile> MoveTargets
		{
			get
			{
				return Map[X, Y].GetBorderTiles().Where(t => ValidMoveTarget(t));
			}
		}

		protected void Explore(int range, bool sea = false)
		{
			if (Game.Instance == null) return;
			Player player = Game.Instance.GetPlayer(Owner);
			if (player == null) return;
			player.Explore(X, Y, range, sea);
		}

		public virtual void Explore()
		{
			Explore(1);
		}
		
		protected BaseUnit(byte price = 1, byte attack = 1, byte defense = 1, byte move = 1)
		{
			Price = price;
			Attack = attack;
			Defense = defense;
			Move = move;
			X = -1;
			Y = -1;
			Owner = 0;
			Status = 0;
			RequiredWonder = null;
			MovesLeft = move;
		}
	}
}