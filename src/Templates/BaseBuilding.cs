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
	internal abstract class BaseBuilding : IBuilding
	{
		private static Picture[,] _iconsCache = new Picture[6, 4], _iconsCacheGrass = new Picture[6, 4];
		
		public virtual Picture Icon { get; protected set; }
		public virtual Picture SmallIcon { get; protected set; }
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
					text = Resources.Instance.GetCivilopediaText("BLURB1/" + Name.ToUpper());
					break;
				case 2:
					text = Resources.Instance.GetCivilopediaText("BLURB1/" + Name.ToUpper() + "2");
					break;
				default:
					Console.WriteLine("Invalid page number: {0}", pageNumber);
					break;
			}
			
			Picture output = new Picture(320, 200);
			
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
				output.DrawText(string.Format("Requires {0}", requiredTech), 6, 9, 12, yy); yy += 8;
				output.DrawText(string.Format("Cost: {0}0 shields.", Price), 6, 9, 12, yy); yy += 8;
				output.DrawText(string.Format("Maintenance: ${0}", Maintenance), 6, 12, 12, yy);
			}
			
			return output;
		}
		
		protected Building Type { get; set; }
		
		public IAdvance RequiredTech { get; protected set; }
		public short SellPrice { get; private set; }
		public short BuyPrice { get; private set; }
		public byte Price { get; protected set; }
		public byte Maintenance { get; protected set; }
		
		protected void SetIcon(int col, int row, bool grassTile)
		{
			if ((grassTile && _iconsCacheGrass[col, row] == null) || (!grassTile && _iconsCache[col, row] == null))
			{
				Icon = new Picture(52, 50, Resources.Instance.LoadPIC("CITYPIX2").Image.Palette.Entries);
				
				if (grassTile)
				{
					Bitmap grass = (Bitmap)Resources.Instance.LoadPIC("CITYPIX2", true).GetPart(250, 0, 52, 50).Clone();
					Picture.ReplaceColours(grass, 1, 0);
					Icon.AddLayer(grass);
				}
				
				Bitmap icon = (Bitmap)Resources.Instance.LoadPIC("CITYPIX2", true).GetPart(col * 50, row * 50, 52, 50).Clone();
				Picture.ReplaceColours(icon, 1, 0);
				Icon.AddLayer(icon);
				Icon.FillRectangle(0, 50, 0, 2, 50);
				
				if (grassTile) _iconsCacheGrass[col, row] = Icon;
				else _iconsCache[col, row] = Icon;
			}
			Icon = (grassTile ? _iconsCacheGrass[col, row] : _iconsCache[col, row]);
		}
		
		protected void SetSmallIcon(int col, int row)
		{
			Bitmap icon = (Bitmap)Resources.Instance.LoadPIC((Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? "SP299" : "SPRITES")).GetPart(160 + (19 * col), 50 + (10 * row), 20, 10).Clone();
			Picture.ReplaceColours(icon, 0, 5);
			SmallIcon = new Picture(20, 10);
			SmallIcon.FillRectangle(5, 0, 0, 20, 10);
			SmallIcon.AddLayer(icon);
			SmallIcon.FillRectangle(0, 0, 0, 1, 10);
			SmallIcon.FillRectangle(0, 19, 0, 1, 10);
		}
		
		public byte Id
		{
			get
			{
				return (byte)Type;
			}
		}
		
		protected BaseBuilding(byte price = 1, byte maintenance = 0, short sell = 40, short buy = 50)
		{
			Price = price;
			Maintenance = maintenance;
			BuyPrice = buy;
			SellPrice = sell;
		}
	}
}