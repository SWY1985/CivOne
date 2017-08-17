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
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;
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

		protected override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;

				Picture background = _menuGfx.GetPart(44, 35, 156, _menuHeight)
					.ColourReplace(new byte[] { 7, 22 }, new byte[] { 11, 3 })
					.As<Picture>();

				Menu<IAdvance> menu = new Menu<IAdvance>("ChooseTech", Palette, background)
				{
					X = 83,
					Y = 92,
					MenuWidth = 156,
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

		public ChooseTech() : base(MouseCursor.Pointer)
		{
			TextSettings DialogText = new TextSettings() { Colour = 15 };
			TextSettings HelpText = new TextSettings() { FontId = 1, Colour = 10, Alignment = TextAlign.Right, VerticalAlignment = VerticalAlign.Bottom };

			_availableAdvances = Human.AvailableResearch.Take(8).ToArray();
			_menuHeight = Resources.Instance.GetFontHeight(0) * _availableAdvances.Count();
			
			bool modernGovernment = Human.HasAdvance<Invention>();
			Picture governmentPortrait = Icons.GovernmentPortrait(Human.Government, Advisor.Science, modernGovernment);
			using (Palette palette = Common.DefaultPalette)
			{
				palette.MergePalette(governmentPortrait.Palette, 144);
				Palette = palette;
			}

			int dialogHeight = 41 + _menuHeight;
			if (dialogHeight < 62) dialogHeight = 62;

			_menuGfx = new Picture(202, dialogHeight)
					.Tile(Patterns.PanelGrey)
					.AddLayer(governmentPortrait, 1, dialogHeight - 61)
					.DrawRectangle3D()
					.DrawText("Science Advisor:", 46, 3, DialogText)
					.FillRectangle(46, 10, 89, 1, 11)
					.DrawText("Which discovery should our", 46, 12, DialogText)
					.DrawText("wise men be pursuing, sire?", 46, 20, DialogText)
					.DrawText("Pick one...", 46, 28, DialogText)
					.DrawText($"(Help available)", 202, dialogHeight, HelpText)
					.As<Picture>();

			this.DrawRectangle(38, 56, 204, dialogHeight + 2)
				.AddLayer(_menuGfx, 39, 57);
		}

		public override void Dispose()
		{
			_menuGfx.Dispose();
			base.Dispose();
		}
	}
}