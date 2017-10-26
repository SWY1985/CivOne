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
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Units;

namespace CivOne.Screens
{
	internal class UnitStack : BaseScreen
	{
		private const int XX = 101;
		private const int WIDTH = 121;

		private readonly int _x, _y;
		private readonly IUnit[] _units;

		private bool _update = true;
		
		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				if (!_units.Any())
				{
					// No units, close the dialog
					Destroy();
					return true;
				}

				int height = (16 * _units.Length) + 6;
				int yy = (200 - height) / 2;

				Picture dialog = new Picture(WIDTH, height)
					.FillRectangle(1, 1, WIDTH - 2, height - 2, 3)
					.DrawRectangle3D()
					.As<Picture>();

				for (int i = 0; i < _units.Length; i++)
				{
					IUnit unit = _units[i];
					dialog.AddLayer(unit.ToBitmap(), 4, (i * 16) + 3)
						.DrawText(unit.Name + (unit.Veteran ? " (V)" : ""), 0, 15, 27, (i * 16) + 4)
						.DrawText(unit.Home == null ? "NONE" : unit.Home.Name, 0, 14, 27, (i * 16) + 12);
				}

				this.FillRectangle(XX - 1, yy - 1, WIDTH + 2, height + 2, 5)
					.AddLayer(dialog, XX, yy);
				
				return true;
			}
			return false;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			Destroy();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			if (args.X >= XX && args.X < (XX + WIDTH))
			{
				int height = (16 * _units.Length) + 6;
				int yy = (200 - height) / 2;
				if (args.Y >= yy && args.Y < (yy + height))
				{
					int y = (args.Y - yy - 3);
					int uid = (y - (y % 16)) / 16;
					if (uid < 0 || uid >= _units.Length)
						return true;
					
					Game.GameState.ActiveUnit = _units[uid];
					_units[uid].Busy = false;
					return true;
				}
			}

			Destroy();
			return true;
		}

		internal UnitStack(int x, int y) : base(MouseCursor.Pointer)
		{
			_x = x;
			_y = y;
			_units = Map[_x, _y].Units.Take(12).ToArray();

			Palette = Common.TopScreen.Palette;
		}
	}
}