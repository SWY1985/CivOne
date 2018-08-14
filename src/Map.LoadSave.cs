// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.IO;
using System.Threading.Tasks;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Graphics.ImageFormats;
using CivOne.IO;
using CivOne.Tiles;

namespace CivOne
{
	public partial class Map
	{
		private void LoadMap(Bytemap bitmap)
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
		
		public void LoadMap(string filename, int randomSeed)
		{
			Log("Map: Loading {0} - Random seed: {1}", filename, randomSeed);
			_terrainMasterWord = randomSeed;
			
			using (Bytemap bitmap = Resources[filename].Bitmap)
			{
				_tiles = new ITile[WIDTH, HEIGHT];
				for (int i = 0; i < _playerExplored.Length; i++)
				{
					_playerExplored[i] = new bool[Map.WIDTH, Map.HEIGHT];
				}
				
				LoadMap(bitmap);
				PlaceHuts();
				CalculateLandValue();
				
				// Load improvement layer
				for (int x = 0; x < WIDTH; x++)
				for (int y = 0; y < HEIGHT; y++)
				{
					byte b = bitmap[x, y + (HEIGHT * 2)];
					// 0x01 = CITY ?
					_tiles[x, y].Irrigation = (b & 0x02) > 0;
					_tiles[x, y].Mine = (b & 0x04) > 0;
					_tiles[x, y].Road = (b & 0x08) > 0;
				}
				
				// Load improvement layer 2
				for (int x = 0; x < WIDTH; x++)
				for (int y = 0; y < HEIGHT; y++)
				{
					byte b = bitmap[x, y + (HEIGHT * 3)];
					_tiles[x, y].RailRoad = (b & 0x01) > 0;
				}
				
				// Remove huts
				for (int x = 0; x < WIDTH; x++)
				for (int y = 0; y < HEIGHT; y++)
				{
					if (!_tiles[x, y].Hut) continue;
					byte b = bitmap[x + (WIDTH * 2), y];
					_tiles[x, y].Hut = (b == 0);
				}
			}
			
			Ready = true;
			Log("Map: Ready");
		}

		public ushort SaveMap(string filename)
		{
			Log($"Map: Saving {filename} - Random seed: {_terrainMasterWord}");

			using (Bytemap bitmap = Resources["SP299"].Bitmap)
			{
				// Save terrainlayer
				for (int x = 0; x < WIDTH; x++)
				for (int y = 0; y < HEIGHT; y++)
				{
					byte b;
					switch (_tiles[x, y].Type)
					{
						case Terrain.Forest: b = 2; break;
						case Terrain.Swamp: b = 3; break;
						case Terrain.Plains: b = 6; break;
						case Terrain.Tundra: b = 7; break;
						case Terrain.River: b = 9; break;
						case Terrain.Grassland1:
						case Terrain.Grassland2: b = 10; break;
						case Terrain.Jungle: b = 11; break;
						case Terrain.Hills: b = 12; break;
						case Terrain.Mountains: b = 13; break;
						case Terrain.Desert: b = 14; break;
						case Terrain.Arctic: b = 15; break;
						default: b = 1; break; // Ocean
					}
					bitmap[x, y] = b;
				}

				// Save improvement layer
				for (int x = 0; x < WIDTH; x++)
				for (int y = 0; y < HEIGHT; y++)
				{
					byte b = 0;
					if (_tiles[x, y].City != null) b |= 0x01;
					if (_tiles[x, y].Irrigation) b |= 0x02;
					if (_tiles[x, y].Mine) b |= 0x04;
					if (_tiles[x, y].Road) b |= 0x08;

					bitmap[x, y + (HEIGHT * 2)] = b;
					bitmap[x + (WIDTH * 1), y + (HEIGHT * 2)] = b; // Visibility layer
				}

				// Save improvement layer 2
				for (int x = 0; x < WIDTH; x++)
				for (int y = 0; y < HEIGHT; y++)
				{
					byte b = 0;
					if (_tiles[x, y].RailRoad) b |= 0x01;

					bitmap[x, y + (HEIGHT * 3)] = b;
					bitmap[x + (WIDTH * 1), y + (HEIGHT * 3)] = b; // Visibility layer
				}

				// Save explored layer
				for (int x = 0; x < WIDTH; x++)
				for (int y = 0; y < HEIGHT; y++)
				{
					bitmap[x + (WIDTH * 2), y] = _tiles[x, y].Visited;
				}

				using (Picture picture = new Picture(bitmap, Resources["SP299"].Palette))
				{
					PicFile picFile = new PicFile(picture)
					{
						HasPalette256 = false
					};
					using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
					{
						bw.Write(picFile.GetBytes());
					}
					return (ushort)_terrainMasterWord;
				}
			}
		}
		
		private void LoadMapThread()
		{
			Log("Map: Loading MAP.PIC");
			
			using (Bytemap bitmap = Resources["MAP"].Bitmap)
			{
				LoadMap(bitmap);
			}
			
			CreatePoles();
			PlaceHuts();
			CalculateLandValue();
			
			Ready = true;
			Log("Map: Ready");
		}
		
		public void LoadMap()
		{
			if (Ready || _tiles != null)
			{
				Log("ERROR: Map is already load{0}/generat{0}", (Ready ? "ed" : "ing"));
				return;
			}
			
			_landMass = -1;
			_temperature = -1;
			_climate = -1;
			_age = -1;
			FixedStartPositions = true;
			
			Task.Run(() => LoadMapThread());
		}
	}
}