// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens.Dialogs
{
	internal class AdvisorMessage : BaseDialog
	{
		private readonly Bitmap[] _textLines;

		private static Bitmap[] TextBitmaps(string[] message)
		{
			Bitmap[] output = new Bitmap[message.Length];
			for (int i = 0; i < message.Length; i++)
				output[i] = Resources.Instance.GetText(message[i], 0, 15);
			return output;
		}

		private static int GetLeft(Advisor advisor)
		{
			switch (advisor)
			{
				case Advisor.Domestic:
					return 58;
				default:
					return 38;
			}
		}

		public AdvisorMessage(Advisor advisor, string[] message) : base(GetLeft(advisor), 72, TextBitmaps(message).Max(b => b.Width) + 52, 62)
		{
			Cursor = MouseCursor.None;

			string[] advisorNames = new string[] { "Defense Minister", "Domestic Advisor", "Foreign Minister", "Science Advisor" };
			bool modernGovernment = Human.Advances.Any(a => a.Id == (int)Advance.Invention);
			Bitmap governmentPortrait = Icons.GovernmentPortrait(Human.Government, advisor, modernGovernment);
			Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			for (int i = 144; i < 256; i++)
			{
				palette[i] = governmentPortrait.Palette.Entries[i];
			}

			_canvas.SetPalette(palette);
			
			_textLines = TextBitmaps(message);
			DialogBox.AddLayer(governmentPortrait, 2, 2);
			DialogBox.DrawText($"{advisorNames[(int)advisor]}:", 0, 15, 47, 4);
			DialogBox.FillRectangle(11, 47, 11, Resources.Instance.GetText($"{advisorNames[(int)advisor]}:", 0, 15).Width + 1, 1);
			for (int i = 0; i < _textLines.Length; i++)
				DialogBox.AddLayer(_textLines[i], 47, (_textLines[i].Height * i) + 13);
		}
	}
}