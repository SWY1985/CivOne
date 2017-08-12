// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Screens;

namespace CivOne.Tasks
{
	internal class ProcessScience : GameTask
	{
		private readonly Player _player;
		private readonly bool _human;
		
		private void CivilopediaClosed(object sender, EventArgs args)
		{
			_player.CurrentResearch = null;
			GameTask.Insert(new TechSelect(_player));
			EndTask();
		}

		private void ClosedDiscovery(object sender, EventArgs args)
		{
			Screens.Civilopedia civilopedia = new Screens.Civilopedia(_player.CurrentResearch, discovered: true);
			civilopedia.Closed += CivilopediaClosed;
			Common.AddScreen(civilopedia);
		}
		
		public override void Run()
		{
			if (_player.CurrentResearch == null)
			{
				if (_human)
					GameTask.Enqueue(new TechSelect(_player));
				else
					AI.ChooseResearch(_player);
				EndTask();
				return;
			}

			if (_player.Science < _player.ScienceCost)
			{
				// Not enough lightbulbs, end the task
				EndTask();
				return;
			}

			_player.Science -= _player.ScienceCost;
			_player.AddAdvance(_player.CurrentResearch);

			if (!_human)
			{
				// This is an AI player, handle everything in the background.
				_player.CurrentResearch = null;
				AI.ChooseResearch(_player);
				EndTask();
				return;
			}

			IScreen discovery;
			if (Settings.Animations)
			{
				discovery = new Discovery(_player.CurrentResearch);
			}
			else
			{
				discovery = new Newspaper(null, new string[] { $"{_player.TribeName} wise men", "discover the secret", $"of {_player.CurrentResearch.Name}!" }, showGovernment: false);
			}
			discovery.Closed += ClosedDiscovery;
			Common.AddScreen(discovery);
		}

		public ProcessScience(Player player)
		{
			_player = player;
			_human = (Human == player);
		}
	}
}