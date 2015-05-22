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
	internal class GamePlay : BaseScreen
	{
		private readonly GameMenu _gameMenu;
		private readonly SideBar _sideBar;
		
		public override bool HasUpdate(uint gameTick)
		{
			_canvas.AddLayer(_gameMenu.Canvas.Image, 0, 0);
			_canvas.AddLayer(_sideBar.Canvas.Image, 0, 8);
			
			return true;
		}
		
		public GamePlay()
		{
			Cursor = MouseCursor.Pointer;
			
			Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			
			_canvas = new Picture(320, 200, palette);
			_canvas.FillRectangle(1, 0, 0, 320, 200);
			_canvas.DrawText("Gameplay placeholder", 3, 15, 160, 160, TextAlign.Center);
			
			_gameMenu = new GameMenu(palette);
			_sideBar = new SideBar(palette);
		}
	}
}