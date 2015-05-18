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
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CustomizeWorld : BaseScreen
	{
		private int _landMass = -1, _temperature = -1, _climate = -1, _age = -1;
		private bool _hasUpdate = true;
		
		private int GetMenuWidth(string title, string[] items)
		{
			int i = 0;
			Bitmap[] texts = new Bitmap[items.Length + 1];
			texts[i++] = Resources.Instance.GetText(" " + title, 0, 15);
			foreach (string item in items)
				texts[i++] = Resources.Instance.GetText(" " + item, 0, 5);			
			return (texts.Select(t => t.Width).Max()) + 6;
		}
		
		private Menu AddMenu(int y, string title, EventHandler setChoice, params string[] menuTexts)
		{
			Menu menu = new Menu(Canvas.Image.Palette.Entries)
			{
				Title = title,
				X = 203,
				Y = y,
				Width = GetMenuWidth(title, menuTexts),
				TitleColour = 15,
				ActiveColour = 11,
				TextColour = 79,
				DisabledColour = 8,
				FontId = 0
			};
			
			Menu.Item menuItem;
			for (int i = 0; i < menuTexts.Length; i++)
			{
				menu.Items.Add(menuItem = new Menu.Item(menuTexts[i], i));
				menuItem.Selected += setChoice;
			}
			menu.ActiveItem = 1;
			Menus.Add(menu);
			return menu;
		}
		
		private void SetLandMass(object sender, EventArgs args)
		{
			_landMass = (sender as Menu.Item).Value;
			_hasUpdate = true;
			Console.WriteLine(string.Format("Customize World - Land Mass: {0}", _landMass));
		}
		
		private void SetTemperature(object sender, EventArgs args)
		{
			_temperature = (sender as Menu.Item).Value;
			_hasUpdate = true;
			Console.WriteLine(string.Format("Customize World - Temperature: {0}", _temperature));
		}
		
		private void SetClimate(object sender, EventArgs args)
		{
			_climate = (sender as Menu.Item).Value;
			_hasUpdate = true;
			Console.WriteLine(string.Format("Customize World - Climate: {0}", _climate));
		}
		
		private void SetAge(object sender, EventArgs args)
		{
			_age = (sender as Menu.Item).Value;
			_hasUpdate = true;
			Console.WriteLine(string.Format("Customize World - Age: {0}", _age));
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (!_hasUpdate) return false;
			
			if (_landMass < 0) Common.AddScreen(AddMenu(6, "LAND MASS:", SetLandMass, "Small", "Normal", "Large"));
			else if (_temperature < 0) Common.AddScreen(AddMenu(56, "TEMPERATURE:", SetTemperature, "Cool", "Temperate", "Warm"));
			else if (_climate < 0) Common.AddScreen(AddMenu(106, "CLIMATE:", SetClimate, "Arid", "Normal", "Wet"));
			else if (_age < 0) Common.AddScreen(AddMenu(156, "AGE:", SetAge, "3 billion years", "4 billion years", "5 billion years"));
			else
			{
				Destroy();
				Common.AddScreen(new Demo());
			}
			
			_hasUpdate = false;
			return true;
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			return false;
		}
		
		public override bool MouseDown(MouseEventArgs args)
		{
			return false;
		}
		
		public CustomizeWorld()
		{
			Cursor = MouseCursor.Pointer;
			
			Picture background = Resources.Instance.LoadPIC("CUSTOM");
						
			_canvas = new Picture(320, 200, background.Image.Palette.Entries);
			_canvas.AddLayer(background.Image, 0, 0);
		}
	}
}