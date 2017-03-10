// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Linq;

namespace CivOne.Templates
{
	public abstract partial class BaseScreen
	{
		private readonly List<Screens.Menu> _menus = new List<Screens.Menu>();

		protected bool HasMenu
		{
			get
			{
				return _menus.Any();
			}
		}
		
		protected void AddMenu(Screens.Menu menu)
		{
			_menus.Add(menu);
			Common.AddScreen(menu);
		}

		protected void CloseMenus()
		{
			foreach (Screens.Menu menu in _menus)
			{
				menu.Close();
			}
			_menus.Clear();
		}
	}
}