// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Interfaces;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class SideBar : BaseScreen
	{
		private bool _update = true;
		
		private readonly Picture _miniMap, _demographics, _gameInfo;
		private readonly Bitmap _background;
		
		private void DrawMiniMap()
		{
			_miniMap.FillRectangle(5, 0, 0, 80, 50);
			
			if (GamePlay != null)
			{
				ITile[,] tile = Map[GamePlay.X - 30, GamePlay.Y - 19, 78, 48];
				for (int yy = 0; yy < 48; yy++)
				for (int xx = 0; xx < 78; xx++)
				{
					if (tile[xx, yy] == null) continue;
					if (Settings.Instance.RevealWorld)
					{
						byte colour = 5;
						switch (tile[xx, yy].Type)
						{
							case Terrain.Ocean: colour = 1; break;
							case Terrain.Forest: colour = 2; break;
							case Terrain.Swamp: colour = 3; break;
							case Terrain.Plains: colour = 6; break;
							case Terrain.Tundra: colour = 7; break;
							case Terrain.River: colour = 9; break;
							case Terrain.Grassland1:
							case Terrain.Grassland2: colour = 10; break;
							case Terrain.Jungle: colour = 11; break;
							case Terrain.Hills: colour = 12; break;
							case Terrain.Mountains: colour = 13; break;
							case Terrain.Desert: colour = 14; break;
							case Terrain.Arctic: colour = 15; break;
						}
						_miniMap[xx + 1, yy + 1] = colour;
					}
					else if (Game.Instance.HumanPlayer.Visible(tile[xx, yy].X, tile[xx, yy].Y))
					{
						if (tile[xx, yy].IsOcean) _miniMap[xx + 1, yy + 1] = 1;
						else _miniMap[xx + 1, yy + 1] = 2;
					}
				}
			}
			//_miniMap.AddBorder(15, 15, 29, 19, 18, 11);
			_miniMap.FillRectangle(15, 29, 19, 18, 1);
			_miniMap.FillRectangle(15, 29, 19, 1, 11);
			_miniMap.FillRectangle(15, 30, 29, 17, 1);
			_miniMap.FillRectangle(15, 46, 20, 1, 9);
			_miniMap.AddBorder(15, 8, 0, 0, 80, 50);
		}

		private void DrawDemographics()
		{
			_demographics.FillLayerTile(_background);
			_demographics.AddBorder(15, 8, 0, 0, 80, 39);
			_demographics.FillRectangle(11, 3, 2, 74, 11);
			_demographics.FillRectangle(2, 3, 13, 74, 1);
			if (Game.Instance.HumanPlayer.Population > 0)
			{
				string population = $"{Game.Instance.HumanPlayer.Population:n0}".Replace(".", ",");
				_demographics.DrawText($"{population} #", 0, 5, 2, 15, TextAlign.Left);
			}
			_demographics.DrawText(Game.Instance.GameYear, 0, 5, 2, 23, TextAlign.Left);
			_demographics.DrawText(string.Format("{0}$ 0.5.5", Game.Instance.HumanPlayer.Gold), 0, 5, 2, 31, TextAlign.Left);
		}
		
		private void DrawGameInfo(uint gameTick = 0)
		{
			IUnit unit = Game.Instance.ActiveUnit;
			
			_gameInfo.FillLayerTile(_background);
			_gameInfo.AddBorder(15, 8, 0, 0, 80, 103);
			
			if (unit != null)
			{
				_gameInfo.DrawText(Game.Instance.HumanPlayer.TribeName, 0, 5, 4, 2, TextAlign.Left);
				_gameInfo.DrawText(unit.Name, 0, 5, 4, 10, TextAlign.Left);
				if (unit.PartMoves > 0)
				{
					_gameInfo.DrawText($"Moves: {unit.MovesLeft}.{unit.PartMoves}", 0, 5, 4, 18, TextAlign.Left);
				}
				else
				{
					_gameInfo.DrawText($"Moves: {unit.MovesLeft}", 0, 5, 4, 18, TextAlign.Left);
				}
				_gameInfo.DrawText((unit.Home == null ? "NONE" : unit.Home.Name), 0, 5, 4, 26, TextAlign.Left);
				_gameInfo.DrawText($"({Map[unit.X, unit.Y].Name})", 0, 5, 4, 34, TextAlign.Left);
			}
			else
			{
				if (gameTick % 4 < 2)
					_gameInfo.DrawText($"End of Turn", 0, 5, 4, 26, TextAlign.Left);
				_gameInfo.DrawText($"Press Enter", 0, 5, 4, 42, TextAlign.Left);
				_gameInfo.DrawText($"to continue", 0, 5, 4, 50, TextAlign.Left);
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update || (gameTick % 2 == 0))
			{
				DrawMiniMap();
				DrawDemographics();
				DrawGameInfo(gameTick);
				
				AddLayer(_miniMap, 0, 0);
				AddLayer(_demographics, 0, 50);
				AddLayer(_gameInfo, 0, 89);
				
				_update = false;
				return true;
			}
			return false;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (args.Y > 50 && args.Y < 62)
			{
				System.Console.WriteLine("Sidebar: Palace View");
				Common.AddScreen(new PalaceView());
			}
			return true;
		}

		private GamePlay GamePlay
		{
			get
			{
				IScreen mapScreen = Common.Screens.FirstOrDefault(s => (s is GamePlay));
				if (mapScreen != null)
					return (mapScreen as GamePlay);
				return null;
			}
		}
		
		public SideBar(Color[] palette)
		{
			_background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
			
			_miniMap = new Picture(80, 50, palette);
			_demographics = new Picture(80, 39, palette);
			_gameInfo = new Picture(80, 103, palette);
			
			DrawMiniMap();
			DrawDemographics();
			DrawGameInfo();
			
			_canvas = new Picture(80, 192, palette);
			AddLayer(_miniMap, 0, 0);
			AddLayer(_demographics, 0, 50);
			AddLayer(_gameInfo, 0, 89);
		}
	}
}