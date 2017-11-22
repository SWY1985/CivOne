// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CivOne.Units;

namespace CivOne
{
	internal static class Extensions
	{
		private static Settings Settings => Settings.Instance;

		public static string GetSoundFile(this string input)
		{
			return Directory.GetFiles(Settings.SoundsDirectory).Where(x => Path.GetFileName(x).ToLower() == $"{input.ToLower()}.wav").FirstOrDefault();
		}

		public static byte[] Clear(this byte[] byteArray, byte value = 0)
		{
			for (int i = byteArray.GetUpperBound(0); i >= 0; i--)
				byteArray[i] = value;
			return byteArray;
		}

		private static CityData GetCityData(this City city, byte id)
		{
			return new CityData {
				Id = id,
				NameId = id,
				Status = 0,
				Buildings = city.Buildings.Select(b => b.Id).ToArray(),
				X = city.X,
				Y = city.Y,
				ActualSize = city.Size,
				CurrentProduction = city.CurrentProduction.ProductionId,
				Owner = city.Owner,
				Food = (ushort)city.Food,
				Shields = (ushort)city.Shields,
				ResourceTiles = city.GetResourceTiles()
			};
		}

		public static IEnumerable<CityData> GetCityData(this IEnumerable<City> cityList)
		{
			byte index = 0;
			foreach (City city in cityList)
			{
				yield return city.GetCityData(index++);
			}
		}

		private static UnitData GetUnitData(this IUnit unit, byte id)
		{
			return new UnitData {
				Id = id,
				Status = unit.Status,
				X = (byte)unit.X,
				Y = (byte)unit.Y,
				TypeId = (byte)unit.Type,
				RemainingMoves = (byte)((unit.MovesLeft * 3) + unit.PartMoves),
				SpecialMoves = 0,
				GotoX = (byte)unit.Goto.X,
				GotoY = (byte)unit.Goto.Y,
				Visibility = 0xFF,
				NextUnitId = id,

			};
		}

		public static IEnumerable<UnitData> GetUnitData(this IEnumerable<IUnit> unitList)
		{
			byte index = 0;
			List<UnitData> unitDataList = new List<UnitData>();
			foreach (IUnit unit in unitList)
			{
				unitDataList.Add(unit.GetUnitData(index++));
			}
			UnitData[] units = unitDataList.ToArray();
			for (int i = 0; i < units.Length; i++)
			{
				if (!units.Any(u => u.Id != units[i].Id && u.X == units[i].X && u.Y == units[i].Y)) continue;
				units[i].NextUnitId = units.Where(u => u.Id != units[i].Id && u.X == units[i].X && u.Y == units[i].Y).OrderBy(u => u.Id > units[i].Id ? 0 : 1).ThenBy(u => u.Id).First().Id;
			}
			return units;
		}
	}
}