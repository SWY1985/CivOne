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
		private readonly string _leaderName, _tribeName, _tribeNamePlural;

		private readonly bool[,] _explored = new bool[Map.WIDTH, Map.HEIGHT];
		private readonly bool[,] _visible = new bool[Map.WIDTH, Map.HEIGHT];
		
		private short _gold;
				
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
				return _leaderName;
			}
		}
		
		public string TribeName
		{
			get
			{
				return _tribeName;
			}
		}
		
		public string TribeNamePlural
		{
			get
			{
				return _tribeNamePlural;
			}
		}
		
		public short Gold
		{
			get
			{
				return _gold;
			}
			internal set
			{
				_gold = value;
			}
		}
		
		public string LatestAdvance
		{
			get
			{
				return "Irrigation";
			}
		}

		public void Explore(int x, int y, int range = 1)
		{
			_explored[x, y] = true;
			for (int relX = -range; relX <= range; relX++)
			for (int relY = -range; relY <= range; relY++)
			{
				int xx = x + relX;
				int yy = y + relY;
				if (yy < 0 || yy >= Map.HEIGHT) continue;
				while (xx < 0) xx += Map.WIDTH;
				while (xx >= Map.WIDTH) xx -= Map.WIDTH;
				_visible[xx, yy] = true;
			} 
		}

		public bool Visible(int x, int y)
		{
			if (y < 0 || y >= Map.HEIGHT) return false;
			while (x < 0) x += Map.WIDTH;
			while (x >= Map.WIDTH) x -= Map.WIDTH;
			return _visible[x, y];
		}
		
		public Player(ICivilization civilization, string customLeaderName = null, string customTribeName = null, string customTribeNamePlural = null)
		{
			_civilization = civilization;
			_leaderName = customLeaderName ?? _civilization.LeaderName;
			_tribeName = customTribeName ?? _civilization.Name;
			_tribeNamePlural = customTribeNamePlural ?? _civilization.NamePlural;
			
			for (int xx = 0; xx < Map.WIDTH; xx++)
			for (int yy = 0; yy < Map.HEIGHT; yy++)
			{
				_explored[xx, yy] = false;
				_visible[xx, yy] = false;
			}
		}
	}
}