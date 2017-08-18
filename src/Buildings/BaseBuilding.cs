// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Advances;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.IO;

namespace CivOne.Buildings
{
	internal abstract class BaseBuilding : BaseInstance, IBuilding
	{
		private static IBitmap[,] _iconsCache = new IBitmap[6, 4], _iconsCacheGrass = new IBitmap[6, 4];
		
		private IBitmap GrassIcon => Resources["CITYPIX2"]
										.GetPart(250, 0, 50, 50)
										.ColourReplace(1, 0);
		
		public virtual IBitmap Icon { get; protected set; }
		public virtual IBitmap SmallIcon { get; protected set; }
		public string Name { get; protected set; }
		public byte PageCount => 2;
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
					Log("Invalid page number: {0}", pageNumber);
					break;
			}
			
			Picture output = new Picture(320, 200);
			
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
		public byte ProductionId => (byte)(255 - Type);
		public byte Price { get; protected set; }
		public byte Maintenance { get; protected set; }
		
		protected void SetIcon(int col, int row, bool grassTile)
		{
			if ((grassTile && _iconsCacheGrass[col, row] == null) || (!grassTile && _iconsCache[col, row] == null))
			{
				Icon = new Picture(50, 50, Resources["CITYPIX2"].Palette);
				
				if (grassTile)
					Icon.AddLayer(GrassIcon);
				
				Icon.AddLayer(Resources["CITYPIX2"]
								.GetPart(col * 50, row * 50, 50, 50)
								.ColourReplace(1, 0));
				
				if (grassTile) _iconsCacheGrass[col, row] = Icon;
				else _iconsCache[col, row] = Icon;
			}
			Icon = (grassTile ? _iconsCacheGrass[col, row] : _iconsCache[col, row]);
		}
		
		protected void SetSmallIcon(int col, int row)
		{
			SmallIcon = Resources[Settings.GraphicsMode == GraphicsMode.Graphics256 ? "SP299" : "SPRITES"]
				.GetPart(160 + (19 * col), 50 + (10 * row), 20, 10)
				.ColourReplace(0, 5)
				.FillRectangle(0, 0, 1, 10, 0)
				.FillRectangle(19, 0, 1, 10, 0);
		}
		
		public byte Id => (byte)Type;
		
		protected BaseBuilding(byte price = 1, byte maintenance = 0)
		{
			Price = price;
			Maintenance = maintenance;
			BuyPrice = (short)(40 * price);
			SellPrice = (short)(10 * price);
		}
	}
}