// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.IO;
using System.Reflection;

namespace CivOne
{
	internal static class Resources
	{
		internal static Stream GetInternalResource(Assembly assembly, string assemblyName) => assembly.GetManifestResourceStream(assemblyName);

		internal static byte[] GetInternalResourceBytes(Assembly assembly, string assemblyName)
		{
			using (Stream stream = GetInternalResource(assembly, assemblyName))
			{
				return stream.GetBytes();
			}
		}
	}
}