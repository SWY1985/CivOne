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
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class GameOptions : BaseScreen
	{
		private bool _update = true;
		
		private void MenuCancel(object sender, EventArgs args)
		{
			CloseMenus();
			Close();
		}

		private void MenuAnimations(object sender, EventArgs args)
		{
			Settings.Animations = !Settings.Animations;
			CloseMenus();
			Close();
		}

		private void MenuCivilopediaText(object sender, EventArgs args)
		{
			Settings.CivilopediaText = !Settings.CivilopediaText;
			CloseMenus();
			Close();
		}

		private void MenuInstantAdvice(object sender, EventArgs args)
		{
			Settings.InstantAdvice = !Settings.InstantAdvice;
			CloseMenus();
			Close();
		}

		private void MenuAutoSave(object sender, EventArgs args)
		{
			Settings.AutoSave = !Settings.AutoSave;
			CloseMenus();
			Close();
		}

		private void MenuEndOfTurn(object sender, EventArgs args)
		{
			Settings.EndOfTurn = !Settings.EndOfTurn;
			CloseMenus();
			Close();
		}

		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;

				Picture background = Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
				Picture menuGfx = new Picture(104, 79);
				menuGfx.FillLayerTile(background);
				menuGfx.AddBorder(15, 8, 0, 0, 103, 79);
				menuGfx.FillRectangle(0, 103, 0, 1, 79);
				menuGfx.DrawText("Options:", 0, 15, 4, 4);

				Picture menuBackground = menuGfx.GetPart(2, 11, 100, 64);
				Picture.ReplaceColours(menuBackground, new byte[] { 7, 22 }, new byte[] { 11, 3 });

				AddLayer(menuGfx, 25, 17);

				Menu menu = new Menu(Canvas.Palette, menuBackground)
				{
					X = 27,
					Y = 28,
					Width = 99,
					ActiveColour = 11,
					TextColour = 5,
					DisabledColour = 3,
					FontId = 0,
					Indent = 2
				};
				menu.MissClick += MenuCancel;
				menu.Cancel += MenuCancel;

				menu.Items.Add(new Menu.Item($"{(Settings.InstantAdvice ? '^' : ' ')}Instant Advice"));
				menu.Items.Add(new Menu.Item($"{(Settings.AutoSave ? '^' : ' ')}AutoSave"));
				menu.Items.Add(new Menu.Item($"{(Settings.EndOfTurn ? '^' : ' ')}End of Turn"));
				menu.Items.Add(new Menu.Item($"{(Settings.Animations ? '^' : ' ')}Animations"));
				menu.Items.Add(new Menu.Item(" Sound") { Enabled = false });
				menu.Items.Add(new Menu.Item(" Enemy Moves") { Enabled = false });
				menu.Items.Add(new Menu.Item($"{(Settings.CivilopediaText ? '^' : ' ')}Civilopedia Text"));
				menu.Items.Add(new Menu.Item(" Palace") { Enabled = false });

				menu.Items[0].Selected += MenuInstantAdvice;
				menu.Items[1].Selected += MenuAutoSave;
				menu.Items[2].Selected += MenuEndOfTurn;
				menu.Items[3].Selected += MenuAnimations;
				menu.Items[6].Selected += MenuCivilopediaText;

				AddMenu(menu);
			}
			return true;
		}

		public void Close()
		{
			Destroy();
		}

		public GameOptions()
		{
			Cursor = MouseCursor.Pointer;

			Color[] palette = Resources.Instance.LoadPIC("SP257").Palette;
			
			_canvas = new Picture(320, 200, palette);
			_canvas.AddLayer(Common.Screens.Last().Canvas, 0, 0);
			_canvas.FillRectangle(5, 24, 16, 105, 81);
		}
	}
}