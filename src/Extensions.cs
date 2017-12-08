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
using System.Text;
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

		public static string ToString(this byte[] bytes, int startIndex, int length)
		{
			StringBuilder output = new StringBuilder();
			for (int i = startIndex; i < startIndex + length; i++)
			{
				if (bytes[i] == 0) break;
				output.Append((char)bytes[i]);
			}
			return output.ToString().Trim();
		}

		public static IEnumerable<byte> FromBitIds(this byte[] bytes, int startIndex, int length)
		{
			byte index = 0;
			for (int i = startIndex; i < startIndex + length; i++)
			for (int b = 0; b < 8; b++)
			{
				if ((bytes[i] & (1 << b)) > 0) yield return index;
				index++;
			}
		}

		public static byte[] ToBitIds(this byte[] bytes, int startIndex, int length, byte[] values)
		{
			foreach (byte value in values)
			{
				int bitNo = value % 8;
				int byteNo = (value - bitNo) / 8;
				if (length <= byteNo) continue;
				bytes[startIndex + byteNo] |= (byte)(1 << bitNo);
			}
			return bytes;
		}

		private static byte GetId(this City city)
		{
			if (city != null)
			{
				City[] cities = Game.Instance.GetCities();
				for (byte c = 0; c < cities.Length; c++)
					if (cities[c] == city) return c;
			}
			return 0xFF;
		}

		private static CityData GetCityData(this City city, byte id)
		{
			IUnit[] units = city.Tile?.Units.Where(x => x.Home == city && x.Fortify).Take(2).ToArray();
			
			return new CityData {
				Id = city.GetId(),
				NameId = (byte)city.NameId,
				Status = 0,
				Buildings = city.Buildings.Select(b => b.Id).ToArray(),
				X = city.X,
				Y = city.Y,
				ActualSize = city.Size,
				CurrentProduction = city.CurrentProduction.ProductionId,
				Owner = city.Owner,
				Food = (ushort)city.Food,
				Shields = (ushort)city.Shields,
				ResourceTiles = city.GetResourceTiles(),
				FortifiedUnits = units?.Select(x => (byte)x.Type).ToArray()
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
			byte gotoX = 0xFF, gotoY = 0;
			if (!unit.Goto.IsEmpty)
			{
				gotoX = (byte)unit.Goto.X;
				gotoY = (byte)unit.Goto.Y;
			}

			return new UnitData {
				Id = id,
				Status = unit.Status,
				X = (byte)unit.X,
				Y = (byte)unit.Y,
				TypeId = (byte)unit.Type,
				RemainingMoves = (byte)((unit.MovesLeft * 3) + unit.PartMoves),
				SpecialMoves = 0,
				GotoX = gotoX,
				GotoY = gotoY,
				Visibility = 0xFF,
				NextUnitId = 0xFF,
				HomeCityId = unit.Home.GetId()
			};
		}

		private static IEnumerable<IUnit> FilterUnits(this List<IUnit> unitList)
		{
			foreach(City city in Game.Instance.GetCities())
			{
				IUnit[] units = unitList.Where(u => u.X == city.X && u.Y == city.Y && u.Home == city).Take(2).ToArray();
				foreach (IUnit unit in units)
				{
					unitList.Remove(unit);
				}
			}
			return unitList;
		}

		public static IEnumerable<UnitData> GetUnitData(this IEnumerable<IUnit> unitList)
		{
			// Remove two fortified units in home city (this data is stored in city data)
			IEnumerable<IUnit> filteredUnits = unitList.ToList().FilterUnits();

			byte index = 0;
			List<UnitData> unitDataList = new List<UnitData>();
			foreach (IUnit unit in filteredUnits)
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