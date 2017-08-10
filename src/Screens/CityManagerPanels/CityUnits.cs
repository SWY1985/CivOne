// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Governments;
using CivOne.Graphics;
using CivOne.Interfaces;
using CivOne.Units;

namespace CivOne.Screens.CityManagerPanels
{
	internal class CityUnits : BaseScreen
	{
		private readonly City _city;

		private readonly Picture _background;
		
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
					if (!(units[i] is Diplomat) && !(units[i] is Caravan))
					{
						int shields = 0, food = 0;
						IGovernment government = Game.GetPlayer(_city.Owner).Government;
						if (government is Anarchy || government is Despotism)
						{
							if (i >= _city.Size)
								shields++;
							if (units[i] is Settlers)
								food++;
						}
						else
						{
							shields++;
							if (units[i] is Settlers)
								food += 2;
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

		public CityUnits(City city, Picture background)
		{
			_city = city;
			_background = background;

			_canvas = new Picture(124, 38, background.Palette);
		}
	}
}