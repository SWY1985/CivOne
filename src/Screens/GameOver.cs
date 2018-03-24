// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Graphics;
using CivOne.IO;

namespace CivOne.Screens
{
	internal class GameOver : BaseScreen
	{
		private readonly Picture _background;
		private readonly string[] _textLines;
		private int _currentLine = 0;
		private int _lineTick = 0;
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (gameTick % 10 != 0) return false;
			_lineTick++;
			
			if (_lineTick % 6 != 0) return false;
			
			if (_textLines.Length <= _currentLine)
			{
				Runtime.Quit();
				return true;
			}
			
			this.AddLayer(_background)
				.DrawText(_textLines[_currentLine], 5, 15, 159, 7, TextAlign.Center)
				.DrawText(_textLines[_currentLine], 5, 13, 159, 9, TextAlign.Center)
				.DrawText(_textLines[_currentLine], 5, 14, 159, 8, TextAlign.Center);
			
			_currentLine++;
			return true;
		}
		
		public GameOver()
		{
			_background = Resources["ARCH"];
			Palette = _background.Palette;
			this.AddLayer(_background);

			PlaySound("lose2");
			
			// Load text and replace strings
			_textLines = TextFile.Instance.GetGameText("KING/ARCH");
			for (int i = 0; i < _textLines.Length; i++)
				_textLines[i] = _textLines[i].Replace("$RPLC1", Human.LatestAdvance).Replace("$US", Human.LeaderName.ToUpper()).Replace("^", "");
		}
	}
}