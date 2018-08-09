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
using CivOne.Advances;
using CivOne.Civilizations;
using CivOne.Governments;
using CivOne.Leaders;
using CivOne.Units;

namespace CivOne.Players
{
	public interface IPlayer : ITurn
	{
		event Action<IPlayer> OnDestroy;

		ICivilization Civilization { get; }
		ILeader Leader { get; }
		string TribeName { get; }
		string TribeNamePlural { get; }
		IGovernment Government { get; set; }
		IEnumerable<IAdvance> Advances { get; }
		IEnumerable<IPlayer> Embassies { get; }
		IAdvance CurrentResearch { get; set; }
		PalaceData Palace { get; }

		int LuxuriesRate { get; set; }
		int TaxesRate { get; set; }
		int ScienceRate { get; }

		bool Destroyed { get; }
		short Gold { get; set; }
		short Science { get; set; }
		short StartX { get; set; }
		int CityNamesSkipped { get; set; }
		byte Handicap { get; set; }

		void AddAdvance(IAdvance advance, bool setOrigin = true);
		void DeleteAdvance(IAdvance advance);
		void Destroy();
		void EstablishEmbassy(IPlayer player);
		void Explore(int x, int y, int range = 1, bool sea = false);
		void Revolt();
		bool Visible(int x, int y);
	}
}