// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CivOne.IO
{
	internal class TextFile
	{
		private readonly string[] TEXT_FILES = new[] { "BLURB0", "BLURB1", "BLURB2", "BLURB3", "BLURB4", "ERROR", "HELP", "KING", "PRODUCE" };
		private readonly Dictionary<string, string[]> _gameTexts = new Dictionary<string,string[]>();

        public string[] LoadArray(string filename)
        {
			filename += ".txt";
			
            Regex rgx = new Regex("[^a-zA-Z0-9 -_]");
            List<string> textLines = new List<string>();
			using (StreamReader sr = new StreamReader(Path.Combine(Settings.Instance.DataDirectory, filename)))
                while (!sr.EndOfStream)
					textLines.Add(rgx.Replace(sr.ReadLine(), "").Trim());
            return textLines.ToArray();
        }

		public string[] GetGameText(string key)
		{
			if (_gameTexts.ContainsKey(key))
				return _gameTexts[key];
			return new string[0];
		}

        private static TextFile _instance;
        public static TextFile Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TextFile();
                return _instance;
            }
        }

        private TextFile()
        {
			//foreach (string file in new[] { "BLURB0", "BLURB1", "BLURB2", "BLURB3", "BLURB4", "ERROR", "HELP", "KING", "PRODUCE" })
			foreach (string file in TEXT_FILES)
			{
				string[] textfile = LoadArray(file);
				List<string> keys = new List<string>();
				List<string> lines = new List<string>();
				for (int i = 0; i < textfile.Length; i++)
				{
					if (!textfile[i].StartsWith("*")) continue;
					if (textfile[i] == "*END") break;
					keys.Clear();
					lines.Clear();
					while (textfile.Length > i && textfile[i].StartsWith("*"))
						keys.Add(textfile[i++].Substring(1));
					while (textfile.Length > i && textfile[i].Length > 0)
						lines.Add(textfile[i++]);

					if (lines.Count == 0) continue;
					foreach (string key in keys)
					{
						if (!_gameTexts.ContainsKey(key))
						{
							_gameTexts.Add(key, lines.ToArray());
						}
					}
				}
			}
        }
	}
}