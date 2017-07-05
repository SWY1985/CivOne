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

namespace CivOne.GFX
{
	internal class Free
	{
		private IEnumerable<byte> GenerateNoise(params byte[] values)
		{
			Random r = new Random(0x4701);
			while (true)
			{
				yield return values[r.Next(values.Length)];
			}
		}

		private Picture _panelGrey, _panelBlue;

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