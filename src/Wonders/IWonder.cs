// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Advances;
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Wonders
{
	public interface IWonder : ICivilopedia, IProduction
	{
		byte Id { get; }
		IAdvance RequiredTech { get; }
		IAdvance ObsoleteTech { get; }
		Picture SmallIcon { get; }
		string FormatWorldWonder(City city);
	}
}