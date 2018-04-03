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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CivOne
{
	internal class Profile
	{
		private const string ROOT_ELEMENT = "CivOneProfile";

		private readonly IRuntime _runtime;
		private readonly string _name;
		private readonly string _filename;

		private void Log(string text, params object[] parameters) => _runtime.Log(text, parameters);

		private XmlWriter CreateXmlWriter(Stream stream)
		{
			XmlWriter xw = XmlWriter.Create(stream, new XmlWriterSettings()
			{
				Indent = true,
				IndentChars = "\t",
				NewLineChars = "\n"
			});
			return xw;
		}

		private void CreateProfile()
		{
			if (File.Exists(_filename))
			{
				Log($"Recreating profile {_name}");
				File.Delete(_filename);
			}

			using (FileStream fs = new FileStream(_filename, FileMode.Create, FileAccess.Write))
			using (XmlWriter xw = CreateXmlWriter(fs))
			{
				XDocument xDoc = new XDocument();
				xDoc.Add(new XElement(ROOT_ELEMENT));
				xDoc.Save(xw);
			}
		}

		public string GetSetting(string key)
		{
			if (!File.Exists(_filename)) CreateProfile();

			using (FileStream fs = new FileStream(_filename, FileMode.Open, FileAccess.Read))
			{
				XDocument xDoc = XDocument.Load(fs);
				XElement xRoot;
				if ((xRoot = xDoc.Element(ROOT_ELEMENT)) == null)
				{
					Log($"Profile {_name} error: Root element missing");
					CreateProfile();
					return null;
				}

				return xRoot.Element(key)?.Value;
			}
		}

		public void SetSetting(string key, string value)
		{
			if (!File.Exists(_filename)) CreateProfile();

			XDocument xDoc;
			XElement xRoot, xElement;
			using (FileStream fs = new FileStream(_filename, FileMode.Open, FileAccess.Read))
			{
				xDoc = XDocument.Load(fs);
				if ((xRoot = xDoc.Element(ROOT_ELEMENT)) == null)
				{
					Log($"Profile {_name} error: Root element missing");
					fs.Close();
					CreateProfile();
					SetSetting(key, value);
					return;
				}
			}

			if ((xElement = xRoot.Element(key)) == null)
			{
				xRoot.Add(xElement = new XElement(key));
			}
			xElement.Value = value;

			using (FileStream fs = new FileStream(_filename, FileMode.Open, FileAccess.Write))
			using (XmlWriter xw = CreateXmlWriter(fs))
			{
				xDoc.Save(xw);
			}
		}

		private static Dictionary<string, Profile> _profiles;
		public static Profile Get(Runtime runtime, string name = "default")
		{
			if (_profiles == null) _profiles = new Dictionary<string, Profile>();
			if (!_profiles.ContainsKey(name.ToLower())) _profiles.Add(name.ToLower(), new Profile(runtime, name.ToLower()));
			return _profiles[name.ToLower()];
		}

		private Profile(IRuntime runtime, string name)
		{
			_runtime = runtime;
			_name = name;
			_filename = Path.Combine(runtime.StorageDirectory, $"{name}.profile");
		}
	}
}