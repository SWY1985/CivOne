// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CivOne.Interfaces;

namespace CivOne
{
	internal static class Reflect
	{
		private static IEnumerable<Assembly> GetAssemblies
		{
			get
			{
				yield return Assembly.GetExecutingAssembly();
			}
		}
		
		private static IEnumerable<T> GetTypes<T>()
		{
			foreach (Assembly asm in GetAssemblies)
			foreach (Type type in asm.GetTypes().Where(t => typeof(T).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract))
			{
				yield return (T)Activator.CreateInstance(type);
			}
		}
		
		internal static IEnumerable<ICivilopedia> GetCivilopediaAll()
		{
			return GetTypes<ICivilopedia>();
		}
		
		internal static IEnumerable<ICivilopedia> GetCivilopediaAdvances()
		{
			return GetTypes<IAdvance>();
		}
		
		internal static IEnumerable<ICivilopedia> GetCivilopediaCityImprovements()
		{
			foreach (ICivilopedia civilopedia in GetTypes<IBuilding>())
				yield return civilopedia;
			foreach (ICivilopedia civilopedia in GetTypes<IWonder>())
				yield return civilopedia;
		}
		
		internal static IEnumerable<ICivilopedia> GetCivilopediaTerrainTypes()
		{
			return GetTypes<ITile>();
		}
	}
}