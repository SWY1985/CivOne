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
		private static int _seed = 0;
		
		private readonly IPlugin _plugin;
		private readonly string _filePath;

		private string DisabledFile => $"{_filePath}.disabled";

		public bool Enabled
		{
			get => !File.Exists(DisabledFile);
			set
			{
				if (value && File.Exists(DisabledFile))
					File.Delete(DisabledFile);
				else if (!value && !File.Exists(DisabledFile))
					File.Open(DisabledFile, FileMode.Create).Close();
				else
					return;

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
		}
	}
}