// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.GFX
{
	public struct Color
	{
		public byte R, G, B, A;

		public static Color Transparent
		{
			get
			{
				return new Color(0, 0, 0, 0);
			}
		}

		public static Color Black
		{
			get
			{
				return new Color(0, 0, 0);
			}
		}

		public static bool operator ==(Color a, Color b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Color a, Color b)
		{
			return !a.Equals(b);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Color)) return false;
			Color color = (Color)obj;
			return GetHashCode() == color.GetHashCode();
		}

		public override int GetHashCode()
		{
			return ((int)A << 24) + ((int)B << 16) + ((int)G << 8) + ((int)R);
		}
		
		public Color(byte a, byte r, byte g, byte b)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}
		
		public Color(byte a, int r, int g, int b)
		{
			R = (byte)r;
			G = (byte)g;
			B = (byte)b;
			A = (byte)a;
		}
		
		public Color(byte r, byte g, byte b)
		{
			R = r;
			G = g;
			B = b;
			A = 255;
		}
		
		public Color(int r, int g, int b)
		{
			R = (byte)r;
			G = (byte)g;
			B = (byte)b;
			A = 255;
		}
	}
}