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
	internal class NuclearPower : BaseAdvance
	{
		public NuclearPower() : base(Advance.NuclearFission, Advance.Electronics)
		{
			Name = "Nuclear Power";
			Type = Advance.NuclearPower;
			SetIcon(2, 2, 2);
		}
	}
}