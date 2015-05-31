// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;
using System.Windows.Forms;

namespace CivOne.Screens
{
	internal class WorldMap : BaseScreen
	{
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
		
		public WorldMap()
		{
			_canvas = new Picture(320, 200, Resources.WorldMapTiles.Image.Palette.Entries);
			
			for (int x = 0; x < Map.WIDTH; x++)
			for (int y = 0; y < Map.HEIGHT; y++)
			{
				ITile tile = Map.Instance.GetTile(x, y);
				Terrain type = tile.Type;
				if (type == Terrain.Grassland2) type = Terrain.Grassland1;
				bool altTile = ((x + y) % 2 == 1);
				int xx = (((int)type) * 4);
				int yy = altTile ? 4 : 0;
				
				AddLayer(Resources.WorldMapTiles.GetPart(xx, yy, 4, 4), x * 4, y * 4);
			}
		}
	}
}