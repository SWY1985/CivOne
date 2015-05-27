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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using CivOne.Interfaces;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Tiles;

namespace CivOne
{
	internal class Map
	{
		public const int WIDTH = 80;
		public const int HEIGHT = 50;
		
		private readonly int _terrainMasterWord;
		private int _landMass, _temperature, _climate, _age;
		private ITile[,] _tiles;
		
		public bool Ready { get; private set; }
		
		public ITile[,] GetMapPart(int x, int y, int width, int height)
		{
			ITile[,] area = new ITile[width, height];
			
			for (int xx = x; xx < x + width; xx++)
			for (int yy = y; yy < y + height; yy++)
			{
				if (yy < 0 || yy >= HEIGHT)
				{
					area[xx - x, yy - y] = null;
					continue;
				}
				
				int mx = xx;
				while (mx < 0) mx += 80;
				while (mx >= WIDTH) mx -= WIDTH;
				
				area[xx - x, yy - y] = _tiles[mx, yy];
			}
			
			return area;
		}
		
		public ITile GetTile(int x, int y)
		{
			while (x < 0) x += WIDTH;
			while (x >= WIDTH) x-= WIDTH;
			if (y < 0) return null;
			if (y >= HEIGHT) return null; 
			return _tiles[x, y];
		}
		
		private bool NearOcean(int x, int y)
		{
			var map = GetMapPart(x - 1, y - 1, 3, 3);
			return ((map[1, 0] != null && map[1, 0].Type == Terrain.Ocean) ||
					(map[2, 1] != null && map[2, 1].Type == Terrain.Ocean) ||
					(map[1, 2] != null && map[1, 2].Type == Terrain.Ocean) ||
					(map[0, 1] != null && map[0, 1].Type == Terrain.Ocean));
		}
		
		private int ModGrid(int x, int y)
		{
			return (x % 4) * 4 + (y % 4);
		}
		
		private bool TileIsSpecial(int x, int y)
		{
			if (y < 2 || y > (HEIGHT - 3)) return false;
			return ModGrid(x, y) == ((x / 4) * 13 + (y / 4) * 11 + _terrainMasterWord) % 16;
		}
		
		private bool TileHasHut(int x, int y)
		{
			if (y < 2 || y > (HEIGHT - 3)) return false;
			return ModGrid(x, y) == ((x / 4) * 13 + (y / 4) * 11 + _terrainMasterWord + 8) % 32;
		}
		
		private bool[,] GenerateLandChunk()
		{
			bool[,] stencil = new bool[WIDTH, HEIGHT];
			
			int x = Common.Random.Next(4, WIDTH - 4);
			int y = Common.Random.Next(8, HEIGHT - 8);
			int pathLength = Common.Random.Next(1, 64);
			
			for (int i = 0; i < pathLength; i++)
			{
				stencil[x, y] = true;
				stencil[x + 1, y] = true;
				stencil[x, y + 1] = true;
				switch (Common.Random.Next(0, 4))
				{
					case 0: y--; break;
					case 1: x++; break;
					case 2: y++; break;
					default: x--; break;
				}

				if (x < 3 || y < 3 || x > (WIDTH - 4) || y > (HEIGHT - 5)) break;
			}

			return stencil;
		}
		
		private int[,] GenerateLandMass()
		{
			Console.WriteLine("Map: Stage 1 - Generate land mass");
			
			int[,] elevation = new int[WIDTH, HEIGHT];
			int landMassSize = (int)((WIDTH * HEIGHT) / 12.5) * (_landMass + 2);
			
			// Generate the landmass
			while ((from int tile in elevation where tile > 0 select 1).Sum() < landMassSize)
			{
				bool[,] chunk = GenerateLandChunk();
				for (int y = 0; y < HEIGHT; y++)
				for (int x = 0; x < WIDTH; x++)
				{
					if (chunk[x, y]) elevation[x, y]++;
				}
			}
			
			// remove narrow passages
			for (int y = 0; y < (HEIGHT - 1); y++)
			for (int x = 0; x < (WIDTH - 1); x++)
			{
				if ((elevation[x, y] > 0 && elevation[x + 1, y + 1] > 0) && (elevation[x + 1, y] == 0 && elevation[x, y + 1] == 0))
				{
					elevation[x + 1, y]++;
					elevation[x, y + 1]++;
				}
				else if ((elevation[x, y] == 0 && elevation[x + 1, y + 1] == 0) && (elevation[x + 1, y] > 0 && elevation[x, y + 1] > 0))
				{
					elevation[x + 1, y + 1]++;
				}
			}
			
			return elevation;
		}
		
		private int[,] TemperatureAdjustments()
		{
			Console.WriteLine("Map: Stage 2 - Temperature adjustments");
			
			int[,] latitude = new int[WIDTH, HEIGHT];
			
			for (int y = 0; y < HEIGHT; y++)
			for (int x = 0; x < WIDTH; x++)
			{
				int l = (int)(((float)y / HEIGHT) * 50) - 29;
				l += Common.Random.Next(0, 7);
				if (l < 0) l = -l;
				l += 1 - _temperature;
				
				l = (l / 6) + 1;
				
				switch (l)
				{
					case 0:
					case 1: latitude[x, y] = 0; break;
					case 2:
					case 3: latitude[x, y] = 1; break;
					case 4:
					case 5: latitude[x, y] = 2; break;
					case 6:
					default: latitude[x, y] = 3; break;
				}
			}
			
			return latitude;
		}
		
		private void MergeElevationAndLatitude(int[,] elevation, int[,] latitude)
		{
			Console.WriteLine("Map: Stage 3 - Merge elevation and latitude into the map");
			
			// merge elevation and latitude into the map
			for (int y = 0; y < HEIGHT; y++)
			for (int x = 0; x < WIDTH; x++)
			{
				bool special = TileIsSpecial(x, y);
				switch (elevation[x, y])
				{
					case 0: _tiles[x, y] = new Ocean(x, y, special); break;
					case 1:
						{
							switch (latitude[x, y])
							{
								case 0: _tiles[x, y] = new Desert(x, y, special); break;
								case 1: _tiles[x, y] = new Plains(x, y, special); break;
								case 2: _tiles[x, y] = new Tundra(x, y, special); break;
								case 3: _tiles[x, y] = new Arctic(x, y, special); break;
							}
						}
						break;
					case 2: _tiles[x, y] = new Hills(x, y, special); break;
					default: _tiles[x, y] = new Mountains(x, y, special); break;
				}
			}
		}
		
		private void ClimateAdjustments()
		{
			Console.WriteLine("Map: Stage 4 - Climate adjustments");
			
			int wetness, latitude;
			
			for (int y = 0; y < HEIGHT; y++)
			{
				int yy = (int)(((float)y / HEIGHT) * 50);
				
				wetness = 0;
				latitude = Math.Abs(25 - yy);
				
				for (int x = 0; x < WIDTH; x++)
				{
					if (_tiles[x, y].Type == Terrain.Ocean)
					{
						// wetness yield
						int wy = latitude - 12;
						if (wy < 0) wy = -wy;
						wy += (_climate * 4);
						
						if (wy > wetness) wetness++;
					}
					else if (wetness > 0)
					{
						bool special = TileIsSpecial(x, y);
						int rainfall = Common.Random.Next(0, 7 - (_climate * 2));
						wetness -= rainfall;
						
						switch (_tiles[x, y].Type)
						{
							case Terrain.Plains: _tiles[x, y] = new Grassland(x, y); break;
							case Terrain.Tundra: _tiles[x, y] = new Arctic(x, y, special); break;
							case Terrain.Hills: _tiles[x, y] = new Forest(x, y, special); break;
							case Terrain.Desert: _tiles[x, y] = new Plains(x, y, special); break;
							case Terrain.Mountains: wetness -= 3; break;
						}
					}
				}
				
				wetness = 0;
				latitude = Math.Abs(25 - yy);
				
				// reset row wetness to 0
				for (int x = WIDTH - 1; x >= 0; x--)
				{
					if (_tiles[x, y].Type == Terrain.Ocean)
					{
						// wetness yield
						int wy = (latitude / 2) + _climate;
						if (wy > wetness) wetness++;
					}
					else if (wetness > 0)
					{
						bool special = TileIsSpecial(x, y);
						int rainfall = Common.Random.Next(0, 7 - (_climate * 2));
						wetness -= rainfall;
						
						switch (_tiles[x, y].Type)
						{
							case Terrain.Swamp: _tiles[x, y] = new Forest(x, y, special); break;
							case Terrain.Plains: new Grassland(x, y); break;
							case Terrain.Grassland1:
							case Terrain.Grassland2: _tiles[x, y] = new Jungle(x, y, special); break;
							case Terrain.Hills: _tiles[x, y] = new Forest(x, y, special); break;
							case Terrain.Mountains: _tiles[x, y] = new Forest(x, y, special); wetness -= 3; break;
							case Terrain.Desert: _tiles[x, y] = new Plains(x, y, special); break;
						}
					}
				}
			}
		}
		
		private void AgeAdjustments()
		{
			Console.WriteLine("Map: Stage 5 - Age adjustments");
			
			int x = 0;
			int y = 0;
			int ageRepeat = (int)(((float)800 * (1 + _age) / (80 * 50)) * (WIDTH * HEIGHT));
			for (int i = 0; i < ageRepeat; i++)
			{
				if (i % 2 == 0)
				{
					x = Common.Random.Next(0, WIDTH);
					y = Common.Random.Next(0, HEIGHT);
				}
				else
				{
					switch (Common.Random.Next(0, 8))
					{
						case 0: { x--; y--; break; }
						case 1: { y--; break; }
						case 2: { x++; y--; break; }
						case 3: { x--; break; }
						case 4: { x++; break; }
						case 5: { x--; y++; break; }
						case 6: { y++; break; }
						default: { x++; y++; break; }
					}
					if (x < 0) x = 1;
					if (y < 0) y = 1;
					if (x >= WIDTH) x = WIDTH - 2;
					if (y >= HEIGHT) y = HEIGHT - 2;
				}
				
				bool special = TileIsSpecial(x, y);
				switch (_tiles[x, y].Type)
				{
					case Terrain.Forest: _tiles[x, y] = new Jungle(x, y, special); break;
					case Terrain.Swamp: _tiles[x, y] = new Grassland(x, y); break;
					case Terrain.Plains: _tiles[x, y] = new Hills(x, y, special); break;
					case Terrain.Tundra: _tiles[x, y] = new Hills(x, y, special); break;
					case Terrain.River: _tiles[x, y] = new Forest(x, y, special); break;
					case Terrain.Grassland1:
					case Terrain.Grassland2: _tiles[x, y] = new Forest(x, y, special); break;
					case Terrain.Jungle: _tiles[x, y] = new Swamp(x, y, special); break;
					case Terrain.Hills: _tiles[x, y] = new Mountains(x, y, special); break;
					case Terrain.Mountains:
						if ((x == 0 || _tiles[x - 1, y - 1].Type != Terrain.Ocean) &&
						    (y == 0 || _tiles[x + 1, y - 1].Type != Terrain.Ocean) &&
							(x == (WIDTH - 1) || _tiles[x + 1, y + 1].Type != Terrain.Ocean) &&
							(y == (HEIGHT - 1) || _tiles[x - 1, y + 1].Type != Terrain.Ocean))
						_tiles[x, y] = new Ocean(x, y, special);
						break;
					case Terrain.Desert: _tiles[x, y] = new Plains(x, y, special); break;
					case Terrain.Arctic: _tiles[x, y] = new Mountains(x, y, special); break;
				}
			}
		}
		
		private void CreateRivers()
		{
			Console.WriteLine("Map: Stage 6 - Create rivers");
			
			int rivers = 0;
			for (int i = 0; i < 256 && rivers < ((_climate + _landMass) * 2) + 6; i++)
			{
				ITile[,] tilesBackup = (ITile[,])_tiles.Clone();
				
				int riverLength = 0;
				int varA = Common.Random.Next(0, 4) * 2;
				bool nearOcean = false;
				
				ITile tile = null;
				while (tile == null)
				{
					int x = Common.Random.Next(0, WIDTH);
					int y = Common.Random.Next(0, HEIGHT);
					if (_tiles[x, y].Type == Terrain.Hills) tile = _tiles[x, y];
				}
				do
				{
					_tiles[tile.X, tile.Y] = new River(tile.X, tile.Y);
					int varB = varA;
					int varC = Common.Random.Next(0, 2);
					varA = (((varC - riverLength % 2) * 2 + varA) & 0x07);
					varB = 7 - varB;
					
					riverLength++;
					
					nearOcean = NearOcean(tile.X, tile.Y);
					switch (varA)
					{
						case 0:
						case 1: tile = _tiles[tile.X, tile.Y - 1]; break;
						case 2:
						case 3: tile = _tiles[tile.X + 1, tile.Y]; break;
						case 4:
						case 5: tile = _tiles[tile.X, tile.Y + 1]; break;
						case 6:
						case 7: tile = _tiles[tile.X - 1, tile.Y]; break;
					}
				}
				while (!nearOcean && (tile.GetType() != typeof(Ocean) && tile.GetType() != typeof(River) && tile.GetType() != typeof(Mountains)));
				
				if ((nearOcean || tile.Type == Terrain.River) && riverLength > 5)
				{
					rivers++;
					ITile[,] mapPart = GetMapPart(tile.X - 3, tile.Y - 3, 7, 7);
					for (int x = 0; x < 7; x++)
					for (int y = 0; y < 7; y++)
					{
						if (mapPart[x, y] == null) continue;
						int xx = mapPart[x, y].X, yy = mapPart[x, y].Y;
						if (_tiles[xx, yy].Type == Terrain.Forest)
							_tiles[xx, yy] = new Jungle(xx, yy, TileIsSpecial(x, y));
					}
				}
				else
				{
					_tiles = (ITile[,])tilesBackup.Clone(); ;
				}
			}
		}
		
		private void CreatePoles()
		{
			Console.WriteLine("Map: Creating poles");
			
			for (int x = 0; x < WIDTH; x++)
			foreach (int y in new int[] { 0, (HEIGHT - 1) })
			{
				_tiles[x, y] = new Arctic(x, y, false);
			}
			
			for (int i = 0; i < (WIDTH / 4); i++)
			foreach (int y in new int[] { 0, 1, (HEIGHT - 2), (HEIGHT - 1) })
			{
				int x = Common.Random.Next(0, WIDTH);
				_tiles[x, y] = new Tundra(x, y, false);
			}
		}
		
		private void PlaceHuts()
		{
			Console.WriteLine("Map: Placing goody huts (not yet implemented)");
		}
		
		private void SaveBitmap()
		{
			Picture bmp = new Picture(WIDTH * 16, HEIGHT * 16, Resources.Instance.LoadPIC("SP257").Image.Palette.Entries);
			
			for (int x = 0; x < WIDTH; x++)
			for (int y = 0; y < HEIGHT; y++)
			{
				bmp.AddLayer(Resources.Instance.GetTile(_tiles[x, y]), x * 16, y * 16);
			}
			
			bmp.Image.Save("map.png", ImageFormat.Png);
			Console.WriteLine("DEBUG: Map saved as bitmap");
		}
		
		private void GenerateThread()
		{
			Console.WriteLine("Generating map (Land Mass: {0}, Temperature: {1}, Climate: {2}, Age: {3})", _landMass, _temperature, _climate, _age);
			
			_tiles = new ITile[WIDTH, HEIGHT];
			
			int[,] elevation = GenerateLandMass();
			int[,] latitude = TemperatureAdjustments();
			MergeElevationAndLatitude(elevation, latitude);
			ClimateAdjustments();
			AgeAdjustments();
			CreateRivers();
			
			CreatePoles();
			PlaceHuts();
			
			Ready = true;
			Console.WriteLine("Map: Ready");
			//SaveBitmap();
		}
		
		private void LoadMap(byte[,] bitmap)
		{
			_tiles = new ITile[WIDTH, HEIGHT];
			
			for (int x = 0; x < WIDTH; x++)
			for (int y = 0; y < HEIGHT; y++)
			{
				ITile tile;
				bool special = TileIsSpecial(x, y);
				switch (bitmap[x, y])
				{
					case 2: tile = new Forest(x, y, special); break;
					case 3: tile = new Swamp(x, y, special); break;
					case 6: tile = new Plains(x, y, special); break;
					case 7: tile = new Tundra(x, y, special); break;
					case 9: tile = new River(x, y); break;
					case 10: tile = new Grassland(x, y); break;
					case 11: tile = new Jungle(x, y, special); break;
					case 12: tile = new Hills(x, y, special); break;
					case 13: tile = new Mountains(x, y, special); break;
					case 14: tile = new Desert(x, y, special); break;
					case 15: tile = new Arctic(x, y, special); break;
					default: tile = new Ocean(x, y, special); break;
				}
				_tiles[x, y] = tile;
			}
		}
		
		private void LoadMapThread()
		{
			Console.WriteLine("Map: Loading MAP.PIC");
			
			byte[,] bitmap = Resources.Instance.LoadPIC("MAP", true).GetBitmap;
			
			LoadMap(bitmap);
			
			CreatePoles();
			PlaceHuts();
			
			Ready = true;
			Console.WriteLine("Map: Ready");
			//SaveBitmap();
		}
		
		public void LoadSaveGame(string filename)
		{
			Console.WriteLine("Map: Loading {0}", filename);
			
			byte[,] bitmap = Resources.Instance.LoadPIC(filename, true).GetBitmap;
			_tiles = new ITile[WIDTH, HEIGHT];
			
			LoadMap(bitmap);
			
			Ready = true;
			Console.WriteLine("Map: Ready");
		}
		
		public void Generate(int landMass = 1, int temperature = 1, int climate = 1, int age = 1)
		{
			if (Ready || _tiles != null)
			{
				Console.WriteLine("ERROR: Map is already load{0}/generat{0}", (Ready ? "ed" : "ing"));
				return;
			}
			
			_landMass = landMass;
			_temperature = temperature;
			_climate = climate;
			_age = age;
			
			new Thread(new ThreadStart(GenerateThread)).Start();
		}
		
		public void LoadMap()
		{
			if (Ready || _tiles != null)
			{
				Console.WriteLine("ERROR: Map is already load{0}/generat{0}", (Ready ? "ed" : "ing"));
				return;
			}
			
			_landMass = -1;
			_temperature = -1;
			_climate = -1;
			_age = -1;
			
			new Thread(new ThreadStart(LoadMapThread)).Start();
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
			_terrainMasterWord = Common.Random.Next(0, 16);
			Ready = false;
			
			Console.WriteLine("Map instance created");
		}
	}
}