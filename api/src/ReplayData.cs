// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;

namespace CivOne
{
	public abstract class ReplayData
	{
		public class CityBuilt : ReplayData
		{
			public readonly byte OwnerId;
			public readonly int CityId, CityNameId, X, Y;

			public CityBuilt(int turn, byte ownerId, int cityId, int cityNameId, int x, int y) : base(turn)
			{
				OwnerId = ownerId;
				CityId = cityId;
				CityNameId = cityNameId;
				X = x;
				Y = y;
			}
		}

		public class CityDestroyed : ReplayData
		{
			public readonly int CityId, CityNameId, X, Y;
			
			public CityDestroyed(int turn, int cityId, int cityNameId, int x, int y) : base(turn)
			{
				CityId = cityId;
				CityNameId = cityNameId;
				X = x;
				Y = y;
			}
		}

		public class CivilizationDestroyed : ReplayData
		{
			public readonly int DestroyedId, DestroyedById;

			public CivilizationDestroyed(int turn, int destroyedId, int destroyedById) : base(turn)
			{
				DestroyedId = destroyedId;
				DestroyedById = destroyedById;
			}
		}

		public int Turn { get; private set; }

		protected ReplayData(int turn)
		{
			Turn = turn;
		}
	}
}