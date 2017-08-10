// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Linq;
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.UserInterface;

namespace CivOne.Screens
{
	internal class ChooseTech : BaseScreen
	{
		private readonly Picture _menuGfx;
		private readonly IAdvance[] _availableAdvances;
		private readonly int _menuHeight;
		
		private bool _update = true;

		private void AdvanceChoice(object sender, MenuItemEventArgs<IAdvance> args)
		{
			Human.CurrentResearch = args.Value;
			Destroy();
		}

		private void AdvanceContext(object sender, MenuItemEventArgs<IAdvance> args)
		{
			Common.AddScreen(new Civilopedia(args.Value));
		}

		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;

				Picture background = _menuGfx.GetPart(44, 35, 156, _menuHeight);
				Picture.ReplaceColours(background, new byte[] { 7, 22 }, new byte[] { 11, 3 });

				Menu<IAdvance> menu = new Menu<IAdvance>("ChooseTech", Palette, background)
				{
					X = 83,
					Y = 92,
					Width = 156,
					ActiveColour = 11,
					TextColour = 5,
					FontId = 0
				};

				foreach (IAdvance advance in _availableAdvances)
				{
					menu.Items.Add(advance.Name, advance)
						.OnSelect(AdvanceChoice)
						.OnContext(AdvanceContext);
				}
				AddMenu(menu);
				return true;
			}
			return true;
		}

		public ChooseTech()
		{
			_availableAdvances = Human.AvailableResearch.Take(8).ToArray();
			_menuHeight = Resources.Instance.GetFontHeight(0) * _availableAdvances.Count();
			
			Cursor = MouseCursor.Pointer;

			bool modernGovernment = Human.HasAdvance<Invention>();
			Picture governmentPortrait = Icons.GovernmentPortrait(Human.Government, Advisor.Science, modernGovernment);
			Color[] palette = Common.DefaultPalette;
			for (int i = 144; i < 256; i++)
			{
				palette[i] = governmentPortrait.Palette[i];
			}
			
			_canvas = new Picture(320, 200, palette);

			int dialogHeight = 41 + _menuHeight;
			if (dialogHeight < 62) dialogHeight = 62;

			_menuGfx = new Picture(204, dialogHeight);
			_menuGfx.FillLayerTile(Patterns.PanelGrey);
			_menuGfx.FillRectangle(0, 202, 0, 2, dialogHeight);
			_menuGfx.AddLayer(governmentPortrait, 1, dialogHeight - 61);

			_menuGfx.AddBorder(15, 8, 0, 0, 202, dialogHeight);
			_menuGfx.DrawText("Science Advisor:", 0, 15, 46, 3);
			_menuGfx.FillRectangle(11, 46, 10, 89, 1);
			_menuGfx.DrawText("Which discovery should our", 0, 15, 46, 12);
			_menuGfx.DrawText("wise men be pursuing, sire?", 0, 15, 46, 20);
			_menuGfx.DrawText("Pick one...", 0, 15, 46, 28);
			_menuGfx.DrawText($"(Help available)", 1, 10, 202, dialogHeight - Resources.Instance.GetFontHeight(1), TextAlign.Right);

			_canvas.FillRectangle(5, 38, 56, 204, dialogHeight + 2);
			AddLayer(_menuGfx, 39, 57);
		}
	}
}