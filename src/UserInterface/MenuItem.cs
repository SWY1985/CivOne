// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Events;

namespace CivOne.UserInterface
{
	public class MenuItem<T>
	{
		private MenuItemEventArgs<T> _args => new MenuItemEventArgs<T>(Value);

		public event MenuItemEventHandler<T> Selected;
		public event MenuItemEventHandler<T> RightClick;
		public T Value { get; private set; }
		public bool Enabled { get; set; }
		public string Text { get; set; }

		internal void Select()
		{
			if (Selected == null) return;
			Selected(this, _args);
		}

		internal void Context()
		{
			if (RightClick == null)
			{
				Select();
				return;
			}
			RightClick(this, _args);
		}

		internal static MenuItem<T> Create(string text, T value = default(T))
		{
			return new MenuItem<T>(text, value);
		}

		protected MenuItem(string text, T value = default(T))
		{
			Enabled = true;
			Text = text;
			Value = value;
		}
	}
}