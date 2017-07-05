// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Linq;
using CivOne.Enums;

namespace CivOne.GFX
{
	internal class Free
	{
		private Picture _panelGrey, _panelBlue;
		private Picture _landBase, _seaBase;

		private IEnumerable<byte> GenerateNoise(params byte[] values)
		{
			Random r = new Random(0x4701);
			while (true)
			{
				yield return values[r.Next(values.Length)];
			}
		}

		public Picture PanelGrey
		{
			get
			{
				if (_panelGrey == null)
				{
					_panelGrey = new Picture(16, 16, GenerateNoise(7, 22).Take(16 * 16).ToArray(), Common.GetPalette256);
				}
				return _panelGrey;
			}
		}

		public Picture PanelBlue
		{
			get
			{
				if (_panelBlue == null)
				{
					_panelBlue = new Picture(16, 16, GenerateNoise(57, 9).Take(16 * 16).ToArray(), Common.GetPalette256);
				}
				return _panelBlue;
			}
		}

		public Picture LandBase
		{
			get
			{
				if (_landBase == null)
				{
					_landBase = new Picture(16, 16, GenerateNoise(35, 36, 37).Take(16 * 16).ToArray(), Common.GetPalette256);
				}
				return _landBase;
			}
		}

		public Picture SeaBase
		{
			get
			{
				if (_seaBase == null)
				{
					_seaBase = new Picture(16, 16, GenerateNoise(72, 73, 74).Take(16 * 16).ToArray(), Common.GetPalette256);
				}
				return _seaBase;
			}
		}

		public Picture Fog(Direction direction)
		{
			Picture output = new Picture(16, 16);
			switch(direction)
			{
				case Direction.West:
					output.AddLayer(new Picture(3, 16, GenerateNoise(0, 28, 29, 30, 31).Take(3 * 16).ToArray(), Common.GetPalette256), 0, 0);
					break;
				case Direction.South:
					output.AddLayer(new Picture(16, 3, GenerateNoise(28, 0, 29, 30, 31).Take(16 * 3).ToArray(), Common.GetPalette256), 0, 13);
					break;
				case Direction.East:
					output.AddLayer(new Picture(3, 16, GenerateNoise(28, 29, 0, 30, 31).Take(3 * 16).ToArray(), Common.GetPalette256), 13, 0);
					break;
				case Direction.North:
					output.AddLayer(new Picture(16, 3, GenerateNoise(28, 29, 30, 0, 31).Take(16 * 3).ToArray(), Common.GetPalette256), 0, 0);
					break;
			}
			return output;
		}

		private static Free _instance;
		public static Free Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Free();
				return _instance;
			}
		}

		private Free()
		{
		}
	}
}