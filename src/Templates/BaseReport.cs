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
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseReport : BaseScreen, IModal
	{
		private bool _update = true;

		protected readonly Picture[] Portrait = new Picture[4];
		
		public override bool HasUpdate(uint gameTick)
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
		
		public BaseReport(string title, byte backgroundColour)
		{
			bool modernGovernment = Human.HasAdvance<Invention>();
			for (int i = 0; i < 4; i++)
			{
				Portrait[i] = Icons.GovernmentPortrait(Human.Government, (Advisor)Enum.Parse(typeof(Advisor), $"{i}"), modernGovernment); 
			}
			Color[] palette = Common.DefaultPalette;
			for (int i = 144; i < 256; i++)
			{
				palette[i] = Portrait[0].Palette[i];
			}

			_canvas = new Picture(320, 200, palette);
			
			_canvas.FillRectangle(backgroundColour, 0, 0, 320, 200);
			_canvas.DrawText(title, 0, 15, 160, 2, TextAlign.Center);
			_canvas.DrawText(string.Format("{0} of the {1}", "Empire", Human.TribeNamePlural), 0, 15, 160, 10, TextAlign.Center);
			_canvas.DrawText(string.Format("{0} {1}: {2}", "Emperor", Human.LeaderName, Game.GameYear), 0, 15, 160, 18, TextAlign.Center);
		}
	}
}