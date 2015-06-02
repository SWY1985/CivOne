// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Tiles
{
	internal class Grassland : BaseTile
	{
		private Terrain CalculateTileType()
		{
			if ((((X * 7) + (Y * 11)) & 0x02) == 0)
				return Terrain.Grassland2;
			return Terrain.Grassland1;
		}
		
		public Grassland(int x, int y) : base(x, y, false)
		{
			Type = CalculateTileType();
			Name = "Grassland";
		}
		public Grassland()
		{
			Type = CalculateTileType();
			Name = "Grassland";
			
			Bitmap icon = Resources.Instance.LoadPIC("ICONPGT2", true).GetPart(108, 1, 108, 86);
			Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
			Icon = new Picture(icon);
			Icon.FillRectangle(0, 106, 0, 2, 86);
		}
	}
}