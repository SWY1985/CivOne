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
using CivOne.Graphics;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.UserInterface;
using CivOne.Buildings;
using CivOne.Advances;
using System.Collections.Generic;

namespace CivOne.Screens.Dialogs
{
	internal class SelectAdvanceAfterCityCapture : BaseDialog
	{
		private const int FONT_ID = 0;

		private readonly Player _player;
		private readonly IList<IAdvance> _advances;

		private void Steal(IAdvance advance)
		{
			GameTask.Enqueue(new Tasks.GetAdvance(_player, advance));
			Cancel();
		}

		protected override void FirstUpdate()
		{
			Menu menu = new Menu(Palette, Selection(3, 5 + (1 * Resources.GetFontHeight(FONT_ID)), 130, ((2 * Resources.GetFontHeight(FONT_ID)) + (_advances.Count * Resources.GetFontHeight(FONT_ID)) + 9)))
			{
				X = 103,
				Y = 95,
				MenuWidth = 130,
				ActiveColour = 11,
				TextColour = 5,
				FontId = FONT_ID
			};

			foreach (IAdvance advance in _advances)
			{
				menu.Items.Add(advance.Name).OnSelect((s, a) => Steal(advance));
			}

			AddMenu(menu);
		}

		private static int DialogHeight(int choices)
		{
			return ((choices + 1) * Resources.GetFontHeight(FONT_ID)) + 10;
		}

		internal SelectAdvanceAfterCityCapture(Player player, IList<IAdvance> advances) : base(100, 80, 135, DialogHeight(advances.Count))
		{
			_player = player ?? throw new ArgumentNullException(nameof(player));
			_advances = advances ?? throw new ArgumentNullException(nameof(advances));

			DialogBox.DrawText($"Select one...", 0, 15, 5, 5);
		}
	}
}