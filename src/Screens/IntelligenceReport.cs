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
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class IntelligenceReport : BaseStatusScreen
	{
		public IntelligenceReport() : base("INTELLIGENCE REPORT", 1)
		{
			int yy = 30;
			foreach (Player player in Game.Players.Where(p => Game.PlayerNumber(p) > 0))
			{
				_canvas.FillRectangle(9, 4, yy, 314, 1);

				byte id = Game.PlayerNumber(player);
				byte colour = Common.ColourLight[id];
				if (player.Human)
				{
					int unitCount = Game.GetUnits().Count(u => u.Owner == id && u.Home != null);

					_canvas.DrawText($"{player.TribeNamePlural}: {player.LeaderName}", 0, 5, 8, yy + 3);
					_canvas.DrawText($"{player.TribeNamePlural}: {player.LeaderName}", 0, colour, 8, yy + 2);
					_canvas.DrawText($"{player.Government}, {player.Gold}$, {unitCount} Units.", 0, colour, 160, yy + 2);
				}
				else
				{
					_canvas.DrawText("No embassy established.", 0, colour, 160, yy + 2, TextAlign.Center);
				}

				yy += 24;
			}
		}
	}
}