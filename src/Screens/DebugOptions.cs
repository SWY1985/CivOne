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
using CivOne.Tasks;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class DebugOptions : BaseScreen
	{
		private bool _update = true;
		
		private void MenuCancel(object sender, EventArgs args)
		{
			Destroy();
		}

		private void MenuSetGameYear(object sender, EventArgs args)
		{
			GameTask.Enqueue(Show.Screen<SetGameYear>());
			Destroy();
		}

		private void MenuSetPlayerGold(object sender, EventArgs args)
		{
			GameTask.Enqueue(Show.Screen<SetPlayerGold>());
			Destroy();
		}

		private void MenuSetPlayerScience(object sender, EventArgs args)
		{
			GameTask.Enqueue(Show.Screen<SetPlayerScience>());
			Destroy();
		}

		private void MenuSetPlayerAdvances(object sender, EventArgs args)
		{
			GameTask.Enqueue(Show.Screen<SetPlayerAdvances>());
			Destroy();
		}

		private void MenuSetCitySize(object sender, EventArgs args)
		{
			GameTask.Enqueue(Show.Screen<SetCitySize>());
			Destroy();
		}

		private void MenuChangeHumanPlayer(object sender, EventArgs args)
		{
			GameTask.Enqueue(Show.Screen<ChangeHumanPlayer>());
			Destroy();
		}

		private void MenuSpawnUnit(object sender, EventArgs args)
		{
			GameTask.Enqueue(Show.Screen<SpawnUnit>());
			Destroy();
		}

		private void MenuMeetWithKing(object sender, EventArgs args)
		{
			GameTask.Enqueue(Show.Screen<MeetWithKing>());
			Destroy();
		}

		private void MenuRevealWorld(object sender, EventArgs args)
		{
			Settings.Instance.RevealWorldCheat();
			Destroy();
		}

		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;

				Picture background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
				Picture menuGfx = new Picture(132, 87);
				menuGfx.FillLayerTile(background);
				menuGfx.AddBorder(15, 8, 0, 0, 131, 87);
				menuGfx.FillRectangle(0, 131, 0, 1, 87);
				menuGfx.DrawText("Debug Options:", 0, 15, 4, 4);

				Picture menuBackground = menuGfx.GetPart(2, 11, 128, 72);
				Picture.ReplaceColours(menuBackground, new byte[] { 7, 22 }, new byte[] { 11, 3 });

				AddLayer(menuGfx, 25, 17);

				Menu menu = new Menu(Canvas.Palette, menuBackground)
				{
					X = 27,
					Y = 28,
					Width = 127,
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
				menu.Items.Add(new Menu.Item("Toggle Reveal World"));

				menu.Items[0].Selected += MenuSetGameYear;
				menu.Items[1].Selected += MenuSetPlayerGold;
				menu.Items[2].Selected += MenuSetPlayerScience;
				menu.Items[3].Selected += MenuSetPlayerAdvances;
				menu.Items[4].Selected += MenuSetCitySize;
				menu.Items[5].Selected += MenuChangeHumanPlayer;
				menu.Items[6].Selected += MenuSpawnUnit;
				menu.Items[7].Selected += MenuMeetWithKing;
				menu.Items[8].Selected += MenuRevealWorld;

				_canvas.FillRectangle(5, 24, 16, 105, menu.RowHeight * (menu.Items.Count + 1));

				AddMenu(menu);
			}
			return true;
		}

		public DebugOptions()
		{
			Cursor = MouseCursor.Pointer;
			
			_canvas = new Picture(320, 200, Common.GamePlay.MainPalette);
			_canvas.AddLayer(Common.Screens.Last().Canvas, 0, 0);
			_canvas.FillRectangle(5, 24, 16, 133, 89);
		}
	}
}