// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.GFX;

namespace CivOne.Templates
{
	public abstract partial class BaseScreen
	{
		protected void DrawBorder(int border)
		{
			border = (border % 2);
			Picture[] borders = new Picture[8];
			int index = 0;
			for (int yy = 0; yy < 2; yy++)
			for (int xx = 0; xx < 4; xx++)
			{
				borders[index] = Resources.Instance.GetPart("SP299", ((border == 0) ? 192 : 224) + (8 * xx), 120 + (8 * yy), 8, 8);
				index++;
			}
			
			for (int x = 8; x < 312; x += 8)
			{
				AddLayer(borders[4], x, 0);
				AddLayer(borders[6], x, 192);
			}
			for (int y = 8; y < 192; y += 8)
			{
				AddLayer(borders[7], 0, y);
				AddLayer(borders[5], 312, y);
			}
			AddLayer(borders[0], 0, 0);
			AddLayer(borders[1], 312, 0);
			AddLayer(borders[2], 0, 192);
			AddLayer(borders[3], 312, 192);
		}
	}
}