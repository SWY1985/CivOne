// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;
using CivOne.Templates;
using CivOne.Tiles;

namespace CivOne.Units
{
	internal class Settlers : BaseUnit
	{
		private bool Busy
		{
			get
			{
				return (BuildingRoad > 0 || BuildingIrrigation > 0);
			}
		}

		public int BuildingRoad { get; private set; }
		public int BuildingIrrigation { get; private set; }

		public bool BuildRoad()
		{
			ITile tile = Map.Instance[X, Y];
			if (!tile.IsOcean && !tile.Road && Game.Instance.GetCity(X, Y) == null)
			{
				BuildingRoad = 2;
				MovesLeft = 0;
				return true;
			}
			return false;
		}

		public bool BuildIrrigation()
		{
			ITile tile = Map.Instance[X, Y];
			if ((tile.GetBorderTiles().Any(t => (t.X == X || t.Y == Y) && (t.IsOcean || t.Irrigation || (t is River)))) || (tile is River))
			{
				if (!tile.IsOcean && !(tile.Irrigation) && ((tile is Desert) || (tile is Grassland) || (tile is Hills) || (tile is Plains) || (tile is River)) && Game.Instance.GetCity(X, Y) == null)
				{
					BuildingIrrigation = 3;
					MovesLeft = 0;
					return true;
				}
			}
			return false;
		}

		public override void NewTurn()
		{
			base.NewTurn();
			if (BuildingRoad > 0)
			{
				BuildingRoad--;
				if (BuildingRoad > 0)
				{
					Map.Instance[X, Y].Road = true;
					MovesLeft = 0;
				}
			}
			else if (BuildingIrrigation > 0)
			{
				BuildingIrrigation--;
				if (BuildingIrrigation > 0)
				{
					MovesLeft = 0;
				}
				else
				{
					Map.Instance[X, Y].Irrigation = true;
				}
			}
		}

		public override Picture GetUnit(byte colour)
		{
			if (BuildingRoad > 0)
			{
				Picture unit = new Picture(base.GetUnit(colour).Image);
				unit.DrawText("R", 0, 5, 8, 9, TextAlign.Center);
				unit.DrawText("R", 0, (byte)(colour == 1 ? 9 : 15), 8, 8, TextAlign.Center);
				return unit; 
			}
			else if (BuildingIrrigation > 0)
			{
				Picture unit = new Picture(base.GetUnit(colour).Image);
				unit.DrawText("I", 0, 5, 8, 9, TextAlign.Center);
				unit.DrawText("I", 0, (byte)(colour == 1 ? 9 : 15), 8, 8, TextAlign.Center);
				return unit; 
			}
			return base.GetUnit(colour);
		}

		public Settlers() : base(4, 0, 1, 1)
		{
			Class = UnitClass.Land;
			Type = Unit.Settlers;
			Name = "Settlers";
			RequiredTech = null;
			ObsoleteTech = null;
			SetIcon('D', 1, 1);
		}
	}
}