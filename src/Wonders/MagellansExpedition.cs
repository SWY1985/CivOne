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
	internal class MagellansExpedition : BaseWonder
	{
		public MagellansExpedition() : base(40)
		{
			Name = "Magellan's Expedition";
			RequiredTech = new Navigation();
			ObsoleteTech = null;
			SetSmallIcon(5, 3);
			Type = Wonder.MagellansExpedition;
		}
	}
}