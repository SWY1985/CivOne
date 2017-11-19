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
using System.Drawing;
using System.Linq;
using CivOne.Buildings;
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Screens;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.Wonders;

namespace CivOne
{
	public class Game : BaseInstance
	{
	    private GameState _gs;

	    public GameState GameState => _gs;

		internal IEnumerable<Player> Players
		{
			get
			{
				foreach (Player player in _gs.Players)
					yield return player;
			}
		}

		public void EndTurn()
		{
			if (++_gs.CurrentPlayerId >= _gs.Players.Length)
			{
				_gs.CurrentPlayerId = 0;
				_gs._gameTurn++;

				if (_gs._gameTurn % 50 == 0 && Settings.AutoSave)
				{
					GameTask.Enqueue(Show.AutoSave);
				}
			}

			if (_gs.CurrentPlayer.DestroyTurn == -1 && _gs.CurrentPlayer.IsDestroyed)
			{
				GameTask.Enqueue(Message.Advisor(Advisor.Defense, false, _gs.CurrentPlayer.Civilization.Name, "civilization", "destroyed", $"by {_gs.GetPlayer(0).Civilization.NamePlural}!"));
			}

			if (!_gs.Players.Any(x => _gs.PlayerNumber(x) != 0 && x != Human && !x.IsDestroyed))
			{
				GameTask conquest;
				GameTask.Enqueue(Message.Newspaper(null, "Your civilization", "has conquered", "the entire planet!"));
				GameTask.Enqueue(conquest = Show.Screen<Conquest>());
				conquest.Done += (s, a) => Runtime.Quit();
			}

			foreach (IUnit unit in _gs.Units.Where(u => u.Owner == _gs.CurrentPlayerId))
			{
				GameTask.Enqueue(Turn.New(unit));
			}

			foreach (City city in _gs.Cities.Where(c => c.Owner == _gs.CurrentPlayerId).ToArray())
			{
				GameTask.Enqueue(Turn.New(city));
			}

			GameTask.Enqueue(Turn.New(_gs.CurrentPlayer));
		}

        public void Update()
		{
			IUnit unit = _gs.ActiveUnit;

			if (_gs.CurrentPlayer == _gs.HumanPlayer)
			{
				if (unit != null && !unit.Goto.IsEmpty)
				{
					int distance = unit.Tile.DistanceTo(unit.Goto);
					ITile[] tiles = (unit as BaseUnit).MoveTargets.OrderBy(x => x.DistanceTo(unit.Goto)).ThenBy(x => x.Movement).ToArray();
					if (tiles.Length == 0 || tiles[0].DistanceTo(unit.Goto) > distance)
					{
						// No valid tile to move to, cancel goto
						unit.Goto = Point.Empty;
						return;
					}

                    if (tiles[0].DistanceTo(unit.Goto) == distance)
					{
						// Distance is unchanged, 50% chance to cancel goto
						if (Common.Random.Next(0, 100) < 50)
						{
							unit.Goto = Point.Empty;
							return;
						}
					}

					unit.MoveTo(tiles[0].X - unit.X, tiles[0].Y - unit.Y);
					return;
				}
				return;
			}
			if (unit != null && (unit.MovesLeft > 0 || unit.PartMoves > 0))
			{
				GameTask.Enqueue(Turn.Move(unit));
				return;
			}
			GameTask.Enqueue(Turn.End());
		}

		private int GetCityIndex(ICivilization civilization)
		{
			// TODO: This should be a lot easier... let me think about it...

			int indexFrom = Array.IndexOf(_gs.CityNames, civilization.CityNames[0]); //CityNames.IndexOf(civilization.CityNames[0]);
			int indexTo = civilization.CityNames.Length + indexFrom;
			for (int i = indexFrom; i < indexTo; i++)
			{
				if (_gs.CityNameUsed[i]) continue;
				return i;
			}
			
			civilization = _gs.Players[0].Civilization;
			indexFrom = Array.IndexOf(_gs.CityNames, civilization.CityNames[0]);
			indexTo = civilization.CityNames.Length + indexFrom;
			for (int i = indexFrom; i < indexTo; i++)
			{
				if (_gs.CityNameUsed[i]) continue;
				return i;
			}

			for (int i = 0; i < _gs.CityNames.Length; i++)
			{
				if (_gs.CityNameUsed[i]) continue;
				return i;
			}

			return 0;
		}

		internal string CityName(Player player)
		{
			ICivilization civilization = player.Civilization;
			int index = GetCityIndex(civilization);
			_gs.CityNameUsed[index] = true;
			return _gs.CityNames[index];
		}

		internal City AddCity(Player player, string name, int x, int y)
		{
			if (_gs.Cities.Any(c => c.X == x && c.Y == y))
				return null;

			City city = new City(_gs.PlayerNumber(player))
			{
				X = (byte)x,
				Y = (byte)y,
				Name = name,
				Size = 1
			};
			if (!_gs.Cities.Any(c => c.Size > 0 && c.Owner == city.Owner))
			{
				Palace palace = new Palace();
				palace.SetFree();
				city.AddBuilding(palace);
			}
			if ((Map[x, y] is Desert) || (Map[x, y] is Grassland) || (Map[x, y] is Hills) || (Map[x, y] is Plains) || (Map[x, y] is River))
			{
				Map[x, y].Irrigation = true;
			}
			if (!Map[x, y].RailRoad)
			{
				Map[x, y].Road = true;
			}
			_gs.Cities.Add(city);
			Game.UpdateResources(city.Tile);
			return city;
		}

		public void DestroyCity(City city)
		{
			foreach (IUnit unit in _gs.Units.Where(u => u.Home == city).ToArray())
				_gs.Units.Remove(unit);
			city.X = 255;
			city.Y = 255;
			city.Owner = 0;
		}
		
		internal City GetCity(int x, int y)
		{
			while (x < 0)
                x += Map.WIDTH;
			while (x >= Map.WIDTH) x-= Map.WIDTH;

			if (y < 0)
                return null;

			if (y >= Map.HEIGHT)
                return null;

			return _gs.Cities.FirstOrDefault(c => c.X == x && c.Y == y && c.Size > 0);
		}

		public IUnit CreateUnit(UnitType type, int x, int y, byte owner, bool endTurn = false)
		{
			IUnit unit = UnitsFactory.CreateUnit((UnitType)type, x, y);
			if (unit == null)
                return null;

			unit.Owner = owner;
			if (unit.Class == UnitClass.Water)
			{
				Player player = _gs.GetPlayer(owner);
				if ((player.HasWonder<Lighthouse>() && !WonderObsolete<Lighthouse>()) ||
					(player.HasWonder<MagellansExpedition>() && !WonderObsolete<MagellansExpedition>()))
				{
					unit.MovesLeft++;
				}
			}

            if (endTurn)
				unit.SkipTurn();

            _instance._gs.Units.Add(unit);

            return unit;
		}

		internal void UpdateResources(ITile tile, bool ownerCities = true)
		{
		    for (int relY = -3; relY <= 3; relY++)
		    {
		        for (int relX = -3; relX <= 3; relX++)
		        {
		            if (tile[relX, relY] == null) continue;
		            City city = tile[relX, relY].City;
		            if (city == null) continue;
		            if (!ownerCities && _gs.CurrentPlayer == city.Owner) continue;
		            city.UpdateResources();
		        }
		    }
		}

		public IWonder[] BuiltWonders
		{
			get
			{
				return _gs.Cities.SelectMany(c => c.Wonders).ToArray();
			}
		}

		public bool WonderBuilt<T>() where T : IWonder
		{
			return BuiltWonders.Any(w => w is T);
		}

		public bool WonderBuilt(IWonder wonder)
		{
			return BuiltWonders.Any(w => w.Id == wonder.Id);
		}

		public bool WonderObsolete<T>() where T : IWonder, new()
		{
			return WonderObsolete(new T());
		}

		public bool WonderObsolete(IWonder wonder)
		{
			return (wonder.ObsoleteTech != null && _gs.Players.Any(x => x.HasAdvance(wonder.ObsoleteTech)));
		}
		
		public void DisbandUnit(IUnit unit)
		{
			IUnit activeUnit = _gs.ActiveUnit;

			if (unit == null) return;
			if (!_gs.Units.Contains(unit)) return;
			if (unit.Tile is Ocean && unit is IBoardable)
			{
				int totalCargo = unit.Tile.Units.Where(u => u is IBoardable).Sum(u => (u as IBoardable).Cargo) - (unit as IBoardable).Cargo;
				while (unit.Tile.Units.Count(u => u.Class != UnitClass.Water) > totalCargo)
				{
					IUnit subUnit = unit.Tile.Units.First(u => u.Class != UnitClass.Water);
					subUnit.X = 255;
					subUnit.Y = 255;
					_gs.Units.Remove(subUnit);
				} 
			}
			unit.X = 255;
			unit.Y = 255;
			_gs.Units.Remove(unit);

			if (_gs.Units.Contains(activeUnit))
			{
				_gs.ActiveUnitId = _gs.Units.IndexOf(activeUnit);
			}
		}

		public void UnitWait()
		{
			_gs.ActiveUnitId++;
		}

		public IUnit MovingUnit
		{
			get
			{
				return _gs.Units.FirstOrDefault(u => u.Moving);
			}
		}

	    public static void CreateGame(GameState gameState)
	    {
	        if (_instance != null)
	        {
	            Logger.Log("ERROR: Game instance already exists");
	            return;
	        }

	        _instance = new Game(gameState);

            foreach (IUnit unit in _instance._gs.Units)
	        {
	            unit.Explore();
	        }
	    }
        
        public static bool Started
		{
			get
			{
				return (_instance != null);
			}
		}

	    internal static Game _instance;
		public static Game Instance
		{
			get
			{
				if (_instance == null)
				{
					Logger.Log("ERROR: Game instance does not exist");
				}

				return _instance;
			}
		}

	    private Game(GameState gameState)
	    {
	        _gs = gameState;
	    }        
	}
}
