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

namespace CivOne.Templates
{
	internal abstract class BaseUnit : IUnit
	{
		private int _x, _y;

		public bool Moving { get; private set; }
		public int MoveFrame { get; private set; }
		public int FromX { get; private set; }
		public int FromY { get; private set; }
		public void MoveUpdate()
		{
			MoveFrame++;
			if (!(Moving = (MoveFrame < 8)))
			{
				MovesLeft--;
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
				_y = value;
			}
		}
		public byte Owner { get; set; }
		public byte Status { get; set; }
		public byte MovesLeft { get; private set; }
		
		public virtual void NewTurn()
		{
			MovesLeft = Move;
		}

		public virtual bool MoveTo(int relX, int relY)
		{
			if (Moving) return false;

			int toX = (X + relX);
			int toY = (Y + relY);

			if (!MoveTargets.Any(t => t.X == toX && t.Y == toY)) return false;
			
			Moving = true;
			MoveFrame = 0;
			FromX = X;
			FromY = Y;
			
			X += relX;
			Y += relY;
			return true;
		}
		
		public virtual void SkipTurn()
		{
			MovesLeft = 0;
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
		
		public Picture GetUnit(byte colour)
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
			return _unitCache[unitId, colour];
		}

		private bool ValidMoveTarget(ITile tile)
		{
			if (tile == null) return false;
			switch (Class)
			{
				case UnitClass.Water:
					return tile.Type == Terrain.Ocean;
				case UnitClass.Land:
					return tile.Type != Terrain.Ocean;
			}
			return true;
		}

		public IEnumerable<ITile> MoveTargets
		{
			get
			{
				return Map.Instance.GetTile(X, Y).GetBorderTiles().Where(t => ValidMoveTarget(t));
			}
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