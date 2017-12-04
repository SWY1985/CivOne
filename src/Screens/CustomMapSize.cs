// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	[Break]
	internal class CustomMapSize : BaseScreen
	{
		private readonly Menu<Size> _sizeSelect;

		private Size _selectedMapSize = Size.Empty;

		public Size MapSize => _selectedMapSize;

		protected override bool HasUpdate(uint gameTick)
		{
			if (_selectedMapSize == Size.Empty && Common.TopScreen.GetType() != typeof(Menu<Size>))
			{
				AddMenu(_sizeSelect);
				return false;
			}
			return false;
		}

		public CustomMapSize() : base(MouseCursor.Pointer)
		{	
			Palette = Common.GetPalette256;

			this.Clear(3)
				.DrawText("Warning! Save games will be disabled on", 1, 16, 159, 176, TextAlign.Center)
				.DrawText("anything other than Normal map size...", 1, 16, 159, 184, TextAlign.Center);

			_sizeSelect = new Menu<Size>("CustomMapSize", Palette)
			{
				X = 100,
				Y = 80,
				Title = "CUSTOM MAP SIZE",
				TitleColour = 15,
				MenuWidth = 120,
				ActiveColour = 11,
				TextColour = 5,
				FontId = 0,
				Indent = 8
			};

			_sizeSelect.Items.Add("Tiny (40x25)", new Size(40, 25));
			_sizeSelect.Items.Add("Small (60x40)", new Size(60, 40));
			_sizeSelect.Items.Add("Normal (80x50)", new Size(80, 50));
			_sizeSelect.Items.Add("Large (120x75)", new Size(120, 75));
			_sizeSelect.Items.Add("Huge (160x100)", new Size(160, 100));

			foreach (MenuItem<Size> item in _sizeSelect.Items)
			{
				item.Selected += (s, a) =>
				{
					_selectedMapSize = item.Value;
					Destroy();
				};
			}

			_sizeSelect.ActiveItem = 2;
		}
	}
}