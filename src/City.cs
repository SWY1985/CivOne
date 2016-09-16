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
using System.Threading;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.Screens;
using CivOne.Tasks;
using CivOne.Units;

namespace CivOne
{
	public class City : ITurn
	{
		private Map Map
		{
			get
			{
				return Map.Instance;
			}
		}

		internal byte X;
		internal byte Y;
		private byte _owner;
		internal byte Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
				ResetResourceTiles();
			}
		}
		internal string Name;
		private byte _size;
		internal byte Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
				if (_size == 0)
				{
					Game.Instance.DestroyCity(this);
					Map[X, Y].Road = false;
					Map[X, Y].Irrigation = false;
					return;
				}
				SetResourceTiles();
			}
		}
		internal int Shields { get; private set; }
		internal int Food { get; private set; }
		internal IProduction CurrentProduction { get; private set; }
		private List<ITile> _resourceTiles = new List<ITile>();
		private List<IBuilding> _buildings = new List<IBuilding>();
		public IBuilding[] Buildings
		{
			get
			{
				return _buildings.OrderBy(b => b.Id).ToArray();
			}
		}

		internal int ShieldCosts
		{
			get
			{
				switch (Game.Instance.GetPlayer(_owner).Government)
				{
					case Government.Anarchy:
					case Government.Despotism:
						int costs = 0;
						for (int i = 0; i < Units.Length; i++)
						{
							if (i < _size) continue;
							costs++;
						}
						return costs;
					default:
						return Units.Length;
				} 
			}
		}

		internal int ShieldIncome
		{
			get
			{
				return ResourceTiles.Sum(t => t.Shield) - ShieldCosts;
			}
		}
		
		internal int FoodCosts
		{
			get
			{
				int costs = (_size * 2);
				switch (Game.Instance.GetPlayer(_owner).Government)
				{
					case Government.Anarchy:
					case Government.Despotism:
						costs += Units.Count(u => (u is Settlers));
						break;
					default:
						costs += (Units.Count(u => (u is Settlers)) * 2);
						break;
				} 
				return costs;
			}
		}

		internal int FoodIncome
		{
			get
			{
				return ResourceTiles.Sum(t => t.Food) - FoodCosts;
			}
		}

		internal int FoodTotal
		{
			get
			{
				return ResourceTiles.Sum(t => t.Food);
			}
		}

		internal int ShieldTotal
		{
			get
			{
				return ResourceTiles.Sum(t => t.Shield);
			}
		}

		internal int TradeTotal
		{
			get
			{
				return ResourceTiles.Sum(t => t.Trade);
			}
		}

		internal short Luxuries
		{
			get
			{
				return (short)Math.Round(((double)(TradeTotal - Science) / (10 - Player.ScienceRate)) * Player.LuxuriesRate);
			}
		}

		internal short Taxes
		{
			get
			{
				short taxes = (short)(TradeTotal - Luxuries - Science);
				if (_buildings.Any(b => (b is MarketPlace))) taxes += (short)Math.Floor((double)taxes / 2);
				return taxes;
			}
		}

		internal short Science
		{
			get
			{
				short science = (short)Math.Round(((double)TradeTotal / 10) * Player.ScienceRate);
				if (_buildings.Any(b => (b is Library))) science += (short)Math.Floor((double)science / 2);
				return science;
			}
		}

		internal IEnumerable<ITile> ResourceTiles
		{
			get
			{
				return CityTiles.Where(t => (t.X == X && t.Y == Y) || _resourceTiles.Contains(t));
			}
		}

		internal bool OccupiedTile(ITile tile)
		{
			if (ResourceTiles.Any(t => t.X == tile.X && t.Y == tile.Y))
				return false;
			return (tile.City != null || Game.Instance.GetCities().Any(c => c.ResourceTiles.Any(t => t.X == tile.X && t.Y == tile.Y)));
		}

		private void SetResourceTiles()
		{
			while (_resourceTiles.Count > Size)
				_resourceTiles.RemoveAt(_resourceTiles.Count - 1);
			if (_resourceTiles.Count == Size) return;
			if (_resourceTiles.Count < Size)
			{
				IEnumerable<ITile> tiles = CityTiles.Where(t => !OccupiedTile(t) && !ResourceTiles.Contains(t)).OrderByDescending(t => t.Food).ThenByDescending(t => t.Shield).ThenByDescending(t => t.Trade);
				if (tiles.Count() > 0)
					_resourceTiles.Add(tiles.First());
			}
		}

		private void ResetResourceTiles()
		{
			_resourceTiles.Clear();
			for (int i = 0; i < Size; i++)
				SetResourceTiles();
		}

		public void SetResourceTile(ITile tile)
		{
			if (tile == null || OccupiedTile(tile) || !CityTiles.Contains(tile) || (tile.X == X && tile.Y == Y) || (_resourceTiles.Count >= Size && !_resourceTiles.Contains(tile)))
			{
				ResetResourceTiles();
				return;
			}
			if (_resourceTiles.Contains(tile))
			{
				_resourceTiles.Remove(tile);
				return;
			}
			_resourceTiles.Add(tile);
		}

		private Player Player
		{
			get
			{
				return Game.Instance.GetPlayer(Owner);
			}
		}

		public IEnumerable<IProduction> AvailableProduction
		{
			get
			{
				foreach (IUnit unit in Reflect.GetUnits().Where(u => Player.ProductionAvailable(u)))
				{
					if (unit.Class == UnitClass.Water && !Map[X, Y].GetBorderTiles().Any(t => t.IsOcean)) continue;
					yield return unit;
				}
				foreach (IBuilding building in Reflect.GetBuildings().Where(b => Player.ProductionAvailable(b) && !_buildings.Any(x => x.Id == b.Id)))
				{
					yield return building;
				}
			}
		}

		public void SetProduction(IProduction production)
		{
			CurrentProduction = production;
		}

		public void Buy()
		{
			// DEBUG CODE
			Shields = (int)CurrentProduction.Price * 10;
		}

		public int Population
		{
			get
			{
				int output = 0;
				for (int i = 1; i <= Size; i++)
				{
					output += 10000 * i;
				}
				return output;
			}
		}

		private IEnumerable<ITile> CityTiles
		{
			get
			{
				ITile[,] tiles = CityRadius;
				for (int xx = 0; xx < 5; xx++)
				for (int yy = 0; yy < 5; yy++)
				{
					if (tiles[xx, yy] == null) continue;
					yield return tiles[xx, yy];
				}
			}
		}

		public ITile[,] CityRadius
		{
			get
			{
				Player player = Game.Instance.GetPlayer(Owner);
				ITile[,] tiles = Map[X - 2, Y - 2, 5, 5];
				for (int xx = 0; xx < 5; xx++)
				for (int yy = 0; yy < 5; yy++)
				{
					ITile tile = tiles[xx, yy];
					if (tile == null) continue;
					if ((xx == 0 || xx == 4) && (yy == 0 || yy == 4)) tiles[xx, yy] = null;
					if (!player.Visible(tile)) tiles[xx, yy] = null;
				}
				return tiles;
			}
		}

		public IUnit[] Units
		{
			get
			{
				return Game.Instance.GetUnits().Where(u => u.Home == this).ToArray();
			}
		}

		public ITile Tile
		{
			get
			{
				return Map[X, Y];
			}
		}

		public void AddBuilding(IBuilding building)
		{
			_buildings.Add(building);
		}

		public void NewTurn()
		{
			Food += FoodIncome;
			if (Food < 0)
			{
				Food = 0;
				Size--;
				if (Owner == Game.Instance.PlayerNumber(Game.Instance.HumanPlayer))
				{
					GameTask.Enqueue(Message.Newspaper(this, "Food storage exhausted", $"in {Name}!", "Famine feared."));
				}
				if (Size == 0) return;
			}
			else if (Food >= (int)(Size + 1) * 10)
			{
				Food -= ((int)(Size + 1) * 10);
				if (Size == 10 && !_buildings.Any(b => b.Id == (int)Building.Aqueduct))
				{
					Common.AddScreen(new AdvisorMessage(Advisor.Domestic, $"{Name} requires an AQUADUCT", "for further growth."));
				}
				else
				{
					Size++;
				}
			}

			if (ShieldIncome > 0)
				Shields += ShieldIncome;
			if (Shields >= (int)CurrentProduction.Price * 10)
			{
				if (CurrentProduction is IUnit)
				{
					if (CurrentProduction is Settlers)
					{
						//TODO: This should do something to the food supply
						if (Size == 1) Size++;
						Size--;
					}
					Shields = 0;
					IUnit unit = Game.Instance.CreateUnit((CurrentProduction as IUnit).Type, X, Y, Owner);
					unit.SetHome(this);
					if ((unit is Settlers) || (unit is Diplomat) || (unit is Caravan))
					{
						AdvisorMessage advisorMessage = new AdvisorMessage(Advisor.Defense, $"{this.Name} builds {unit.Name}.");
						advisorMessage.Closed += (s, a) => Common.AddScreen(new CityManager(this));
						Common.AddScreen(advisorMessage);
					}
				}
				if (CurrentProduction is IBuilding && !_buildings.Any(b => b.Id == (CurrentProduction as IBuilding).Id))
				{
					Shields = 0;
					_buildings.Add(CurrentProduction as IBuilding);
					GameTask.Enqueue(new ImprovementBuilt(this, (CurrentProduction as IBuilding)));
				}
			}

			// TODO: Handle luxuries
			Player.Gold += Taxes;
			Player.Science += Science;
			GameTask.Enqueue(new ProcessScience(Player));
		}

		internal City()
		{
			CurrentProduction = new Militia();
			SetResourceTiles();
		}
	}
}