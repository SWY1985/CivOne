// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Runtime.InteropServices;
using System.Text;
using CivOne.IO;

namespace CivOne
{
	internal unsafe class SaveDataAdapter : IGameData
	{
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

		public string[] LeaderNames
		{
			get => _saveData.GetLeaderNames();
			set => throw new NotImplementedException();
		}

		public string[] CivilizationNames
		{
			get => _saveData.GetCivilizationNames();
			set => throw new NotImplementedException();
		}

		public string[] CitizenNames
		{
			get => _saveData.GetCitizenNames();
			set => throw new NotImplementedException();
		}

		public string[] CityNames
		{
			get => _saveData.GetCityNames();
			set => throw new NotImplementedException();
		}

		public short[] PlayerGold
		{
			get => _saveData.GetPlayerGold();
			set => throw new NotImplementedException();
		}

		public short[] ResearchProgress
		{
			get => _saveData.GetResearchProgress();
			set => throw new NotImplementedException();
		}

		public byte[][] DiscoveredAdvanceIDs
		{
			get => _saveData.GetDiscoveredAdvanceIDs();
			set => throw new NotImplementedException();
		}

		public ushort[] TaxRate
		{
			get => _saveData.GetTaxRate();
			set => throw new NotImplementedException();
		}

		public ushort[] ScienceRate
		{
			get => _saveData.GetScienceRate();
			set => throw new NotImplementedException();
		}

		public ushort[] StartingPositionX
		{
			get => _saveData.GetStartingPositionX();
			set => throw new NotImplementedException();
		}

		public ushort[] Government
		{
			get => _saveData.GetGovernment();
			set => throw new NotImplementedException();
		}

		public CityData[] CityData
		{
			get => _saveData.GetCityData();
			set => throw new NotImplementedException();
		}

		public UnitData[][] UnitData
		{
			get => _saveData.GetUnitData();
			set => throw new NotImplementedException();
		}

		public ushort[] Wonders
		{
			get => _saveData.GetWonders();
			set => throw new NotImplementedException();
		}

		public bool[][,] TileVisibility
		{
			get => _saveData.GetTileVisibility();
			set => throw new NotImplementedException();
		}

		public ushort[] AdvanceFirstDiscovery
		{
			get => _saveData.GetAdvanceFirstDiscovery();
			set => throw new NotImplementedException();
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

		void IDisposable.Dispose()
		{
		}
	}
}