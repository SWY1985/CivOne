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
using CivOne.Advances;
using CivOne.Civilizations;
using CivOne.Governments;
using CivOne.Leaders;
using CivOne.Tasks;
using CivOne.Wonders;

namespace CivOne.Players
{
	internal abstract class BasePlayer : BaseInstance, IPlayer
	{
		private readonly List<byte> _embassies = new List<byte>();

		public ICivilization Civilization { get; }
		public ILeader Leader => Civilization.Leader;
		public PalaceData Palace { get; } = new PalaceData();

		public IEnumerable<IPlayer> Embassies => _embassies.OrderBy(x => x).Select(e => Game.Instance.GetPlayer(e));

		public IAdvance CurrentResearch { get; set; }
		
		public short StartX { get; set; }

		public int CityNamesSkipped { get; set; }

		public abstract void ChooseGovernment();

		public void EstablishEmbassy(IPlayer player)
		{
			byte playerNumber = Game.PlayerNumber(player);
			if (_embassies.Contains(playerNumber)) return;
			_embassies.Add(playerNumber);
		}

		public BasePlayer(ICivilization civilization, string leaderName = null, string civilizationName = null, string citizenName = null)
		{
			Civilization = civilization;
			if (leaderName != null) Civilization.Leader.Name = leaderName;
			if (civilizationName != null) Civilization.Name = civilizationName;
			if (citizenName != null) Civilization.NamePlural = citizenName;
		}

		public BasePlayer(BasePlayer player)
		{
			Civilization = player.Civilization;

			_embassies.AddRange(player._embassies);
			
			Palace = player.Palace;

			CurrentResearch = player.CurrentResearch;

			StartX = player.StartX;
			CityNamesSkipped = player.CityNamesSkipped;
		}
	}
}