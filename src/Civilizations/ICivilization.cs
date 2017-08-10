// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Leaders;

namespace CivOne.Civilizations
{
	public interface ICivilization
	{
		int Id { get; }
		string Name { get; }
		string NamePlural { get; }
		ILeader Leader { get; }
		byte PreferredPlayerNumber { get; }
		byte StartX { get; }
		byte StartY { get; }
		string[] CityNames { get; }
		string Tune { get; }
	}
}