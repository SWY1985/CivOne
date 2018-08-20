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
	public interface IPlayer
	{
		ICivilization Civilization { get; }
		ILeader Leader { get; }
		IEnumerable<IPlayer> Embassies { get; }
		IAdvance CurrentResearch { get; set; }
		PalaceData Palace { get; }

		short StartX { get; set; }
		int CityNamesSkipped { get; set; }

		void ChooseGovernment();
		void EstablishEmbassy(IPlayer player);
	}
}