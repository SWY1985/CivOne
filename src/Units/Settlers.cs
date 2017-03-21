// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Linq;
using CivOne.Advances;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.IO;
using CivOne.Screens;
using CivOne.Tasks;
using CivOne.Templates;
using CivOne.Tiles;

namespace CivOne.Units
{
	internal class Settlers : BaseUnitLand
	{
		public override bool Busy
		{
			get
			{
				return (base.Busy || BuildingRoad > 0 || BuildingIrrigation > 0 || BuildingMine > 0 || BuildingFortress > 0);
			}
			set
			{
				base.Busy = false;
				BuildingRoad = 0;
				BuildingIrrigation = 0;
				BuildingMine = 0;
				BuildingFortress = 0;
			}
		}
		public int BuildingRoad { get; private set; }
		public int BuildingIrrigation { get; private set; }
		public int BuildingMine { get; private set; }
		public int BuildingFortress { get; private set; }

		internal void SetStatus(bool[] bits)
		{
			BuildingRoad = (bits[1] && !bits[6] && !bits[7]) ? 2 : 0;
			BuildingIrrigation = (!bits[1] && bits[6] && !bits[7]) ? 3 : 0;
			BuildingMine = (!bits[1] && !bits[6] && bits[7]) ? 4 : 0;
			BuildingFortress = (!bits[1] && bits[6] && bits[7]) ? 5 : 0;
		}

		public bool BuildRoad()
		{
			ITile tile = Map[X, Y];
			if (tile.RailRoad)
			{
				// There is already a RailRoad here, don't build another one
				return false;
			}
			if (!tile.IsOcean && !tile.Road && tile.City == null)
			{
				if ((tile is River) && !Game.CurrentPlayer.HasAdvance<BridgeBuilding>())
					return false;
				BuildingRoad = 2;
				MovesLeft = 0;
				PartMoves = 0;
				return true;
			}
			else if (Game.CurrentPlayer.HasAdvance<RailRoad>() && !tile.IsOcean && tile.Road && !tile.RailRoad && tile.City == null)
			{
				BuildingRoad = 3;
				MovesLeft = 0;
				PartMoves = 0;
				return true;
			}
			return false;
		}

		public bool BuildIrrigation()
		{
			ITile tile = Map[X, Y];
			if (tile.Irrigation)
			{
				// Tile already irrigated, ignore
				return false;
			}

			if ((tile is Forest) || (tile is Jungle) || (tile is Swamp))
			{
				BuildingIrrigation = 4;
				MovesLeft = 0;
				PartMoves = 0;
				return true;
			}
			else if ((tile.GetBorderTiles().Any(t => (t.X == X || t.Y == Y) && (t.City == null) && (t.IsOcean || t.Irrigation || (t is River)))) || (tile is River))
			{
				if (!tile.IsOcean && !(tile.Irrigation) && ((tile is Desert) || (tile is Grassland) || (tile is Hills) || (tile is Plains) || (tile is River)))
				{
					BuildingIrrigation = 3;
					MovesLeft = 0;
					PartMoves = 0;
					return true;
				}
				if (Human == Owner)
					GameTask.Enqueue(Message.Error("-- Civilization Note --", TextFile.Instance.GetGameText("ERROR/NOIRR")));
				return false;
			}
			else
			{
				if (((tile is Desert) || (tile is Grassland) || (tile is Hills) || (tile is Plains) || (tile is River)) && tile.City == null)
				{
					if (Human == Owner)
						GameTask.Enqueue(Message.Error("-- Civilization Note --", TextFile.Instance.GetGameText("ERROR/NOWATER")));
					return true;
				}
				if (Human == Owner)
					GameTask.Enqueue(Message.Error("-- Civilization Note --", TextFile.Instance.GetGameText("ERROR/NOIRR")));
			}
			return false;
		}

		public bool BuildMines()
		{
			ITile tile = Map[X, Y];
			if (!tile.IsOcean && !(tile.Mine) && ((tile is Desert) || (tile is Hills) || (tile is Mountains) || (tile is Jungle) || (tile is Grassland) || (tile is Plains) || (tile is Swamp)))
			{
				BuildingMine = 4;
				MovesLeft = 0;
				PartMoves = 0;
				return true;
			}
			return false;
		}

		public bool BuildFortress()
		{
			if (!Game.CurrentPlayer.HasAdvance<Construction>())
				return false;
			
			ITile tile = Map[X, Y];
			if (!tile.IsOcean && !(tile.Fortress) && tile.City == null)
			{
				BuildingFortress = 5;
				MovesLeft = 0;
				PartMoves = 0;
				return true;
			}
			return false;
		}

		public override void NewTurn()
		{
			base.NewTurn();
			if (BuildingRoad > 0)
			{
				BuildingRoad--;
				if (BuildingRoad > 0)
				{
					if (Map[X, Y].Road)
					{
						if (Human.HasAdvance<RailRoad>())
						{
							Map[X, Y].RailRoad = true;
						}
						else
						{
							foreach (Settlers settlers in Map[X, Y].Units.Where(u => (u is Settlers) && (u as Settlers).BuildingRoad > 0).Select(u => (u as Settlers)))
							{
								settlers.BuildingRoad = 0;
							}
						}
					}
					Map[X, Y].Road = true;
					MovesLeft = 0;
					PartMoves = 0;
				}
			}
			else if (BuildingIrrigation > 0)
			{
				BuildingIrrigation--;
				if (BuildingIrrigation > 0)
				{
					MovesLeft = 0;
					PartMoves = 0;
				}
				else if (Map[X, Y] is Forest)
				{
					Map[X, Y].Irrigation = false;
					Map[X, Y].Mine = false;
					Map.ChangeTileType(X, Y, Terrain.Plains);
				}
				else if ((Map[X, Y] is Jungle) || (Map[X, Y] is Swamp))
				{
					Map[X, Y].Irrigation = false;
					Map[X, Y].Mine = false;
					Map.ChangeTileType(X, Y, Terrain.Grassland1);
				}
				else
				{
					Map[X, Y].Irrigation = true;
					Map[X, Y].Mine = false;
				}
			}
			else if (BuildingMine > 0)
			{
				BuildingMine--;
				if (BuildingMine > 0)
				{
					MovesLeft = 0;
					PartMoves = 0;
				}
				else if ((Map[X, Y] is Jungle) || (Map[X, Y] is Grassland) || (Map[X, Y] is Plains) || (Map[X, Y] is Swamp))
				{
					Map[X, Y].Irrigation = false;
					Map[X, Y].Mine = false;
					Map.ChangeTileType(X, Y, Terrain.Forest);
				}
				else
				{
					Map[X, Y].Irrigation = false;
					Map[X, Y].Mine = true;
				}
			}
			else if (BuildingFortress > 0)
			{
				BuildingFortress--;
				if (BuildingFortress > 0)
				{
					MovesLeft = 0;
					PartMoves = 0;
				}
				else
				{
					Map[X, Y].Fortress = true;
				}
			}
		}

		private GameMenu.Item MenuFoundCity()
		{
			GameMenu.Item item;
			if (Map[X, Y].City == null)
			{
				item = new GameMenu.Item("Found New City", "b");
			}
			else
			{
				item = new GameMenu.Item("Add to City", "b");
			}
			item.Selected += (s, a) => GameTask.Enqueue(Orders.FoundCity(this));
			return item;
		}

		private GameMenu.Item MenuBuildRoad()
		{
			GameMenu.Item item;
			if (Map[X, Y].Road)
			{
				item = new GameMenu.Item("Build RailRoad", "r");
			}
			else
			{
				item = new GameMenu.Item("Build Road", "r");
			}
			item.Selected += (s, a) => BuildRoad();
			return item;
		}

		private GameMenu.Item MenuBuildIrrigation()
		{
			GameMenu.Item item;
			if (Map[X, Y] is Forest)
			{
				item = new GameMenu.Item("Change to Plains", "i");
			}
			else if ((Map[X, Y] is Jungle) || (Map[X, Y] is Swamp))
			{
				item = new GameMenu.Item("Change to Grassland", "i");
			}
			else
			{
				item = new GameMenu.Item("Build Irrigation", "i");
			}
			item.Selected += (s, a) => GameTask.Enqueue(Orders.BuildIrrigation(this));
			return item;
		}

		private GameMenu.Item MenuBuildMines()
		{
			GameMenu.Item item;
			if ((Map[X, Y] is Jungle) || (Map[X, Y] is Grassland) || (Map[X, Y] is Plains) || (Map[X, Y] is Swamp))
			{
				item = new GameMenu.Item("Change to Forest", "m");
			}
			else
			{
				item = new GameMenu.Item("Build Mines", "m");
			}
			item.Selected += (s, a) => GameTask.Enqueue(Orders.BuildMines(this));
			return item;
		}

		private GameMenu.Item MenuBuildFortress()
		{
			GameMenu.Item item = new GameMenu.Item("Build fortress", "f");
			if (!Game.CurrentPlayer.HasAdvance<Construction>())
				item.Enabled = false;
			else
				item.Selected += (s, a) => GameTask.Enqueue(Orders.BuildFortress(this));
			return item;
		}
		
		public override IEnumerable<GameMenu.Item> MenuItems
		{
			get
			{
				ITile tile = Map[X, Y];

				yield return MenuNoOrders();
				if (!tile.IsOcean)
				{
					yield return MenuFoundCity();
				}
				if (!tile.IsOcean && (!tile.Road || (Human.HasAdvance<RailRoad>() && !tile.RailRoad)))
				{	
					yield return MenuBuildRoad();
				}
				if (!tile.Irrigation && ((tile is Desert) || (tile is Grassland) || (tile is Hills) || (tile is Plains) || (tile is River) || (tile is Forest) || (tile is Jungle) || (tile is Swamp)))
				{
					yield return MenuBuildIrrigation();
				}
				if (!tile.Mine && ((tile is Desert) || (tile is Hills) || (tile is Mountains) || (tile is Jungle) || (tile is Grassland) || (tile is Plains) || (tile is Swamp)))
				{
					yield return MenuBuildMines();
				}
				if (!tile.IsOcean && !tile.Fortress)
				{
					yield return MenuBuildFortress();
				}
				//
				yield return MenuWait();
				yield return MenuSentry();
				yield return MenuGoTo();
				if (tile.Irrigation || tile.Mine || tile.Road || tile.RailRoad)
				{
					yield return MenuPillage();
				}
				if (tile.City != null)
				{
					yield return MenuHomeCity();
				}
				yield return new GameMenu.Item(null);
				yield return MenuDisbandUnit();
			}
		}

		public override Picture GetUnit(byte colour, bool showState = true)
		{
			if (!showState)
				return base.GetUnit(colour);

			if (BuildingRoad > 0)
			{
				Picture unit = new Picture(base.GetUnit(colour));
				unit.DrawText("R", 0, 5, 8, 9, TextAlign.Center);
				unit.DrawText("R", 0, (byte)(colour == 1 ? 9 : 15), 8, 8, TextAlign.Center);
				return unit; 
			}
			else if (BuildingIrrigation > 0)
			{
				Picture unit = new Picture(base.GetUnit(colour));
				unit.DrawText("I", 0, 5, 8, 9, TextAlign.Center);
				unit.DrawText("I", 0, (byte)(colour == 1 ? 9 : 15), 8, 8, TextAlign.Center);
				return unit; 
			}
			else if (BuildingMine > 0)
			{
				Picture unit = new Picture(base.GetUnit(colour));
				unit.DrawText("M", 0, 5, 8, 9, TextAlign.Center);
				unit.DrawText("M", 0, (byte)(colour == 1 ? 9 : 15), 8, 8, TextAlign.Center);
				return unit; 
			}
			else if (BuildingFortress > 0)
			{
				Picture unit = new Picture(base.GetUnit(colour));
				unit.DrawText("F", 0, 5, 8, 9, TextAlign.Center);
				unit.DrawText("F", 0, (byte)(colour == 1 ? 9 : 15), 8, 8, TextAlign.Center);
				return unit; 
			}
			else if (Human == Owner && GotoX != -1 && GotoY != -1)
			{
				Picture unit = new Picture(base.GetUnit(colour));
				unit.DrawText("G", 0, 5, 8, 9, TextAlign.Center);
				unit.DrawText("G", 0, (byte)(colour == 1 ? 9 : 15), 8, 8, TextAlign.Center);
				return unit; 
			}
			return base.GetUnit(colour);
		}

		public Settlers() : base(4, 0, 1, 1)
		{
			Type = Unit.Settlers;
			Name = "Settlers";
			RequiredTech = null;
			ObsoleteTech = null;
			SetIcon('D', 1, 1);
		}
	}
}