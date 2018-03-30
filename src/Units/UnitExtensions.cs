// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
// using CivOne.Graphics;
using CivOne.Graphics.Sprites;
using CivOne.IO;

namespace CivOne.Units
{
	public static class UnitExtensions
	{
		private static Player Human => Game.Instance.HumanPlayer;

		public static Bytemap ToBitmap(this IUnit unit, bool showState = true) => ToBitmap(unit, unit.Owner, showState);
		public static Bytemap ToBitmap(this IUnit unit, byte colour, bool showState = true)
		{
			if (showState)
			{
				switch (unit)
				{
					case Settlers settlers:
						if (settlers.BuildingRoad > 0)
						{
							return Unit.Letter(settlers.Type, 'R', colour).Bitmap;
						}
						else if (settlers.BuildingIrrigation > 0)
						{
							return Unit.Letter(settlers.Type, 'I', colour).Bitmap;
						}
						else if (settlers.BuildingMine > 0)
						{
							return Unit.Letter(settlers.Type, 'M', colour).Bitmap;
						}
						else if (settlers.BuildingFortress > 0)
						{
							return Unit.Letter(settlers.Type, 'F', colour).Bitmap;
						}
						break;
				}
				if (unit.Sentry)
				{
					return Unit.Sentry(unit.Type, colour).Bitmap;
				}
				else if (unit.FortifyActive)
				{
					return Unit.Letter(unit.Type, 'F', colour).Bitmap;
				}
				else if (unit.Fortify)
				{
					return Unit.Fortify(unit.Type, colour).Bitmap;
				}
				else if (Human == unit.Owner && unit.Goto != Point.Empty)
				{
					return Unit.Letter(unit.Type, 'G', colour).Bitmap;
				}
			}
			return Unit.Base(unit.Type, colour).Bitmap;
		}
	}
}