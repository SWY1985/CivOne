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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CivOne.Interfaces;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.GFX.ImageFormats;
using CivOne.Tiles;

namespace CivOne
{
	public class Map
	{
		public const int WIDTH = 80;
		public const int HEIGHT = 50;
		
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
		
		internal static bool TileIsType(ITile tile, params Terrain[] terrain)
		{
			return terrain.Any(x => tile.Type == x);
		}

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
				switch (Common.Random.Next(4))
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
				l += Common.Random.Next(7);
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
						int rainfall = Common.Random.Next(7 - (_climate * 2));
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
						int rainfall = Common.Random.Next(7 - (_climate * 2));
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
					x = Common.Random.Next(WIDTH);
					y = Common.Random.Next(HEIGHT);
				}
				else
				{
					switch (Common.Random.Next(8))
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
				int varA = Common.Random.Next(4) * 2;
				bool nearOcean = false;
				
				ITile tile = null;
				while (tile == null)
				{
					int x = Common.Random.Next(WIDTH);
					int y = Common.Random.Next(HEIGHT);
					if (_tiles[x, y].Type == Terrain.Hills) tile = _tiles[x, y];
				}
				do
				{
					_tiles[tile.X, tile.Y] = new River(tile.X, tile.Y);
					int varB = varA;
					int varC = Common.Random.Next(2);
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
					ITile[,] mapPart = this[tile.X - 3, tile.Y - 3, 7, 7];
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
		
		private void CalculateContinentSize()
		{
			// TODO: This function needs to be been checked against the original function. It does not yet work as intended.
			Console.WriteLine("Map: Calculate continent and ocean sizes");
			
			// Initial continents
			byte continentId = 0;
			for (int y = 0; y < HEIGHT; y++)
			for (int x = 0; x < WIDTH; x++)
			{
				ITile tile = this[x, y], north = this[x, y - 1], west = this[x - 1, y];
				
				if (north != null && (north.IsOcean == tile.IsOcean) && north.ContinentId > 0)
				{
					tile.ContinentId = north.ContinentId;
				}
				else if (west != null && (west.IsOcean == tile.IsOcean) && west.ContinentId > 0)
				{
					tile.ContinentId = west.ContinentId;
				}
				else
				{
					tile.ContinentId = ++continentId;
				}
				
				if (north == null || west == null) continue;
				if (north.IsOcean != west.IsOcean) continue;
				
				// Merge continents
				if (north.ContinentId != west.ContinentId && north.ContinentId > 0 && west.ContinentId > 0)
				{
					int northCount = AllTiles().Count(t => t.ContinentId == north.ContinentId);
					int westCount = AllTiles().Count(t => t.ContinentId == west.ContinentId);
					if (northCount > westCount)
					{
						foreach (ITile westTile in AllTiles().Where(t => t.ContinentId == west.ContinentId))
						{
							westTile.ContinentId = north.ContinentId;
						}
						continue;
					}
					foreach (ITile northTile in AllTiles().Where(t => t.ContinentId == north.ContinentId))
					{
						northTile.ContinentId = west.ContinentId;
					}
				}
			}
			
			for (int x = 0; x < WIDTH; x++)
			for (int y = 0; y < HEIGHT; y++)
			{
				ITile tile = this[x, y], north = this[x, y - 1], west = this[x - 1, y];
				
				if (north == null || west == null) continue;
				if (north.IsOcean != west.IsOcean) continue;
				
				// Merge continents
				if (north.ContinentId != west.ContinentId && north.ContinentId > 0 && west.ContinentId > 0)
				{
					int northCount = AllTiles().Count(t => t.ContinentId == north.ContinentId);
					int westCount = AllTiles().Count(t => t.ContinentId == west.ContinentId);
					if (northCount > westCount)
					{
						foreach (ITile westTile in AllTiles().Where(t => t.ContinentId == west.ContinentId))
						{
							westTile.ContinentId = north.ContinentId;
						}
						continue;
					}
					foreach (ITile northTile in AllTiles().Where(t => t.ContinentId == north.ContinentId))
					{
						northTile.ContinentId = west.ContinentId;
					}
				}
			}
			
			List<ITile[]> continents = new List<ITile[]>();
			for (int i = 0; i <= 255; i++)
			{
				if (!AllTiles().Any(x => x.ContinentId == i)) continue;
				continents.Add(AllTiles().Where(x => x.ContinentId == i).ToArray());
			}
			for (byte i = 1; i < 15; i++)
			{
				ITile[] continent = continents.OrderByDescending(x => x.Length).FirstOrDefault();
				if (continent == null) break;
				
				continents.Remove(continent);
				foreach (ITile tile in continent)
				{
					tile.ContinentId = i;
				}
			}
			foreach (ITile[] continent in continents)
			foreach (ITile tile in continent)
			{
				tile.ContinentId = 15;
			}
			
			// Enable this line to visualize the result
			//SaveContinentBitmap();
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
				int x = Common.Random.Next(WIDTH);
				_tiles[x, y] = new Tundra(x, y, false);
			}
		}
		
		private void PlaceHuts()
		{
			Console.WriteLine("Map: Placing goody huts");
			
			for (int y = 0; y < HEIGHT; y++)
			for (int x = 0; x < WIDTH; x++)
			{
				if (_tiles[x, y].Type == Terrain.Ocean) continue;
				_tiles[x, y].Hut = TileHasHut(x, y);
			}
		}
		
		public IEnumerable<ITile> ContinentTiles(int continentId)
		{
			return AllTiles().Where(t => t.ContinentId == continentId);
		}
		
		public IEnumerable<City> ContentCities(int continentId)
		{
			return ContinentTiles(continentId).Where(x => x.City != null).Select(x => x.City).ToArray();
		}
		
		private void CalculateLandValue()
		{
			Console.WriteLine("Map: Calculating land value");
			
			// This code is a translation of Darkpanda's forum post here: http://forums.civfanatics.com/showthread.php?t=498532
			// Comments are pasted from the original forum thread to make the code more readable.
			
			// map squares for which the land value is calculated are in the range [2,2] - [77,47]
			for (int y = 2; y < HEIGHT - 2; y++)
			for (int x = 2; x < WIDTH - 2; x++)
			{
				// initial value is 0
				_tiles[x, y].LandValue = 0;
				
				// If the square's terrain type is not Plains, Grassland or River, then its land value is 0
				if (!TileIsType(_tiles[x, y], Terrain.Plains, Terrain.Grassland1, Terrain.Grassland2, Terrain.River)) continue;
				
				// for each 'city square' neighbouring the map square (i.e. each square following the city area pattern,
				// including the map square itself, so totally 21 'neighbours'), compute the following neighbour value (initially 0):
				int landValue = 0;
				for (int yy = -2; yy <= 2; yy++)
				for (int xx = -2; xx <= 2; xx++)
				{
					// Skip the corners of the square to create a city area pattern
					if (Math.Abs(xx) == 2 && Math.Abs(yy) == 2) continue;
					
					// initial value is 0
					int val = 0;
					
					ITile tile = _tiles[x + xx, y + yy];
					if (tile.Special && TileIsType(tile, Terrain.Grassland1, Terrain.Grassland2, Terrain.River))
					{
						// If the neighbour square type is Grassland special or River special, add 2,
						// then add the non-special Grassland or River terrain type score to the neighbour value
						val += 2;
						if (tile.Type == Terrain.River)
							val += (new River()).LandScore;
						else
							val += (new Grassland()).LandScore;
					}
					else
					{
						// Else add neighbour's terrain type score to the neighbour value
						val += tile.LandScore;
					}
					
					// If the neighbour square is in the map square inner circle, i.e. one of the 8 neighbours immediatly
					// surrounding the map square, then multiply the neighbour value by 2
					if (Math.Abs(xx) <= 1 && Math.Abs(yy) <= 1 && (xx != 0 || yy != 0)) val *= 2;
					
					// If the neighbour square is the North square (relative offset 0,-1), then multiply the neighbour value by 2 ;
					// note: I actually think that this is a bug, and that the intention was rather to multiply by 2 if the 'neighbour'
					// was the central map square itself... the actual CIV code for this is to check if the 'neighbour index' is '0';
					// the neighbour index is used to retrieve the neighbour's relative offset coordinates (x,y) from the central square,
					// and the central square itself is actually the last in the list (index 20), the first one (index 0) being
					// the North neighbour; another '7x7 neighbour pattern' table found in CIV code does indeed set the central square
					// at index 0, and this why I believe ths is a programmer's mistake...
					if (xx == 0 && yy == -1) val *= 2;
					
					// Add the neighbour's value to the map square total value and loop to next neighbour
					landValue += val;
				}
				
				// After all neighbours are processed, if the central map square's terrain type is non-special Grassland or River,
				// subtract 16 from total land value
				if (!_tiles[x, y].Special && TileIsType(_tiles[x, y], Terrain.Grassland1, Terrain.River)) landValue -= 16;
				
				landValue -= 120; // Substract 120 (0x78) from the total land value,
				bool negative = (landValue < 0); // and remember its sign
				landValue = Math.Abs(landValue); // Set the land value to the absolute land value (i.e. negate it if it is negative)
				landValue /= 8; // Divide the land value by 8
				if (negative) landValue = 1 - landValue; // If the land value was negative 3 steps before, then negate the land value and add 1
				
				// Adjust the land value to the range [1..15]
				if (landValue < 1) landValue = 1;
				if (landValue > 15) landValue = 15;
				
				landValue /= 2; // Divide the land value by 2
				landValue += 8; // And finally, add 8 to the land value
				_tiles[x, y].LandValue = (byte)landValue;
			}
		}
		
		public void SaveBitmap()
		{
			// string filename = Common.CaptureFilename;
			// if (filename == null) return;
			
			// Picture bmp = new Picture(WIDTH * 16, HEIGHT * 16, Resources.Instance.LoadPIC("SP257").Palette);
			
			// for (int x = 0; x < WIDTH; x++)
			// for (int y = 0; y < HEIGHT; y++)
			// {
			// 	bmp.AddLayer(Resources.Instance.GetTile(_tiles[x, y]), x * 16, y * 16);
			// }
			
			// bmp.Image.Save(filename, ImageFormat.Png);
			// Console.WriteLine("DEBUG: Map saved as bitmap");
			throw new NotImplementedException();
		}
		
		private void SaveContinentBitmap()
		{
			// Picture bmp = new Picture(WIDTH * 16, HEIGHT * 16, Resources.Instance.LoadPIC("SP257").Palette);
			
			// for (int x = 0; x < WIDTH; x++)
			// for (int y = 0; y < HEIGHT; y++)
			// {
			// 	bmp.AddLayer(Resources.Instance.GetTile(_tiles[x, y]), x * 16, y * 16);
			// 	//bmp.FillRectangle(_tiles[x, y].ContinentId, (x * 16) + 4, (y * 16) + 4, 8, 8);
			// 	bmp.DrawText(_tiles[x, y].ContinentId.ToString(), 0, 5, (x * 16 + 8), (y * 16 + 3), TextAlign.Center);
			// 	bmp.DrawText(_tiles[x, y].ContinentId.ToString(), 0, 5, (x * 16 + 7), (y * 16 + 4), TextAlign.Center);
			// 	bmp.DrawText(_tiles[x, y].ContinentId.ToString(), 0, 5, (x * 16 + 9), (y * 16 + 4), TextAlign.Center);
			// 	bmp.DrawText(_tiles[x, y].ContinentId.ToString(), 0, 5, (x * 16 + 8), (y * 16 + 5), TextAlign.Center);
			// 	bmp.DrawText(_tiles[x, y].ContinentId.ToString(), 0, _tiles[x, y].ContinentId, (x * 16 + 8), (y * 16 + 4), TextAlign.Center);
			// }
			
			// bmp.Image.Save("capture/map.png", ImageFormat.Png);
			// Console.WriteLine("DEBUG: Map saved as bitmap");
			throw new NotImplementedException();
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
			
			CalculateContinentSize();
			CreatePoles();
			PlaceHuts();
			CalculateLandValue();
			
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
		
		public void LoadMap(string filename, int randomSeed)
		{
			Console.WriteLine("Map: Loading {0} - Random seed: {1}", filename, randomSeed);
			_terrainMasterWord = randomSeed;
			
			byte[,] bitmap = Resources.Instance.LoadPIC(filename, true).GetBitmap;
			_tiles = new ITile[WIDTH, HEIGHT];
			
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
			
			// Remove huts
			for (int x = 0; x < WIDTH; x++)
			for (int y = 0; y < HEIGHT; y++)
			{
				if (!_tiles[x, y].Hut) continue;
				byte b = bitmap[x + (WIDTH * 2), y];
				_tiles[x, y].Hut = (b == 0);
			}
			
			Ready = true;
			Console.WriteLine("Map: Ready");
		}

		public ushort SaveMap(string filename)
		{
			Console.WriteLine($"Map: Saving {filename} - Random seed: {_terrainMasterWord}");

			byte[,] bitmap = Resources.Instance.LoadPIC("SP299").GetBitmap;

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
				// 0x01 = CITY ?
				if (_tiles[x, y].Irrigation) b |= 0x02;
				if (_tiles[x, y].Mine) b |= 0x04;
				if (_tiles[x, y].Road) b |= 0x08;

				bitmap[x, y + (HEIGHT * 2)] = b;
			}

			// Save explored layer (unfinished)
			for (int x = 0; x < WIDTH; x++)
			for (int y = 0; y < HEIGHT; y++)
			{
				// Right now, we don't record where units have moved so this step is not saved correctly
				// At the moment, only save active unit location
				IUnit unit = Game.Instance.GetUnits(x, y).FirstOrDefault();
				if (unit == null) continue;
				bitmap[x + (WIDTH * 2), y] = (byte)(unit.Owner + 8);
			}

			PicFile picFile = new PicFile(new Picture(bitmap, Resources.Instance.LoadPIC("SP299").Palette))
			{
				HasPalette256 = false
			};
			using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
			{
				bw.Write(picFile.GetBytes());
			}
			return (ushort)_terrainMasterWord;
		}
		
		private void LoadMapThread()
		{
			Console.WriteLine("Map: Loading MAP.PIC");
			
			byte[,] bitmap = Resources.Instance.LoadPIC("MAP", true).GetBitmap;
			
			LoadMap(bitmap);
			
			CreatePoles();
			PlaceHuts();
			CalculateLandValue();
			
			Ready = true;
			Console.WriteLine("Map: Ready");
			//SaveBitmap();
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
			
			Task.Run(() => GenerateThread());
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
			FixedStartPositions = true;
			
			Task.Run(() => LoadMapThread());
		}
		
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
			
			Console.WriteLine("Map instance created");
		}
	}
}