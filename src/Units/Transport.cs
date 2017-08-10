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
using CivOne.Interfaces;

namespace CivOne.Units
{
	internal class Transport : BaseUnitSea, IBoardable
	{
		public int Cargo
		{
			get
			{
				return 8;
			}
		}

		public Transport() : base(5, 0, 3, 4)
		{
			Type = Unit.Transport;
			Name = "Transport";
			RequiredTech = new Industrialization();
			ObsoleteTech = null;
			SetIcon('A', 0, 2);
		}
	}
}