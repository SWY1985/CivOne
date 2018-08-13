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
using CivOne.Players;
using CivOne.Screens;

namespace CivOne.Tasks
{
	internal class TechSelect : GameTask
	{
		private readonly Player _player;
		private readonly bool _human;

		private void ClosedChooseTech(object sender, EventArgs args)
		{
			EndTask();
		}

		public override void Run()
		{
			if (GameTask.Count<TechSelect>() > 1)
			{
				// Dialog already open
				EndTask();
				return;
			}

			if (!_human)
			{
				// This task is only for human players
				EndTask();
				return;
			}

			if (_player.Science == 0 && _player.GetCities().Sum(x => x.Science) == 0)
			{
				// This task is only for human players
				EndTask();
				return;
			}

			if (!_player.AvailableResearch().Any())
			{
				// No more research Available
				//TODO: Implement future techs.
				EndTask();
				return;
			}
			
			ChooseTech chooseTech = new ChooseTech();
			chooseTech.Closed += ClosedChooseTech;
			Common.AddScreen(chooseTech);
		}

		public TechSelect(Player player)
		{
			_player = player;
			_human = (Human == player);
		}
	}
}