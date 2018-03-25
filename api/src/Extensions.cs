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
	internal static class Extensions
	{
		internal static bool HasAttribute<T>(this object checkObject) where T : Attribute
		{
			if (checkObject == null)
				return false;
			return Attribute.IsDefined(checkObject.GetType(), typeof(T));
		}

		internal static T GetAttribute<T>(this object checkObject) where T : Attribute
		{
			if (!checkObject.HasAttribute<T>()) return null;
			return (T)Attribute.GetCustomAttribute(checkObject.GetType(), typeof(T));
		}
	}
}