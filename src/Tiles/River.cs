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
	internal class River : BaseTile
	{
		public River(int x, int y) : base(x, y, false)
		{
			Type = Terrain.River;
			Name = "River";
		}
		public River()
		{
			Type = Terrain.River;
			Name = "River";
			
			Bitmap icon = Resources.Instance.LoadPIC("ICONPGT1", true).GetPart(215, 88, 104, 86);
			Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
			Icon = new Picture(icon);
		}
	}
}