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
using CivOne.Players;
using CivOne.Wonders;

namespace CivOne
{
	internal partial class Game : IGameData
	{
		ushort IGameData.GameTurn { get; set; }
		ushort IGameData.HumanPlayer
		{
			get => PlayerNumber(Human);
			set => SetHumanPlayer(GetPlayer((byte)value));
		}
		ushort IGameData.RandomSeed
		{
			get => (ushort)Map.Instance.TerrainMasterWord;
			set
			{
				// read-only
			}
		}
		ushort IGameData.Difficulty { get; set; }
		bool[] IGameData.ActiveCivilizations { get; set; } = new bool[MAX_PLAYER_COUNT];
		byte[] IGameData.CivilizationIdentity
		{
			get => _players.Select(x => (byte)(x.Civilization.Id > 7 ? 1 : 0)).ToArray();
			set
			{
				// read-only
			}
		}
		ushort IGameData.CurrentResearch
		{
			get => Human.CurrentResearch?.Id ?? 0;
			set
			{
				// read-only
			}
		}
		byte[][] IGameData.DiscoveredAdvanceIDs { get; set; } = Enumerable.Range(0, MAX_PLAYER_COUNT).Select(x => new byte[0]).ToArray();
		string[] IGameData.LeaderNames
		{
			get => _players.Select(x => x.Leader.Name).ToArray();
			set
			{
				for (int i = 0; i < value.Length && i < _players.Length; i++)
					_players[i].Leader.Name = value[i];
			}
		}
		string[] IGameData.CivilizationNames
		{
			get => _players.Select(x => x.Civilization.NamePlural).ToArray();
			set
			{
				for (int i = 0; i < value.Length && i < _players.Length; i++)
					_players[i].Civilization.NamePlural = value[i];
			}
		}
		string[] IGameData.CitizenNames
		{
			get => _players.Select(x => x.Civilization.Name).ToArray();
			set
			{
				for (int i = 0; i < value.Length && i < _players.Length; i++)
					_players[i].Civilization.Name = value[i];
			}
		}
		string[] IGameData.CityNames { get; set; } = Common.AllCityNames.ToArray();
		short[] IGameData.PlayerGold { get; set; } = new short[MAX_PLAYER_COUNT];
		short[] IGameData.ResearchProgress { get; set; } = new short[MAX_PLAYER_COUNT];
		ushort[] IGameData.TaxRate { get; set; } = Enumerable.Repeat((ushort)5, MAX_PLAYER_COUNT).ToArray();
		ushort[] IGameData.ScienceRate { get; set; } = Enumerable.Repeat((ushort)5, MAX_PLAYER_COUNT).ToArray();
		ushort[] IGameData.StartingPositionX
		{
			get => _players.Select(x => (ushort)x.StartX).ToArray();
			set
			{
				// read-only
			}
		}
		ushort[] IGameData.Government { get; set; } = new ushort[MAX_PLAYER_COUNT];
		CityData[] IGameData.Cities
		{
			get => _cities.GetCityData().ToArray();
			set
			{
				// read-only
			}
		}
		UnitData[][] IGameData.Units
		{
			get => _players.Select(p => _units.Where(u => p.Is(u.Owner)).GetUnitData().ToArray()).ToArray();
			set
			{
				// read-only
			}
		}
		ushort[] IGameData.Wonders
		{
			get
			{
				ushort[] wonders = Enumerable.Repeat(ushort.MaxValue, 22).ToArray();
				for (byte i = 0; i < _cities.Count(); i++)
				foreach (IWonder wonder in _cities[i].Wonders)
				{
					wonders[wonder.Id] = i;
				}
				return wonders;
			}
			set
			{
				// read-only
			}
		}
		bool[][,] IGameData.TileVisibility { get; set; } = new bool[Game.MAX_PLAYER_COUNT][,];
		ushort[] IGameData.AdvanceFirstDiscovery { get; set; } = new ushort[72];
		bool[] IGameData.GameOptions { get; set; } = new bool[8];
		ushort IGameData.NextAnthologyTurn { get; set; }
		ushort IGameData.OpponentCount
		{
			get => (ushort)_competition;
			set
			{
				// read-only
			}
		}
		ReplayData[] IGameData.ReplayData
		{
			get => _replayData.ToArray();
			set
			{
				_replayData.Clear();
				_replayData.AddRange(new List<ReplayData>(value));
			}
		}

		bool IGameData.ValidData => true;

		byte[] IGameData.GetBytes() => new byte[0];

		bool IGameData.ValidMapSize(int width, int height) => (width == Map.WIDTH && height == Map.HEIGHT);
		
		void IDisposable.Dispose()
		{
		}
	}
}