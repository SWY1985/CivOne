// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;

namespace CivOne
{
	public abstract class BaseAttribute : Attribute
	{
		private readonly object _value;

		internal T GetValue<T>() => Valid ? (T)_value : default(T);

		public bool Valid { get; }

		internal BaseAttribute(Type type, object value, Func<object, bool> checkValue = null)
		{
			_value = value;
			Valid = (value.GetType() == type) && (checkValue == null || checkValue(value));
		}
	}
}