// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Interfaces;

namespace CivOne
{
	internal class Player
	{
		private readonly ICivilization _civilization;
		public ICivilization Civilization
		{
			get
			{
				return _civilization;
			}
		}
		
		public string LeaderName
		{
			get
			{
				// TODO: Implement custom leader name
				return _civilization.LeaderName;
			}
		}
		
		public string TribeName
		{
			get
			{
				// TODO: Implement custom civilization name
				return _civilization.Name;
			}
		}
		
		public string TribeNamePlural
		{
			get
			{
				// TODO: Implement custom civilization name
				return _civilization.NamePlural;
			}
		}
		
		public Player(ICivilization civilization, string customLeaderName = null, string customCivilizationName = null)
		{
			// TODO: Implement custom leader name and custom civilization name
			_civilization = civilization;
		}
	}
}