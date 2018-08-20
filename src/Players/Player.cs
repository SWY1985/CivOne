// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using CivOne.Advances;
using CivOne.Civilizations;
using CivOne.Governments;
using CivOne.Leaders;

namespace CivOne.Players
{
	internal class Player : IPlayer
	{
		private readonly IPlayer _player;

		public IPlayer InnerPlayer => _player;

		public bool IsHuman => (_player is HumanPlayer);

		public short Gold
		{
			get => _player.GetGold();
			set => _player.SetGold(value);
		}

		public short Science
		{
			get => _player.GetScience();
			set => _player.SetScience(value);
		}

		public ICivilization Civilization => _player.Civilization;
		public ILeader Leader => _player.Leader;
		public IGovernment Government
		{
			get => _player.GetGovernment();
			set => _player.SetGovernment(value);
		}
		public IEnumerable<IPlayer> Embassies => _player.Embassies;
		public IAdvance CurrentResearch
		{
			get => _player.CurrentResearch;
			set => _player.CurrentResearch = value;
		}
		public PalaceData Palace => _player.Palace;

		public int TaxesRate
		{
			get => _player.GetTaxRate();
			set => _player.SetTaxRate(value);
		}
		public int ScienceRate
		{
			get => _player.GetScienceRate();
			set => _player.SetScienceRate(value);
		}
		public int LuxuriesRate
		{
			get => _player.GetLuxuryRate();
			set => _player.SetLuxuryRate(value);
		}

		public short StartX
		{
			get => _player.StartX;
			set => _player.StartX = value;
		}
		public int CityNamesSkipped
		{
			get => _player.CityNamesSkipped;
			set => _player.CityNamesSkipped = value;
		}

		public void AddAdvance(IAdvance advance, bool setOrigin = true) => _player.AddAdvance(advance, setOrigin);
		public void ChooseGovernment() => _player.ChooseGovernment();
		public void DeleteAdvance(IAdvance advance) => _player.DeleteAdvance(advance);
		public void EstablishEmbassy(IPlayer player) => _player.EstablishEmbassy(player);

		internal Player(IPlayer player) => _player = player;
	}
}