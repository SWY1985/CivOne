// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.Templates
{
	public abstract class BaseInstance
	{
		protected static Game Game
		{
			get
			{
				return Game.Instance;
			}
		}

		protected static Map Map
		{
			get
			{
				return Map.Instance;
			}
		}

		protected static Player Human
		{
			get
			{
				return Game.Instance.HumanPlayer;
			}
		}

		protected static Settings Settings
		{
			get
			{
				return Settings.Instance;
			}
		}
	}
}