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
	internal class Jungle : BaseTile
	{
		public Jungle(int x, int y, bool special) : base(x, y, special)
		{
			Type = Terrain.Jungle;
			Name = "Jungle";
		}
		public Jungle()
		{
			Name = "Jungle";
			
			Bitmap icon = Resources.Instance.LoadPIC("ICONPGT1", true).GetPart(1, 88, 108, 86);
			Picture.ReplaceColours(icon, 253, 0);
			Icon = new Picture(icon);
			Icon.FillRectangle(0, 106, 0, 2, 86);
		}
	}
}