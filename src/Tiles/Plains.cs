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
	internal class Plains : BaseTile
	{
		public Plains(int x, int y, bool special) : base(x, y, special)
		{
			Type = Terrain.Plains;
			Name = "Plains";
		}
		public Plains()
		{
			Name = "Plains";
			
			Bitmap icon = Resources.Instance.LoadPIC("ICONPGT2", true).GetPart(1, 88, 108, 86);
			Picture.ReplaceColours(icon, 253, 0);
			Icon = new Picture(icon);
			Icon.FillRectangle(0, 106, 0, 2, 86);
		}
	}
}