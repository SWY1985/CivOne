// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;

namespace CivOne.IO
{
	internal unsafe static partial class SaveDataExtensions
	{
		public static SaveData SetDiscoveredAdvanceIDs(this SaveData saveData, byte[][] input)
		{
			byte[] bytes = new byte[8 * 10];
			for (int p = 0; p < 8; p++)
				SetBitIds(ref bytes, (p * 10), 10, input[p]);
			SetByteArray(&saveData.DiscoveredAdvances[0], bytes);
			return saveData;
		}

		public static SaveData SetAdvancesCount(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.AdvancesCount[0], values);
			return saveData;
		}

		public static SaveData SetLeaderNames(this SaveData saveData, string[] values)
		{
			SetStringArray(&saveData.LeaderNames[0], 14, values);
			return saveData;
		}

		public static SaveData SetCivilizationNames(this SaveData saveData, string[] values)
		{
			SetStringArray(&saveData.CivilizationNames[0], 12, values);
			return saveData;
		}

		public static SaveData SetCitizenNames(this SaveData saveData, string[] values)
		{
			SetStringArray(&saveData.CitizensName[0], 11, values);
			return saveData;
		}

		public static SaveData SetCityNames(this SaveData saveData, string[] values)
		{
			SetStringArray(&saveData.CityNames[0], 13, values);
			return saveData;
		}

		public static SaveData SetPlayerGold(this SaveData saveData, short[] values)
		{
			SetShortArray(&saveData.PlayerGold[0], values);
			return saveData;
		}

		public static SaveData SetResearchProgress(this SaveData saveData, short[] values)
		{
			SetShortArray(&saveData.ResearchProgress[0], 8);
			return saveData;
		}

		public static SaveData SetUnitsActive(this SaveData saveData, UnitData[][] unitData)
		{
			ushort[] data = new ushort[8 * 28];
			for (int p = 0; p < unitData.Length; p++)
			for (int i = 0; i < 28; i++)
			{
				data[(p * 28) + i] = (ushort)unitData[p].Count(u => u.TypeId == i);
			}
			SetUShortArray(&saveData.UnitsActive[0], data);
			return saveData;
		}

		public static SaveData SetTaxRate(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.TaxRate[0], values);
			return saveData;
		}

		public static SaveData SetScienceRate(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.ScienceRate[0], values);
			return saveData;
		}

		public static SaveData SetStartingPositionX(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.StartingPositionX[0], values);
			return saveData;
		}

		public static SaveData SetGovernment(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.Government[0], values);
			return saveData;
		}

		public static SaveData SetCityCount(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.CityCount[0], values);
			return saveData;
		}

		public static SaveData SetSettlerCount(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.SettlerCount[0], values);
			return saveData;
		}

		public static SaveData SetUnitCount(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.UnitCount[0], values);
			return saveData;
		}

		public static SaveData SetTotalCitySize(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.TotalCitySize[0], values);
			return saveData;
		}

		public static SaveData SetCityData(this SaveData saveData, CityData[] values)
		{
			for (int i = 0; i < new[] { values.Length, 128 }.Min(); i++)
			{
				CityData data = values[i];
				byte[] bytes = new byte[28];
				if (data.Buildings != null) SetBitIds(ref bytes, 0, 4, data.Buildings);
				bytes[4] = data.X;
				bytes[5] = data.Y;
				bytes[6] = data.Status;
				bytes[7] = data.ActualSize;
				bytes[8] = data.ActualSize;
				bytes[9] = data.CurrentProduction;
				bytes[11] = data.Owner;
				bytes[12] = (byte)(data.Food & 0xFF);
				bytes[13] = (byte)((data.Food & 0xFF00) >> 8);
				bytes[14] = (byte)(data.Shields & 0xFF);
				bytes[15] = (byte)((data.Shields & 0xFF00) >> 8);
				if (data.ResourceTiles != null)
				{
					fixed(byte* b = &bytes[16])
					{
						SetByteArray(b, data.ResourceTiles);
					}
				}
				bytes[22] = data.NameId;
				SetByteArray(&saveData.Cities[i * 28], bytes);
			}
			return saveData;
		}

		public static SaveData SetUnitTypes(this SaveData saveData, byte[] bytes)
		{
			SetByteArray(&saveData.UnitTypes[0], bytes);
			return saveData;
		}

		public static SaveData SetUnitData(this SaveData saveData, UnitData[][] values)
		{
			for (int p = 0; p < new[] { values.Length, 8 }.Min(); p++)
			for (int u = 0; u < new[] { values[p].Length, 128 }.Min(); u++)
			{
				UnitData data = values[p][u];
				byte[] bytes = new byte[12];
				bytes[0] = data.Status;
				bytes[1] = data.X;
				bytes[2] = data.Y;
				bytes[3] = data.TypeId;
				bytes[4] = data.RemainingMoves;
				bytes[5] = data.SpecialMoves;
				bytes[6] = data.GotoX;
				bytes[7] = data.GotoY;
				bytes[9] = data.Visibility;
				bytes[10] = data.NextUnitId;
				bytes[11] = data.HomeCityId;
				SetByteArray(&saveData.Units[(p * 12 * 128) + (12 * u)], bytes);
			}
			return saveData;
		}

		public static SaveData SetWonders(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.Wonders[0], values);
			return saveData;
		}

		public static SaveData SetTileVisibility(this SaveData saveData, bool[][,] values)
		{
			byte[] bytes = new byte[80 * 50];
			for (int p = 0; p < values.Length; p++)
			{
				for (int y = 0; y < 50; y++)
				for (int x = 0; x < 80; x++)
				{
					bytes[(x * 50) + y] |= (byte)(values[p][x, y] ? (1 << p) : 0);
				}
			}
			SetByteArray(&saveData.MapVisibility[0], bytes);
			return saveData;
		}

		public static SaveData SetAdvanceFirstDiscovery(this SaveData saveData, ushort[] values)
		{
			SetUShortArray(&saveData.AdvanceFirstDiscovery[0], values);
			return saveData;
		}

		public static SaveData SetCityX(this SaveData saveData, byte[] values)
		{
			SetByteArray(&saveData.CityX[0], values);
			return saveData;
		}
		
		public static SaveData SetCityY(this SaveData saveData, byte[] values)
		{
			SetByteArray(&saveData.CityY[0], values);
			return saveData;
		}
	}
}