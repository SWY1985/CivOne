// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne.Screens.Reports
{
	internal class IntelligenceReport : BaseReport
	{
		private readonly Dictionary<Player, Rectangle> _infoButtons = new Dictionary<Player, Rectangle>();

		private void MouseDown(object sender, ScreenEventArgs args)
		{
			if (_infoButtons.Count == 0) return;

			foreach (KeyValuePair<Player, Rectangle> infoButton in _infoButtons)
			{
				if (!infoButton.Value.Contains(args.X, args.Y)) continue;

				Player player = infoButton.Key;

				this.FillRectangle(0, 25, 320, 172, BackgroundColour)
					.DrawText($"Subject: the {player.TribeNamePlural}", 0, 5, 16, 33)
					.DrawText($"Subject: the {player.TribeNamePlural}", 0, 15, 16, 32)
					.DrawText("Leader:", 0, 9, 16, 44)
					.DrawText($"Emperor {player.LeaderName}", 0, 15, 62, 44)
					.DrawText("Capital:", 0, 9, 16, 80)
					.DrawText(player.Capital, 0, 15, 63, 80)
					.DrawText("Government:", 0, 9, 16, 88)
					.DrawText(player.Government.Name, 0, 15, 83, 88)
					.DrawText("Treasury:", 0, 9, 16, 96)
					.DrawText($"{player.Gold}$", 0, 15, 73, 96)
					.DrawText("Military:", 0, 9, 16, 104)
					.DrawText($"{Game.GetUnits().Count(x => player == x.Owner)} Units", 0, 15, 67, 104)
					.DrawText("Foreign Affairs", 0, 9, 16, 116)
					.DrawText("Technologies:", 0, 9, 16, 136);

				args.Handled = true;
				SetUpdate();
			}

			if (args.Handled) _infoButtons.Clear();
		}

		public IntelligenceReport() : base("INTELLIGENCE REPORT", 1, MouseCursor.Pointer)
		{
			OnMouseDown += MouseDown;

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
						_infoButtons.Add(player, new Rectangle(281, yy + 14, 38, Resources.GetFontHeight(0) + 2));
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