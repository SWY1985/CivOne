// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Interfaces;

namespace CivOne.Leaders
{
	public class Atilla : ILeader
	{
		public string Name { get; set; }

		public Picture GetPortrait(FaceState state)
		{
			return new Picture(139, 133, Common.GetPalette256);
		}

		public Picture PortraitSmall
		{
			get
			{
				return new Picture(27, 33, Common.GetPalette256);
			}
		}

		public Atilla()
		{
			Name = "Atilla";
		}
	}
}