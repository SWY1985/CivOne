// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Civilizations;
using CivOne.Governments;

namespace CivOne.Players
{
	internal class BarbarianPlayer : BasePlayer
	{
		public override IGovernment Government
		{
			get => new Anarchy();
			set
			{
				// Do nothing, always Anarchy
			}
		}

		public override void ChooseGovernment()
		{
			// Do nothing, always Anarchy
		}

		public BarbarianPlayer() : base(new Barbarian())
		{
			Government = new Anarchy();
		}

		public BarbarianPlayer(HumanPlayer humanPlayer) : base(humanPlayer)
		{
			Government = new Anarchy();
		}
	}
}