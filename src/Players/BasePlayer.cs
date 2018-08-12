// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Linq;
using CivOne.Advances;
using CivOne.Civilizations;
using CivOne.Governments;
using CivOne.Leaders;
using CivOne.Tasks;
using CivOne.Wonders;

namespace CivOne.Players
{
	internal abstract class BasePlayer : BaseInstance, IPlayer
	{
		private readonly bool[,] _explored = new bool[Map.WIDTH, Map.HEIGHT];
		private readonly bool[,] _visible = new bool[Map.WIDTH, Map.HEIGHT];
		private readonly List<byte> _advances = new List<byte>();
		private readonly List<byte> _embassies = new List<byte>();

		public ICivilization Civilization { get; }
		public ILeader Leader => Civilization.Leader;
		public virtual IGovernment Government { get; set; } = new Despotism();
		public PalaceData Palace { get; } = new PalaceData();

		public IEnumerable<IAdvance> Advances => _advances.Select(a => Common.Advances.First(x => x.Id == a));
		public IEnumerable<IPlayer> Embassies => _embassies.OrderBy(x => x).Select(e => Game.Instance.GetPlayer(e));

		public IAdvance CurrentResearch { get; set; }
		
		private int _luxuriesRate = 0, _taxesRate = 5, _scienceRate = 5;
		public int LuxuriesRate
		{
			get => _luxuriesRate;
			set
			{
				int diff = _luxuriesRate - value;
				_luxuriesRate = value;
				_scienceRate += diff;
			}
		}
		public int TaxesRate
		{
			get => _taxesRate;
			set
			{
				int diff = _taxesRate - value;
				_taxesRate = value;
				_scienceRate += diff;
			}
		}
		public int ScienceRate => _scienceRate;

		private short _gold;
		public short Gold
		{
			get => _gold;
			set
			{
				if (value < 0)
				{
					//TODO: Implement sold improvements task
					value = 0;
				}
				if (value > 30000)
					value = 30000;
				_gold = value;
			}
		}
		public short Science { get; set; }
		
		public short StartX { get; set; }

		public int CityNamesSkipped { get; set; }

		public byte Handicap { get; set; }

		public void AddAdvance(IAdvance advance, bool setOrigin = true)
		{
			if (Game.Started && Game.CurrentPlayer.CurrentResearch?.Id == advance.Id)
				GameTask.Enqueue(new TechSelect(Game.Instance.CurrentPlayer));
			_advances.Add(advance.Id);
			if (!setOrigin) return;
			Game.Instance.SetAdvanceOrigin(advance, this);
		}

		public abstract void ChooseGovernment();

		public void DeleteAdvance(IAdvance advance) => _advances.RemoveAll(x => advance.Id == x);

		public void EstablishEmbassy(IPlayer player)
		{
			byte playerNumber = Game.PlayerNumber(player);
			if (_embassies.Contains(playerNumber)) return;
			_embassies.Add(playerNumber);
		}

		public void Explore(int x, int y, int range = 1, bool sea = false)
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
				if (sea && !Map[xx, yy].IsOcean && (Math.Abs(relX) > 1 || Math.Abs(relY) > 1))
					continue;
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

		public BasePlayer(ICivilization civilization, string leaderName = null, string civilizationName = null, string citizenName = null)
		{
			Civilization = civilization;
			if (leaderName != null) Civilization.Leader.Name = leaderName;
			if (civilizationName != null) Civilization.Name = civilizationName;
			if (citizenName != null) Civilization.NamePlural = citizenName;
			
			for (int yy = 0; yy < Map.HEIGHT; yy++)
			for (int xx = 0; xx < Map.WIDTH; xx++)
			{
				_explored[xx, yy] = false;
				_visible[xx, yy] = false;
			}
		}

		public BasePlayer(BasePlayer player)
		{
			Civilization = player.Civilization;
			
			for (int yy = 0; yy < Map.HEIGHT; yy++)
			for (int xx = 0; xx < Map.WIDTH; xx++)
			{
				_explored[xx, yy] = player._explored[xx, yy];
				_visible[xx, yy] = player._visible[xx, yy];
			}

			_advances.AddRange(player._advances);
			_embassies.AddRange(player._embassies);
			
			Government = player.Government;
			Palace = player.Palace;

			CurrentResearch = player.CurrentResearch;

			_gold = player._gold;
			_luxuriesRate = player._luxuriesRate;
			_scienceRate = player._scienceRate;
			_taxesRate = player._taxesRate;

			Science = player.Science;
			StartX = player.StartX;
			CityNamesSkipped = player.CityNamesSkipped;

			Handicap = player.Handicap;
		}
	}
}