// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne
{
	public class AttributeValue<T>
	{
		public bool HasValue { get; }
		public T Value { get; }

		internal static AttributeValue<T> Set(BaseAttribute attribute) => new AttributeValue<T>(attribute);

		private AttributeValue(BaseAttribute attribute)
		{
			if (!(HasValue = (attribute != null && attribute.Valid))) return;
			Value = attribute.GetValue<T>();
		}
	}
}