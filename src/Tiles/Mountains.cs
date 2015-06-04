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
	internal class Mountains : BaseTile
	{
		public Mountains(int x, int y, bool special) : base(x, y, special)
		{
			Type = Terrain.Mountains;
			Name = "Mountains";
		}
		public Mountains() : this(-1, -1, false)
		{
		}
	}
}