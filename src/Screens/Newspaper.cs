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
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Events;
using CivOne.IO;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Newspaper : BaseScreen
	{
		private bool _update = true;

		public void Close()
		{
			Destroy();
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_update = false;
			}
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			Close();
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			Close();
			return true;
		}

		//public Newspaper(bool showGovernment, City city = null, params string[] message)
		public Newspaper(City city, string[] message, bool showGovernment = false)
		{
			Cursor = MouseCursor.None;

			bool modernGovernment = Human.HasAdvance<Invention>();
			Color[] palette = Common.DefaultPalette;

			Picture[] governmentPortraits = new Picture[4];
			if (showGovernment)
			{
				for (int i = 0; i < 4; i++)
				{
					governmentPortraits[i] = Icons.GovernmentPortrait(Human.Government, (Advisor)Enum.Parse(typeof(Advisor), i.ToString()), modernGovernment);
				}

				for (int i = 144; i < 256; i++)
				{
					palette[i] = governmentPortraits[0].Palette[i];
				}
			}
			
			string newsflash = TextFile.Instance.GetGameText($"KING/NEWS{(char)Common.Random.Next((int)'A', (int)'O')}")[0];
			string shout = (Common.Random.Next(0, 2) == 0) ? "FLASH" : "EXTRA!";
			string date = $"January 1, {Common.YearString(Game.GameTurn)}";
			string name = "NONE";
			if (city != null)
				name = city.Name;
			else if (Human.Cities.Length > 0)
				name = Human.Cities[0].Name;
			switch (Common.Random.Next(0, 3))
			{
				case 0: name = $"The {name} Times"; break;
				case 1: name = $"The {name} Tribune"; break;
				case 2: name = $"{name} Weekly"; break;
			}

			_canvas = new Picture(320, 200, palette);
			_canvas.FillRectangle(15, 0, 0, 320, 100);
			
			_canvas.DrawText("FLASH", 2, 5, 6, 3);
			_canvas.DrawText("FLASH", 2, 5, 272, 3);
			_canvas.DrawText(newsflash, 1, 5, 158, 3, TextAlign.Center);
			_canvas.DrawText(newsflash, 1, 5, 158, 3, TextAlign.Center);
			_canvas.DrawText(",-.", 4, 5, 8, 11);
			_canvas.DrawText(",-.", 4, 5, 268, 11);
			_canvas.DrawText(name, 4, 5, 160, 11, TextAlign.Center);
			_canvas.DrawText(date, 0, 5, 8, 28);
			_canvas.DrawText("10 cents", 0, 5, 272, 28);
			_canvas.FillRectangle(5, 1, 1, 318, 1);
			_canvas.FillRectangle(5, 1, 2, 1, 33);
			_canvas.FillRectangle(5, 318, 2, 1, 33);
			_canvas.FillRectangle(5, 0, 35, 320, 1);
			_canvas.FillRectangle(5, 0, 97, 320, 1);

			for (int i = 0; i < message.Length; i++)
			{
				_canvas.DrawText(message[i], 3, 5, 16, 40 + (i * 17));
			}

			if (showGovernment)
			{
				string[] advisorNames = new string[] { "Defense Minister", "Domestic Advisor", "Foreign Minister", "Science Advisor" };
				_canvas.FillRectangle(15, 0, 100, 320, 100);
				_canvas.DrawText("New Cabinet:", 5, 5, 106, 102);
				for (int i = 0; i < 4; i++)
				{
					AddLayer(governmentPortraits[i], 20 + (80 * i), 118);
					_canvas.DrawText(advisorNames[i], 1, 5, 40 + (80 * i), (i % 2) == 0 ? 180 : 186, TextAlign.Center);
				}
			}
			else
			{
				for (int xx = 0; xx < _canvas.Width; xx += Icons.Newspaper.Width)
				{
					AddLayer(Icons.Newspaper, xx, 100);
				}
				Picture background = Resources["SP299"].GetPart(288, 120, 32, 16);
				Picture dialog = new Picture(152, 15);
				dialog.FillLayerTile(background);
				dialog.FillRectangle(0, 151, 0, 1, 15);
				dialog.AddBorder(15, 8, 0, 0, 151, 15);
				dialog.DrawText("Press any key to continue.", 0, 15, 4, 4);
				_canvas.FillRectangle(5, 80, 128, 153, 17);
				AddLayer(dialog, 81, 129);
			}
		}
	}
}