// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Enums;
using CivOne.Graphics;

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
			if (sender is Input)
				((Input)sender)?.Close();
			Destroy();
		}

		private void CityName_Cancel(object sender, EventArgs args)
		{
			if (Cancel != null)
				Cancel(this, null);
			if (sender is Input)
				((Input)sender)?.Close();
			Destroy();
		}

		protected override bool HasUpdate(uint gameTick)
		{
			if (!Common.HasScreenType<Input>())
			{
				Common.AddScreen(_input);
			}
			return false;
		}

		public CityName(string cityName)
		{
			Palette = Common.DefaultPalette;

			this.FillRectangle(80, 80, 161, 33, 11)
				.FillRectangle(81, 81, 159, 31, 15)
				.DrawText("City Name...", 0, 5, 88, 82)
				.FillRectangle(88, 95, 105, 14, 5)
				.FillRectangle(89, 96, 103, 12, 15);

			_input = new Input(Palette, cityName, 0, 5, 11, 90, 97, 101, 10, 12);
			_input.Accept += CityName_Accept;
			_input.Cancel += CityName_Cancel;
		}
	}
}