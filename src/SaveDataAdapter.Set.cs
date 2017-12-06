// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using CivOne.IO;

namespace CivOne
{
	internal partial class SaveDataAdapter
	{
		private void SetArray<T>(ref T structure, string fieldName, params byte[] values) where T : struct
		{
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
			Marshal.StructureToPtr(structure, ptr, false);
			IntPtr offset = IntPtr.Add(ptr, (int)Marshal.OffsetOf<T>(fieldName));
			Marshal.Copy(values, 0, offset, values.Length);
			structure = Marshal.PtrToStructure<T>(ptr);
			Marshal.FreeHGlobal(ptr);
		}

		private void SetArray(string fieldName, params byte[] values) => SetArray<SaveData>(ref _saveData, fieldName, values);

		private void SetArray<T>(string fieldName, params T[] values) where T : struct
		{
			int itemSize = Marshal.SizeOf<T>();
			byte[] bytes = new byte[values.Length * itemSize];
			for (int i = 0; i < values.Length; i++)
			{
				T value = values[i];
				IntPtr ptr = Marshal.AllocHGlobal(itemSize);
				Marshal.StructureToPtr<T>(value, ptr, false);
				Marshal.Copy(ptr, bytes, (i * itemSize), itemSize);
				Marshal.FreeHGlobal(ptr);
			}
			SetArray(fieldName, bytes);
		}
		
		private void SetArray(string fieldName, params short[] values)
		{
			byte[] bytes = new byte[values.Length * 2];
			Buffer.BlockCopy(values, 0, bytes, 0, values.Length);
			SetArray(fieldName, bytes);
		}

		private void SetArray(string fieldName, params ushort[] values)
		{
			byte[] bytes = new byte[values.Length * 2];
			Buffer.BlockCopy(values, 0, bytes, 0, values.Length);
			SetArray(fieldName, bytes);
		}

		private void SetArray<T>(ref T structure, string fieldName, int itemLength, params string[] values) where T : struct
		{
			byte[] bytes = new byte[itemLength * values.Length];
			for (int i = 0; i < values.Length; i++)
			for (int c = 0; c < itemLength; c++)
				bytes[(i * itemLength) + c] = (c >= values[i].Length) ? (byte)0 : (byte)values[i][c];
			SetArray<T>(ref structure, fieldName, bytes);
		}

		private void SetArray(string fieldName, int itemLength, params string[] values) => SetArray<SaveData>(ref _saveData, fieldName, itemLength, values);

		private void SetDiscoveredAdvanceIDs(byte[][] input)
		{
			byte[] bytes = new byte[8 * 10];
			for (int p = 0; p < 8; p++)
			{
				if (p >= input.Length) continue;
				bytes = bytes.ToBitIds(p * 10, 10, input[p]);
			}
			SetArray(nameof(SaveData.DiscoveredAdvances), bytes);
		}

		private void SetAdvancesCount(ushort[] values) => SetArray(nameof(SaveData.AdvancesCount), values);

		private void SetLeaderNames(string[] values) => SetArray(nameof(SaveData.LeaderNames), 14, values);

		private void SetCivilizationNames(string[] values) => SetArray(nameof(SaveData.CivilizationNames), 12, values);

		private void SetCitizenNames(string[] values) => SetArray(nameof(SaveData.CitizensName), 11, values);

		private void SetCityNames(string[] values) => SetArray(nameof(SaveData.CityNames), 13, values);

		private void SetPlayerGold(short[] values) => SetArray(nameof(SaveData.PlayerGold), values);

		private void SetResearchProgress(short[] values) => SetArray(nameof(SaveData.ResearchProgress), values);

		private void SetUnitsActive(UnitData[][] unitData)
		{
			ushort[] data = new ushort[8 * 28];
			for (int p = 0; p < unitData.Length; p++)
			for (int i = 0; i < 28; i++)
			{
				data[(p * 28) + i] = (ushort)unitData[p].Count(u => u.TypeId == i);
			}
			SetArray(nameof(SaveData.UnitsActive), data);
		}

		private void SetTaxRate(ushort[] values) => SetArray(nameof(SaveData.TaxRate), values);

		private void SetScienceRate(ushort[] values) => SetArray(nameof(SaveData.ScienceRate), values);

		private void SetStartingPositionX(ushort[] values) => SetArray(nameof(SaveData.StartingPositionX), values);

		private void SetGovernment(ushort[] values) => SetArray(nameof(SaveData.Government), values);

		private void SetCityCount(ushort[] values) => SetArray(nameof(SaveData.CityCount), values);

		private void SetSettlerCount(ushort[] values) => SetArray(nameof(SaveData.SettlerCount), values);

		private void SetUnitCount(ushort[] values) => SetArray(nameof(SaveData.UnitCount), values);

		private void SetTotalCitySize(ushort[] values) => SetArray(nameof(SaveData.TotalCitySize), values);

		private void SetCities(CityData[] values)
		{
			SaveData.City[] cities = GetArray<SaveData.City>(nameof(SaveData.Cities), 128);
			
			for (int i = 0; i < new[] { values.Length, 128 }.Min(); i++)
			{
				CityData data = values[i];
				SetArray<SaveData.City>(ref cities[i], nameof(SaveData.City.Buildings), new byte[4].ToBitIds(0, 4, data.Buildings));
				cities[i].X = data.X;
				cities[i].Y = data.Y;
				cities[i].Status = data.Status;
				cities[i].ActualSize = data.ActualSize;
				cities[i].VisibleSize = data.ActualSize; // TODO: Implement Visible Size
				cities[i].CurrentProduction = data.CurrentProduction;
				cities[i].BaseTrade = 0; // TODO: Implement trade
				cities[i].Owner = data.Owner;
				cities[i].Food = data.Food;
				cities[i].Shields = data.Shields;
				SetArray<SaveData.City>(ref cities[i], nameof(SaveData.City.ResourceTiles), data.ResourceTiles);
				cities[i].NameId = data.NameId;
			}
			SetArray<SaveData.City>(nameof(SaveData.Cities), cities);
		}

		private void SetUnitTypes(SaveData.UnitType[] types) => SetArray<SaveData.UnitType>(nameof(SaveData.UnitTypes), types);

		private void SetUnits(UnitData[][] values)
		{
			SaveData.Unit[] units = GetArray<SaveData.Unit>(nameof(SaveData.Units), 8 * 128);

			for (int p = 0; p < new[] { values.Length, 8 }.Min(); p++)
			for (int u = 0; u < new[] { values[p].Length, 128 }.Min(); u++)
			{
				UnitData data = values[p][u];

				int i = (p * 128) + u;
				units[i].Status = data.Status;
				units[i].X = data.X;
				units[i].Y = data.Y;
				units[i].Type = data.TypeId;
				units[i].RemainingMoves = data.RemainingMoves;
				units[i].SpecialMoves = data.SpecialMoves;
				units[i].GotoX = data.GotoX;
				units[i].GotoY = data.GotoY;
				units[i].Visibility = data.Visibility;
				units[i].NextUnitId = data.NextUnitId;
				units[i].HomeCityId = data.HomeCityId;
			}

			SetArray(nameof(SaveData.Units), units);
		}

		private void SetWonders(ushort[] values) => SetArray<ushort>(nameof(SaveData.Wonders), values);

		private void SetTileVisibility(bool[][,] values)
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
			SetArray(nameof(SaveData.MapVisibility), bytes);
		}

		private void SetAdvanceFirstDiscovery(ushort[] values) => SetArray(nameof(SaveData.AdvanceFirstDiscovery), values);

		private void SetCityX(byte[] values) => SetArray(nameof(SaveData.CityX), values);
		
		private void SetCityY(byte[] values) => SetArray(nameof(SaveData.CityY), values);
	}
}