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
			foreach (Player player in Game.Players.Where(p => p != 0 && !p.IsDestroyed()))
			{
				this.FillRectangle(4, yy, 313, 1, 9);

				byte id = Game.PlayerNumber(player);
				byte colour = Common.ColourLight[id];
				if (player.IsHuman || Human.HasEmbassy(player))
				{
					int unitCount = Game.GetUnits().Count(u => u.Owner == id && u.Home != null);

					this.DrawText($"{player.TribeNamePlural}: {player.LeaderName}", 0, 5, 8, yy + 3)
						.DrawText($"{player.TribeNamePlural}: {player.LeaderName}", 0, 15, 8, yy + 2)
						.DrawText($"{player.Government.Name}, {player.Gold}$, {unitCount} Units.", 0, colour, 160, yy + 2);

					if (!player.IsHuman)
					{
						this.DrawButton($"INFO{id}", 0, colour, Common.ColourDark[id], 281, yy + 14, 38, Resources.GetFontHeight(0) + 2);
					}
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