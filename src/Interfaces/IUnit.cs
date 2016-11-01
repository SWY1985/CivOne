// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Screens;
using CivOne.Tasks;

namespace CivOne.Interfaces
{
	public interface IUnit : ICivilopedia, IProduction, ITurn
	{
		IAdvance RequiredTech { get; }
		IWonder RequiredWonder { get; }
		IAdvance ObsoleteTech { get; }
		UnitClass Class { get; }
		Unit Type { get; }
		City Home { get; }
		byte Attack { get; }
		byte Defense { get; }
		byte Move { get; }
		int X { get; set; }
		int Y { get; set; }
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
		byte MovesLeft { get; }
		byte PartMoves { get; }
		void SkipTurn();
		void Explore();
		void SetHome();
		void SetHome(City city);
		Picture GetUnit(byte colour, bool showState = true);
		IEnumerable<GameMenu.Item> MenuItems { get; }
	}
}