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
using System.Runtime.InteropServices;
using CivOne.IO;

namespace CivOne
{
	internal partial class SaveDataAdapter
	{
		private void GetByteArray(string fieldName, ref byte[] bytes)
		{
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<SaveData>());
			Marshal.StructureToPtr(_saveData, ptr, false);
			IntPtr offset = IntPtr.Add(ptr, (int)Marshal.OffsetOf<SaveData>(fieldName));
			Marshal.Copy(offset, bytes, 0, bytes.Length);
			Marshal.FreeHGlobal(ptr);
		}

		private byte[] GetArray(string fieldName, int length)
		{
			byte[] output = new byte[length];
			GetByteArray(fieldName, ref output);
			return output;
		}

		private string[] GetArray(string fieldName, int itemLength, int itemCount)
		{
			byte[] bytes = GetArray(fieldName, itemLength * itemCount);
			string[] output = new string[itemCount];
			for (int i = 0; i < itemCount; i++)
				output[i] = bytes.ToString(i * itemLength, itemLength);
			return output;
		}

		private T[] GetArray<T>(string fieldName, int length)
		{
			T[] output = new T[length];
			Buffer.BlockCopy(GetArray(fieldName, length * Marshal.SizeOf<T>()), 0, output, 0, length);
			return output;
		}

		private byte[][] GetDiscoveredAdvanceIDs()
		{
			byte[] bytes = GetArray(nameof(SaveData.DiscoveredAdvances), 8 * 10);
			byte[][] output = new byte[8][];
			for (int i = 0; i < 8; i++)
				output[i] = bytes.FromBitIds(i * 10, 10).ToArray();
			return output;
		}

		private string[] GetLeaderNames() => GetArray(nameof(SaveData.LeaderNames), 8, 14);

		private string[] GetCivilizationNames() => GetArray(nameof(SaveData.CivilizationNames), 8, 12);

		private string[] GetCitizenNames() => GetArray(nameof(SaveData.CitizensName), 8, 11);

		private string[] GetCityNames() => GetArray(nameof(SaveData.CityNames), 256, 13);

		private short[] GetPlayerGold() => GetArray<short>(nameof(SaveData.PlayerGold), 8);

		private short[] GetResearchProgress() => GetArray<short>(nameof(SaveData.ResearchProgress), 8);

		private ushort[] GetTaxRate() => GetArray<ushort>(nameof(SaveData.TaxRate), 8);

		private ushort[] GetScienceRate() => GetArray<ushort>(nameof(SaveData.ScienceRate), 8);
		
		private ushort[] GetStartingPositionX() => GetArray<ushort>(nameof(SaveData.StartingPositionX), 8);

		private ushort[] GetGovernment() => GetArray<ushort>(nameof(SaveData.Government), 8);

		private CityData[] GetCities()
		{
			byte[] bytes = GetArray(nameof(SaveData.Cities), 28 * 128);
			List<CityData> output = new List<CityData>();

			for (byte c = 0; c < 128; c++)
			{
				int offset = 28 * c;
				if (bytes[offset + 6] == 0xFF) continue;

				output.Add(new CityData()
				{
					Id = c,
					NameId = bytes[offset + 22],
					Buildings = bytes.FromBitIds(offset, 4).ToArray(),
					X = bytes[offset + 4],
					Y = bytes[offset + 5],
					Status = bytes[offset + 6],
					ActualSize = bytes[offset + 7],
					CurrentProduction = bytes[offset + 9],
					Owner = bytes[offset + 11],
					Food = (ushort)((bytes[offset + 13] << 8) + bytes[offset + 12]),
					Shields = (ushort)((bytes[offset + 13] << 8) + bytes[offset + 12]),
					ResourceTiles = bytes.Skip(offset + 16).Take(6).ToArray()
				});
			}
			return output.ToArray();
		}

		private UnitData[][] GetUnits()
		{
			byte[] bytes = GetArray(nameof(SaveData.Units), (8 * 128 * 12));
			UnitData[][] output = new UnitData[8][];

			for (int p = 0; p < 8; p++)
			{
				List<UnitData> unitData = new List<UnitData>();
				
				for (byte u = 0; u < 128; u++)
				{
					int offset = p * 128 * 12;
					
					if (bytes[offset + 3] == 0xFF) continue;
					unitData.Add(new UnitData()
					{
						Id = u,
						Status = bytes[offset + 0],
						X = bytes[offset + 1],
						Y = bytes[offset + 2],
						TypeId = bytes[offset + 3],
						RemainingMoves = bytes[offset + 4],
						SpecialMoves = bytes[offset + 5],
						GotoX = bytes[offset + 6],
						GotoY = bytes[offset + 7],
						Visibility = bytes[offset + 9],
						NextUnitId = bytes[offset + 10],
						HomeCityId = bytes[offset + 11]
					});
				}
				output[p] = unitData.ToArray();
			}
			return output;
		}

		private ushort[] GetWonders() => GetArray<ushort>(nameof(SaveData.Wonders), 22);

		private ushort[] GetAdvanceFirstDiscovery() => GetArray<ushort>(nameof(SaveData.AdvanceFirstDiscovery), 72);

		private bool[][,] GetTileVisibility()
		{
			byte[] bytes = GetArray(nameof(SaveData.MapVisibility), (80 * 50));
			bool[][,] output = new bool[8][,];
			for (int p = 0; p < 8; p++)
			{
				output[p] = new bool[80, 50];
				for (int i = 0; i < (80 * 50); i++)
				{
					int y = (i % 50), x = (i - y) / 50;
					output[p][x, y] = ((bytes[i] & (1 << p)) > 0);
				}
			}
			return output;
		}
	}
}