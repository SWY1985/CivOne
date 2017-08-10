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
	internal class French : BaseCivilization<Napoleon>
	{
		public French() : base(10, 3, "French", "French", "napo")
		{
			StartX = 33;
			StartY = 16;
			CityNames = new string[]
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