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
		private Picture _landBase, _seaBase, _city, _fortify;

		private IEnumerable<byte> GenerateNoise(params byte[] values)
		{
			Random r = new Random(0x4701);
			while (true)
			{
				yield return values[r.Next(values.Length)];
			}
		}

		private IEnumerable<byte> GenerateUnit()
		{
			for (int yy = 0; yy < 16; yy++)
			for (int xx = 0; xx < 16; xx++)
			{
				if ((xx == 0 || xx == 15 || yy == 0 || yy == 15) || ((xx == 1 || xx == 14) && (yy == 1 || yy == 14)))
				{
					yield return 0;
				}
				else if (xx == 1 || yy == 14)
				{
					yield return 15;
				}
				else if (xx == 14 || yy == 1)
				{
					yield return 2;
				}
				else
				{
					yield return 10;
				}
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
					_landBase = new Picture(16, 16, GenerateNoise(37, 38, 39).Take(16 * 16).ToArray(), Common.GetPalette256);
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
					_seaBase = new Picture(16, 16, GenerateNoise(77, 78, 79).Take(16 * 16).ToArray(), Common.GetPalette256);
				}
				return _seaBase;
			}
		}

		public Picture City
		{
			get
			{
				if (_city == null)
				{
					Random r = new Random(0x4701);
					_city = new Picture(16, 16);
					_city.AddLine(5, 7, 3, 11, 3);
					_city.AddLine(5, 4, 5, 9, 5);
					_city.AddLine(5, 3, 7, 11, 7);
					_city.AddLine(5, 5, 9, 9, 9);
					_city.AddLine(5, 3, 11, 6, 11);

					_city.AddLine(5, 3, 6, 3, 8);
					_city.AddLine(5, 7, 3, 7, 11);
					_city.AddLine(5, 11, 5, 11, 11);
				}
				return _city;
			}
		}

		public Picture Fortify
		{
			get
			{
				if (_fortify == null)
				{
					_fortify = new Picture(16, 16, GenerateNoise(26, 27, 28).Take(16 * 16).ToArray(), Common.GetPalette256);
					_fortify.AddLayer(new Picture(14, 14, GenerateNoise(24, 25, 26).Take(14 * 14).ToArray(), Common.GetPalette256));
					_fortify.FillRectangle(0, 2, 2, 12, 12);
				}
				return _fortify;
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

		public Picture GetUnit(Unit type)
		{
			Picture output = new Picture(16, 16, GenerateUnit().ToArray(), Common.GetPalette256);
			char text = ' ';
			switch (type)
			{
				case Unit.Settlers: text = 'S'; break;
				case Unit.Militia: text = 'm'; break;
				case Unit.Phalanx: text = 'P'; break;
				case Unit.Legion: text = 'L'; break;
				case Unit.Musketeers: text = 'M'; break;
				case Unit.Riflemen: text = 'R'; break;
				case Unit.Cavalry: text = 'c'; break;
				case Unit.Knights: text = 'K'; break;
				case Unit.Catapult: text = 'C'; break;
				case Unit.Cannon: text = 'X'; break;
				case Unit.Chariot: text = 'W'; break;
				case Unit.Armor: text = 'a'; break;
				case Unit.MechInf: text = 'I'; break;
				case Unit.Artillery: text = 'A'; break;
				case Unit.Fighter: text = 'F'; break;
				case Unit.Bomber: text = 'B'; break;
				case Unit.Trireme: text = 'T'; break;
				case Unit.Sail: text = 's'; break;
				case Unit.Frigate: text = 'f'; break;
				case Unit.Ironclad: text = 'i'; break;
				case Unit.Cruiser: text = 'Y'; break;
				case Unit.Battleship: text = 'Z'; break;
				case Unit.Submarine: text = 'U'; break;
				case Unit.Carrier: text = 'G'; break;
				case Unit.Transport: text = 'H'; break;
				case Unit.Nuclear: text = 'N'; break;
				case Unit.Diplomat: text = 'D'; break;
				case Unit.Caravan: text = 't'; break;
			}
			output.DrawText(text.ToString(), 0, 8, 8, 5, TextAlign.Center);
			output.DrawText(text.ToString(), 0, 7, 8, 4, TextAlign.Center);
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