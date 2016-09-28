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
	internal abstract class BaseGovernment : BaseConcept, IGovernment
	{
		public byte Id { get; private set; }
		public string NameAdjective { get; private set; }
		public IAdvance RequiredTech { get; private set; }

		internal BaseGovernment(byte id, string name, IAdvance requiredTech = null)
		{
			Id = id;
			Name = name;
			NameAdjective = name;
			RequiredTech = requiredTech;
		}
		
		internal BaseGovernment(byte id, string name, string nameAdjective, IAdvance requiredTech = null)
		{
			Id = id;
			Name = name;
			NameAdjective = nameAdjective;
			RequiredTech = requiredTech;
		}
	}
}