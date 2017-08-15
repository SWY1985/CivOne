// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;

namespace CivOne.Graphics
{
	public class TextSettings
	{
		public TextAlign Alignment { get; set; }
		public int FontId { get; set; }
		public byte Colour { get; set; }
		public byte FirstLetterColour { get; private set; }
		public byte TopColour { get; private set; }
		public byte BottomColour { get; private set; }

		public static TextSettings DifferentFirstLetter(byte firstLetterColour, byte colour) => new TextSettings()
		{
			FirstLetterColour = firstLetterColour,
			Colour = colour
		};

		public static TextSettings ShadowText(byte colour, byte shadowColour) => new TextSettings()
		{
			Colour = colour,
			BottomColour = shadowColour
		};

		public static TextSettings ThreeLayers(byte colour, byte topColour, byte bottomColour) => new TextSettings()
		{
			Colour = colour,
			TopColour = topColour,
			BottomColour = bottomColour
		};

		public TextSettings()
		{
			Alignment = TextAlign.Left;
			FontId = 0;
			Colour = 5;
		}
	}
}