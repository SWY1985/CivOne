// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Civilizations;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne.Screens
{
	internal class PowerGraph : BaseScreen
	{
		private bool _update = true;

		protected override bool HasUpdate(uint gameTick)
		{
			if (!_update) return false;
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

		public PowerGraph() : base(MouseCursor.None)
		{	
			Palette = Common.TopScreen.Palette.Copy();

			this.Clear(8)
				.DrawText("CIVILIZATION POWERGraph", 0, 5, 100, 3)
				.DrawText("CIVILIZATION POWERGraph", 0, 15, 100, 2)
				.DrawRectangle(4, 9, 312, 184);
			
			for (int i = 0; i < 13; i++)
			{
				int xx = 4 + (i * 25);
				ushort turn = (ushort)(i * 50);
				if (turn > Game.GameTurn) break;
				this.DrawLine(xx, 9, xx, 192);
				if (turn % 100 != 0) continue;
				this.DrawText(Common.YearString(turn).Replace(" ", ""), 1, 15, xx - 4, 194);
			}
			
			Player[] players = Game.Players.Where(x => !(x.Civilization is Barbarian)).ToArray();
			for (int i = 0; i < players.Length; i++)
			{
				this.DrawText(players[i].TribeName, 0, Common.ColourLight[Game.PlayerNumber(players[i])], 8, 12 + (i * 8));
			}
		}
	}
}