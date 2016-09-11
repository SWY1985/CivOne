// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.GFX;

namespace CivOne.Interfaces
{
	public interface IUnit : ICivilopedia, IProduction
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
		bool Busy { get; }
		bool Sentry { get; set; }
		bool FortifyActive { get; }
		bool Fortify { get; set; }
		bool Moving { get; }
		int MoveFrame { get; }
		int FromX { get; }
		int FromY { get; }
		void MoveUpdate();
		byte Owner { get; set; }
		byte Status { get; set; }
		byte MovesLeft { get; }
		byte PartMoves { get; }
		void NewTurn();
		void SkipTurn();
		void Explore();
		bool MoveTo(int relX, int relY);
		void SetHome(City city);
		Picture GetUnit(byte colour, bool showState = true);
	}
}