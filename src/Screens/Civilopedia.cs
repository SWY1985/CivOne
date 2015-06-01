// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Windows.Forms;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;
using CivOne.Tiles;

namespace CivOne.Screens
{
	internal class Civilopedia : BaseScreen
	{
		internal static ICivilopedia[] Terrain = new ICivilopedia[] { new Arctic(), new Desert(), new Forest(), new Grassland(), new Hills(), new Jungle(), new Mountains(), new Ocean(), new Plains(), new River(), new Swamp(), new Tundra() }; 
		
		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update) return false;
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public override bool MouseDown(MouseEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public Civilopedia(params ICivilopedia[] pages)
		{
			_canvas = new Picture(320, 200, Resources.WorldMapTiles.Image.Palette.Entries);
			
			_canvas.FillRectangle(14, 0, 0, 320, 200);
			_canvas.FillRectangle(15, 60, 2, 200, 9);
			_canvas.FillRectangle(15, 2, 14, 316, 184);
			
			_canvas.DrawText("ENCYCLOPEDIA of CIVILIZATION", 0, 5, 67, 4);
			_canvas.DrawText("ENCYCLOPEDIA of CIVILIZATION", 0, 10, 66, 3);
		}
	}
}