// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;

namespace CivOne
{
	public interface IGameData : IDisposable
	{
		ushort GameTurn { get; set; }
		ushort HumanPlayer { get; set; }
		ushort RandomSeed { get; set; }
		ushort Difficulty { get; set; }
		bool[] ActiveCivilizations { get; set; }
		byte[] CivilizationIdentity { get; set; }
		ushort CurrentResearch { get; set; }
		byte[][] DiscoveredAdvanceIDs { get; set ;}
		string[] LeaderNames { get; set; }
		string[] CivilizationNames { get; set; }
		string[] CitizenNames { get; set; }
		string[] CityNames { get; set; }
		short[] PlayerGold { get; set; }
		short[] ResearchProgress { get; set; }
		ushort[] TaxRate { get; set; }
		ushort[] ScienceRate { get; set; }
		ushort[] StartingPositionX { get; set; }
		ushort[] Government { get; set; }
		CityData[] Cities { get; set; }
		UnitData[][] Units { get; set; }
		ushort[] Wonders { get; set; }
		bool[][,] TileVisibility { get; set; }
		ushort[] AdvanceFirstDiscovery { get; set; }
		bool[] GameOptions { get; set; }
		ushort NextAnthologyTurn { get; set; }
		ushort OpponentCount { get; set; }

		bool ValidData { get; }
		byte[] GetBytes();
		bool ValidMapSize(int width, int height);
	}
}