// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Advances;
using CivOne.Enums;
using CivOne.Events;
using CivOne.IO;
using CivOne.Graphics;
using CivOne.Graphics.Sprites;

namespace CivOne.Screens
{
	internal class Newspaper : BaseScreen
	{
		private bool _update = true;

		public void Close()
		{
			Destroy();
		}
		
		protected override bool HasUpdate(uint gameTick)
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
		
		private void MouseDown(object sender, ScreenEventArgs args)
		{
			Close();
			args.Handled = true;
		}

		public Newspaper(City city, string[] message, bool showGovernment = false)
		{
			OnMouseDown += MouseDown;
			
			bool modernGovernment = Human.HasAdvance<Invention>();
			Palette palette = Common.DefaultPalette;

			IBitmap[] governmentPortraits = new IBitmap[4];
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

			Palette = palette;
			this.FillRectangle(0, 0, 320, 100, 15)
				.DrawText("FLASH", 2, 5, 6, 3)
				.DrawText("FLASH", 2, 5, 272, 3)
				.DrawText(newsflash, 1, 5, 158, 3, TextAlign.Center)
				.DrawText(newsflash, 1, 5, 158, 3, TextAlign.Center)
				.DrawText(",-.", 4, 5, 8, 11)
				.DrawText(",-.", 4, 5, 268, 11)
				.DrawText(name, 4, 5, 160, 11, TextAlign.Center)
				.DrawText(date, 0, 5, 8, 28)
				.DrawText("10 cents", 0, 5, 272, 28)
				.FillRectangle(1, 1, 318, 1, 5)
				.FillRectangle(1, 2, 1, 33, 5)
				.FillRectangle(318, 2, 1, 33, 5)
				.FillRectangle(0, 35, 320, 1, 5)
				.FillRectangle(0, 97, 320, 1, 5);

			for (int i = 0; i < message.Length; i++)
			{
				this.DrawText(message[i], 3, 5, 16, 40 + (i * 17));
			}

			if (showGovernment)
			{
				string[] advisorNames = new string[] { "Defense Minister", "Domestic Advisor", "Foreign Minister", "Science Advisor" };
				this.FillRectangle(0, 100, 320, 100, 15)
					.DrawText("New Cabinet:", 5, 5, 106, 102);
				for (int i = 0; i < 4; i++)
				{
					this.AddLayer(governmentPortraits[i], 20 + (80 * i), 118)
						.DrawText(advisorNames[i], 1, 5, 40 + (80 * i), (i % 2) == 0 ? 180 : 186, TextAlign.Center);
				}
			}
			else
			{
				for (int xx = 0; xx < Width; xx += Icons.Newspaper.Width())
				{
					this.AddLayer(Icons.Newspaper, xx, 100);
				}
				using (IBitmap dialog = new Picture(151, 15)
						.Tile(Pattern.PanelGrey)
						.DrawRectangle3D()
						.DrawText("Press any key to continue.", 0, 15, 4, 4))
				{
					this.FillRectangle(80, 128, 153, 17, 5)
						.AddLayer(dialog, 81, 129);
				}
			}
		}
	}
}