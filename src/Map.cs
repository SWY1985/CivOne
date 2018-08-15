// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Linq;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Tiles;

namespace CivOne
{
	public partial class Map
	{
		private static Resources Resources = Resources.Instance;
		private static void Log(string text, params object[] parameters) => RuntimeHandler.Runtime.Log(text, parameters);

		private static int _width = 80, _height = 50;
		public static int WIDTH => _width;
		public static int HEIGHT => _height;
		
		private int _terrainMasterWord;
		private int _landMass, _temperature, _climate, _age;
		private ITile[,] _tiles;
		
		public bool Ready { get; private set; }
		public bool FixedStartPositions { get; private set; }

		public IEnumerable<ITile> QueryMapPart(int x, int y, int width, int height)
		{
			ITile[,] area = this[x, y, width, height];
			for (int yy = 0; yy < height; yy++)
			for (int xx = 0; xx < width; xx++)
			{
				yield return area[xx, yy];
			}
		}
		
		public IEnumerable<ITile> AllTiles()
		{
			for (int y = 0; y < HEIGHT; y++)
			for (int x = 0; x < WIDTH; x++)
			{
				yield return this[x, y];
			}
		}
		
		private bool NearOcean(int x, int y)
		{
			for (int relY = -1; relY <= 1; relY++)
			for (int relX = -1; relX <= 1; relX++)
			{
				if (Math.Abs(relX) == Math.Abs(relY)) continue;
				if (_tiles[x + relX, y + relY] is Ocean) return true;
			}
			return false;
		}
		
		internal static bool TileIsType(ITile tile, params Terrain[] terrain) => terrain.Any(x => tile.Type == x);

		public void ChangeTileType(int x, int y, Terrain type)
		{
			bool special = TileIsSpecial(x, y);
			bool road = _tiles[x, y].Road;
			bool railRoad = _tiles[x, y].RailRoad;
			switch(type)
			{
				case Terrain.Forest: _tiles[x, y] = new Forest(x, y, special); break;
				case Terrain.Swamp: _tiles[x, y] = new Swamp(x, y, special); break;
				case Terrain.Plains: _tiles[x, y] = new Plains(x, y, special); break;
				case Terrain.Tundra: _tiles[x, y] = new Tundra(x, y, special); break;
				case Terrain.River: _tiles[x, y] = new River(x, y); break;
				case Terrain.Grassland1:
				case Terrain.Grassland2: _tiles[x, y] = new Grassland(x, y); break;
				case Terrain.Jungle: _tiles[x, y] = new Jungle(x, y, special); break;
				case Terrain.Hills: _tiles[x, y] = new Hills(x, y, special); break;
				case Terrain.Mountains: _tiles[x, y] = new Mountains(x, y, special); break;
				case Terrain.Desert: _tiles[x, y] = new Desert(x, y, special); break;
				case Terrain.Arctic: _tiles[x, y] = new Arctic(x, y, special); break;
				case Terrain.Ocean: _tiles[x, y] = new Ocean(x, y, special); break;
			}
			_tiles[x, y].Road = road;
			_tiles[x, y].RailRoad = railRoad;
		}
		
		private int ModGrid(int x, int y) => (x % 4) * 4 + (y % 4);
		
		private bool TileIsSpecial(int x, int y)
		{
			if (y < 2 || y > (HEIGHT - 3)) return false;
			return ModGrid(x, y) == ((x / 4) * 13 + (y / 4) * 11 + _terrainMasterWord) % 16;
		}
		
		public IEnumerable<ITile> ContinentTiles(int continentId) => AllTiles().Where(t => t.ContinentId == continentId);
		
		public IEnumerable<City> ContentCities(int continentId) => ContinentTiles(continentId).Where(x => x.City != null).Select(x => x.City).ToArray();
		
		public ITile this[int x, int y]
		{
			get
			{
				if (y < 0 || y >= HEIGHT) return null;
				
				while (x < 0) x += WIDTH;
				x = (x % WIDTH);
				
				return _tiles[x, y];
			}
			private set
			{
				while (x < 0) x += WIDTH;
				while (y < 0) y += HEIGHT;
				x = (x % WIDTH);
				y = (y % HEIGHT);
				
				_tiles[x, y] = value;
			}
		}
		
		public ITile[,] this[int x, int y, int width, int height]
		{
			get
			{
				if (width < 0)
				{
					width = Math.Abs(width);
					x -= width;
				}
				if (height < 0)
				{
					height = Math.Abs(height);
					y -= height;
				}

				ITile[,] output = new ITile[width, height];
				
				for (int yy = y; yy < y + height; yy++)
				for (int xx = x; xx < x + width; xx++)
				{
					output[xx - x, yy - y] = this[xx, yy];
				}
				
				return output;
			}
		}
		
		private static Map _instance;
		public static Map Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Map();
				return _instance;
			}
		}
		
		private Map()
		{
			_terrainMasterWord = Common.Random.Next(16);
			Ready = false;
			
			Log("Map instance created");
		}

		public float GetMoveCost( int x, int y )			// Used by AStar
		{
			float fCost = 3;		// plain etc...

			if ( x < 0 || x >= WIDTH ) return float.PositiveInfinity; ;
			if ( y < 0 || y >= HEIGHT ) return float.PositiveInfinity;
			
			bool road = _tiles[ x, y ].Road;
			bool railRoad = _tiles[ x, y ].RailRoad;
			if( road || railRoad ) return 1f ;

			switch( _tiles[ x, y ].Type )
			{
				case Terrain.Forest: fCost = 6; break;
				case Terrain.Swamp: fCost = 6; break;
				case Terrain.Jungle: fCost = 6; break;
				case Terrain.Hills: fCost = 6; break;
				case Terrain.Mountains: fCost = 9; break;
				case Terrain.Arctic: fCost = 6; break;
				case Terrain.Ocean: fCost = float.PositiveInfinity; break;
			}

			return fCost;

		}
	}
}