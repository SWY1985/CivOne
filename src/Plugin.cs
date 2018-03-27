// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Reflection;

namespace CivOne
{
	internal class Plugin
	{
		private static int _seed = 0;
		
		private readonly IPlugin _plugin;

		public int Id { get; }
		public Assembly Assembly { get; }
		public string Name => _plugin.Name;
		public string Author => _plugin.Author;
		public string Version => _plugin.Version;

		public Plugin(IPlugin plugin, Assembly assembly)
		{
			_plugin = plugin;
			Id = ++_seed;
			Assembly = assembly;
		}
	}
}