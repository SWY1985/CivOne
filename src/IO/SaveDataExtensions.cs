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
using System.Text;

namespace CivOne.IO
{
	internal unsafe static class SaveDataExtensions
	{
		private static byte[] GetByteArray(byte* ptr, int length)
		{
			byte[] output = new byte[length];
			for (int i = 0; i < length; i++)
				output[i] = ptr[i];
			return output;
		}

		private static short[] GetShortArray(short* ptr, int length)
		{
			short[] output = new short[length];
			for (int i = 0; i < length; i++)
				output[i] = ptr[i];
			return output;
		}

		private static ushort[] GetUShortArray(ushort* ptr, int length)
		{
			ushort[] output = new ushort[length];
			for (int i = 0; i < length; i++)
				output[i] = ptr[i];
			return output;
		}

		private static IEnumerable<byte> GetBitIds(byte[] bytes, int startIndex, int length)
		{
			byte index = 0;
			for (int i = startIndex; i < startIndex + length; i++)
			for (int b = 0; b < 8; b++)
			{
				if ((bytes[i] & (1 << b)) > 0) yield return index;
				index++;
			}
		}

		private static string BytesToString(byte[] bytes, int startIndex, int length)
		{
			StringBuilder output = new StringBuilder();
			for (int i = startIndex; i < startIndex + length; i++)
			{
				if (bytes[i] == 0) break;
				output.Append((char)bytes[i]);
			}
			return output.ToString().Trim();
		}

		private static string[] GetStringArray(byte* ptr, int itemCount, int itemLength)
		{
			byte[] bytes = GetByteArray(ptr, (itemCount * itemLength));
			string[] output = new string[itemCount];
			for (int i = 0; i < itemCount; i++)
				output[i] = BytesToString(bytes, (i * itemLength), itemLength);
			return output;
		}

		public static string[] GetLeaderNames(this SaveData saveData) => GetStringArray(&saveData.LeaderNames[0], 8, 14);

		public static string[] GetCivilizationNames(this SaveData saveData) => GetStringArray(&saveData.CivilizationNames[0], 8, 12);

		public static string[] GetCitizenNames(this SaveData saveData) => GetStringArray(&saveData.CitizensName[0], 8, 11);

		public static string[] GetCityNames(this SaveData saveData) => GetStringArray(&saveData.CityNames[0], 256, 13);

		public static short[] GetPlayerGold(this SaveData saveData) => GetShortArray(&saveData.PlayerGold[0], 8);

		public static short[] GetResearchProgress(this SaveData saveData) => GetShortArray(&saveData.ResearchProgress[0], 8);

		public static byte[][] GetDiscoveredAdvanceIDs(this SaveData saveData)
		{
			byte[] bytes = GetByteArray(&saveData.DiscoveredAdvances[0], 8 * 10);
			byte[][] output = new byte[8][];
			for (int i = 0; i < 8; i++)
				output[i] = GetBitIds(bytes, i * 10, 10).ToArray();
			return output;
		}

		public static ushort[] GetGovernment(this SaveData saveData) => GetUShortArray(&saveData.Government[0], 8);

		public static ushort[] GetTaxRate(this SaveData saveData) => GetUShortArray(&saveData.TaxRate[0], 8);

		public static ushort[] GetScienceRate(this SaveData saveData) => GetUShortArray(&saveData.ScienceRate[0], 8);

		public static ushort[] GetStartingPositionX(this SaveData saveData) => GetUShortArray(&saveData.StartingPositionX[0], 8);

		public static CityData[] GetCityData(this SaveData saveData)
		{
			List<CityData> output = new List<CityData>();
			for (byte i = 0; i < 128; i++)
			{
				byte[] bytes = GetByteArray(&saveData.Cities[i * 28], 28);
				if (bytes[6] == 0xFF) continue;
				
				output.Add(new CityData()
				{
					Id = i,
					NameId = bytes[22],
					Buildings = GetBitIds(bytes, 0, 4).ToArray(),
					X = bytes[4],
					Y = bytes[5],
					ActualSize = bytes[7],
					CurrentProduction = bytes[9],
					Owner = bytes[11],
					Food = (ushort)((bytes[13] << 8) + bytes[12]),
					Shields = (ushort)((bytes[15] << 8) + bytes[14]),
					ResourceTiles = bytes.Skip(16).Take(6).ToArray()
				});
			}
			return output.ToArray();
		}

		public static UnitData[][] GetUnitData(this SaveData saveData)
		{
			UnitData[][] output = new UnitData[8][];
			for (int p = 0; p < 8; p++)
			{
				List<UnitData> unitList = new List<UnitData>();
				for (byte u = 0; u < 128; u++)
				{
					byte[] bytes = GetByteArray(&saveData.Units[(p * 128 * 12) + (u * 12)], 12);
					if (bytes[3] == 0xFF) continue;
					unitList.Add(new UnitData()
					{
						Id = u,
						Status = bytes[0],
						X = bytes[1],
						Y = bytes[2],
						TypeId = bytes[3],
						RemainingMoves = bytes[4],
						SpecialMoves = bytes[5],
						GotoX = bytes[6],
						GotoY = bytes[7],
						Visibility = bytes[9],
						NextUnitId = bytes[10],
						HomeCityId = bytes[11]
					});
				}
				output[p] = unitList.ToArray();
			}
			return output;
		}
		
		public static ushort[] GetWonders(this SaveData saveData) => GetUShortArray(&saveData.Wonders[0], 22);

		public static ushort[] GetAdvanceFirstDiscovery(this SaveData saveData) => GetUShortArray(&saveData.AdvanceFirstDiscovery[0], 72);

		public static bool[][,] GetTileVisibility(this SaveData saveData)
		{
			byte[] bytes = GetByteArray(&saveData.MapVisibility[0], (80 * 50));
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