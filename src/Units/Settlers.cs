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
using CivOne.Screens;
using CivOne.Templates;

namespace CivOne.Units
{
	internal class Settlers : BaseUnit
	{
		public int BuildingRoad { get; private set; }

		public bool BuildRoad()
		{
			ITile tile = Map.Instance[X, Y];
			if (!tile.IsOcean && !tile.Road && Game.Instance.GetCity(X, Y) == null)
			{
				System.Console.WriteLine($"X: {tile.X} - Y: {tile.Y} - {tile.Type}");
				BuildingRoad = 2;
				MovesLeft = 0;
				return true;
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