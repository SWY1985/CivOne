// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Advances;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Governments;

using Gov = CivOne.Governments;

namespace CivOne.Screens
{
	internal class King : BaseScreen
	{
		private readonly Player _player;

		private readonly Picture _background;

		private bool _update = true;
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_update) return false;
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
		
		public King(Player player)
		{
			_player = player;

			bool modern = player.HasAdvance<Invention>();
			int govId = 0;
			if (player.Government is Gov.Monarchy)
				govId = 1;
			else if (player.Government is Republic || player.Government is Gov.Democracy)
				govId = 2;
			else if (player.Government is Gov.Communism)
			{
				govId = 3;
				modern = false;
			}

			_background = Resources.Instance.LoadPIC($"BACK{govId}{(modern ? "M" : "A")}");

			Picture.ReplaceColours(_background, 0, 5);
			Picture portrait = _player.Civilization.Leader.GetPortrait();
			
			using (Palette palette = _background.Palette.Copy())
			{
				palette.MergePalette(portrait.Palette, 64, 80);
				Palette = palette;
			}
			
			this.AddLayer(_background)
				.AddLayer(portrait, 90, 0);
		}
	}
}