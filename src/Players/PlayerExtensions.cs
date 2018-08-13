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
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Governments;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.Wonders;

namespace CivOne.Players
{
	internal static class PlayerExtensions
	{
		public static IEnumerable<City> GetCities(this IPlayer player) => Game.Instance.GetCities().Where(x => x.Owner == Game.Instance.PlayerNumber(player) && x.Size > 0);

		public static IEnumerable<IUnit> GetUnits(this IPlayer player) => Game.Instance.GetUnits().Where(x => x.Owner == Game.Instance.PlayerNumber(player));

		public static bool IsDestroyed(this IPlayer player)
		{
			if (Game.Instance.PlayerNumber(player) == 0) return false;
			if (!player.GetCities().Any() && !player.GetUnits().Any(x => (x is Settlers && x.Home == null)))
			{
				while (true)
				{
					IUnit unit = player.GetUnits().FirstOrDefault();
					if (unit == null) break;
					Game.Instance.DisbandUnit(unit);
				}
				return true;
			}
			return false;
		}

		public static bool HasWonder<T>(this IPlayer player, bool checkObsolete = false) where T : IWonder, new() => (!checkObsolete || !Game.Instance.WonderObsolete<T>()) && player.GetCities().Any(c => c.HasWonder<T>());

		public static bool HasAdvance<T>(this IPlayer player) where T : IAdvance => player.Advances.Any(a => a is T);

		public static bool HasAdvance(this IPlayer player, IAdvance advance) => (advance == null || player.Advances.Any(a => a.Id == advance.Id));

		public static bool HasEmbassy(this IPlayer player, IPlayer checkPlayer) => player.Embassies.Any(p => p == checkPlayer);

		public static City GetCapital(this IPlayer player) => player.GetCities().FirstOrDefault(c => c.HasBuilding<Palace>());

		public static IEnumerable<IAdvance> AvailableResearch(this IPlayer player)
		{
			foreach (IAdvance advance in Common.Advances.Where(a => !player.HasAdvance(a)))
			{
				if (advance.RequiredTechs.Length > 0 && !advance.RequiredTechs.All(a => player.HasAdvance(a))) continue;
				yield return advance;
			}
		}

		public static IEnumerable<IGovernment> AvailableGovernments(this IPlayer player)
		{
			bool allGovernments = !Game.Instance.WonderObsolete<Pyramids>() && player.HasWonder<Pyramids>();
			foreach (IGovernment government in Reflect.GetGovernments().Where(g => g.Id > 0))
			{
				if (!allGovernments && !player.HasAdvance(government.RequiredTech)) continue;
				yield return government; 
			}
		}

		public static short ScienceCost(this IPlayer player)
		{
			short cost = (short)((Game.Instance.Difficulty + 3) * 2 * (player.Advances.Count() + 1) * (Common.TurnToYear(Game.Instance.GameTurn) > 0 ? 2 : 1));
			if (cost < 12)
				return 12;
			return cost;
		}

		public static int GetPopulation(this IPlayer player) => player.GetCities().Sum(c => c.Population);

		public static bool Visible(this IPlayer player, ITile tile) => tile == null ? false : player.Visible(tile.X, tile.Y);

		public static bool Visible(this IPlayer player, ITile tile, Direction direction) => player.Visible(tile.GetBorderTile(direction));

		public static bool Is(this IPlayer player, byte playerNumber) => Game.Instance.PlayerNumber(player) == playerNumber;

		public static bool BuildingAvailable(this IPlayer player, IBuilding building)
		{
			// Only allow spaceship to be built if Apollo Program exists
			if ((building is ISpaceShip) && !Game.Instance.WonderBuilt<ApolloProgram>())
				return false;

			// Determine if the Player has the required tech (if any)
			return (building.RequiredTech == null || player.HasAdvance(building.RequiredTech));
		}

		public static bool UnitAvailable(this IPlayer player, IUnit unit)
		{
			// Determine if the unit is obsolete
			if (unit.ObsoleteTech != null && player.HasAdvance(unit.ObsoleteTech))
				return false;
			
			// Require Manhattan Project to be built for Nuclear unit
			if ((unit is Nuclear) && !Game.Instance.WonderBuilt<ManhattanProject>())
				return false;
			
			// Determine if the Player has the required tech (if any)
			return (unit.RequiredTech == null || player.HasAdvance(unit.RequiredTech));
		}

		public static bool WonderAvailable(this IPlayer player, IWonder wonder)
		{
			// Determine if the wonder has already been built
			if (Game.Instance.BuiltWonders.Any(w => w.Id == wonder.Id))
				return false;

			// Determine if the Player has the required tech (if any)
			return (wonder.RequiredTech == null || player.HasAdvance(wonder.RequiredTech));
		}

		public static bool ProductionAvailable(this IPlayer player, IProduction production)
		{
			if (production is IUnit)
				return player.UnitAvailable(production as IUnit);
			if (production is IBuilding)
				return player.BuildingAvailable(production as IBuilding);
			if (production is IWonder)
				return player.WonderAvailable(production as IWonder);
			return true;
		}

		public static bool HasGovernment<T>(this IPlayer player) where T : IGovernment => player.Government is T;

		public static bool AnarchyDespotism(this IPlayer player) => player.HasGovernment<Anarchy>() || player.HasGovernment<Despotism>();

		public static bool MonarchyCommunist(this IPlayer player) => player.HasGovernment<CivOne.Governments.Monarchy>() || player.HasGovernment<CivOne.Governments.Communism>();

		public static bool RepublicDemocratic(this IPlayer player) => player.HasGovernment<Republic>() || player.HasGovernment<CivOne.Governments.Democracy>();

		public static void Revolt(this IPlayer player) => player.Government = new Anarchy();

		public static void Explore(this IPlayer player, int x, int y, int range = 1, bool sea = false) => Map.Instance.Explore(Game.Instance.PlayerNumber(player), x, y, range, sea);

		public static bool Visible(this IPlayer player, int x, int y) => Map.Instance.Visible(Game.Instance.PlayerNumber(player), x, y);
	}
}