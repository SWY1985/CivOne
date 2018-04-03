// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CivOne
{
	internal class Plugin
	{
		private static void Log(string text, params object[] parameters) => RuntimeHandler.Runtime.Log(text, parameters);
		private static Settings Settings => Settings.Instance;
		private static int _seed = 0;
		
		private readonly IPlugin _plugin;
		private readonly string _filePath;
		private readonly string _fileName;

		public bool Enabled
		{
			get => !Settings.DisabledPlugins.Any(x => x == _fileName);
			set
			{
				if (value)
					Settings.DisabledPlugins = Settings.DisabledPlugins.Where(x => x != _fileName).ToArray();
				else
					Settings.DisabledPlugins = Settings.DisabledPlugins.Concat(new [] { _fileName }).Distinct().ToArray();

				Reflect.ApplyPlugins();
			}
		}

		public int Id { get; }
		public Assembly Assembly { get; }
		public string Name => _plugin.Name;
		public string Filename => Path.GetFileName(_filePath);
		public string Author => _plugin.Author;
		public string Version => _plugin.Version;

		public static Plugin Load(string filePath)
		{
			Assembly assembly = Assembly.LoadFile(filePath);
			Type[] types = assembly.GetTypes().Where(x => x.Namespace == "CivOne" && x.Name == "Plugin" && x.GetInterfaces().Contains(typeof(IPlugin))).ToArray();
			if (types.Count() != 1)
			{
				Log($" - Invalid plugin format: {filePath}");
				return null;
			}
			
			IPlugin plugin = (IPlugin)Activator.CreateInstance(types[0]);

			return new Plugin(filePath, plugin, assembly);
		}

		private Plugin(string filePath, IPlugin plugin, Assembly assembly)
		{
			_plugin = plugin;
			Id = _seed++;
			Assembly = assembly;
			_filePath = filePath;
			_fileName = Path.GetFileName(filePath);
		}
	}
}