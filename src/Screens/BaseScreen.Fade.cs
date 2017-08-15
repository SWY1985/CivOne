// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Graphics;

namespace CivOne.Screens
{
	public abstract partial class BaseScreen
	{
		private const float FADE_STEP_SLOW = 0.0625F;
		private const float FADE_STEP_NORMAL = 0.1F;
		private const float FADE_STEP_FAST = 0.125F;

		private float _fadeStep = 1.0F;
		
		private float FadeStep
		{
			get
			{
				return _fadeStep;
			}
			set
			{
				_fadeStep = value;
				if (_fadeStep < 0.0F) _fadeStep = 0.0F;
				if (_fadeStep > 1.0F) _fadeStep = 1.0F;
			}
		}
		
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
			
			using (Palette palette = Palette.Copy())
			{
				for (int i = 1; i < 256; i++)
					palette[i] = FadeColour(new Colour(0, 0, 0), OriginalColours[i]);
				_canvas.SetPalette(palette);
			}
		}
		
		protected bool HandleScreenFadeIn(Speed speed = Speed.Normal)
		{
			if (FadeStep < 1.0F)
			{
				switch (speed)
				{
					case Speed.Slow: FadeStep += FADE_STEP_SLOW; break;
					case Speed.Normal: FadeStep += FADE_STEP_NORMAL; break;
					case Speed.Fast: FadeStep += FADE_STEP_FAST; break;
				}
				
				FadeColours();
				return true;
			}
			return false;
		}
		
		protected bool HandleScreenFadeOut(Speed speed = Speed.Normal)
		{
			if (FadeStep > 0.0F)
			{
				switch (speed)
				{
					case Speed.Slow: FadeStep -= FADE_STEP_SLOW; break;
					case Speed.Normal: FadeStep -= FADE_STEP_NORMAL; break;
					case Speed.Fast: FadeStep -= FADE_STEP_FAST; break;
				}
				
				FadeColours();
				return true;
			}
			return false;
		}
	}
}