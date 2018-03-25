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
using CivOne.Screens;

namespace CivOne.UserInterface
{
	public class MenuCollection
	{
		public IEnumerable<Menu> All => Common.Screens.Where(x => (x is Menu)).Select(x => (Menu)x);

		public IEnumerable<Menu> this[string id] => All.Where(x => x.Id == id);

		private static MenuCollection _instance;
		public static MenuCollection Instance
		{
			get
			{
				if (_instance == null)
					_instance = new MenuCollection();
				return _instance;
			}
		}

		private MenuCollection()
		{
		}
	}
}