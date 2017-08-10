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

namespace CivOne.Wonders
{
	internal class GreatLibrary : BaseWonder
	{
		public GreatLibrary() : base(30)
		{
			Name = "Great Library";
			RequiredTech = new Literacy();
			ObsoleteTech = new University();
			SetSmallIcon(5, 0);
			Type = Wonder.GreatLibrary;
		}
	}
}