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
using CivOne.Graphics;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	internal class GameOptions : BaseScreen
	{
		private bool _update = true;
		
		private void MenuCancel(object sender, EventArgs args)
		{
			Destroy();
		}

		private void MenuAnimations(object sender, EventArgs args)
		{
			Settings.Animations = !Settings.Animations;
			Update();
		}

		private void MenuSound(object sender, EventArgs args)
		{
			Settings.Sound = !Settings.Sound;
			Update();
		}

		private void MenuCivilopediaText(object sender, EventArgs args)
		{
			Settings.CivilopediaText = !Settings.CivilopediaText;
			Update();
		}

		private void MenuInstantAdvice(object sender, EventArgs args)
		{
			Settings.InstantAdvice = !Settings.InstantAdvice;
			Update();
		}

		private void MenuAutoSave(object sender, EventArgs args)
		{
			Settings.AutoSave = !Settings.AutoSave;
			Update();
		}

		private void MenuEndOfTurn(object sender, EventArgs args)
		{
			Settings.EndOfTurn = !Settings.EndOfTurn;
			Update();
		}

		private void Update()
		{
			CloseMenus();
			_update = true;
		}

		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;

				Picture menuGfx = new Picture(103, 79)
					.Tile(Patterns.PanelGrey)
					.DrawRectangle3D()
					.DrawText("Options:", 0, 15, 4, 4)
					.As<Picture>();

				Picture menuBackground = menuGfx.GetPart(2, 11, 100, 64);
				Picture.ReplaceColours(menuBackground, new byte[] { 7, 22 }, new byte[] { 11, 3 });

				this.AddLayer(menuGfx, 25, 17);

				Menu menu = new Menu(Palette, menuBackground)
				{
					X = 27,
					Y = 28,
					MenuWidth = 99,
					ActiveColour = 11,
					TextColour = 5,
					DisabledColour = 3,
					FontId = 0,
					Indent = 2
				};
				menu.MissClick += MenuCancel;
				menu.Cancel += MenuCancel;

				menu.Items.Add($"{(Settings.InstantAdvice ? '^' : ' ')}Instant Advice").OnSelect(MenuInstantAdvice);
				menu.Items.Add($"{(Settings.AutoSave ? '^' : ' ')}AutoSave").OnSelect(MenuAutoSave);
				menu.Items.Add($"{(Settings.EndOfTurn ? '^' : ' ')}End of Turn").OnSelect(MenuEndOfTurn);
				menu.Items.Add($"{(Settings.Animations ? '^' : ' ')}Animations").OnSelect(MenuAnimations);
				menu.Items.Add($"{(Settings.Sound ? '^' : ' ')}Sound").OnSelect(MenuSound);
				menu.Items.Add(" Enemy Moves").Disable();
				menu.Items.Add($"{(Settings.CivilopediaText ? '^' : ' ')}Civilopedia Text").OnSelect(MenuCivilopediaText);
				menu.Items.Add(" Palace").Disable();

				AddMenu(menu);
			}
			return true;
		}

		public GameOptions() : base(MouseCursor.Pointer)
		{
			_canvas = new Picture(320, 200, Common.DefaultPalette);
			this.AddLayer(Common.Screens.Last(), 0, 0)
				.FillRectangle(5, 24, 16, 105, 81);
		}
	}
}