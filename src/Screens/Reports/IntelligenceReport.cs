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
using CivOne.Players;

namespace CivOne.Screens.Reports
{
	internal class IntelligenceReport : BaseReport
	{
		private readonly Dictionary<IPlayer, Rectangle> _infoButtons = new Dictionary<IPlayer, Rectangle>();

		private void MouseDown(object sender, ScreenEventArgs args)
		{
			if (_infoButtons.Count == 0) return;

			foreach (KeyValuePair<IPlayer, Rectangle> infoButton in _infoButtons)
			{
				if (!infoButton.Value.Contains(args.X, args.Y)) continue;

				int y = 32;
				int fontHeight = Resources.GetFontHeight(0);

				IPlayer player = infoButton.Key;

				this.FillRectangle(0, 25, 320, 172, BackgroundColour)
					.DrawText($"Subject: the {player.Civilization.NamePlural}", 0, 5, 16, (y + 1))
					.DrawText($"Subject: the {player.Civilization.NamePlural}", 0, 15, 16, y)
					.DrawText("Leader:", 0, 9, 16, (y += fontHeight + 4))
					.DrawText($"Emperor {player.Leader.Name}", 0, 15, 62, y);
				
				foreach (string line in player.Civilization.Leader.Traits())
					this.DrawText(line, 0, 7, 24, (y += fontHeight));

				this.DrawText("Capital:", 0, 9, 16, (y += fontHeight + 4))
					.DrawText(player.GetCapital()?.Name ?? "NONE", 0, 15, 63, y)
					.DrawText("Government:", 0, 9, 16, (y += fontHeight))
					.DrawText(player.Government.Name, 0, 15, 83, y)
					.DrawText("Treasury:", 0, 9, 16, (y += fontHeight))
					.DrawText($"{player.Gold}$", 0, 15, 73, y)
					.DrawText("Military:", 0, 9, 16, (y += fontHeight))
					.DrawText($"{Game.GetUnits().Count(x => player.Is(x.Owner))} Units", 0, 15, 67, y)
					.DrawText("Foreign Affairs:", 0, 9, 16, (y += fontHeight + 4))
					.DrawText("Technologies:", 0, 9, 16, (y += fontHeight + 4));

				args.Handled = true;
				SetUpdate();
			}

			if (args.Handled) _infoButtons.Clear();
		}

		public IntelligenceReport() : base("INTELLIGENCE REPORT", 1, MouseCursor.Pointer)
		{
			OnMouseDown += MouseDown;

			int yy = 30;
			foreach (IPlayer player in Game.Players.Where(p => !p.Is(0) && !p.IsDestroyed()))
			{
				this.FillRectangle(4, yy, 313, 1, 9);

				byte id = Game.PlayerNumber(player);
				byte colour = Common.ColourLight[id];
				if (player is HumanPlayer || Human.HasEmbassy(player))
				{
					int unitCount = Game.GetUnits().Count(u => u.Owner == id && u.Home != null);

					this.DrawText($"{player.Civilization.NamePlural}: {player.Leader.Name}", 0, 5, 8, yy + 3)
						.DrawText($"{player.Civilization.NamePlural}: {player.Leader.Name}", 0, 15, 8, yy + 2)
						.DrawText($"{player.Government.Name}, {player.Gold}$, {unitCount} Units.", 0, colour, 160, yy + 2);

					if (!(player is HumanPlayer))
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