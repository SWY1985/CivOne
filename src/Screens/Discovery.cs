// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Discovery : BaseScreen
	{
		private const float FADE_STEP = 0.025f;

		private readonly IAdvance _advance;
		private readonly bool _modern;
		
		private float _fadeStep = 0.0f;
		
		private Color FadeColour(Color colour1, Color colour2)
		{
			int r = (int)(((float)colour1.R * (1.0F - _fadeStep)) + ((float)colour2.R * _fadeStep));
			int g = (int)(((float)colour1.G * (1.0F - _fadeStep)) + ((float)colour2.G * _fadeStep));
			int b = (int)(((float)colour1.B * (1.0F - _fadeStep)) + ((float)colour2.B * _fadeStep));
			return Color.FromArgb(r, g, b);
		}
		
		private void FadeColours()
		{
			if (Settings.GraphicsMode != GraphicsMode.Graphics256) return;
			
			Color[] palette = _canvas.Palette;
			for (int i = 86; i < 256; i++)
				palette[i] = FadeColour(_canvas.OriginalColours[i], _advance.Icon.OriginalColours[i]);
			_canvas.SetPalette(palette);
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_fadeStep < 1.0F)
			{
				_fadeStep += FADE_STEP;
				if (_fadeStep >= 1.0F)
				{
					_fadeStep = 1.0F;
				}
				FadeColours();
			}
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (_fadeStep >= 1.0F)
				Destroy();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (_fadeStep >= 1.0F)
				Destroy();
			return true;
		}
		
		public Discovery(IAdvance advance)
		{
			_advance = advance;
			_modern = Human.HasAdvance<Electricity>() && advance.Not<Electricity>();
			string scientistName = Human.HasAdvance<Invention>() && (advance.Not<Invention>()) ? "scientists" : "wise men";

			Picture background = Resources.Instance.LoadPIC(_modern ? "DISCOVR2" : "DISCOVR1");
			
			_canvas = new Picture(320, 200, background.Palette);
			_canvas.FillRectangle(32, 0, 0, 320, 200);

			AddLayer(background);

			string[] text = new string[]
			{
				$"{Human.TribeName} {scientistName}",
				"discover the secret",
				$"of {advance.Name}!"
			};

			
			for (int i = 0; i < text.Length; i++)
			{
				if (_modern)
				{
					_canvas.DrawText(text[i], 0, 3, 101, 30 + (8 * i));
				}
				else
				{
					_canvas.DrawText(text[i], 5, 32, 101, 7 + (15 * i));
					_canvas.DrawText(text[i], 5, 15, 101, 6 + (15 * i));
				}
			}

			AddLayer(advance.Icon, 119, _modern ? 53 : 61);

			Runtime.PlaySound(Human.Civilization.Tune);
		}
	}
}