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
using CivOne.Templates;

namespace CivOne.Screens.Reports
{
	internal class MilitaryStatus : BaseReport
	{
		public MilitaryStatus() : base("MILITARY STATUS", 1)
		{
			byte player = Game.PlayerNumber(HumanPlayer);
			IUnit[] units = Game.GetUnits().Where(u => u.Owner == player && u.Home != null).ToArray();
			IUnit[] production = Game.GetCities().Where(c => c.Owner == player).Where(c => (c.CurrentProduction is IUnit)).Select(c => (c.CurrentProduction as IUnit)).ToArray();

			int i = 0;
			foreach (IUnit unit in Reflect.GetUnits())
			{
				if (!units.Any(u => u.Type == unit.Type) && !production.Any(u => u.Type == unit.Type)) continue;

				int active = units.Count(u => u.Type == unit.Type);
				int inProduction = production.Count(u => u.Type == unit.Type);

				Picture icon = unit.GetUnit(player, false);
				AddLayer(icon, ((i % 2 == 0) ? 1 : 18), 27 + (9 * i));
				_canvas.FillRectangle(9, 36, 30 + (i * 9), 284, 1);
				_canvas.DrawText(unit.Name, 0, 15, 36, 32 + (i * 9));
				_canvas.DrawText($"({unit.Attack}/{unit.Defense}/{unit.Move})", 0, 11, 112, 32 + (i * 9));
				if (active > 0)
					_canvas.DrawText($"{active} active", 0, 15, 168, 32 + (i * 9));
				if (inProduction > 0)
					_canvas.DrawText($"{inProduction} in production", 0, 11, 232, 32 + (i * 9));
				
				i++;
			}
			
			AddLayer(Portrait[(int)Advisor.Defense], 278, 2);
		}
	}
}