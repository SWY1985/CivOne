// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;

namespace CivOne.Advances
{
	internal class Feudalism : BaseAdvance
	{
		public Feudalism() : base(Advance.Masonry, Advance.Monarchy)
		{
			Name = "Feudalism";
			Type = Advance.Feudalism;
			SetIcon(1, 1, 0);
		}
	}
}