// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Units;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityUnits : BaseScreen
	{
		private readonly City _city;

		private readonly Bitmap _background;
		
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas.FillLayerTile(_background);
				_canvas.AddBorder(1, 1, 0, 0, 123, 38);
				_canvas.FillRectangle(0, 123, 0, 1, 38);

				IUnit[] units = _city.Units.Take(14).ToArray();
				for (int i = 0; i < units.Length; i++)
				{
					int xx = 5 + ((i % 7) * 16);
					int yy = 1 + (((i - (i % 7)) / 7) * 16);

					// Diplomat and Caravan units cost nothing.
					if (!(units[i] is Diplomat) || (units[i] is Caravan))
					{
						int shields = 0, food = 0;
						switch (Game.Instance.GetPlayer(_city.Owner).Government)
						{
							case Government.Anarchy:
							case Government.Despotism:
								if (i >= _city.Size)
									shields++;
								if (units[i] is Settlers)
									food++;
								break;
							default:
								shields++;
								if (units[i] is Settlers)
									food += 2;
								break;
						}
						if (food > 0)
						{
							for (int ix = 0; ix < food; ix++)
								AddLayer(Icons.Food, xx + (4 * ix), yy + 12);
						}
						if (shields > 0)
						{
							AddLayer(Icons.Shield, xx + 8, yy + 12);
						}
					}

					AddLayer(units[i].GetUnit(units[0].Owner, showState: false), xx, yy);
				}
				
				_update = false;
			}
			return true;
		}

		public void Close()
		{
			Destroy();
		}

		public CityUnits(City city, Bitmap background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(124, 38, background.Palette.Entries);
		}
	}
}