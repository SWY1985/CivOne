// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.Graphics
{
	public class TileSettings
	{
		public bool Improvements { get; private set; }
		public bool Roads { get; private set; }
		public bool Cities { get; private set; }
		public bool CityLabels { get; private set; }
		public bool CitySmallFonts { get; private set; }
		public bool EnemyUnits { get; private set; }
		public bool Units { get; private set; }
		public bool ActiveUnit { get; private set; }

		public static TileSettings Default => new TileSettings()
		{
			Improvements = true,
			Roads = true,
			Cities = true,
			CityLabels = true,
			EnemyUnits = true,
			Units = true,
			ActiveUnit = true
		};

		public static TileSettings BlinkOn => new TileSettings()
		{
			Improvements = true,
			Roads = true,
			Cities = true,
			CityLabels = false,
			Units = true
		};

		public static TileSettings BlinkOff => new TileSettings()
		{
			Improvements = true,
			Roads = true,
			Cities = true,
			CityLabels = false
		};

		public static TileSettings Terrain => new TileSettings()
		{
			Roads = true
		};

		public static TileSettings CityManager => new TileSettings()
		{
			Improvements = true,
			Roads = true,
			Cities = true,
			CitySmallFonts = true,
			EnemyUnits = true
		};

		private TileSettings()
		{
		}
	}
}