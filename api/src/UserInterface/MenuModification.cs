// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Interfaces;

namespace CivOne.UserInterface
{
	public abstract class MenuModification : IModification
	{
		public string MenuId { get; private set; }

		public virtual (string MenuText, string ShortcutText) ChangeMenuItemText(string menuText, string shortcutText)
		{
			return (menuText, shortcutText);
		}

		public MenuModification(string menuId)
		{
			MenuId = menuId;
		}
	}
}