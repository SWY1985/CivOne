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

namespace CivOne.Buildings
{
	internal class Bank : BaseBuilding
	{
		public Bank() : base(12, 3)
		{
			Name = "Bank";
			RequiredTech = new Banking();
			SetIcon(2, 0, true);
			SetSmallIcon(1, 4);
			Type = Building.Bank;
		}
	}
}