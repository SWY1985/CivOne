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
using CivOne.Tasks;
using CivOne.Units;

namespace CivOne.Templates
{
	internal abstract class BaseUnit : IUnit
	{
		protected int _x, _y;
		
		protected Map Map
		{
			get
			{
				return Map.Instance;
			}
		}

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
		public bool Sentry { get; set; }

		public bool Moving
		{
			get
			{
				return (Movement != null); 
			}
		}
		public MoveUnit Movement { get; private set; }
		
		public virtual bool MoveTo(int relX, int relY)
		{
			if (Movement != null) return false;
			
			ITile moveTarget = Map[X, Y][relX, relY];
			if (moveTarget == null) return false;
			if (moveTarget.Units.Any(u => u.Owner != Owner))
			{
				// TODO: Attack, or perform other unit action (confront)
				return false;
			}
			if (moveTarget.City != null && Map[X, Y][relX, relY].City.Owner != Owner)
			{
				// TODO: Attack, take city or perform other unit action (confront)
				return false;
			}

			if (!MoveTargets.Any(t => t.X == moveTarget.X && t.Y == moveTarget.Y))
			{
				// Target tile is invalid
				// TODO: For some tiles, display a message detailing why the move is illegal
				return false;
			}

			Movement = new MoveUnit(relX, relY);
			Movement.Done += MoveEnd;
			GameTask.Insert(Movement);

			return true;
		}

		private void MoveEnd(object sender, EventArgs args)
		{
			ITile previousTile = Map[_x, _y];
			X += Movement.RelX;
			Y += Movement.RelY;
			Movement = null;
			
			Explore();
			MovementDone(previousTile);
		}

		protected virtual void MovementDone(ITile previousTile)
		{
			MovesLeft--;

			if (Tile.Hut)
			{
				Tile.Hut = false;
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
		public ITile Tile
		{
			get
			{
				return Map[_x, _y];
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
				if (Common.ColourLight[colour] == 15) Picture.ReplaceColours(image, new byte[] { 15, 10, 2 }, new byte[] { 11, Common.ColourLight[colour], Common.ColourDark[colour] });
				else if (Common.ColourDark[colour] == 8) Picture.ReplaceColours(image, new byte[] { 7, 10, 2 }, new byte[] { 3, Common.ColourLight[colour], Common.ColourDark[colour] });
				else Picture.ReplaceColours(image, new byte[] { 10, 2 }, new byte[] { Common.ColourLight[colour], Common.ColourDark[colour] });
				
				Picture icon = new Picture(image);
				icon.FillRectangle(0, 0, 0, 16, 1);
				icon.FillRectangle(0, 0, 1, 1, 15);
				_unitCache[unitId, colour] = icon;
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

		protected GameMenu.Item MenuNoOrders()
		{
			GameMenu.Item item = new GameMenu.Item("No Orders", "space");
			item.Selected += (s, a) => SkipTurn();
			return item;
		}
		
		protected GameMenu.Item MenuFortify()
		{
			GameMenu.Item item = new GameMenu.Item("Fortify", "f");
			item.Selected += (s, a) => Fortify = true;
			return item;
		}
		
		protected GameMenu.Item MenuWait()
		{
			GameMenu.Item item = new GameMenu.Item("Wait", "w");
			item.Selected += (s, a) => Game.Instance.UnitWait();
			return item;
		}
		
		protected GameMenu.Item MenuSentry()
		{
			GameMenu.Item item = new GameMenu.Item("Sentry", "s");
			item.Selected += (s, a) => Sentry = true;
			return item;
		}
		
		protected GameMenu.Item MenuGoTo()
		{
			GameMenu.Item item = new GameMenu.Item("GoTo");
			item.Selected += (s, a) => GameTask.Enqueue(Show.Goto);
			return item;
		}
		
		protected GameMenu.Item MenuPillage()
		{
			GameMenu.Item item = new GameMenu.Item("Pillage", "P");
			item.Enabled = false;
			// TODO: Add action
			return item;
		}
		
		protected GameMenu.Item MenuHomeCity()
		{
			GameMenu.Item item = new GameMenu.Item("Home City", "h");
			item.Selected += (s, a) => SetHome(Map[X, Y].City);
			return item;
		}
		
		protected GameMenu.Item MenuDisbandUnit()
		{
			GameMenu.Item item = new GameMenu.Item("Disband Unit", "D");
			item.Selected += (s, a) => Game.Instance.DisbandUnit(this);
			return item;
		}

		public abstract IEnumerable<GameMenu.Item> MenuItems { get; }

		protected abstract bool ValidMoveTarget(ITile tile);

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