// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Linq;
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Graphics;

namespace CivOne.Screens.Dialogs
{
	internal class AdvisorMessage : BaseDialog
	{
		private readonly Picture[] _textLines;

		private static Picture[] TextBitmaps(string[] message)
		{
			Picture[] output = new Picture[message.Length];
			for (int i = 0; i < message.Length; i++)
				output[i] = Resources.Instance.GetText(message[i], 0, 15);
			return output;
		}

		private static int DialogWidth(string[] message)
		{
			int maxWidth = TextBitmaps(message).Max(b => b.Width) + 52;
			if (maxWidth < 94) maxWidth = 94;
			return maxWidth;
		}

		public AdvisorMessage(Advisor advisor, string[] message, bool leftAlign) : base((leftAlign ? 38 : 58), 72, DialogWidth(message) + 52, 62)
		{
			string[] advisorNames = new string[] { "Defense Minister", "Domestic Advisor", "Foreign Minister", "Science Advisor" };
			bool modernGovernment = Human.HasAdvance<Invention>();
			Picture governmentPortrait = Icons.GovernmentPortrait(Human.Government, advisor, modernGovernment);
			
			Palette palette = Common.DefaultPalette;
			for (int i = 144; i < 256; i++)
			{
				palette[i] = governmentPortrait.Palette[i];
			}
			this.SetPalette(palette);
			
			_textLines = TextBitmaps(message);
			DialogBox.AddLayer(governmentPortrait, 2, 2);
			DialogBox.DrawText($"{advisorNames[(int)advisor]}:", 0, 15, 47, 4);
			DialogBox.FillRectangle(11, 47, 11, Resources.Instance.GetText($"{advisorNames[(int)advisor]}:", 0, 15).Width + 1, 1);
			for (int i = 0; i < _textLines.Length; i++)
				DialogBox.AddLayer(_textLines[i], 47, (_textLines[i].Height * i) + 13);
		}
	}
}