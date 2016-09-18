// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseStatusScreen : BaseScreen, IModal
	{
		private bool _update = true;

		protected readonly Bitmap[] Portrait = new Bitmap[4];
		
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
		
		public BaseStatusScreen(string title, byte backgroundColour)
		{
			bool modernGovernment = Game.Instance.HumanPlayer.Advances.Any(a => a.Id == (int)Advance.Invention);
			for (int i = 0; i < 4; i++)
			{
				Portrait[i] = Icons.GovernmentPortrait(Game.Instance.HumanPlayer.Government, (Advisor)Enum.Parse(typeof(Advisor), $"{i}"), modernGovernment); 
			}
			Color[] palette = Resources.Instance.LoadPIC("SP299").Image.Palette.Entries;
			for (int i = 144; i < 256; i++)
			{
				palette[i] = Portrait[0].Palette.Entries[i];
			}

			_canvas = new Picture(320, 200, palette);
			
			_canvas.FillRectangle(backgroundColour, 0, 0, 320, 200);
			_canvas.DrawText(title, 0, 15, 160, 2, TextAlign.Center);
			_canvas.DrawText(string.Format("{0} of the {1}", "Empire", Game.Instance.HumanPlayer.TribeNamePlural), 0, 15, 160, 10, TextAlign.Center);
			_canvas.DrawText(string.Format("{0} {1}: {2}", "Emperor", Game.Instance.HumanPlayer.LeaderName, Game.Instance.GameYear), 0, 15, 160, 18, TextAlign.Center);
		}
	}
}