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
using CivOne.Templates;

namespace CivOne.Units
{
	internal class Artillery : BaseUnit
	{
		public Artillery() : base(6, 12, 2, 2)
		{
			Class = UnitClass.Land;
			Type = Unit.Artillery;
			Name = "Artillery";
			RequiredTech = null;
			ObsoleteTech = null;
			SetIcon('B', 0, 0);
		}
	}
}