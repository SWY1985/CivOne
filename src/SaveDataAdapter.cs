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
using System.Text;
using CivOne.Enums;
using CivOne.IO;

namespace CivOne
{
	internal unsafe partial class SaveDataAdapter : IGameData
	{
		private CityData[] DefaultCityData => Enumerable.Range(0, 128).Select(x => new CityData() { Status = 0xFF }).ToArray();
		private UnitData[][] DefaultUnitData => Enumerable.Repeat(Enumerable.Range(0, 128).Select(id => new UnitData() { Id = (byte)id, TypeId = 0xFF }).ToArray(), 8).ToArray();

		private byte[] DefaultUnitTypes => new byte[952]; // TODO

		private SaveData _saveData;

		public ushort GameTurn
		{
			get => _saveData.GameTurn;
			set
			{
				_saveData.GameTurn = value;
				_saveData.GameYear = (short)Common.TurnToYear(value);
			}
		}

		public ushort HumanPlayer
		{
			get => _saveData.HumanPlayer;
			set
			{
				_saveData.HumanPlayer = value;
				_saveData.HumanPlayerBit = (byte)(0x01 << value);
			}
		}

		public ushort RandomSeed
		{
			get => _saveData.RandomSeed;
			set => _saveData.RandomSeed = value;
		}

		public ushort Difficulty
		{
			get => _saveData.Difficulty;
			set => _saveData.Difficulty = value;
		}

		public bool[] ActiveCivilizations
		{
			get
			{
				bool[] output = new bool[8];
				for (int i = 0; i < 8; i++)
					output[i] = (_saveData.ActiveCivilizations & (1 << i)) > 0;
				return output;
			}
			set
			{
				ushort setValue = 0;
				for (int i = 0; i < value.Length; i++)
					setValue |= (ushort)(value[i] ? (1 << i) : 0);
				_saveData.ActiveCivilizations = setValue;
			}
		}

		public byte[] CivilizationIdentity
		{
			get
			{
				byte[] output = new byte[8];
				for (int i = 0; i < 8; i++)
					output[i] = (byte)(((_saveData.CivilizationIdentityFlag & (1 << i)) > 0) ? 1 : 0);
				return output;
			}
			set
			{
				ushort setValue = 0;
				for (int i = 0; i < value.Length; i++)
					setValue |= (ushort)(value[i] % 2);
				_saveData.CivilizationIdentityFlag = setValue;
			}
		}

		public ushort CurrentResearch
		{
			get => _saveData.CurrentResearch;
			set => _saveData.CurrentResearch = value;
		}

		public byte[][] DiscoveredAdvanceIDs
		{
			get => _saveData.GetDiscoveredAdvanceIDs();
			set
			{
				SetDiscoveredAdvanceIDs(value);
				SetArray(nameof(SaveData.AdvancesCount), value.Select(x => (ushort)x.Count()).ToArray());
			}
		}

		public string[] LeaderNames
		{
			get => _saveData.GetLeaderNames();
			set => SetArray(nameof(SaveData.LeaderNames), 14, value);
		}

		public string[] CivilizationNames
		{
			get => _saveData.GetCivilizationNames();
			set => SetCivilizationNames(value);
		}

		public string[] CitizenNames
		{
			get => _saveData.GetCitizenNames();
			set => SetCitizenNames(value);
		}

		public string[] CityNames
		{
			get => _saveData.GetCityNames();
			set => SetCityNames(value);
		}

		public short[] PlayerGold
		{
			get => _saveData.GetPlayerGold();
			set => SetPlayerGold(value);
		}

		public short[] ResearchProgress
		{
			get => _saveData.GetResearchProgress();
			set => SetResearchProgress(value);
		}

		public ushort[] TaxRate
		{
			get => _saveData.GetTaxRate();
			set => SetTaxRate(value);
		}

		public ushort[] ScienceRate
		{
			get => _saveData.GetScienceRate();
			set => SetScienceRate(value);
		}

		public ushort[] StartingPositionX
		{
			get => _saveData.GetStartingPositionX();
			set => SetStartingPositionX(value);
		}

		public ushort[] Government
		{
			get => _saveData.GetGovernment();
			set => SetGovernment(value);
		}

		public CityData[] CityData
		{
			get => _saveData.GetCityData();
			set
			{
				SetCityData(value);
				SetCityX(value.Select(c => c.X).ToArray());
				SetCityY(value.Select(c => c.Y).ToArray());
				SetCityCount(Enumerable.Range(0, 8).Select(i => (ushort)value.Count(c => c.Owner == i)).ToArray());
				SetTotalCitySize(Enumerable.Range(0, 8).Select(i => (ushort)value.Sum(c => c.ActualSize)).ToArray());
			}
		}

		public UnitData[][] UnitData
		{
			get => _saveData.GetUnitData();
			set
			{
				SetUnitData(value);
				SetUnitCount(value.Select(p => (ushort)p.Count()).ToArray());
				SetUnitsActive(value);
				SetSettlerCount(value.Select(p => (ushort)p.Count(u => u.TypeId == (byte)UnitType.Settlers)).ToArray());
			}
		}

		public ushort[] Wonders
		{
			get => _saveData.GetWonders();
			set => SetWonders(value);
		}

		public bool[][,] TileVisibility
		{
			get => _saveData.GetTileVisibility();
			set => SetTileVisibility(value);
		}

		public ushort[] AdvanceFirstDiscovery
		{
			get => _saveData.GetAdvanceFirstDiscovery();
			set => SetAdvanceFirstDiscovery(value);
		}

		public bool[] GameOptions
		{
			get
			{
				bool[] output = new bool[8];
				for (int i = 0; i < output.Length; i++)
					output[i] = ((_saveData.GameOptions & (1 << i)) > 0);
				return output;
			}
			set
			{
				ushort setValue = 0;
				for (int i = 0; i < value.Length; i++)
					setValue |= (ushort)(value[i] ? 1 << i : 0);
				_saveData.GameOptions = setValue;
			}
		}

		public ushort NextAnthologyTurn
		{
			get => _saveData.NextAnthologyTurn;
			set => _saveData.NextAnthologyTurn = value;
		}

		public ushort OpponentCount
		{
			get => _saveData.OpponentCount;
			set => _saveData.OpponentCount = value;
		}

		public bool ValidData { get; private set; }

		public byte[] GetBytes()
		{
			if (!ValidData) return new byte[0];

			byte[] output = new byte[Marshal.SizeOf(typeof(SaveData))];
			IntPtr buffer = Marshal.AllocHGlobal(output.Length);
			Marshal.StructureToPtr<SaveData>(_saveData, buffer, false);
			Marshal.Copy(buffer, output, 0, output.Length);
			Marshal.FreeHGlobal(buffer);
			return output;
		}

		public static SaveDataAdapter Load(byte[] input) => new SaveDataAdapter(input);

		private SaveDataAdapter(byte[] input)
		{
			int expectedSize = Marshal.SizeOf(typeof(SaveData));
			if (input.Length != expectedSize)
			{
				RuntimeHandler.Runtime.Log($"SaveDataAdapter: Invalid file size {input.Length} (expected {expectedSize})");

				ValidData = false;
				return;
			}

			IntPtr dataPtr = Marshal.AllocHGlobal(expectedSize);
			Marshal.Copy(input, 0, dataPtr, input.Length);
			_saveData = Marshal.PtrToStructure<SaveData>(dataPtr);
			Marshal.FreeHGlobal(dataPtr);

			ValidData = true;
		}

		internal SaveDataAdapter()
		{
			_saveData = new SaveData();
			SetCityData(DefaultCityData);
			SetUnitTypes(DefaultUnitTypes);
			SetUnitData(DefaultUnitData);
			SetWonders(Enumerable.Repeat(ushort.MaxValue, 22).ToArray());
			SetCityX(Enumerable.Repeat((byte)0xFF, 256).ToArray());
			SetCityY(Enumerable.Repeat((byte)0xFF, 256).ToArray());
			ValidData = true;
		}

		void IDisposable.Dispose()
		{
		}
	}
}