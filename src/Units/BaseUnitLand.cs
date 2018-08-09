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
using CivOne.Enums;
using CivOne.Players;
using CivOne.Screens;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.UserInterface;

namespace CivOne.Units
{
	internal abstract class BaseUnitLand : BaseUnit
	{
		protected override void MovementDone(ITile previousTile)
		{
			if (previousTile.IsOcean || Tile.IsOcean)
			{
				MovesLeft = 0;
				PartMoves = 0;
			}

			Tile.Visit(Owner);

			if (Tile.Hut)
			{
				Tile.Hut = false;
				TribalHut();
			}

			if ((previousTile.Road || previousTile.RailRoad) && (Tile.Road || Tile.RailRoad))
			{
				if ((Tile.RailRoad || Tile.City != null) && previousTile.RailRoad)
				{
					// No moves lost
				}
				else if (PartMoves > 0)
				{
					PartMoves--;
				}
				else
				{
					if (MovesLeft > 0)
						MovesLeft--;
					PartMoves = 2;
				}
			}
			else if (Tile.Type == Terrain.Ocean)
			{
				if (!previousTile.IsOcean)
				{
					MovesLeft = 0;
					PartMoves = 0;
				}
				Sentry = true;
				foreach (IUnit unit in Tile.Units.Where(u => u is IBoardable))
				{
					unit.Sentry = false;
				}
			}
			else
			{
				if (MovesLeft > 0)
				{
					byte moveCosts = 1;
					if (Class == UnitClass.Land)
						moveCosts = Map[X, Y].Movement;
					if (MovesLeft < moveCosts)
						moveCosts = MovesLeft;
					MovesLeft -= moveCosts;
				}
				else if (PartMoves > 0)
				{
					PartMoves = 0;
				}
			}
		}

		private void TribalHutMessage(EventHandler method, params string[] message)
		{
			if (Player is HumanPlayer)
			{
				Message msgBox = Message.General(message);
				msgBox.Done += method;
				GameTask.Insert(msgBox);
				return;
			}
			method(this, null);
		}

		private int NearestCity
		{
			get
			{
				int output = 0;
				if (Game.Instance.GetCities().Any())
					output = Game.Instance.GetCities().Select(c => Common.DistanceToTile(_x, _y, c.X, c.Y)).Min();
				return output;
			}
		}

		protected void TribalHut(HutResult result = HutResult.Random)
		{
			switch(result)
			{
				case HutResult.MetalDeposits:
					TribalHutMessage((s, e) => {
						 Player.Gold += 50;
					}, "You have discovered", "valuable metal deposits", "worth 50$");
					return;
				case HutResult.FriendlyTribe:
					TribalHutMessage((s, e) => {
						Game.Instance.CreateUnit(Common.Random.Next(0, 100) < 50 ? UnitType.Cavalry : UnitType.Legion, X, Y, Owner, true);
					}, "You have discovered", "a friendly tribe of", "skilled mercenaries.");
					return;
				case HutResult.AdvancedTribe:
					TribalHutMessage((s, e) => {
						GameTask.Enqueue(Orders.NewCity(Player, _x, _y));
					}, "You have discovered", "an advanced tribe.");
					return;
				case HutResult.AncientScrolls:
					TribalHutMessage((s, e) => {
						// This seems curious but this is how it actually probably happens in the original game
						IAdvance[] available = Game.Instance.CurrentPlayer.AvailableResearch().ToArray();
						int advanceId = Common.Random.Next(0, 72);
						for (int i = 0; i < 1000; i++)
						{
							if (!available.Any(a => a.Id == (advanceId + i) % 72)) continue;
							GameTask.Enqueue(new GetAdvance(Game.Instance.CurrentPlayer, available.First(a => a.Id == (advanceId + i) % 72)));
							break;
						}
					}, "You have discovered", "scrolls of ancient wisdom.");
					return;
				case HutResult.Barbarians:
					TribalHutMessage((s, e) => {
						//TODO: Find out how the barbarians should be created
						// This implementation is an approximation
						int count = 0;
						for (int i = 0; i < 1000; i++)
						{
							foreach (ITile tile in Map[X, Y].GetBorderTiles())
							{
								if (tile.City != null || tile.Units.Length > 0) continue;
								if (Common.Random.Next(0, 10) < 6) continue;
								if (tile.IsOcean) continue;
								Game.Instance.CreateUnit(Common.Random.Next(0, 100) < 50 ? UnitType.Cavalry : UnitType.Legion, tile.X, tile.Y, 0, true);
								count++;
							}
							if (count > 0) break;
						}
					}, "You have unleashed", "a horde of barbarians!");
					return;
			}

			// Tribal hut outcome, as described here: http://forums.civfanatics.com/showthread.php?t=510312
			switch (Common.Random.Next(0, 4))
			{
				case 0:
					if (NearestCity > 3)
					{
						if (Map[_x, _y].LandValue > 12)
						{
							TribalHut(HutResult.AdvancedTribe);
							break;
						}
						TribalHut(HutResult.MetalDeposits);
						break;
					}
					TribalHut(HutResult.FriendlyTribe);
					break;
				case 1:
					if (Game.Instance.GameTurn == 0 || Common.TurnToYear(Game.Instance.GameTurn) >= 1000)
					{
						TribalHut(HutResult.MetalDeposits);
						break;
					}
					TribalHut(HutResult.AncientScrolls);
					break;
				case 2:
					TribalHut(HutResult.MetalDeposits);
					break;
				case 3:
					if (NearestCity < 4 || !Game.Instance.GetCities().Any(c => Player.Is(c.Owner)))
					{
						TribalHut(HutResult.FriendlyTribe);
						break;
					}
					TribalHut(HutResult.Barbarians);
					break;
				default:
					TribalHut(HutResult.FriendlyTribe);
					break;
			}
		}
		
		public override IEnumerable<MenuItem<int>> MenuItems
		{
			get
			{
				yield return MenuNoOrders();
				yield return MenuFortify();
				yield return MenuWait();
				yield return MenuSentry();
				yield return MenuGoTo();
				if (Map[X, Y].Irrigation || Map[X, Y].Mine || Map[X, Y].Road || Map[X, Y].RailRoad)
				{
					yield return MenuPillage();
				}
				if (Map[X, Y].City != null)
				{
					yield return MenuHomeCity();
				}
				yield return null;
				yield return MenuDisbandUnit();
			}
		}

		protected override bool ValidMoveTarget(ITile tile)
		{
			if (tile == null)
				return false;

			// If the tile is not an ocean tile, movement is allowed
			if (tile.Type != Terrain.Ocean)
				return true;
			
			// This query checks if there's a boardable cargo vessel with free slots on the tile.
			return (tile.Units.Any(x => x.Owner == Owner) && tile.Units.Any(u => (u is IBoardable)) && tile.Units.Where(u => u is IBoardable).Sum(u => (u as IBoardable).Cargo) > tile.Units.Count(u => u.Class == UnitClass.Land));
		}

		protected BaseUnitLand(byte price = 1, byte attack = 1, byte defense = 1, byte move = 1) : base(price, attack, defense, move)
		{
			Class = UnitClass.Land;
		}
	}
}