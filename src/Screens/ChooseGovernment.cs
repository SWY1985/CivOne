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
using System.Drawing;
using System.Linq;
using CivOne.Advances;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class ChooseGovernment : BaseScreen
	{
		private readonly Bitmap _background;
		private readonly Picture _menuGfx;
		private readonly Government[] _availableGovernments;
		private readonly int _menuHeight;
		
		private bool _update = true;

		public Government Result { get; private set; }

		private void GovernmentChoice(object sender, EventArgs args)
		{
			Result = _availableGovernments[(sender as Menu.Item).Value];
			CloseMenus();
			Close();
		}

		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;

				Bitmap background = (Bitmap)_menuGfx.GetPart(2, 19, 84, _menuHeight).Clone();
				Picture.ReplaceColours(background, new byte[] { 7, 22 }, new byte[] { 11, 3 });

				Menu menu = new Menu(Canvas.Image.Palette.Entries, background)
				{
					X = 103,
					Y = 84,
					Width = 82,
					ActiveColour = 11,
					TextColour = 5,
					FontId = 0
				};

				Menu.Item menuItem;
				for (int i = 0; i < _availableGovernments.Length; i++)
				{
					menu.Items.Add(menuItem = new Menu.Item($"{_availableGovernments[i]}", i));
					menuItem.Selected += GovernmentChoice;
				}
				Menus.Add(menu);
				Common.AddScreen(menu);
				return true;
			}
			return true;
		}

		public void Close()
		{
			HandleClose();
			Destroy();
		}

		public ChooseGovernment()
		{
			_background = (Bitmap)Resources.Instance.GetPart("SP299", 288, 120, 32, 16);
			_availableGovernments = Game.Instance.HumanPlayer.AvailableGovernments.ToArray();
			_menuHeight = Resources.Instance.GetFontHeight(0) * _availableGovernments.Count();
			
			Cursor = MouseCursor.Pointer;

			Color[] palette = Resources.Instance.LoadPIC("SP257").Image.Palette.Entries;
			_canvas = new Picture(320, 200, palette);

			int dialogHeight = 23 + _menuHeight;

			_menuGfx = new Picture(88, dialogHeight);
			_menuGfx.FillLayerTile(_background);
			_menuGfx.FillRectangle(0, 86, 0, 2, dialogHeight);

			_menuGfx.AddBorder(15, 8, 0, 0, 86, dialogHeight);
			_menuGfx.DrawText("Select type of", 0, 15, 4, 4);
			_menuGfx.DrawText("Government...", 0, 15, 4, 12);

			_canvas.FillRectangle(5, 100, 64, 88, dialogHeight + 2);
			AddLayer(_menuGfx, 101, 65);
		}
	}
}