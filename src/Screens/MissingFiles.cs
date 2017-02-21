// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	public class MissingFiles : BaseScreen, IModal
	{
		private readonly string[] _text = new string[]
		{
			"One or more data files are missing from the",
			"data folder. Please copy the original",
			"Civilization for DOS files to the following",
			"path:"
		};
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update) return false;
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public MissingFiles()
		{
			_canvas = new Picture(320, 200, Common.GetPalette256);
			_canvas.FillRectangle(8, 0, 0, 320, 200);
			_canvas.FillRectangle(15, 40, 50, 240, 100);

			_canvas.DrawText("Warning!", 1, 4, 160, 54, TextAlign.Center);

			for (int i = 0; i < _text.Length; i++)
			{
				_canvas.DrawText(_text[i], 1, 5, 44, 66 + (i * 9), TextAlign.Left);
			}
			
			_canvas.DrawText(Settings.DataDirectory, -1, 1, 160, 75 + (9 * _text.Length), TextAlign.Center);
			
			_canvas.DrawText("Press any key to ignore this warning...", 1, 5, 44, 93 + (9 * _text.Length), TextAlign.Left);
		}
	}
}