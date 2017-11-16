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
using CivOne.Enums;
using CivOne.Governments;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.Wonders;

using UniversityBuilding = CivOne.Buildings.University;

namespace CivOne
{
	public class City : BaseInstance, ITurn
	{
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
				if (X == 255 || Y == 255) return;

				_size = value;
				if (_size == 0)
				{
					Map[X, Y].Road = false;
					Map[X, Y].Irrigation = false;
					if (Game.Started) Game.DestroyCity(this);
					return;
				}
				if (Food > FoodRequired) Food = FoodRequired;
				SetResourceTiles();
			}
		}
		internal int Shields { get; set; }
		internal int Food { get; set; }
		internal IProduction CurrentProduction { get; private set; }
		private List<ITile> _resourceTiles = new List<ITile>();
		private List<IBuilding> _buildings = new List<IBuilding>();
		private List<IWonder> _wonders = new List<IWonder>();

		public IBuilding[] Buildings => _buildings.OrderBy(b => b.Id).ToArray();
		public IWonder[] Wonders => _wonders.OrderBy(b => b.Id).ToArray();

		public bool HasBuilding(IBuilding building) => _buildings.Any(b => b.Id == building.Id);
		public bool HasBuilding(Type type) => _buildings.Any(b => b.GetType() == type);
		public bool HasBuilding<T>() where T : IBuilding => _buildings.Any(b => b is T);

		public bool HasWonder(IWonder wonder) => _wonders.Any(w => w.Id == wonder.Id);
		public bool HasWonder(Type type) => _wonders.Any(w => w.GetType() == type);
		public bool HasWonder<T>() where T : IWonder => _wonders.Any(w => w is T);

		internal int ShieldCosts
		{
			get
			{
				IGovernment government = Game.GetPlayer(_owner).Government;
				if (government is Anarchy || government is Despotism)
				{
					int costs = 0;
					for (int i = 0; i < Units.Count(u => (!(u is Diplomat) && !(u is Caravan))); i++)
					{
						if (i < _size) continue;
						costs++;
					}
					return costs;
				}
				return Units.Count(u => (!(u is Diplomat) && !(u is Caravan)));
			}
		}

		internal int ShieldIncome => ShieldTotal - ShieldCosts;
		
		internal int FoodCosts
		{
			get
			{
				int costs = (_size * 2);
				IGovernment government = Game.GetPlayer(_owner).Government;
				if (government is Anarchy || government is Despotism)
				{
					costs += Units.Count(u => (u is Settlers));
				}
				else
				{
					costs += (Units.Count(u => (u is Settlers)) * 2);
				}
				return costs;
			}
		}

		internal int FoodIncome => ResourceTiles.Sum(t => FoodValue(t)) - FoodCosts;
		internal int FoodRequired => (int)(Size + 1) * 10;
		internal int FoodTotal => ResourceTiles.Sum(t => FoodValue(t));

		internal int FoodValue(ITile tile)
		{
			int output = tile.Food;
			switch (tile.Type)
			{
				case Terrain.Desert:
				case Terrain.Forest:
				case Terrain.Grassland1:
				case Terrain.Grassland2:
				case Terrain.River:
					if (!Player.AnarchyDespotism && tile.Irrigation) output += 1;
					break;
				case Terrain.Ocean:
				case Terrain.Tundra:
					if (!Player.AnarchyDespotism && tile.Special) output += 1;
					break;
			}
			if (tile.RailRoad) output = (int)Math.Floor((double)output * 1.5);
			return output;
		}

		internal int ShieldValue(ITile tile)
		{
			int output = tile.Shield;
			switch (tile.Type)
			{
				case Terrain.Hills:
					if (!Player.AnarchyDespotism && tile.Mine) output += 1;
					break;
			}
			if (tile.RailRoad) output = (int)Math.Floor((double)output * 1.5);
			return output;
		}

		internal int ShieldTotal
		{
			get
			{
				int shields = ResourceTiles.Sum(t => ShieldValue(t));
				if (_buildings.Any(b => (b is Factory))) shields += (short)Math.Floor((double)shields * (_buildings.Any(b => (b is NuclearPlant)) ? 1.0 : 0.5));
				if (_buildings.Any(b => (b is MfgPlant))) shields += (short)Math.Floor((double)shields * 1.0);
				return shields;
			}
		}

		internal int TradeValue(ITile tile)
		{
			int output = tile.Trade;

			if (tile.RailRoad) output = (int)Math.Floor((double)output * 1.5);
			switch (tile.Type)
			{
				case Terrain.Desert:
				case Terrain.Grassland1:
				case Terrain.Grassland2:
				case Terrain.Plains:
					if (!tile.Road) break;
					if (Player.RepublicDemocratic) output += 1;
					break;
				case Terrain.Ocean:
				case Terrain.River:
					if (Player.RepublicDemocratic) output += 1;
					break;
				case Terrain.Jungle:
					if (!tile.Special) break;
					if (Player.MonarchyCommunist) output += 1;
					if (Player.RepublicDemocratic) output += 2;
					break;
				case Terrain.Mountains:
					if (!tile.Special) break;
					if (Player.MonarchyCommunist) output += 1;
					if (Player.RepublicDemocratic) output += 2;
					break;
			}
			if (output > 0 && HasWonder<Colossus>() && !Game.WonderObsolete<Colossus>()) output += 1;
			return output;
		}

		internal int TradeTotal => ResourceTiles.Sum(t => TradeValue(t));
		private short TradeScience => (short)(TradeTotal - TradeLuxuries - TradeTaxes);
		private short TradeLuxuries => (short)Math.Round(((double)(TradeTotal - TradeTaxes) / (10 - Player.TaxesRate)) * Player.LuxuriesRate, MidpointRounding.AwayFromZero);
		private short TradeTaxes => (short)Math.Round(((double)TradeTotal / 10) * Player.TaxesRate, MidpointRounding.AwayFromZero);

		internal short Luxuries
		{
			get
			{
				short luxuries = TradeLuxuries;
				if (HasBuilding<MarketPlace>()) luxuries += (short)Math.Floor((double)luxuries * 0.5);
				if (HasBuilding<Bank>()) luxuries += (short)Math.Floor((double)luxuries * 0.5);
				luxuries += (short)(_specialists.Count(c => c == Citizen.Entertainer) * 2);
				return luxuries;
			}
		}

		internal short Taxes
		{
			get
			{
				short taxes = TradeTaxes;
				if (HasBuilding<MarketPlace>()) taxes += (short)Math.Floor((double)taxes * 0.5);
				if (HasBuilding<Bank>()) taxes += (short)Math.Floor((double)taxes * 0.5);
				taxes += (short)(_specialists.Count(c => c == Citizen.Taxman) * 2);
				return taxes;
			}
		}

		internal short Science
		{
			get
			{
				short science = TradeScience;
				if (HasBuilding<Library>()) science += (short)Math.Floor((double)science * 0.5);
				if (HasBuilding<UniversityBuilding>()) science += (short)Math.Floor((double)science * 0.5);
				if (!Game.WonderObsolete<CopernicusObservatory>() && HasWonder<CopernicusObservatory>()) science += (short)Math.Floor((double)science * 1.0);
				if (Player.HasWonder<SETIProgram>()) science += (short)Math.Floor((double)science * 0.5);
				science += (short)(_specialists.Count(c => c == Citizen.Scientist) * 2);
				return science;
			}
		}

		internal short TotalMaintenance => (short)_buildings.Sum(b => b.Maintenance);

		internal byte Status
		{
			get
			{
				if (Size == 0) return 0;
				
				byte output = 0;
				if (Map[X, Y].GetBorderTiles().Any(t => t.IsOcean)) output |= (0x01 << 1); // Coastal city
				if (BuildingSold) output |= (0x01 << 7); // Building sold this turn
				return output;
			}
		}

		internal IEnumerable<ITile> ResourceTiles => CityTiles.Where(t => (t.X == X && t.Y == Y) || _resourceTiles.Contains(t));

		internal bool OccupiedTile(ITile tile)
		{
			if (ResourceTiles.Any(t => t.X == tile.X && t.Y == tile.Y))
				return false;
			return InvalidTile(tile);
		}

		internal bool InvalidTile(ITile tile)
		{
			return (tile.City != null || Game.GetCities().Any(c => c.ResourceTiles.Any(t => t.X == tile.X && t.Y == tile.Y)) || tile.Units.Any(u => u.Owner != Owner));
		}

		private void UpdateSpecialists()
		{
			while (_specialists.Count < (_size - ResourceTiles.Count())) _specialists.Add(Citizen.Entertainer);
			while (_specialists.Count > 0 && _specialists.Count > (_size - ResourceTiles.Count() - 1)) _specialists.RemoveAt(_specialists.Count - 1);
		}

		private void SetResourceTiles()
		{
			if (!Game.Started) return;
			while (_resourceTiles.Count > Size)
				_resourceTiles.RemoveAt(_resourceTiles.Count - 1);
			if (_resourceTiles.Count == Size) return;
			if (_resourceTiles.Count < Size)
			{
				IEnumerable<ITile> tiles = CityTiles.Where(t => !OccupiedTile(t) && !ResourceTiles.Contains(t)).OrderByDescending(t => FoodValue(t)).ThenByDescending(t => ShieldValue(t)).ThenByDescending(t => TradeValue(t));
				if (tiles.Count() > 0)
					_resourceTiles.Add(tiles.First());
			}
			
			UpdateSpecialists();
		}

		internal byte[] GetResourceTiles()
		{
			byte[] output = new byte[3];
			foreach (ITile tile in _resourceTiles)
			{
				int x = tile.X - X;
				int y = tile.Y - Y;
				switch(x)
				{
					case -2:
						if (y == -1) output[2] |= (byte)(0x01 << 3);
						if (y == 0) output[1] |= (byte)(0x01 << 3);
						if (y == 1) output[2] |= (byte)(0x01 << 2);
						continue;
					case -1:
						if (y == -2) output[1] |= (byte)(0x01 << 4);
						if (y == -1) output[0] |= (byte)(0x01 << 7);
						if (y == 0) output[0] |= (byte)(0x01 << 3);
						if (y == 1) output[0] |= (byte)(0x01 << 6);
						if (y == 2) output[2] |= (byte)(0x01 << 1);
						continue;
					case 0:
						if (y == -2) output[1] |= (byte)(0x01 << 0);
						if (y == -1) output[0] |= (byte)(0x01 << 0);
						if (y == 1) output[0] |= (byte)(0x01 << 2);
						if (y == 2) output[1] |= (byte)(0x01 << 2);
						continue;
					case 1:
						if (y == -2) output[1] |= (byte)(0x01 << 5);
						if (y == -1) output[1] |= (byte)(0x01 << 6);
						if (y == 0) output[0] |= (byte)(0x01 << 1);
						if (y == 1) output[0] |= (byte)(0x01 << 5);
						if (y == 2) output[2] |= (byte)(0x01 << 0);
						continue;
					case 2:
						if (y == -1) output[1] |= (byte)(0x01 << 6);
						if (y == 0) output[1] |= (byte)(0x01 << 1);
						if (y == 1) output[1] |= (byte)(0x01 << 7);
						continue;
				}
			}
			return output;
		}

		internal void SetResourceTiles(byte[] gameData)
		{
			if (gameData.Length != 6)
			{
				Log($"Invalid Resource game data for {Name}");
				return;
			}

			_resourceTiles.Clear();
			if (((gameData[0] >> 0) & 1) > 0) _resourceTiles.Add(Tile[0, -1]);
			if (((gameData[0] >> 1) & 1) > 0) _resourceTiles.Add(Tile[1, 0]);
			if (((gameData[0] >> 2) & 1) > 0) _resourceTiles.Add(Tile[0, 1]);
			if (((gameData[0] >> 3) & 1) > 0) _resourceTiles.Add(Tile[-1, 0]);
			if (((gameData[0] >> 4) & 1) > 0) _resourceTiles.Add(Tile[1, -1]);
			if (((gameData[0] >> 5) & 1) > 0) _resourceTiles.Add(Tile[1, 1]);
			if (((gameData[0] >> 6) & 1) > 0) _resourceTiles.Add(Tile[-1, 1]);
			if (((gameData[0] >> 7) & 1) > 0) _resourceTiles.Add(Tile[-1, -1]);
			
			if (((gameData[1] >> 0) & 1) > 0) _resourceTiles.Add(Tile[0, -2]);
			if (((gameData[1] >> 1) & 1) > 0) _resourceTiles.Add(Tile[2, 0]);
			if (((gameData[1] >> 2) & 1) > 0) _resourceTiles.Add(Tile[0, 2]);
			if (((gameData[1] >> 3) & 1) > 0) _resourceTiles.Add(Tile[-2, 0]);
			if (((gameData[1] >> 4) & 1) > 0) _resourceTiles.Add(Tile[-1, -2]);
			if (((gameData[1] >> 5) & 1) > 0) _resourceTiles.Add(Tile[1, -2]);
			if (((gameData[1] >> 6) & 1) > 0) _resourceTiles.Add(Tile[2, -1]);
			if (((gameData[1] >> 7) & 1) > 0) _resourceTiles.Add(Tile[2, 1]);
			
			if (((gameData[2] >> 0) & 1) > 0) _resourceTiles.Add(Tile[1, 2]);
			if (((gameData[2] >> 1) & 1) > 0) _resourceTiles.Add(Tile[-1, 2]);
			if (((gameData[2] >> 2) & 1) > 0) _resourceTiles.Add(Tile[-2, 1]);
			if (((gameData[2] >> 3) & 1) > 0) _resourceTiles.Add(Tile[-2, -1]);

			//TODO: Correctly load specialists
		}

		private void ResetResourceTiles()
		{
			_resourceTiles.Clear();
			for (int i = 0; i < Size; i++)
				SetResourceTiles();
		}

		public void RelocateResourceTile(ITile tile)
		{
			if (tile.X == X && tile.Y == Y) return;
			SetResourceTile(tile);
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
			UpdateSpecialists();
		}

		private Player Player => Game.Instance.GetPlayer(Owner);

		public IEnumerable<IProduction> AvailableProduction
		{
			get
			{
				foreach (IUnit unit in Reflect.GetUnits().Where(u => Player.ProductionAvailable(u)))
				{
					if (unit.Class == UnitClass.Water && !Map[X, Y].GetBorderTiles().Any(t => t.IsOcean)) continue;
					if (unit is Nuclear && !Game.WonderBuilt<ManhattanProject>()) continue;
					yield return unit;
				}
				foreach (IBuilding building in Reflect.GetBuildings().Where(b => Player.ProductionAvailable(b) && !_buildings.Any(x => x.Id == b.Id)))
				{
					if (HasBuilding<Palace>() && building is Courthouse) continue;
					yield return building;
				}
				foreach (IWonder wonder in Reflect.GetWonders().Where(b => Player.ProductionAvailable(b)))
				{
					yield return wonder;
				}
			}
		}

		public void SetProduction(IProduction production) => CurrentProduction = production;

		internal void SetProduction(byte productionId)
		{
			IProduction production = Reflect.GetProduction().FirstOrDefault(p => p.ProductionId == productionId);
			if (production == null)
			{
				Log($"Invalid production ID for {Name}: {productionId}");
				return;
			}
			CurrentProduction = production;
		}

		internal short BuyPrice
		{
			get
			{
				if (Shields > 0)
				{
					// Thanks to Tristan_C (http://forums.civfanatics.com/threads/buy-unit-building-wonder-price.576026/#post-14490920)
					if (CurrentProduction is IUnit)
					{
						double x = (double)((CurrentProduction.Price * 10) - Shields) / 10;
						double price = 5 * (x * x) + (20 * x);
						return (short)(Math.Floor(price));
					}
					return (short)(((CurrentProduction.Price * 10) - Shields) * (CurrentProduction is IWonder ? 4 : 2));
				}
				return CurrentProduction.BuyPrice;
			}
		}

		public bool Buy()
		{
			if (Game.CurrentPlayer.Gold < BuyPrice) return false;

			Game.CurrentPlayer.Gold -= BuyPrice;
			Shields = (int)CurrentProduction.Price * 10;
			return true;
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

		private readonly List<Citizen> _specialists = new List<Citizen>();
		internal IEnumerable<Citizen> Citizens
		{
			get
			{
				// Update specialist count
				while (_specialists.Count < Size - (ResourceTiles.Count() - 1)) _specialists.Add(Citizen.Entertainer);
				while (_specialists.Count > Size - (ResourceTiles.Count() - 1)) _specialists.Remove(_specialists.Last());

				int happyCount = (int)Math.Floor((double)Luxuries / 2);
				if (Player.HasWonder<HangingGardens>() && !Game.WonderObsolete<HangingGardens>()) happyCount++;
				if (Player.HasWonder<CureForCancer>()) happyCount++;

				int unhappyCount = Size - (6 - Game.Difficulty) - happyCount;
				if (HasWonder<ShakespearesTheatre>() && !Game.WonderObsolete<ShakespearesTheatre>())
				{
					unhappyCount = 0;
				}
				else
				{
					if (HasBuilding<Temple>())
					{
						int templeEffect = 1;
						if (Player.HasAdvance<Mysticism>()) templeEffect <<= 1;
						if (Player.HasWonder<Oracle>() && !Game.WonderObsolete<Oracle>()) templeEffect <<= 1;
						unhappyCount -= templeEffect;
					}
					if (Map.ContentCities(Tile.ContinentId).Any(x => x.Size > 0 && x.Owner == Owner && x.HasWonder<JSBachsCathedral>()))
					{
						unhappyCount -= 2;
					}
					if (HasBuilding<Colosseum>()) unhappyCount -= 3;
					if (HasBuilding<Cathedral>()) unhappyCount -= 4;
				}

				int content = 0;
				int unhappy = 0;
				int working = (ResourceTiles.Count() - 1);
				int specialist = 0;

				for (int i = 0; i < Size; i++)
				{
					if (i < working)
					{
						if (happyCount-- > 0)
						{
							yield return (i % 2 == 0) ? Citizen.HappyMale : Citizen.HappyFemale;
							continue;
						}
						if ((unhappyCount - (working - i)) >= 0)
						{
							unhappyCount--;
							yield return ((unhappy++) % 2 == 0) ? Citizen.UnhappyMale : Citizen.UnhappyFemale;
							continue;
						}
						yield return ((content++) % 2 == 0) ? Citizen.ContentMale : Citizen.ContentFemale;
						continue;
					}
					yield return _specialists[specialist++];
				}
			}
		}
		internal void ChangeSpecialist(int index)
		{
			while (_specialists.Count < (index + 1)) _specialists.Add(Citizen.Entertainer);
			_specialists[index] = (Citizen)((((int)_specialists[index] - 5) % 3) + 6);
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

		public IUnit[] Units => Game.Instance.GetUnits().Where(u => u.Home == this).ToArray();

		public ITile Tile => Map[X, Y];

		public bool BuildingSold { get; private set; }

		public void AddBuilding(IBuilding building) => _buildings.Add(building);

		public void SellBuilding(IBuilding building)
		{
			RemoveBuilding(building);
			Game.CurrentPlayer.Gold += building.SellPrice;
			BuildingSold = true;
		}

		private void RemoveBuilding(IBuilding building) => _buildings.RemoveAll(b => b.Id == building.Id);
		public void RemoveBuilding<T>() where T : IBuilding => _buildings.RemoveAll(b => b is T);

		public void AddWonder(IWonder wonder)
		{
			_wonders.Add(wonder);
			if (Game.Started)
			{
				if (wonder is Colossus && !Game.WonderObsolete<Colossus>())
				{
					ResetResourceTiles();
				}
				if ((wonder is Lighthouse && !Game.WonderObsolete<Lighthouse>()) ||
					(wonder is MagellansExpedition && !Game.WonderObsolete<MagellansExpedition>()))
				{
					// Apply Lighthouse/Magellan's Expedition wonder effects in the first turn
					foreach (IUnit unit in Game.GetUnits().Where(x => x.Owner == Owner && x.Class ==  UnitClass.Water && x.MovesLeft == x.Move))
					{
						unit.MovesLeft++;
					}
				}
			}
		}

		public void UpdateResources()
		{
			foreach (ITile tile in ResourceTiles.Where(t => InvalidTile(t)))
			{
				RelocateResourceTile(tile);
			}
		}

		public void NewTurn()
		{
			UpdateResources();

			Food += FoodIncome;
			if (Food < 0)
			{
				Food = 0;
				Size--;
				if (Human == Owner)
				{
					GameTask.Enqueue(Message.Newspaper(this, "Food storage exhausted", $"in {Name}!", "Famine feared."));
				}
				if (Size == 0) return;
			}
			else if (Food > FoodRequired)
			{
				Food -= FoodRequired;

				if (Size == 10 && !_buildings.Any(b => b.Id == (int)Building.Aqueduct))
				{
					GameTask.Enqueue(Message.Advisor(Advisor.Domestic, false, $"{Name} requires an AQUEDUCT", "for further growth."));
				}
				else
				{
					Size++;
				}

				if (_buildings.Any(b => (b is Granary)))
				{
					if (Food < (FoodRequired / 2))
					{
						Food = (FoodRequired / 2);
					}
				}
			}

			if (ShieldIncome < 0)
			{
				int maxDistance = Units.Max(u => Common.DistanceToTile(X, Y, u.X, u.Y));
				IUnit unit = Units.Last(u => Common.DistanceToTile(X, Y, u.X, u.Y) == maxDistance);
				if (Human == Owner)
				{
					Message message = Message.DisbandUnit(this, unit);
					message.Done += (s, a) => {
						Game.DisbandUnit(unit);
					};
					GameTask.Enqueue(message);
				}
				else
				{
					Game.DisbandUnit(unit);
				}
			}
			else if (ShieldIncome > 0)
			{
				Shields += ShieldIncome;
			}

			if (Shields >= (int)CurrentProduction.Price * 10)
			{
				if (CurrentProduction is Settlers && Size == 1 && Game.Difficulty == 0)
				{
					// On Chieftain level, it's not possible to create a Settlers in a city of size 1
				}
				else if (CurrentProduction is IUnit)
				{
					Shields = 0;
					IUnit unit = Game.Instance.CreateUnit((CurrentProduction as IUnit).Type, X, Y, Owner);
					unit.SetHome();
					unit.Veteran = (_buildings.Any(b => (b is Barracks)));
					if (CurrentProduction is Settlers)
					{
						if (Size == 1 && Player.Cities.Length == 1) Size++;
						if (Size == 1)
						{
							unit.SetHome(null);
						}
						Size--;
					}
					if (Human == Owner && (unit is Settlers || unit is Diplomat || unit is Caravan))
					{
						GameTask advisorMessage = Message.Advisor(Advisor.Defense, true, $"{this.Name} builds {unit.Name}.");
						advisorMessage.Done += (s, a) => GameTask.Insert(Show.CityManager(this));
						GameTask.Enqueue(advisorMessage);
					}
				}
				if (CurrentProduction is IBuilding && !_buildings.Any(b => b.Id == (CurrentProduction as IBuilding).Id))
				{
					Shields = 0;
					if (CurrentProduction is ISpaceShip)
					{
						Message message = Message.Newspaper(this, $"{this.Name} builds", $"{(CurrentProduction as ICivilopedia).Name}.");
						message.Done += (s, a) => {
							// TODO: Add space ship component
							GameTask.Insert(Show.CityManager(this));
						};
						GameTask.Enqueue(message);
					}
					else if (CurrentProduction is Palace)
					{
						foreach (City city in Game.Instance.GetCities().Where(c => c.Owner == Owner))
						{
							// Remove palace from all cites.
							city.RemoveBuilding<Palace>();
						}
						if (HasBuilding<Courthouse>())
						{
							_buildings.RemoveAll(x => x is Courthouse);
						}
						_buildings.Add(CurrentProduction as IBuilding);
						
						Message message = Message.Newspaper(this, $"{this.Name} builds", $"{(CurrentProduction as ICivilopedia).Name}.");
						message.Done += (s, a) => {
							GameTask advisorMessage = Message.Advisor(Advisor.Foreign, true, $"{Player.TribeName} capital", $"moved to {Name}.");
							advisorMessage.Done += (s1, a1) => GameTask.Insert(Show.CityManager(this));
							GameTask.Enqueue(advisorMessage);
						};
						GameTask.Enqueue(message);
					}
					else
					{
						_buildings.Add(CurrentProduction as IBuilding);
						GameTask.Enqueue(new ImprovementBuilt(this, (CurrentProduction as IBuilding)));
					}
				}
				if (CurrentProduction is IWonder && !Game.Instance.BuiltWonders.Any(w => w.Id == (CurrentProduction as IWonder).Id))
				{
					Shields = 0;
					AddWonder(CurrentProduction as IWonder);
					GameTask.Enqueue(new ImprovementBuilt(this, (CurrentProduction as IWonder)));
				}
			}

			// TODO: Handle luxuries
			Player.Gold += Taxes;
			Player.Gold -= TotalMaintenance;
			Player.Science += Science;
			BuildingSold = false;
			GameTask.Enqueue(new ProcessScience(Player));

			if (Player == Human) return;
			
			AI.CityProduction(this);
		}

		internal City(byte owner)
		{
			Owner = owner;
			if (!Game.Started) return;
			CurrentProduction = Reflect.GetUnits().Where(u => Player.ProductionAvailable(u)).OrderBy(u => Common.HasAttribute<Default>(u) ? -1 : (int)u.Type).First();
			SetResourceTiles();
		}
	}
}