// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Interfaces;

namespace CivOne.Templates
{
	public abstract class BaseCivilization<T> : ICivilization where T : ILeader, new()
	{
		public int Id { get; private set; }

		public string Name { get; private set; }

		public string NamePlural { get; private set; }

		public ILeader Leader { get; private set; }
		
		public byte PreferredPlayerNumber { get; private set; }

		public byte StartX { get; protected set; }

		public byte StartY { get; protected set; }

		public string[] CityNames { get; protected set; }

		public BaseCivilization(int id, byte preferredPlayerNumber, string name, string namePlural)
		{
			Id = id;
			PreferredPlayerNumber = preferredPlayerNumber;
			Name = name;
			NamePlural = namePlural;
			Leader = new T();
		}
	}
}