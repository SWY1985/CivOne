// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.GFX;

namespace CivOne.Concepts
{
	internal abstract class BaseConcept : IConcept
	{
		private static void Log(string text, params object[] parameters) => RuntimeHandler.Runtime.Log(text, parameters);

		public string Name { get; protected set; }
		public Picture Icon { get { return null; } }
		public byte PageCount { get { return 2; } }
		public Picture DrawPage(byte pageNumber)
		{
			string[] text = new string[0];
			switch (pageNumber)
			{
				case 1:
					text = Resources.Instance.GetCivilopediaText("BLURB4/" + Name.ToUpper());
					break;
				case 2:
					text = Resources.Instance.GetCivilopediaText("BLURB4/" + Name.ToUpper() + "2");
					break;
				default:
					Log("Invalid page number: {0}", pageNumber);
					break;
			}
			
			Picture output = new Picture(320, 200);
			
			int yy = 76;
			foreach (string line in text)
			{
				Log(line);
				output.DrawText(line, 6, 1, 12, yy);
				yy += 9;
			}
			
			return output;
		}
		
		protected BaseConcept()
		{
			
		}
	}
}