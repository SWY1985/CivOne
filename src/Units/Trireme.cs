// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.IO;
using CivOne.Tasks;
using CivOne.Templates;
using CivOne.Tiles;

namespace CivOne.Units
{
	internal class Trireme : BaseUnitSea, IBoardable
	{
		protected override void MovementDone(ITile previousTile)
		{
			base.MovementDone(previousTile);
			
			if (MovesLeft > 0) return;

			// Check if the Trireme is at open sea
			if (Tile.GetBorderTiles().Any(t => !(t is Ocean))) return;

			// The Trireme unit is surrounded by oceans, there's a 50% chance it will be lost at sea
			if (Common.Random.Next(0, 100) < 50) return;

			Game.Instance.DisbandUnit(this);
			GameTask.Enqueue(Message.Error("-- Civilization Note --", TextFile.Instance.GetGameText("ERROR/TRIREME")));
		}

		public int Cargo
		{
			get
			{
				return 2;
			}
		}

		public Trireme() : base(4, 1, 0, 3)
		{
			Type = Unit.Trireme;
			Name = "Trireme";
			RequiredTech = new MapMaking();
			ObsoleteTech = new Navigation();
			SetIcon('B', 0, 1);
		}
	}
}