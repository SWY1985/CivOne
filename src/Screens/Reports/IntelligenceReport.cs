// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Enums;
using CivOne.Graphics;

namespace CivOne.Screens.Reports
{
	internal class IntelligenceReport : BaseReport
	{
		public IntelligenceReport() : base("INTELLIGENCE REPORT", 1)
		{
			int yy = 30;
			foreach (Player player in Game.Players.Where(p => p != 0))
			{
				if (player.DestroyTurn >= 0) continue;
				this.FillRectangle(4, yy, 314, 1, 9);

				byte id = Game.GameState.PlayerNumber(player);
				byte colour = Common.ColourLight[id];
				if (player.IsHuman)
				{
					int unitCount = Game.GameState.GetUnits().Count(u => u.Owner == id && u.Home != null);

					this.DrawText($"{player.TribeNamePlural}: {player.LeaderName}", 0, 5, 8, yy + 3)
						.DrawText($"{player.TribeNamePlural}: {player.LeaderName}", 0, colour, 8, yy + 2)
						.DrawText($"{player.Government.Name}, {player.Gold}$, {unitCount} Units.", 0, colour, 160, yy + 2);
				}
				else
				{
					this.DrawText("No embassy established.", 0, colour, 160, yy + 2, TextAlign.Center);
				}

				yy += 24;
			}
		}
	}
}