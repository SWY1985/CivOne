// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;

namespace CivOne.Civilizations
{
	public abstract class CivilizationModification : IModification
	{
		public Civilization Civilization { get; }

		public AttributeValue<CivilizationName> Name => AttributeValue<CivilizationName>.Set(this.GetAttribute<Name>());

		/// <summary>
		/// Modifiy an existing civilization.
		/// </summary>
		/// <param name="civilizationId">The civilization to override.</param>
		public CivilizationModification(Civilization civilizationId)
		{
			Civilization = civilizationId;
		}
	}
}