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
using CivOne.Leaders;

namespace CivOne.Civilizations
{
	public abstract class BaseCivilization<T> : BaseCivilization, ICivilization where T : ILeader, new()
	{
		public int Id { get; private set; }

		private string _name;
		public string Name
		{
			get => Modifications.LastOrDefault(x => x.Name.HasValue)?.Name.Value.Name ?? _name;
			private set => _name = value;
		}

		private string _namePlural;
		public string NamePlural
		{
			get => Modifications.LastOrDefault(x => x.Name.HasValue)?.Name.Value.Plural ?? _namePlural;
			private set => _namePlural = value;
		}

		public ILeader Leader { get; private set; }
		
		public byte PreferredPlayerNumber { get; private set; }

		public byte StartX { get; protected set; }

		public byte StartY { get; protected set; }

		public string[] CityNames { get; protected set; }

		public string Tune { get; private set; }

		public BaseCivilization(int id, byte preferredPlayerNumber, string name, string namePlural, string tune = null)
		{
			Id = id;
			PreferredPlayerNumber = preferredPlayerNumber;
			Name = name;
			NamePlural = namePlural;
			Leader = new T();
			Tune = tune;
		}
	}

	public abstract class BaseCivilization : BaseInstance
	{
		protected abstract Civilization Civilization { get; }

		private static Dictionary<Civilization, List<CivilizationModification>> _modifications = new Dictionary<Civilization, List<CivilizationModification>>();
		internal static void LoadModifications()
		{
			_modifications.Clear();

			CivilizationModification[] modifications = Reflect.GetModifications<CivilizationModification>().ToArray();
			if (modifications.Length == 0) return;

			Log("Applying civilization modifications");

			foreach (CivilizationModification modification in modifications)
			{
				if (!_modifications.ContainsKey(modification.Civilization))
					_modifications.Add(modification.Civilization, new List<CivilizationModification>());
				_modifications[modification.Civilization].Add(modification);
			}

			Log("Finished applying civilization modifications");
		}
		public IEnumerable<CivilizationModification> Modifications => _modifications.ContainsKey(Civilization) ? _modifications[Civilization].ToArray() : new CivilizationModification[0];
	}
}