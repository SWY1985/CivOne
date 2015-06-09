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
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseBuilding : IBuilding
	{
		private static Picture[,] _iconsCache = new Picture[6, 4], _iconsCacheGrass = new Picture[6, 4];
		
		public virtual Picture Icon { get; protected set; }
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
				output.DrawText(string.Format("Maintenance: ${0}", Maintainance), 6, 12, 12, yy);
			}
			
			return output;
		}
		
		public IAdvance RequiredTech { get; protected set; }
		public byte Price { get; protected set; }
		public byte Maintainance { get; protected set; }
		
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
		
		protected BaseBuilding(byte price = 1, byte maintainance = 0)
		{
			Price = price;
			Maintainance = maintainance;
		}
	}
}