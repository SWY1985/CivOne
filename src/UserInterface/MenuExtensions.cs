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
using System.Linq;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.Screens;

namespace CivOne.UserInterface
{
	public static class MenuExtensions
	{
		private static Resources Resources => Resources.Instance;

		private static IEnumerable<string> GetMenuItemTexts<T>(this Menu<T> menu)
		{
			if (menu.Title != null) yield return menu.Title;
			foreach (MenuItem<T> item in menu.Items)
				yield return item.Text;
		}

		public static int GetMenuWidth<T>(this Menu<T> menu) => menu.GetMenuItemTexts().Max(x => Resources.GetText($" {x}", menu.FontId, 5).Width + 2);

		public static int GetMenuHeight<T>(this Menu<T> menu) => menu.GetMenuItemTexts().Count() * Resources.GetFontHeight(menu.FontId);

		public static Menu<T> Items<T>(this Menu<T> menu, params MenuItem<T>[] menuItems)
		{
			menu.Items.AddRange(menuItems);
			menu.MenuWidth = menu.GetMenuWidth();
			return menu;
		}

		public static Menu<T> Always<T>(this Menu<T> menu, MenuItemEventHandler<T> action)
		{
			if (action != null)
			{
				foreach (MenuItem<T> item in menu.Items)
				{
					item.OnSelect(action);
				}
			}
			return menu;
		}

		public static Menu<T> Center<T>(this Menu<T> menu, IScreen screen)
		{
			menu.MenuWidth = menu.GetMenuWidth();
			menu.X = (int)Math.Floor(((double)screen.Width() - menu.GetMenuWidth()) / 2);
			menu.Y = (int)Math.Floor(((double)screen.Height() - menu.GetMenuHeight()) / 2);
			return menu;
		}

		public static Menu<T> SetActiveItem<T>(this Menu<T> menu, int activeItem)
		{
			if (activeItem >= 0) menu.ActiveItem = activeItem;
			return menu;
		}
	}
}