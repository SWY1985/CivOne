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
	internal class CityName : BaseScreen
	{
		private readonly Input _input;

		public string Value { get; private set; }

		public event EventHandler Accept, Cancel;

		private void CityName_Accept(object sender, EventArgs args)
		{
			Value = (sender as Input).Text;
			if (Accept != null)
				Accept(this, null);
			((Input)sender).Close();
			Destroy();
		}

		private void CityName_Cancel(object sender, EventArgs args)
		{
			if (Cancel != null)
				Cancel(this, null);
			((Input)sender).Close();
			Destroy();
		}

		public override bool HasUpdate(uint gameTick)
		{
			if (!Common.HasScreenType(typeof(Input)))
			{
				Common.AddScreen(_input);
			}
			return false;
		}

		public CityName(string cityName)
		{
			Cursor = MouseCursor.None;

			_canvas = new Picture(320, 200, Common.Screens.Last().Canvas.OriginalColours);

			_canvas.FillRectangle(11, 80, 80, 161, 33);
			_canvas.FillRectangle(15, 81, 81, 159, 31);
			_canvas.DrawText("City Name...", 0, 5, 88, 82);
			_canvas.FillRectangle(5, 88, 95, 105, 14);
			_canvas.FillRectangle(15, 89, 96, 103, 12);

			_input = new Input(_canvas.Image.Palette.Entries, cityName, 0, 5, 11, 90, 97, 101, 10, 12);
			_input.Accept += CityName_Accept;
			_input.Cancel += CityName_Cancel;
		}
	}
}