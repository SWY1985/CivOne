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
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens.Reports
{
	internal class ScienceReport : BaseReport
	{
		public ScienceReport() : base("SCIENCE REPORT", 1)
		{
			double width = 8;
			while ((width * Human.ScienceCost) > 200 || width <= 0.1)
			{
				width -= 0.1;
			}

			int barWidth = (int)Math.Ceiling(width * Human.ScienceCost);
			int barX = (320 - barWidth) / 2;
			_canvas.FillRectangle(9, barX, 25, barWidth, 16);

			if (Human.CurrentResearch != null)
			{
				_canvas.DrawText($"Researching {Human.CurrentResearch.Name}", 0, 5, 160, 26, TextAlign.Center);
				_canvas.DrawText($"Researching {Human.CurrentResearch.Name}", 0, 15, 159, 26, TextAlign.Center);

				int xx = -1;
				for (int i = 0; i < Human.Science; i++)
				{
					if (xx == (int)Math.Floor((width * i) + barX - 1)) continue;
					xx = (int)Math.Floor((width * i) + barX - 1);
					AddLayer(Icons.Science, xx, 32);
				}
			}

			int c = 0;
			foreach (IAdvance advance in Human.Advances.OrderBy(a => a.Id))
			{
				bool first = Game.GetAdvanceOrigin(advance, Human);
				int xx = 8 + ((c % 3) * 100);
				int yy = 42 + (((c - (c % 3)) / 3) * 7);
				_canvas.DrawText(advance.Name, 0, (byte)(first ? 15 : 11), xx, yy);
				c++;
			}

			if (barWidth > 205)
			{
				// Bar too wide, do not draw advisor
				return;
			}
			AddLayer(Portrait[(int)Advisor.Science], 278, 2);
		}
	}
}