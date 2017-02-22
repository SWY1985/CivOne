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

		private void MenuSetPlayerScience(object sender, EventArgs args)
		{
			Common.AddScreen(new SetPlayerScience());
			CloseMenus();
			Close();
		}

		private void MenuSetPlayerAdvances(object sender, EventArgs args)
		{
			Common.AddScreen(new SetPlayerAdvances());
			CloseMenus();
			Close();
		}

		private void MenuSetCitySize(object sender, EventArgs args)
		{
			Common.AddScreen(new SetCitySize());
			CloseMenus();
			Close();
		}

		private void MenuChangeHumanPlayer(object sender, EventArgs args)
		{
			Common.AddScreen(new ChangeHumanPlayer());
			CloseMenus();
			Close();
		}

		private void MenuSpawnUnit(object sender, EventArgs args)
		{
			Common.AddScreen(new SpawnUnit());
			CloseMenus();
			Close();
		}

		private void MenuMeetWithKing(object sender, EventArgs args)
		{
			Common.AddScreen(new MeetWithKing());
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
					Indent = 8
				};
				menu.MissClick += MenuCancel;
				menu.Cancel += MenuCancel;

				menu.Items.Add(new Menu.Item("Set Game Year"));
				menu.Items.Add(new Menu.Item("Set Player Gold"));
				menu.Items.Add(new Menu.Item("Set Player Science"));
				menu.Items.Add(new Menu.Item("Set Player Advances"));
				menu.Items.Add(new Menu.Item("Set City Size"));
				menu.Items.Add(new Menu.Item("Change Human Player"));
				menu.Items.Add(new Menu.Item("Spawn Unit"));
				menu.Items.Add(new Menu.Item("Meet With King"));

				menu.Items[0].Selected += MenuSetGameYear;
				menu.Items[1].Selected += MenuSetPlayerGold;
				menu.Items[2].Selected += MenuSetPlayerScience;
				menu.Items[3].Selected += MenuSetPlayerAdvances;
				menu.Items[4].Selected += MenuSetCitySize;
				menu.Items[5].Selected += MenuChangeHumanPlayer;
				menu.Items[6].Selected += MenuSpawnUnit;
				menu.Items[7].Selected += MenuMeetWithKing;

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