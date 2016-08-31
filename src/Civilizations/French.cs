// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Interfaces;

namespace CivOne.Civilizations
{
	internal class French : ICivilization
	{
		public string Name
		{
			get { return "French"; }
		}

		public string NamePlural
		{
			get { return "French"; }
		}

		public string LeaderName
		{
			get { return "Napoleon"; }
		}

		public byte PreferredPlayerNumber
		{
			get { return 3; }
		}
		
		public string[] CityNames
		{
			get
			{
				return new string[]
				{
					"Paris",
					"Orleans",
					"Lyons",
					"Tours",
					"Chartres",
					"Bordeaux",
					"Rouen",
					"Avignon",
					"Marseilles",
					"Grenoble",
					"Dijon",
					"Amiens",
					"Cherbourg",
					"Poitiers",
					"Toulouse",
					"Bayonne"
				};
			}
		}
	}
}