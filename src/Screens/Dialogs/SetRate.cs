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
using System.Linq;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Templates;
using CivOne.UserInterface;

namespace CivOne.Screens.Dialogs
{
	internal class SetRate : BaseDialog
	{
		private const int FONT_ID = 0;

		private readonly bool _luxuries;
		private readonly string[] _menuItems;

		private void TaxesChoice(object sender, MenuItemEventArgs<int> args)
		{
			Human.TaxesRate = args.Value;
			Cancel();
		}

		private void LuxuriesChoice(object sender, MenuItemEventArgs<int> args)
		{
			Human.LuxuriesRate = args.Value;
			Cancel();
		}

		private string ScreenName
		{
			get
			{
				if (_luxuries)
					return "Luxuries";
				return "Tax";
			}
		}

		private int ItemWidth
		{
			get
			{
				return MenuOptions(_luxuries).Max(x => Resources.Instance.GetTextSize(0, x).Width) + 11;
			}
		}

		private MenuItemEventHandler<int> ChoiceMethod
		{
			get
			{
				if (_luxuries)
					return LuxuriesChoice;
				return TaxesChoice;
			}
		}

		protected override void FirstUpdate()
		{
			Menu menu = new Menu(Palette, Selection(3, 12, ItemWidth, (_menuItems.Length * Resources.Instance.GetFontHeight(FONT_ID)) + 4))
			{
				X = 103,
				Y = 92,
				Width = ItemWidth,
				ActiveColour = 11,
				TextColour = 5,
				FontId = FONT_ID
			};
			for (int i = 0; i < _menuItems.Length; i++)
			{
				menu.Items.Add(_menuItems[i], i).OnSelect(ChoiceMethod);
			}

			menu.MissClick += Cancel;
			menu.Cancel += Cancel;

			if (_luxuries)
				menu.ActiveItem = Human.LuxuriesRate;
			else
				menu.ActiveItem = Human.TaxesRate;
			
			AddMenu(menu);
		}

		private static IEnumerable<string> MenuOptions(bool luxuries)
		{
			if (luxuries)
			{
				for (int i = 0; i <= (10 - Human.TaxesRate); i++)
				{
					int science = 10 - Human.TaxesRate - i;
					yield return $"{i * 10}% Luxuries, ({science * 10}% Science)";
				}
				yield break;
			}

			for (int i = 0; i <= (10 - Human.LuxuriesRate); i++)
			{
				int science = 10 - Human.LuxuriesRate - i;
				yield return $"{i * 10}% Tax, ({science * 10}% Science)";
			}
		}

		private static int DialogWidth(bool luxuries)
		{
			return MenuOptions(luxuries).Max(x => Resources.Instance.GetTextSize(0, x).Width) + 15;
		}

		private static int DialogHeight(bool luxuries)
		{
			return (MenuOptions(luxuries).Count() * Resources.Instance.GetFontHeight(FONT_ID)) + 15;
		}

		public static SetRate Taxes
		{
			get
			{
				return new SetRate(luxuries: false);
			}
		}

		public static SetRate Luxuries
		{
			get
			{
				return new SetRate(luxuries: true);
			}
		}

		private SetRate(bool luxuries) : base(100, 80, DialogWidth(luxuries), DialogHeight(luxuries))
		{
			_luxuries = luxuries;
			_menuItems = MenuOptions(luxuries).ToArray();
			
			DialogBox.DrawText($"Select new {ScreenName} rate...", 0, 15, 5, 5);
		}
	}
}