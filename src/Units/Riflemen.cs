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
using CivOne.Templates;

namespace CivOne.Units
{
	internal class Riflemen : BaseUnitLand, IDefault
	{
		public Riflemen() : base(3, 3, 5, 1)
		{
			Type = Unit.Riflemen;
			Name = "Riflemen";
			RequiredTech = new Conscription();
			ObsoleteTech = null;
			SetIcon('D', 1, 2);
		}
	}
}