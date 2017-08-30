// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Enums;

namespace CivOne.Graphics
{
	public class TextSettings
	{
		public event EventHandler Changed;

		private TextAlign _alignment;
		private VerticalAlign _verticalAlignment;
		private int _fontId;
		private byte _colour;

		public TextAlign Alignment
		{
			get
			{
				return _alignment;
			}
			set
			{
				_alignment = value;
				if (Changed != null) Changed(this, EventArgs.Empty);
			}
		}

		public VerticalAlign VerticalAlignment
		{
			get
			{
				return _verticalAlignment;
			}
			set
			{
				_verticalAlignment = value;
				if (Changed != null) Changed(this, EventArgs.Empty);
			}
		}

		public int FontId
		{
			get
			{
				return _fontId;
			}
			set
			{
				_fontId = value;
				if (Changed != null) Changed(this, EventArgs.Empty);
			}
		}

		public byte Colour
		{
			get
			{
				return _colour;
			}
			set
			{
				_colour = value;
				if (Changed != null) Changed(this, EventArgs.Empty);
			}
		}
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

		public static TextSettings UnitText(byte playerNumber) => new TextSettings()
		{
			Colour = (byte)(playerNumber == 1 ? 9 : 15),
			BottomColour = 5,
			Alignment = TextAlign.Center
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