// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Drawing;
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Screens;
using CivOne.Tasks;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.UserInterface;
using CivOne.Wonders;

namespace CivOne.Units
{
	public interface IUnit : ICivilopedia, IProduction, ITurn
	{
		IAdvance RequiredTech { get; }
		IWonder RequiredWonder { get; }
		IAdvance ObsoleteTech { get; }
		UnitClass Class { get; }
		Unit Type { get; }
		City Home { get; }
		UnitRole Role { get; }
		byte Attack { get; }
		byte Defense { get; }
		byte Move { get; }
		int X { get; set; }
		int Y { get; set; }
		Point Goto { get; set; }
		ITile Tile { get; }
		bool Busy { get; set; }
		bool Veteran { get; set; }
		bool Sentry { get; set; }
		bool FortifyActive { get; }
		bool Fortify { get; set; }
		bool Moving { get; }
		MoveUnit Movement { get; }
		bool MoveTo(int relX, int relY);
		byte Owner { get; set; }
		byte Status { get; set; }
		byte MovesLeft { get; set; }
		byte PartMoves { get; set; }
		void SkipTurn();
		void Explore();
		void SetHome();
		void SetHome(City city);
		Picture GetUnit(byte colour, bool showState = true);
		IEnumerable<MenuItem<int>> MenuItems { get; }
	}
}