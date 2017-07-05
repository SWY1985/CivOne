// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Interfaces;

namespace CivOne.GFX
{
	internal static class TileResources
	{
		private static Resources Resources => Resources.Instance;
		
		private static Picture[] _icons = new Picture[12];
		
		private static void DrawOceanBorderNorth(ref Picture output, ITile tile)
		{
			if (tile.GetBorderType(Direction.North) == Terrain.Ocean) return;
			if (tile.GetBorderType(Direction.West) == Terrain.Ocean)
			{
				if (tile.GetBorderType(Direction.NorthWest) == Terrain.Ocean)
					output.AddLayer(Resources["TER257"].GetPart(64, 176, 8, 8), 0, 0);
				else
					output.AddLayer(Resources["TER257"].GetPart(96, 176, 8, 8), 0, 0);
			}
			else
			{
				output.AddLayer(Resources["TER257"].GetPart(80, 176, 8, 8), 0, 0);
			}
			if (tile.GetBorderType(Direction.East) == Terrain.Ocean)
			{
				if (tile.GetBorderType(Direction.NorthEast) == Terrain.Ocean)
					output.AddLayer(Resources["TER257"].GetPart(24, 176, 8, 8), 8, 0);
				else
					output.AddLayer(Resources["TER257"].GetPart(56, 176, 8, 8), 8, 0);
			}
			else
			{
				output.AddLayer(Resources["TER257"].GetPart(88, 176, 8, 8), 8, 0);
			}
		}
		
		private static void DrawOceanBorderEast(ref Picture output, ITile tile)
		{
			if (tile.GetBorderType(Direction.East) == Terrain.Ocean) return;
			if (tile.GetBorderType(Direction.North) == Terrain.Ocean)
			{
				if (tile.GetBorderType(Direction.NorthEast) == Terrain.Ocean)
					output.AddLayer(Resources["TER257"].GetPart(72, 176, 8, 8), 8, 0);
				else
					output.AddLayer(Resources["TER257"].GetPart(104, 176, 8, 8), 8, 0);
			}
			else
			{
				output.AddLayer(Resources["TER257"].GetPart(88, 176, 8, 8), 8, 0);
			}
			if (tile.GetBorderType(Direction.South) == Terrain.Ocean)
			{
				if (tile.GetBorderType(Direction.SouthEast) == Terrain.Ocean)
					output.AddLayer(Resources["TER257"].GetPart(24, 184, 8, 8), 8, 8);
				else
					output.AddLayer(Resources["TER257"].GetPart(56, 184, 8, 8), 8, 8);
			}
			else
			{
				output.AddLayer(Resources["TER257"].GetPart(88, 184, 8, 8), 8, 8);
			}
		}
		
		private static void DrawOceanBorderSouth(ref Picture output, ITile tile)
		{
			if (tile.GetBorderType(Direction.South) == Terrain.Ocean) return;
			if (tile.GetBorderType(Direction.West) == Terrain.Ocean)
			{
				if (tile.GetBorderType(Direction.SouthWest) == Terrain.Ocean)
					output.AddLayer(Resources["TER257"].GetPart(16, 184, 8, 8), 0, 8);
				else
					output.AddLayer(Resources["TER257"].GetPart(48, 184, 8, 8), 0, 8);
			}
			else
			{
				output.AddLayer(Resources["TER257"].GetPart(80, 184, 8, 8), 0, 8);
			}
			if (tile.GetBorderType(Direction.East) == Terrain.Ocean)
			{
				if (tile.GetBorderType(Direction.SouthEast) == Terrain.Ocean)
					output.AddLayer(Resources["TER257"].GetPart(72, 184, 8, 8), 8, 8);
				else
					output.AddLayer(Resources["TER257"].GetPart(104, 184, 8, 8), 8, 8);
			}
			else
			{
				output.AddLayer(Resources["TER257"].GetPart(88, 184, 8, 8), 8, 8);
			}
		}
		
		private static void DrawOceanBorderWest(ref Picture output, ITile tile)
		{
			if (tile.GetBorderType(Direction.West) == Terrain.Ocean) return;
			if (tile.GetBorderType(Direction.North) == Terrain.Ocean)
			{
				if (tile.GetBorderType(Direction.NorthWest) == Terrain.Ocean)
					output.AddLayer(Resources["TER257"].GetPart(16, 176, 8, 8), 0, 0);
				else
					output.AddLayer(Resources["TER257"].GetPart(48, 176, 8, 8), 0, 0);
			}
			else
			{
				output.AddLayer(Resources["TER257"].GetPart(80, 176, 8, 8), 0, 0);
			}
			if (tile.GetBorderType(Direction.South) == Terrain.Ocean)
			{
				if (tile.GetBorderType(Direction.SouthWest) == Terrain.Ocean)
					output.AddLayer(Resources["TER257"].GetPart(64, 184, 8, 8), 0, 8);
				else
					output.AddLayer(Resources["TER257"].GetPart(96, 184, 8, 8), 0, 8);
			}
			else
			{
				output.AddLayer(Resources["TER257"].GetPart(80, 184, 8, 8), 0, 8);
			}
		}
		
		private static bool DrawOceanCorners(ref Picture output, ITile tile)
		{
			int borders = 0;
			if (tile.GetBorderType(Direction.North) != Terrain.Ocean) borders += 1;
			if (tile.GetBorderType(Direction.East) != Terrain.Ocean) borders += 2;
			if (tile.GetBorderType(Direction.South) != Terrain.Ocean) borders += 4;
			if (tile.GetBorderType(Direction.West) != Terrain.Ocean) borders += 8;
			
			if (borders == 6) // South East
			{
				if (tile.GetBorderType(Direction.NorthEast) != Terrain.Ocean) return false;
				if (tile.GetBorderType(Direction.SouthWest) != Terrain.Ocean) return false;
				output.AddLayer(Resources["SP299"].GetPart(224, 100, 16, 16), 0, 0);
			}
			if (borders == 9) // North West
			{
				if (tile.GetBorderType(Direction.NorthEast) != Terrain.Ocean) return false;
				if (tile.GetBorderType(Direction.SouthWest) != Terrain.Ocean) return false;
				output.AddLayer(Resources["SP299"].GetPart(240, 100, 16, 16), 0, 0);
			}
			else if (borders == 3) // North East
			{
				if (tile.GetBorderType(Direction.NorthWest) != Terrain.Ocean) return false;
				if (tile.GetBorderType(Direction.SouthEast) != Terrain.Ocean) return false;
				output.AddLayer(Resources["SP299"].GetPart(256, 100, 16, 16), 0, 0);
			}
			else if (borders == 12) // South West
			{
				if (tile.GetBorderType(Direction.NorthWest) != Terrain.Ocean) return false;
				if (tile.GetBorderType(Direction.SouthEast) != Terrain.Ocean) return false;
				output.AddLayer(Resources["SP299"].GetPart(272, 100, 16, 16), 0, 0);
			}
			else return false;
			return true;
		}
		
		private static void DrawDiagonalCoast(ref Picture output, ITile tile)
		{
			bool north = (tile.GetBorderType(Direction.North) == Terrain.Ocean);
			bool east = (tile.GetBorderType(Direction.East) == Terrain.Ocean);
			bool south = (tile.GetBorderType(Direction.South) == Terrain.Ocean);
			bool west = (tile.GetBorderType(Direction.West) == Terrain.Ocean);
			
			if (north && west && (tile.GetBorderType(Direction.NorthWest) != Terrain.Ocean)) output.AddLayer(Resources["TER257"].GetPart(32, 176, 8, 8), 0, 0);
			if (north && east && (tile.GetBorderType(Direction.NorthEast) != Terrain.Ocean)) output.AddLayer(Resources["TER257"].GetPart(40, 176, 8, 8), 8, 0);
			if (south && west && (tile.GetBorderType(Direction.SouthWest) != Terrain.Ocean)) output.AddLayer(Resources["TER257"].GetPart(32, 184, 8, 8), 0, 8);
			if (south && east && (tile.GetBorderType(Direction.SouthEast) != Terrain.Ocean)) output.AddLayer(Resources["TER257"].GetPart(40, 184, 8, 8), 8, 8);
		}
		
		private static void DrawIrrigation(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.Irrigation) return;
			if (Game.Instance != null && tile.City == null)
			
			output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"].GetPart(64, 32, 16, 16), 0, 0);
		}
		
		private static void DrawMine(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.Mine) return;
			
			output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"].GetPart(80, 32, 16, 16), 0, 0);
		}
		
		private static void DrawFortress(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.Fortress) return;
			if (Game.Instance != null && tile.City == null)
			
			output.AddLayer(Icons.Fortress, 0, 0);
		}
		
		private static void DrawRoad(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.Road || tile.RailRoad) return;
						
			bool connected = false;
			ITile borderTile = null;
			Direction[] directions = new [] { Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest };
			for (int i = 0; i < directions.Length; i++)
			{
				if ((borderTile = tile.GetBorderTile(directions[i])) == null) continue;
				if (!(borderTile.Road || borderTile.RailRoad) || (tile.City != null && borderTile.City != null)) continue;
				output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"].GetPart((i * 16), 48, 16, 16), 0, 0);
				connected = true;
			}
			if (connected) return;
			output.FillRectangle(6, 7, 7, 2, 2);
		}
		
		private static void DrawRailRoad(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.RailRoad && tile.City == null) return;
			
			bool connected = false;
			ITile borderTile = null;
			Direction[] directions = new [] { Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest };
			for (int i = 0; i < directions.Length; i++)
			{
				if ((borderTile = tile.GetBorderTile(directions[i])) == null) continue;
				// if (!borderTile.Road || borderTile.RailRoad || (tile.City != null && borderTile.City != null)) continue;
				if (!borderTile.Road || borderTile.RailRoad || borderTile.City != null) continue;
				output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"].GetPart((i * 16), 48, 16, 16), 0, 0);
				connected = true;
			}
			for (int i = 0; i < directions.Length; i++)
			{
				if ((borderTile = tile.GetBorderTile(directions[i])) == null) continue;
				if (!borderTile.RailRoad && tile.City == null && borderTile.City == null) continue;
				if (tile.City != null && borderTile.City == null && !borderTile.RailRoad) continue;
				output.AddLayer(Resources[graphics16 ? "SPRITES" : "SP257"].GetPart(128 + (i * 16), 96, 16, 16), 0, 0);
				connected = true;
			}
			if (connected) return;
			output.FillRectangle(5, 7, 7, 2, 2);
		}
		
		private static void DrawHut(ref Picture output, ITile tile, bool graphics16 = false)
		{
			if (!tile.Hut) return;
			
			Picture resource = Resources[graphics16 ? "SPRITES" : "SP257"].GetPart(240, 112, 16, 16);
			Picture.ReplaceColours(resource, 3, 0);
			output.AddLayer(resource, 0, 0);
		}
		
		internal static Picture GetTile16(ITile tile, bool improvements, bool roads)
		{
			Picture output = new Picture(16, 16);
			
			bool altTile = ((tile.X + tile.Y) % 2 == 1);
			
			// Set tile base
			if (tile.Type != Terrain.Ocean)
			{
				output.AddLayer(Resources["SPRITES"].GetPart(0, 80, 16, 16));
			}
			
			// Add tile terrain
			switch (tile.Type)
			{
				case Terrain.Ocean:
				case Terrain.River:
					bool ocean = (tile.Type == Terrain.Ocean);
					output.AddLayer(Resources["SPRITES"].GetPart(tile.Borders * 16, (ocean ? 64 : 80), 16, 16));
					break;
				default:
					int terrainId = (int)tile.Type;
					if (tile.Type == Terrain.Grassland1) altTile = false;
					else if (tile.Type == Terrain.Grassland2) { altTile = true; terrainId = (int)Terrain.Grassland1; }
					output.AddLayer(Resources["SPRITES"].GetPart(terrainId * 16, (altTile ? 0 : 16), 16, 16));
					break;
			}
			
			// Add special resources
			if (tile.Special)
			{
				int terrainId = (int)tile.Type;
				Picture resource = Resources["SPRITES"].GetPart(terrainId * 16, 112, 16, 16);
				Picture.ReplaceColours(resource, 3, 0);
				output.AddLayer(resource);
			}
			
			// Add tile improvements
			if (improvements && tile.Type != Terrain.River)
			{
				DrawIrrigation(ref output, tile, true);
				DrawMine(ref output, tile, true);
			}
			if (roads)
			{
				DrawRoad(ref output, tile, true);
				DrawRailRoad(ref output, tile, true);
			}
			DrawFortress(ref output, tile, true);
			DrawHut(ref output, tile, true);
			
			return output;
		}
		
		internal static Picture GetTile256(ITile tile, bool improvements, bool roads)
		{
			Picture output = new Picture(16, 16);

			if (!Resources.Exists("SP257"))
			{
				switch (tile.Type)
				{
					case Terrain.Ocean:
						output.AddLayer(Free.Instance.SeaBase);
						break;
					default:
						output.AddLayer(Free.Instance.LandBase);
						break;
				}
				return output;
			}
			
			// Set tile base
			switch (tile.Type)
			{
				case Terrain.Ocean: output.AddLayer(Resources["TER257"].GetPart(0, 160, 16, 16)); break;
				default: output.AddLayer(Resources["SP257"].GetPart(0, 64, 16, 16)); break;
			}
			
			// Add tile terrain
			switch (tile.Type)
			{
				case Terrain.Ocean:
					if (!DrawOceanCorners(ref output, tile))
					{
						DrawOceanBorderNorth(ref output, tile);
						DrawOceanBorderEast(ref output, tile);
						DrawOceanBorderSouth(ref output, tile);
						DrawOceanBorderWest(ref output, tile);
					}
					
					if (tile.GetBorderType(Direction.North) == Terrain.River) output.AddLayer(Resources["TER257"].GetPart(128, 176, 16, 16));
					if (tile.GetBorderType(Direction.East) == Terrain.River) output.AddLayer(Resources["TER257"].GetPart(144, 176, 16, 16));
					if (tile.GetBorderType(Direction.South) == Terrain.River) output.AddLayer(Resources["TER257"].GetPart(160, 176, 16, 16));
					if (tile.GetBorderType(Direction.West) == Terrain.River) output.AddLayer(Resources["TER257"].GetPart(176, 176, 16, 16));
					
					DrawDiagonalCoast(ref output, tile);
					break;
				case Terrain.River:
					if (improvements)
					{
						DrawIrrigation(ref output, tile);
						DrawMine(ref output, tile);
					}
					output.AddLayer(Resources["SP257"].GetPart(tile.Borders * 16, 80, 16, 16));
					break;
				default:
					int terrainId = (int)tile.Type;
					if (tile.Type == Terrain.Grassland2) { terrainId = (int)Terrain.Grassland1; }
					if (improvements) DrawIrrigation(ref output, tile);
					output.AddLayer(Resources["TER257"].GetPart(tile.Borders * 16, terrainId * 16, 16, 16));
					break;
			}
			
			// Add special resources
			if (!Map.TileIsType(tile, Terrain.Grassland1, Terrain.Grassland2) && tile.Special)
			{
				int terrainId = (int)tile.Type;
				Picture resource = Resources["SP257"].GetPart(terrainId * 16, 112, 16, 16);
				Picture.ReplaceColours(resource, 3, 0);
				output.AddLayer(resource);
			}
			else if (tile.Type == Terrain.Grassland2)
			{
				Picture resource = Resources["SP257"].GetPart(152, 40, 8, 8);
				Picture.ReplaceColours(resource, 3, 0);
				output.AddLayer(resource, 4, 4);
			}
			
			// Add tile improvements
			if (tile.Type != Terrain.River && improvements)
			{
				DrawMine(ref output, tile);
			}
			if (roads)
			{
				DrawRoad(ref output, tile);
				DrawRailRoad(ref output, tile, true);
			}
			DrawFortress(ref output, tile, true);
			DrawHut(ref output, tile);
			
			return output;
		}
		
		internal static Picture GetIcon(Terrain terrain)
		{
			int terrainId = (int)terrain;
			if (terrainId == 12) terrainId = 2;
			if (_icons[terrainId] != null)
				return _icons[terrainId];
			
			Picture icon;
			switch (terrain)
			{
				case Terrain.Arctic:
					icon = Resources.LoadPIC("ICONPGT1", true).GetPart(108, 1, 108, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					_icons[terrainId].FillRectangle(0, 106, 0, 2, 86);
					break;
				case Terrain.Desert:
					icon = Resources.LoadPIC("ICONPGT2", true).GetPart(1, 1, 108, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					_icons[terrainId].FillRectangle(0, 106, 0, 2, 86);
					break;
				case Terrain.Forest:
					icon = Resources.LoadPIC("ICONPGT2", true).GetPart(215, 1, 104, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					break;
				case Terrain.Grassland1:
				case Terrain.Grassland2:
					icon = Resources.LoadPIC("ICONPGT2", true).GetPart(108, 1, 108, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					_icons[terrainId].FillRectangle(0, 106, 0, 2, 86);
					break;
				case Terrain.Hills:
					icon = Resources.LoadPIC("ICONPGT2", true).GetPart(108, 88, 108, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					_icons[terrainId].FillRectangle(0, 106, 0, 2, 86);
					break;
				case Terrain.Jungle:
					icon = Resources.LoadPIC("ICONPGT1", true).GetPart(1, 88, 108, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					_icons[terrainId].FillRectangle(0, 106, 0, 2, 86);
					break;
				case Terrain.Mountains:
					icon = Resources.LoadPIC("ICONPGT2", true).GetPart(215, 88, 104, 86);
					Picture.ReplaceColours(icon, 253, 0);
					_icons[terrainId] = new Picture(icon);
					break;
				case Terrain.Ocean:
					icon = Resources.LoadPIC("ICONPGT1", true).GetPart(108, 88, 108, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					_icons[terrainId].FillRectangle(0, 106, 0, 2, 86);
					break;
				case Terrain.Plains:
					icon = Resources.LoadPIC("ICONPGT2", true).GetPart(1, 88, 108, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					_icons[terrainId].FillRectangle(0, 106, 0, 2, 86);
					break;
				case Terrain.River:
					icon = Resources.LoadPIC("ICONPGT1", true).GetPart(215, 88, 104, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					break;
				case Terrain.Swamp:
					icon = Resources.LoadPIC("ICONPGT1", true).GetPart(215, 1, 104, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					break;
				case Terrain.Tundra:
					icon = Resources.LoadPIC("ICONPGT1", true).GetPart(1, 1, 108, 86);
					Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
					_icons[terrainId] = new Picture(icon);
					_icons[terrainId].FillRectangle(0, 106, 0, 2, 86);
					break;
			}
			return _icons[terrainId];
		}
	}
}