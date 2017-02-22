// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Linq;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Screens.Debug;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class DebugOptions : BaseScreen
	{
		private bool _update = true;
		
		private void MenuCancel(object sender, EventArgs args)
		{
			CloseMenus();
			Close();
		}

		private void MenuSetGameYear(object sender, EventArgs args)
		{
			Common.AddScreen(new SetGameYear());
			CloseMenus();
			Close();
		}

		private void MenuSetPlayerGold(object sender, EventArgs args)
		{
			Common.AddScreen(new SetPlayerGold());
			CloseMenus();
			Close();
		}

		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;

				Picture background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
				Picture menuGfx = new Picture(124, 79);
				menuGfx.FillLayerTile(background);
				menuGfx.AddBorder(15, 8, 0, 0, 123, 79);
				menuGfx.FillRectangle(0, 123, 0, 1, 79);
				menuGfx.DrawText("Debug Options:", 0, 15, 4, 4);

				Picture menuBackground = menuGfx.GetPart(2, 11, 120, 64);
				Picture.ReplaceColours(menuBackground, new byte[] { 7, 22 }, new byte[] { 11, 3 });

				AddLayer(menuGfx, 25, 17);

				Menu menu = new Menu(Canvas.Palette, menuBackground)
				{
					X = 27,
					Y = 28,
					Width = 119,
					ActiveColour = 11,
					TextColour = 5,
					DisabledColour = 3,
					FontId = 0,
					Indent = 2
				};
				menu.MissClick += MenuCancel;
				menu.Cancel += MenuCancel;

				menu.Items.Add(new Menu.Item("Set game year"));
				menu.Items.Add(new Menu.Item("Set player gold"));
				menu.Items.Add(new Menu.Item("Set player lightbulbs") { Enabled = false });
				menu.Items.Add(new Menu.Item("Set player advances") { Enabled = false });
				menu.Items.Add(new Menu.Item("Set city size") { Enabled = false });
				menu.Items.Add(new Menu.Item("Change human player") { Enabled = false });
				menu.Items.Add(new Menu.Item("Spawn unit") { Enabled = false });
				menu.Items.Add(new Menu.Item("Meet with king") { Enabled = false });

				menu.Items[0].Selected += MenuSetGameYear;
				menu.Items[1].Selected += MenuSetPlayerGold;

				_canvas.FillRectangle(5, 24, 16, 105, menu.RowHeight * (menu.Items.Count + 1));

				AddMenu(menu);
			}
			return true;
		}

		public void Close()
		{
			HandleClose();
			Destroy();
		}

		public DebugOptions()
		{
			Cursor = MouseCursor.Pointer;

			Color[] palette = Resources.Instance.LoadPIC("SP257").Palette;
			
			_canvas = new Picture(320, 200, palette);
			_canvas.AddLayer(Common.Screens.Last().Canvas, 0, 0);
			_canvas.FillRectangle(5, 24, 16, 125, 81);
		}
	}
}