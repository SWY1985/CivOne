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
using CivOne.Buildings;
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Governments;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.Wonders;

using Gov = CivOne.Governments;

namespace CivOne
{
	public class Player : BaseInstance, ITurn
	{
		public class PalaceData
		{
			private byte[] PalaceStyle = new byte[7];
			private byte[] PalaceLevel = new byte[7];
			private byte[] GardenLevel = new byte[3];

			public int PalaceLeft
			{
				get
				{
					for (int i = 0; i < 3; i++)
					{
						if (PalaceLevel[i] > 0) return i;
					}
					return 2;
				}
			}

			public int PalaceRight
			{
				get
				{
					for (int i = 6; i > 3; i--)
					{
						if (PalaceLevel[i] > 0) return i;
					}
					return 4;
				}
			}

			public PalaceStyle GetPalaceStyle(int index)
			{
				if (index < 0 || index > 6) throw new Exception("Invalid palace index");
				return (PalaceStyle)PalaceStyle[index];
			}

			public byte GetPalaceLevel(int index)
			{
				if (index < 0 || index > 6) throw new Exception("Invalid palace index");
				return PalaceLevel[index];
			}

			public byte GetGardenLevel(int index)
			{
				if (index < 0 || index > 2) throw new Exception("Invalid garden index");
				return GardenLevel[index];
			}

			public void SetPalace(int index, byte style, byte level)
			{
				if (index < 0 || index > 6)
					throw new Exception("Invalid palace index");
				if (style < 0 || style > 3)
					throw new Exception("Invalid palace style");
				if (level < 0 || level > 4)
					throw new Exception("Invalid palace level");
				
				if (level == 0 || style == 0)
				{
					PalaceStyle[index] = 0;
					PalaceLevel[index] = 0;
					return;
				}
				PalaceStyle[index] = style;
				PalaceLevel[index] = level;
			}

			public void SetGarden(int index, byte level)
			{
				if (index < 0 || index > 2) throw new Exception("Invalid garden index");
				if (level < 0 || level > 3) throw new Exception("Invalid garden level");
				
				GardenLevel[index] = level;
			}
		}

		private readonly ICivilization _civilization;
		private readonly string _tribeName, _tribeNamePlural;

		private readonly bool[,] _explored = new bool[Map.WIDTH, Map.HEIGHT];
		private readonly bool[,] _visible = new bool[Map.WIDTH, Map.HEIGHT];
		private readonly List<byte> _advances = new List<byte>();
		
		private short _anarchy = 0;
		private short _gold;
		private IAdvance _currentResearch = null;

		private int _destroyTurn = -1;

		internal short StartX { get; set; }
		
		internal bool AnarchyDespotism
		{
			get
			{
				if (!Game.Started)
					return false;
				return (Government is Anarchy || Government is Despotism);
			}
		}

		internal bool MonarchyCommunist
		{
			get
			{
				if (!Game.Started)
					return false;
				return (Government is Gov.Monarchy || Government is Gov.Communism);
			}
		}

		internal bool RepublicDemocratic
		{
			get
			{
				if (!Game.Started)
					return false;
				return (Government is Republic || Government is Gov.Democracy);
			}
		}

		public ICivilization Civilization => _civilization;
		
		public string LeaderName => _civilization.Leader.Name;
		public string TribeName => _tribeName;
		public string TribeNamePlural => _tribeNamePlural;

		public byte Handicap { get; internal set; }

		public readonly PalaceData Palace = new PalaceData();
		
		private IGovernment _government;
		public IGovernment Government
		{
			get
			{
				return _government;
			}
			internal set
			{
				if (value == null) return;
				_government = value;
			}
		}

		private int _luxuriesRate = 0, _taxesRate = 5, _scienceRate = 5;
		public int LuxuriesRate
		{
			get
			{
				return _luxuriesRate;
			}
			set
			{
				int diff = _luxuriesRate - value;
				_luxuriesRate = value;
				_scienceRate += diff;
			}
		}
		public int TaxesRate
		{
			get
			{
				return _taxesRate;
			}
			set
			{
				int diff = _taxesRate - value;
				_taxesRate = value;
				_scienceRate += diff;
			}
		}
		public int ScienceRate => _scienceRate;

		public void Revolt()
		{
			_anarchy = (short)((HasWonder<Pyramids>() && !Game.WonderObsolete<Pyramids>()) ? 0 : 4 - (Game.GameTurn % 4) - 1);
			Government = new Anarchy();
			if (!IsHuman) return;
			GameTask.Enqueue(Message.Newspaper(null, $"The {Game.Instance.HumanPlayer.TribeNamePlural} are", "revolting! Citizens", "demand new govt."));
		}

		public bool IsHuman => (Game.Instance.HumanPlayer == this);

		public City[] Cities => Game.Instance.GetCities().Where(c => this == c.Owner && c.Size > 0).ToArray();

		public int Population => Cities.Sum(c => c.Population);
		
		public short Gold
		{
			get
			{
				return _gold;
			}
			internal set
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

		internal short ScienceCost
		{
			get
			{
				short cost = (short)((Game.Instance.Difficulty + 3) * 2 * (_advances.Count() + 1) * (Common.TurnToYear(Game.Instance.GameTurn) > 0 ? 2 : 1));
				if (cost < 12)
					return 12;
				return cost;
			}
		}
		
		public short Science { get; internal set; }

		public void AddAdvance(IAdvance advance, bool setOrigin = true)
		{
			if (Game.Started && Game.CurrentPlayer.CurrentResearch?.Id == advance.Id)
				GameTask.Enqueue(new TechSelect(Game.CurrentPlayer));
			_advances.Add(advance.Id);
			if (!setOrigin) return;
			Game.Instance.SetAdvanceOrigin(advance, this);
		}

		public void DeleteAdvance(IAdvance advance)
		{
			_advances.RemoveAll(x => x == advance.Id);
		}
		
		public string LatestAdvance
		{
			get
			{
				if (_advances.Count == 0)
					return "Irrigation";
				return Reflect.GetAdvances().First(a => a.Id == _advances.Last()).Name;
			}
		}

		public IAdvance[] Advances
		{
			get
			{
				return _advances.Select(a => Common.Advances.First(x => x.Id == a)).ToArray();
			}
		}
		
		public bool HasAdvance<T>() where T : IAdvance
		{
			return Advances.Any(a => a is T);
		}

		public bool HasAdvance(IAdvance advance)
		{
			return (advance == null || Advances.Any(a => a.Id == advance.Id));
		}

		public IAdvance CurrentResearch
		{
			get
			{
				return _currentResearch;
			}
			set
			{
				_currentResearch = value;
			}
		}

		public IEnumerable<IAdvance> AvailableResearch
		{
			get
			{
				foreach (IAdvance advance in Common.Advances.Where(a => !_advances.Contains(a.Id)))
				{
					if (advance.RequiredTechs.Length > 0 && !advance.RequiredTechs.All(a => _advances.Contains(a.Id))) continue;
					yield return advance;
				}
			}
		}

		public IEnumerable<IGovernment> AvailableGovernments
		{
			get
			{
				bool allGovernments = !Game.WonderObsolete<Pyramids>() && HasWonder<Pyramids>();
				foreach (IGovernment government in Reflect.GetGovernments().Where(g => g.Id > 0))
				{
					if (!allGovernments && !HasAdvance(government.RequiredTech)) continue;
					yield return government; 
				}
			}
		}

		private bool UnitAvailable(IUnit unit)
		{
			// Determine if the unit is obsolete
			if (_advances.Any(a => unit.ObsoleteTech != null && unit.ObsoleteTech.Id == a))
				return false;
			
			// Require Manhattan Project to be built for Nuclear unit
			if ((unit is Nuclear) && !Game.Instance.WonderBuilt<ManhattanProject>())
				return false;
			
			// Determine if the unit requires a tech
			if (unit.RequiredTech == null)
				return true;
			
			// Determine if the Player has the required tech
			if (_advances.Any(a => unit.RequiredTech.Id == a))
				return true;
			
			return false;
		}

		private bool BuildingAvailable(IBuilding building)
		{
			// Only allow spaceship to be built if Apollo Program exists
			if ((building is ISpaceShip) && !Game.Instance.WonderBuilt<ApolloProgram>())
				return false;

			// Determine if the building requires a tech
			if (building.RequiredTech == null)
				return true;
			
			// Determine if the Player has the required tech
			if (_advances.Any(a => building.RequiredTech.Id == a))
				return true;
			
			return false;
		}

		private bool WonderAvailable(IWonder wonder)
		{
			// Determine if the wonder has already been built
			if (Game.Instance.BuiltWonders.Any(w => w.Id == wonder.Id))
				return false;

			// Determine if the building requires a tech
			if (wonder.RequiredTech == null)
				return true;
			
			// Determine if the Player has the required tech
			if (_advances.Any(a => wonder.RequiredTech.Id == a))
				return true;
			
			return false;
		}
		
		public bool HasWonder<T>() where T : IWonder => Cities.Any(c => c.HasWonder<T>());

		public bool ProductionAvailable(IProduction production)
		{
			if (production is IUnit)
				return UnitAvailable(production as IUnit);
			if (production is IBuilding)
				return BuildingAvailable(production as IBuilding);
			if (production is IWonder)
				return WonderAvailable(production as IWonder);
			return true;
		}

		public int DestroyTurn => _destroyTurn;

		public bool IsDestroyed
		{
			get
			{
				if (this == 0) return false;
				if (_destroyTurn != -1) return true;
				if (Cities.Length == 0 && !Game.GetUnits().Any(x => this == x.Owner && (x is Settlers && x.Home == null)))
				{
					while (true)
					{
						IUnit unit = Game.GetUnits().FirstOrDefault(x => this == x.Owner);
						if (unit == null) break;
						Game.DisbandUnit(unit);
					}
					_destroyTurn = Game.GameTurn;
					return true;
				}
				return false;
			}
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

		public bool Visible(ITile tile)
		{
			if (tile == null) return false;
			return Visible(tile.X, tile.Y);
		}

		public bool Visible(ITile tile, Direction direction)
		{
			if (tile == null) return false;
			return Visible(tile.GetBorderTile(direction));
		}

		public void NewTurn()
		{
			if (!Game.GetCities().Any(x => this == x.Owner) && !Game.Instance.GetUnits().Any(x => this == x.Owner))
			{
				GameTask.Enqueue(Turn.GameOver(this));
			}

			if (_anarchy == 0 && Government is Anarchy)
			{
				GameTask.Enqueue(Show.ChooseGovernment);
			}
			if (_anarchy > 0) _anarchy--;
		}

		public override bool Equals (object obj)
		{
			if (obj is byte)
				return Game.PlayerNumber(this) == (byte)obj;
			if (obj is Player)
				return Game.PlayerNumber(this) == Game.PlayerNumber(obj as Player);
			return false;
		}
		
		public override int GetHashCode() => Game.PlayerNumber(this);

		public static explicit operator Player(byte playerNumber) => Game.GetPlayer(playerNumber);
		public static explicit operator byte(Player player) => Game.PlayerNumber(player);
		
		public static bool operator ==(Player p1, byte p2) => Game.PlayerNumber(p1) == p2;
		public static bool operator !=(Player p1, byte p2) => Game.PlayerNumber(p1) != p2;
		
		public Player(ICivilization civilization, string customLeaderName = null, string customTribeName = null, string customTribeNamePlural = null)
		{
			_civilization = civilization;
			if (customLeaderName != null) _civilization.Leader.Name = customLeaderName;
			_tribeName = customTribeName ?? _civilization.Name;
			_tribeNamePlural = customTribeNamePlural ?? _civilization.NamePlural;
			Government = new Despotism();
			
			for (int xx = 0; xx < Map.WIDTH; xx++)
			for (int yy = 0; yy < Map.HEIGHT; yy++)
			{
				_explored[xx, yy] = false;
				_visible[xx, yy] = false;
			}
		}
	}
}