// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseUnit : IUnit
	{
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
					text = Resources.Instance.GetCivilopediaText(Name.ToUpper());
					break;
				case 2:
					text = Resources.Instance.GetCivilopediaText(Name.ToUpper() + "2");
					break;
				default:
					Console.WriteLine("Invalid page number: {0}", pageNumber);
					break;
			}
			
			Picture output = new Picture(320, 200);
			
			output.AddLayer(GetUnit(1).Image, 215, 47);
			
			//12 76
			int yy = 76;
			foreach (string line in text)
			{
				Console.WriteLine(line);
				output.DrawText(line, 6, 1, 12, yy);
				yy += 9;
			}
			
			if (pageNumber == 2)
			{
				string requiredTech = "";
				if (RequiredTech != null) requiredTech = RequiredTech.Name;
				output.DrawText(string.Format("Requires {0}", requiredTech), 6, 9, 100, 120);
				output.DrawText(string.Format("Cost: {0}0 resources.", Price), 6, 9, 100, 129);
				output.DrawText(string.Format("Attack Strength: {0}", Attack), 6, 12, 100, 138);
				output.DrawText(string.Format("Defense Strength: {0}", Defense), 6, 12, 100, 147);
				output.DrawText(string.Format("Movement Rate: {0}", Move), 6, 5, 100, 156);
			}
			
			return output;
		}
		
		public IAdvance RequiredTech { get; protected set; }
		public IAdvance ObsoleteTech { get; protected set; }
		public UnitClass Class { get; protected set; }
		public Unit Type { get; protected set; }
		public byte Price { get; protected set; }
		public byte Attack { get; protected set; }
		public byte Defense { get; protected set; }
		public byte Move { get; protected set; }
		
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
		
		protected Picture GetUnit(byte colour)
		{
			//0, 160
			int unitId = (int)Type;
			if (_unitCache[unitId, colour] == null)
			{
				int xx = (unitId % 20) * 16;
				int yy = unitId < 20 ? 160 : 176;
				
				Bitmap image = (Bitmap)Resources.Instance.GetPart((Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? "SP257" : "SPRITES"), xx, yy, 16, 16);
				if (Common.ColourLight[colour] == 15) Picture.ReplaceColours(image, new byte[] { 3, 15, 10, 2 }, new byte[] { 0, 11, Common.ColourLight[colour], Common.ColourDark[colour] });
				else if (Common.ColourDark[colour] == 8) Picture.ReplaceColours(image, new byte[] { 3, 7, 10, 2 }, new byte[] { 0, 3, Common.ColourLight[colour], Common.ColourDark[colour] });
				else Picture.ReplaceColours(image, new byte[] { 3, 10, 2 }, new byte[] { 0, Common.ColourLight[colour], Common.ColourDark[colour] });
				
				_unitCache[unitId, colour] = new Picture(image);
			}
			return _unitCache[unitId, colour];
		}
		
		protected BaseUnit(byte price = 1, byte attack = 1, byte defense = 1, byte move = 1)
		{
			Price = price;
			Attack = attack;
			Defense = defense;
			Move = move;
		}
	}
}