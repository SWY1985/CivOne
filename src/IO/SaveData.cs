// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Runtime.InteropServices;

namespace CivOne.IO
{
	// Save file structure:
	// - http://forums.civfanatics.com/showthread.php?p=12422448
	// - http://forums.civfanatics.com/showthread.php?t=493581
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct SaveData
	{
		public ushort GameTurn;								// 0:1
		public ushort HumanPlayer;							// 2:3
		public ushort HumanPlayerBit;						// 4:5
		public ushort RandomSeed;							// 6:7
		public short GameYear;								// 8:9
		public ushort Difficulty;							// 10:11
		public ushort ActiveCivilizations;					// 12:13
		public ushort CurrentResearch;						// 14:15
		public fixed byte LeaderNames[8 * 14];				// 16:127
		public fixed byte CivilizationNames[8 * 12];		// 128:223
		public fixed byte CitizensName[8 * 11];				// 224:311
		public fixed short PlayerGold[8];					// 312:327
		public fixed short ResearchProgress[8];				// 328:343
		public fixed ushort UnitsActive[8 * 28];			// 344:791
		public fixed ushort UnitsInProduction[8 * 28];		// 792:1239
		public fixed ushort AdvancesCount[8];				// 1240:1255
		public fixed byte DiscoveredAdvances[8 * 10];		// 1256:1335
		public fixed ushort Government[8];					// 1336:1351
		public fixed ushort ContinentStrategy[8 * 16];		// 1352:1607
		public fixed ushort Diplomacy[8 * 8];				// 1608:1735
		public fixed ushort CityCount[8];					// 1736:1751
		public fixed ushort UnitCount[8];					// 1752:1767
		public fixed ushort LandCount[8];					// 1768:1783
		public fixed ushort SettlerCount[8];				// 1784:1799
		public fixed ushort TotalCitySize[8];				// 1800:1815
		public fixed ushort MilitaryPower[8];				// 1816:1831
		public fixed ushort Ranking[8];						// 1832:1847
		public fixed ushort TaxRate[8];						// 1848:1863
		public fixed ushort CivilizationScore[8];			// 1864:1879
		public fixed ushort HumanContactTurns[8];			// 1880:1895
		public fixed ushort StartingPositionX[8];			// 1896:1911
		public fixed ushort LeaderGraphics[8];				// 1912:1927
		public fixed ushort ContinentDefense[8 * 16];		// 1928:2183
		public fixed ushort ContinentAttack[8 * 16];		// 2184:2439
		public fixed ushort ContintentCities[8 * 16];		// 2440:2695
		public fixed ushort ContinentSizes[16];				// 2696:2727
		private fixed byte _padding1[96];					// 2828:2823 (unknown data)
		public fixed ushort OceanSizes[16];					// 2824:2855
		private fixed byte _padding2[96];					// 2856:2951 (unknown data)
		public fixed ushort ContinentBuildingSites[16];		// 2952:2983
		public fixed byte ScoreChart[8 * 150];				// 2984:4183
		public fixed byte PeaceChart[8 * 150];				// 4184:5383
		public fixed byte Cities[128 * 28];					// 5384:8967
		public fixed byte UnitTypes[28 * 34];				// 8968:9919
		public fixed byte Units[8 * 128 * 12];				// 9920:22207
		public fixed byte MapVisibility[80 * 50];			// 22208:26207
		public fixed sbyte StrategicLocationStatus[8 * 16];	// 26208:26335
		public fixed byte StrategicLocationPolicy[8 * 16];	// 26336:26463
		public fixed byte StrategicLocationX[8 * 16];		// 26464:26591
		public fixed byte StrategicLocationY[8 * 16];		// 26592:26719
		public fixed ushort AdvanceFirstDiscovery[72];		// 26720:26863
		public fixed byte UnitsDestroyedBy[8 * 16];			// 26864:26991
		public fixed byte CityNames[256 * 13];				// 26992:30319
		public ushort ReplayLength;							// 30320:30321
		public fixed byte ReplayData[4096];					// 30322:34417
		public fixed ushort Wonders[22];					// 34418:34461
		public fixed ushort UnitsLost[8 * 28];				// 34462:34909
		public fixed byte AdvanceOrigin[8 * 72];			// 34910:35485
		public ushort PollutionSquares;						// 35486:35487
		public ushort PollutionEffect;						// 35488:35489
		public ushort GlobalWarming;						// 35490:35491
		public ushort GameOptions;							// 35492:35493
		public fixed byte LandPathfinding[260];				// 35494:35753
		public ushort MaxAdvances;							// 35754:35755
		public ushort PlayerFutureTech;						// 35756:35757
		public ushort DebugSwitches;						// 35758:35759
		public fixed ushort ScienceRate[8];					// 35760:35775
		public ushort NextAnthologyTurn;					// 35776:35777
		public fixed ushort EpicRanking[8];					// 35778:35793
		public fixed byte SpaceShips[1462];					// 35794:37255
		public fixed byte Palace[8 * 6];					// 37256:37303
		public fixed byte CityX[256];						// 37304:37559
		public fixed byte CityY[256];						// 37560:37815
		public ushort PalaceLevel;							// 37816:37817
		public ushort PeaceTurns;							// 37818:37819
		public ushort OpponentCount;						// 37820:37821
		public fixed ushort SpaceShipPopulation[8];			// 37822:37837
		public fixed short SpaceShipLaunchYear[8];			// 37838:37853
		public ushort CivilizationIdentityFlag;				// 37854:37855
	}
}