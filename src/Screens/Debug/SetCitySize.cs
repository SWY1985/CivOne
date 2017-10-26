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
using CivOne.Graphics.Sprites;
using CivOne.Tasks;
using CivOne.UserInterface;

namespace CivOne.Screens.Debug
{
	internal class SetCitySize : BaseScreen
	{
		private readonly City[] _cities = Game.GameState._cities.OrderBy(x => x.Name).ToArray();

		private Menu _citySelect;

		private Input _input;

		private int _index = 0;

		private City _selectedCity = null;

		public string Value { get; private set; }

		public event EventHandler Accept, Cancel;

		private void CitiesMenu()
		{
			Palette = Common.Screens.Last().OriginalColours;

			City[] cities = _cities.Skip(_index).Take(15).ToArray();

			bool more = (cities.Length < _cities.Length);

			int fontHeight = Resources.GetFontHeight(0);
			int hh = (fontHeight * (cities.Length + (more ? 2 : 1))) + 5;
			int ww = 136;

			int xx = (320 - ww) / 2;
			int yy = (200 - hh) / 2;

			Picture menuGfx = new Picture(ww, hh)
				.Tile(Pattern.PanelGrey)
				.DrawRectangle3D()
				.As<Picture>();
			IBitmap menuBackground = menuGfx[2, 11, ww - 4, hh - 11].ColourReplace((7, 11), (22, 3));

			this.FillRectangle(xx - 1, yy - 1, ww + 2, hh + 2, 5)
				.AddLayer(menuGfx, xx, yy)
				.DrawText("Set City Size...", 0, 15, xx + 8, yy + 3);

			_citySelect = new Menu(Palette, menuBackground)
			{
				X = xx + 2,
				Y = yy + 11,
				MenuWidth = ww - 4,
				ActiveColour = 11,
				TextColour = 5,
				DisabledColour = 3,
				FontId = 0,
				Indent = 8
			};

			foreach (City city in cities)
			{
				_citySelect.Items.Add($"{city.Name} ({Game.GameState.GetPlayer(city.Owner).TribeName})").OnSelect(CitySize_Accept);
			}

			if (more)
			{
				_citySelect.Items.Add($" ---MORE---").OnSelect(CitySize_More);
			}

			_citySelect.Cancel += CitySize_Cancel;
			_citySelect.MissClick += CitySize_Cancel;
			_citySelect.ActiveItem = (_citySelect.Items.Count - 1);
		}

		private void CitySizeSet_Accept(object sender, EventArgs args)
		{
			Value = (sender as Input).Text;
			
			byte citySize;
			if (!byte.TryParse(Value, out citySize) || citySize < 1 || citySize > 99)
			{
				GameTask.Enqueue(Message.Error("-- DEBUG: Set City Size --", $"The value {Value} is invalid or out of range.", "Please enter a value between 1 and 99."));
			}
			else
			{
				_selectedCity.Size = citySize;
				GameTask.Enqueue(Message.General($"{_selectedCity.Name} size set to {citySize}."));
			}

			if (Accept != null)
				Accept(this, null);
			if (sender is Input)
				((Input)sender)?.Close();
			Destroy();
		}

		private void CitySize_More(object sender, EventArgs args)
		{
			_index += 15;
			if (_index > _cities.Count()) _index = 0;
			CloseMenus();
		}

		private void CitySize_Accept(object sender, EventArgs args)
		{
			Palette = Common.Screens.Last().OriginalColours;

			this.FillRectangle(80, 80, 161, 33, 11)
				.FillRectangle(81, 81, 159, 31, 15)
				.DrawText("Set City Size...", 0, 5, 88, 82)
				.FillRectangle(88, 95, 105, 14, 5)
				.FillRectangle(89, 96, 103, 12, 15);

			_selectedCity = _cities[_citySelect.ActiveItem + _index];

			_input = new Input(Palette, _selectedCity.Size.ToString(), 0, 5, 11, 90, 97, 101, 10, 3);
			_input.Accept += CitySizeSet_Accept;
			_input.Cancel += CitySize_Cancel;

			CloseMenus();
		}

		private void CitySize_Cancel(object sender, EventArgs args)
		{
			if (Cancel != null)
				Cancel(this, null);
			if (sender is Input)
				((Input)sender)?.Close();
			Destroy();
		}

		protected override bool HasUpdate(uint gameTick)
		{
			if (_cities.Length == 0)
			{
				Destroy();
				return false;
			}

			if (_selectedCity == null && Common.TopScreen.GetType() != typeof(Menu))
			{
				AddMenu(_citySelect);
				return false;
			}
			else if (_selectedCity != null && Common.TopScreen.GetType() != typeof(Input))
			{
				Common.AddScreen(_input);
			}
			return false;
		}

		public SetCitySize() : base(MouseCursor.Pointer)
		{
			if (_cities.Length == 0)
			{
				GameTask.Enqueue(Message.General($"There are no cities yet."));
				return;
			}

			CitiesMenu();
		}
	}
}