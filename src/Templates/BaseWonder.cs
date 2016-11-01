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
	internal abstract class BaseWonder : IWonder
	{
		public string Name { get; protected set; }
		public virtual Picture Icon
		{
			get
			{
				return null;
			}
		}
		public virtual Picture SmallIcon { get; protected set; }
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
				output.DrawText(string.Format("Maintenance: ${0}", 0), 6, 12, 12, yy);
			}
			
			return output;
		}
		
		protected Wonder Type { get; set; }
		
		public IAdvance RequiredTech { get; protected set; }
		public IAdvance ObsoleteTech { get; protected set; }
		public short BuyPrice { get; private set; }
		public byte ProductionId
		{
			get
			{
				return (byte)(Math.Abs((int)Type - 232));
			}
		}
		public byte Price { get; protected set; }
		
		protected void SetSmallIcon(int col, int row)
		{
			Picture icon = Resources.Instance.LoadPIC((Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? "SP299" : "SPRITES")).GetPart(160 + (19 * col), 50 + (10 * row), 20, 10);
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
		
		protected BaseWonder(byte price = 1)
		{
			Price = price;
			BuyPrice = (short)(80 * price);
		}
	}
}