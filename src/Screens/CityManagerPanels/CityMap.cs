// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Graphics.Sprites;
using CivOne.Tiles;

namespace CivOne.Screens.CityManagerPanels
{
	internal class CityMap : BaseScreen
	{
		private readonly City _city;
		
		private bool _update = true;
		
		public event EventHandler MapUpdate;

		private void DrawResources(ITile tile, int x, int y)
		{
			int food = _city.FoodValue(tile);
			int shield = _city.ShieldValue(tile);
			int trade = _city.TradeValue(tile);
			int count = food + shield + trade;

			if (count == 0)
			{
				this.AddLayer(Icons.Unhappy, x + 4, y + 4);
				return;
			}

			int iconsPerLine = 2;
			int iconWidth = 8;
			if (count > 4) iconsPerLine = (int)Math.Ceiling((double)count / 2);
			if (iconsPerLine == 3) iconWidth = 4;
			if (iconsPerLine >= 4) iconWidth = 2;

			for (int i = 0; i < count; i++)
			{
				IBitmap icon;
				if (i >= food + shield) icon = Icons.Trade;
				else if (i >= food) icon = Icons.Shield;
				else icon = Icons.Food; 

				int xx = (x + ((i % iconsPerLine) * iconWidth));
				int yy = (y + (((i - (i % iconsPerLine)) / iconsPerLine) * 8));
				this.AddLayer(icon, xx, yy);
			}
		}
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				this.Tile(Patterns.PanelBlue)
					.DrawRectangle(colour: 1);
				
				ITile[,] tiles = _city.CityRadius;
				this.AddLayer(tiles.ToBitmap(TileSettings.CityManager, Settings.RevealWorld ? null : Game.GetPlayer(_city.Owner)), 1, 1, dispose: true);

				for (int xx = 0; xx < 5; xx++)
				for (int yy = 0; yy < 5; yy++)
				{
					ITile tile = tiles[xx, yy];
					if (tile == null) continue;

					if (_city.OccupiedTile(tile))
					{
						this.FillRectangle((xx * 16) + 1, (yy * 16) + 1, 16, 1, 12)
							.FillRectangle((xx * 16) + 1, (yy * 16) + 2, 1, 14, 12)
							.FillRectangle((xx * 16) + 1, (yy * 16) + 16, 16, 1, 12)
							.FillRectangle((xx * 16) + 16, (yy * 16) + 2, 1, 14, 12);
					}

					if (_city.ResourceTiles.Contains(tile))
						DrawResources(tile, (xx * 16) + 1, (yy * 16) + 1);
				}
				
				_update = false;
			}
			return true;
		}

		public void Update()
		{
			_update = true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (args.X < 1 || args.X > 81 || args.Y < 1 || args.Y > 81) return false;
			int tileX = (int)Math.Floor(((double)args.X - 1) / 16);
			int tileY = (int)Math.Floor(((double)args.Y - 1) / 16);

			if (tileX < 0 || tileY < 0 || tileX > 4 || tileY > 4) return false;

			_city.SetResourceTile(_city.CityRadius[tileX, tileY]);
			_update = true;
			if (MapUpdate != null) MapUpdate(this, null);
			return true;
		}

		public CityMap(City city) : base(82, 82)
		{
			_city = city;
		}
	}
}