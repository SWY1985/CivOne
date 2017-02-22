// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Advances;
using CivOne.Civilizations;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Governments;
using CivOne.Templates;

using Gov = CivOne.Governments;

namespace CivOne.Screens
{
	internal class King : BaseScreen
	{
		private readonly Picture _background, _leader;

		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
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

			string leader = null;
			if (player.Civilization is Roman) leader = "KING10";
			if (player.Civilization is Babylonian) leader = "KING07";
			if (player.Civilization is German) leader = "KING12";
			if (player.Civilization is Egyptian) leader = "KING01";
			if (player.Civilization is American) leader = "KING04";
			if (player.Civilization is Greek) leader = "KING13";
			if (player.Civilization is Indian) leader = "KING02";
			if (player.Civilization is Russian) leader = "KING08";
			if (player.Civilization is Zulu) leader = "KING03";
			if (player.Civilization is French) leader = "KING11";
			if (player.Civilization is Aztec) leader = "KING09";
			if (player.Civilization is Chinese) leader = "KING06";
			if (player.Civilization is English) leader = "KING00";
			if (player.Civilization is Mongol) leader = "KING05";

			if (leader != null)
			{
				_leader = Resources.Instance.LoadPIC(leader);
			}
			else
			{
				_leader = new Picture(320, 200, _background.Palette);
			}

			Picture.ReplaceColours(_background, 0, 5);
			
			_canvas = new Picture(320, 200, _background.Palette);
			for (int i = 64; i < 144; i++)
			{
				_canvas.Palette[i] = _leader.Palette[i];
			}
			
			AddLayer(_background);
			AddLayer(_leader.GetPart(181, 67, 139, 133), 90, 0);
		}
	}
}