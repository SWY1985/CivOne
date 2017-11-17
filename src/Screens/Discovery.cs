// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Advances;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne.Screens
{
	internal class Discovery : BaseScreen
	{
		private const float FADE_STEP = 0.025f;

		private readonly IAdvance _advance;
		private readonly bool _modern;
		
		private float _fadeStep = 0.0f;
		
		private Colour FadeColour(Colour colour1, Colour colour2)
		{
			int r = (int)(((float)colour1.R * (1.0F - _fadeStep)) + ((float)colour2.R * _fadeStep));
			int g = (int)(((float)colour1.G * (1.0F - _fadeStep)) + ((float)colour2.G * _fadeStep));
			int b = (int)(((float)colour1.B * (1.0F - _fadeStep)) + ((float)colour2.B * _fadeStep));
			return new Colour(r, g, b);
		}
		
		private void FadeColours()
		{
			if (Settings.GraphicsMode != GraphicsMode.Graphics256) return;
			
			Palette palette = Palette;
			for (int i = 86; i < 256; i++)
				palette[i] = FadeColour(OriginalColours[i], _advance.OriginalColours[i]);
			this.SetPalette(palette);
		}
		
		protected override bool HasUpdate(uint gameTick)
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
		
		private void KeyDown(object sender, KeyboardEventArgs args)
		{
			args.Handled = true;
			if (_fadeStep >= 1.0F) Destroy();
		}
		
		private void MouseDown(object sender, ScreenEventArgs args)
		{
			if (_fadeStep >= 1.0F)
				Destroy();
			args.Handled = true;
		}
		
		public Discovery(IAdvance advance)
		{
			_advance = advance;
			_modern = Human.HasAdvance<Electricity>() && advance.Not<Electricity>();
			string scientistName = Human.HasAdvance<Invention>() && (advance.Not<Invention>()) ? "scientists" : "wise men";

			Picture background = Resources[$"DISCOVR{(_modern ? 2 : 1)}"];
			
			Palette = background.Palette;
			this.Clear(32)
				.AddLayer(background);

			OnKeyDown += KeyDown;
			OnMouseDown += MouseDown;

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
					this.DrawText(text[i], 0, 3, 101, 30 + (8 * i));
				}
				else
				{
					this.DrawText(text[i], 5, 32, 101, 7 + (15 * i))
						.DrawText(text[i], 5, 15, 101, 6 + (15 * i));
				}
			}

			this.AddLayer(advance.Icon, 119, _modern ? 53 : 61);

			Runtime.PlaySound(Human.Civilization.Tune);
		}
	}
}