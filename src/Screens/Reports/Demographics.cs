// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Linq;
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne.Screens.Reports
{
	internal class Demographics : BaseScreen
	{
		private struct TableRow
		{
			public string Title;
			public string Value;
			public int Place;

			public TableRow(string title, string value, int place)
			{
				Title = title;
				Value = value;
				Place = place;
			}
		}

		private readonly TextSettings _shadowText, _normalText;
		private bool _update = true;

		private TableRow Population()
		{
			string value = "00,000";
			if (Human.Population > 0) value = Common.NumberSeperator(Human.Population);
			
			int rank = 1;
			Player[] players = Game.Players
                .Where(x => !(x.Civilization is Barbarian))
                .OrderByDescending(x => x.Population)
                .ThenBy(x => Game.GameState.PlayerNumber(x))
                .ToArray();

			for (int i = 0; i < players.Length; i++)
			{
				if (Human != players[i]) continue;
				rank = (i + 1);
				break;
			}

			return new TableRow("Population", value, rank);
		}

		private IEnumerable<TableRow> GetTable()
		{
			yield return new TableRow("Approval Rating", "0%", 1);
			yield return Population();
			yield return new TableRow("GNP", "0 million $", 1);
			yield return new TableRow("Mfg. Goods", "0 Mtons", 1);
			yield return new TableRow("Land Area", "0 sq.miles", 1);
			yield return new TableRow("Literacy", "0%", 1);
			yield return new TableRow("Disease", "0%", 1);
			yield return new TableRow("Pollution", "00 tons/year", 1);
			yield return new TableRow("Life expectancy", "20 years", 1);
			yield return new TableRow("Family Size", "2.0 children", 1);
			yield return new TableRow("Military Service", "0 years", 1);
			yield return new TableRow("Annual Income", "0$ per capita", 1);
			yield return new TableRow("Productivity", "0", 1);
		}

		private string Ordinal(int number)
		{
			switch (number)
			{
				case 1: return $"{number}st";
				case 2: return $"{number}nd";
				case 3: return $"{number}rd";
				default: return $"{number}th";
			}
		}

		protected override bool HasUpdate(uint gameTick)
		{
			if (!_update) return false;

			int yy = 21;
			foreach (TableRow tableEntry in GetTable())
			{
				this.DrawRectangle(4, yy, 312, 1, 9)
					.DrawText($"{tableEntry.Title}:", 8, yy + 3, _shadowText)
					.DrawText(tableEntry.Value, 104, yy + 3, _normalText)
					.DrawText(Ordinal(tableEntry.Place), 192, yy + 3, _shadowText);
				yy += 12;
			}

			_update = false;
			return true;
		}

		public override bool KeyDown(KeyboardEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			Destroy();
			return true;
		}

		public Demographics()
		{
			Palette = Common.DefaultPalette;

			_normalText = new TextSettings() { Colour = 15 };
			_shadowText = TextSettings.ShadowText(15, 5);
			
			this.Clear(1)
				.DrawText($"{Human.TribeName} Demographics", 0, 15, 160, 4, TextAlign.Center);
		}
	}
}