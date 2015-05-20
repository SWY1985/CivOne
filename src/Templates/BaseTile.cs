// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseTile : ITile
	{
		public Terrain Type { get; protected set; }
		public string Name { get; protected set; }
		public int X { get; private set; }
		public int Y { get; private set; }
		public bool Special { get; private set; }
		
		public BaseTile(int x, int y, bool special = false)
		{
			X = x;
			Y = y;
			Special = special;
		}
	}
}