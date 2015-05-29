// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class SideBar : BaseScreen
	{
		private bool _update = true;
		
		private readonly Picture _miniMap, _demographics, _gameInfo;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;
				return true;
			}
			return false;
		}
		
		public SideBar(Color[] palette)
		{
			Bitmap background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
			
			_miniMap = new Picture(80, 50, palette);
			_miniMap.FillRectangle(5, 0, 0, 80, 50);
			_miniMap.AddBorder(15, 8, 0, 0, 80, 50);
			
			_demographics = new Picture(80, 39, palette);
			_demographics.FillLayerTile(background);
			_demographics.AddBorder(15, 8, 0, 0, 80, 39);
			_demographics.FillRectangle(11, 3, 2, 74, 11);
			_demographics.FillRectangle(2, 3, 13, 74, 1);
			_demographics.DrawText("10,000 #", 0, 5, 2, 15, TextAlign.Left);
			_demographics.DrawText(Game.Instance.GameYear, 0, 5, 2, 23, TextAlign.Left);
			_demographics.DrawText(string.Format("{0}$ 0.5.5", Game.Instance.HumanPlayer.Gold), 0, 5, 2, 31, TextAlign.Left);
			
			_gameInfo = new Picture(80, 103, palette);
			_gameInfo.FillLayerTile(background);
			_gameInfo.AddBorder(15, 8, 0, 0, 80, 103);
			_gameInfo.DrawText(string.Format("({0})", Game.Instance.HumanPlayer.TribeName), 0, 5, 4, 2, TextAlign.Left);
			_gameInfo.DrawText("Settlers", 0, 5, 4, 10, TextAlign.Left);
			_gameInfo.DrawText("Moves: 1", 0, 5, 4, 18, TextAlign.Left);
			_gameInfo.DrawText("NONE", 0, 5, 4, 26, TextAlign.Left);
			_gameInfo.DrawText("(Plains)", 0, 5, 4, 34, TextAlign.Left);
			
			_canvas = new Picture(80, 192, palette);
			AddLayer(_miniMap, 0, 0);
			AddLayer(_demographics, 0, 50);
			AddLayer(_gameInfo, 0, 89);
		}
	}
}