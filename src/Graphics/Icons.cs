// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Buildings;
using CivOne.Enums;
using CivOne.Graphics.Sprites;
using CivOne.Governments;

namespace CivOne.Graphics
{
	internal class Icons
	{
		private static Resources Resources => Resources.Instance;
		private static IBitmap _food;
		public static IBitmap Food
		{
			get
			{
				if (_food == null)
				{
					if (RuntimeHandler.Runtime.Settings.Free || !Resources.Exists("SP257"))
					{
						_food = new Picture(Free.Instance.Food, Common.GetPalette256);
					}
					else
					{
						_food = Resources["SP257"][128, 32, 8, 8]
							.ColourReplace(3, 0)
							.FillRectangle(0, 0, 1, 8, 0);
					}
				}
				return _food;
			}
		}

		private static IBitmap _foodLoss;
		public static IBitmap FoodLoss
		{
			get
			{
				if (_foodLoss == null)
				{
					_foodLoss = Resources["SP257"][128, 32, 8, 8]
						.ColourReplace((3, 0), (15, 5))
						.FillRectangle(0, 0, 1, 8, 0);
				}
				return _foodLoss;
			}
		}
		
		private static IBitmap _shield;
		public static IBitmap Shield
		{
			get
			{
				if (_shield == null)
				{
					if (RuntimeHandler.Runtime.Settings.Free || !Resources.Exists("SP257"))
					{
						_shield = new Picture(Free.Instance.Shield, Common.GetPalette256);
					}
					else
					{
						_shield = Resources["SP257"][136, 32, 8, 8].ColourReplace(3, 0);
					}
				}
				return _shield;
			}
		}
		
		private static IBitmap _shieldLoss;
		public static IBitmap ShieldLoss
		{
			get
			{
				if (_shieldLoss == null)
				{
					_shieldLoss = Resources["SP257"][136, 32, 8, 8].ColourReplace((3, 0), (15, 5));
				}
				return _shieldLoss;
			}
		}
		
		private static IBitmap _trade;
		public static IBitmap Trade
		{
			get
			{
				if (_trade == null)
				{
					if (RuntimeHandler.Runtime.Settings.Free || !Resources.Exists("SP257"))
					{
						_trade = new Picture(Free.Instance.Trade, Common.GetPalette256);
					}
					else
					{
						_trade = Resources["SP257"][144, 32, 8, 8].ColourReplace(3, 0);
					}
				}
				return _trade;
			}
		}

		private static IBitmap _corruption;
		public static IBitmap Corruption
		{
			get
			{
				if (_corruption == null)
				{
					_corruption = Resources["SP257"][144, 32, 8, 8].ColourReplace((3, 0), (15, 5));
				}
				return _corruption;
			}
		}
		
		private static IBitmap _unhappy;
		public static IBitmap Unhappy
		{
			get
			{
				if (_unhappy == null)
				{
					_unhappy = Resources["SP257"][136, 40, 8, 8].ColourReplace(3, 0);
				}
				return _unhappy;
			}
		}
		
		private static IBitmap _luxuries;
		public static IBitmap Luxuries
		{
			get
			{
				if (_luxuries == null)
				{
					if (RuntimeHandler.Runtime.Settings.Free || !Resources.Exists("SP257"))
					{
						_luxuries = new Picture(Free.Instance.Luxuries, Common.GetPalette256);
					}
					else
					{
						_luxuries = Resources["SP257"][144, 40, 8, 8].ColourReplace(3, 0);
					}
				}
				return _luxuries;
			}
		}
		
		private static IBitmap _taxes;
		public static IBitmap Taxes
		{
			get
			{
				if (_taxes == null)
				{
					if (RuntimeHandler.Runtime.Settings.Free || !Resources.Exists("SP257"))
					{
						_taxes = new Picture(Free.Instance.Taxes, Common.GetPalette256);
					}
					else
					{
						_taxes = Resources["SP257"][152, 32, 8, 8].ColourReplace(3, 0);
					}
				}
				return _taxes;
			}
		}
		
		private static IBitmap _science;
		public static IBitmap Science
		{
			get
			{
				if (_science == null)
				{
					if (RuntimeHandler.Runtime.Settings.Free || !Resources.Exists("SP257"))
					{
						_science = new Picture(Free.Instance.Science, Common.GetPalette256);
					}
					else
					{
						_science = Resources["SP257"][128, 40, 8, 8].ColourReplace(3, 0);
					}
				}
				return _science;
			}
		}
		
		private static IBitmap _spy;
		public static IBitmap Spy
		{
			get
			{
				if (_spy == null)
				{
					if (RuntimeHandler.Runtime.Settings.Free || !Resources.Exists("SP299"))
					{
						_spy = new Picture(Free.Instance.PanelGrey, Common.GetPalette256);
					}
					else
					{
						_spy = Resources["SP299"][160, 142, 40, 52].ColourReplace(3, 0);
					}
				}
				return _spy;
			}
		}
		
		private static IBitmap _newspaper;
		public static IBitmap Newspaper
		{
			get
			{
				if (_newspaper == null)
				{
					_newspaper = Resources["SP257"][176, 128, 32, 16];
				}
				return _newspaper;
			}
		}

		private static IBitmap _sellButton;
		public static IBitmap SellButton
		{
			get
			{
				if (_sellButton == null)
				{
					byte[] bytemap = new byte[] {
						0,  0,  5,  5,  5,  0,  0,  0,
						0,  5, 15, 15, 15,  5,  0,  0,
						5, 15, 12, 12, 12, 15,  5,  0,
						5, 15, 12, 12, 12, 15,  5,  0,
						5, 15, 12, 12, 12, 15,  5,  0,
						0,  5, 15, 15, 15,  5,  0,  0,
						0,  0,  5,  5,  5,  0,  0,  0
					};
					_sellButton = new Picture(8, 7, bytemap, Food.Palette);
				}
				return _sellButton;
			}
		}

		private static IBitmap[] _helperArrow;
		public static IBitmap HelperArrow(Direction direction)
		{
			if (_helperArrow == null)
			{
				_helperArrow = new IBitmap[4];
				_helperArrow[0] = new Picture(16, 16, new byte[] {
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5,  5,  5,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  5,  5,  5,  5,  5, 15, 15,  5,  5,  5,  5,  5,  0,  0,
					0,  0,  0,  5, 15, 15, 15, 15, 15, 15, 15, 15,  5,  0,  0,  0,
					0,  0,  0,  0,  5, 15, 15, 15, 15, 15, 15,  5,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  5, 15, 15, 15, 15,  5,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  5,  5,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				}, Food.Palette);
				_helperArrow[1] = new Picture(16, 16, new byte[] {
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  5,  5,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  5, 15, 15, 15, 15,  5,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  5, 15, 15, 15, 15, 15, 15,  5,  0,  0,  0,  0,
					0,  0,  0,  5, 15, 15, 15, 15, 15, 15, 15, 15,  5,  0,  0,  0,
					0,  0,  5,  5,  5,  5,  5, 15, 15,  5,  5,  5,  5,  5,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5,  5,  5,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				}, Food.Palette);
				_helperArrow[2] = new Picture(16, 16, new byte[] {
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  5,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5,  5,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  5, 15,  5,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  5, 15, 15, 15,  5,  5,  5,  5,  5,  5,  5,  0,  0,
					0,  0,  5, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,  5,  0,  0,
					0,  0,  5, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,  5,  0,  0,
					0,  0,  0,  5, 15, 15, 15,  5,  5,  5,  5,  5,  5,  5,  0,  0,
					0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  5, 15,  5,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  5,  5,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  5,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				}, Food.Palette);
				_helperArrow[3] = new Picture(16, 16, new byte[] {
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  5,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  5,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  5, 15,  5,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,
					0,  0,  5,  5,  5,  5,  5,  5,  5, 15, 15, 15,  5,  0,  0,  0,
					0,  0,  5, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,  5,  0,  0,
					0,  0,  5, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,  5,  0,  0,
					0,  0,  5,  5,  5,  5,  5,  5,  5, 15, 15, 15,  5,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  5, 15, 15,  5,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  5, 15,  5,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  5,  5,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  5,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
					0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				}, Food.Palette);
			}

			switch (direction)
			{
				case Direction.South: return _helperArrow[0];
				case Direction.North: return _helperArrow[1];
				case Direction.West: return _helperArrow[2];
				case Direction.East: return _helperArrow[3];
			}
			return null;
		}

		private static IBitmap[] _citizen = new Picture[9];
		public static IBitmap Citizen(Citizen citizen)
		{
			if (_citizen[(int)citizen] == null)
			{
				_citizen[(int)citizen] = Resources["SP257"][(8 * (int)citizen), 128, 8, 16];
			}
			return _citizen[(int)citizen];
		}

		private static IBitmap[] _lamp = new Picture[4];
		public static IBitmap Lamp(int stage)
		{
			if (stage < 0 || stage > 3)
				return null;
			
			if (_lamp[stage] == null)
			{
				_lamp[stage] = Resources["SP257"][128 + (8 * stage), 48, 8, 8];
			}
			return _lamp[stage];
		}

		private static IBitmap[,] _governmentPortrait = new Picture[7, 4];
		public static IBitmap GovernmentPortrait(IGovernment government, Advisor advisor, bool modern)
		{
			string filename;
			int governmentId;
			if (government is Monarchy)
			{
				governmentId = (modern ? 3 : 2);
				filename = $"GOVT1" + (modern ? "M" : "A");
			}
			else if (government is Republic || government is Democracy)
			{
				governmentId = (modern ? 5 : 4);
				filename = $"GOVT2" + (modern ? "M" : "A");
			}
			else if (government is Communism)
			{
				governmentId = 6;
				filename = "GOVT3A";
			}
			else // Anarchy or Despotism
			{
				governmentId = (modern ? 1 : 0);
				filename = "GOVT0" + (modern ? "M" : "A");
			}
			if (_governmentPortrait[governmentId, (int)advisor] == null)
				_governmentPortrait[governmentId, (int)advisor] = Resources[filename][(40 * (int)advisor), 0, 40, 60];
			return _governmentPortrait[governmentId, (int)advisor];
		}

		public static IBitmap City(City city, bool smallFont = false)
		{
			IBitmap output = new Picture(16, 16);
			TextSettings settings = new TextSettings()
			{
				FontId = smallFont ? 1 : 0,
				Alignment = TextAlign.Center
			};
			
			if (city.Tile.Units.Length > 0)
				output.FillRectangle(0, 0, 16, 16, 5);
			output.FillRectangle(1, 1, 14, 14, 15)
				.FillRectangle(2, 1, 13, 13, Common.ColourDark[city.Owner])
				.FillRectangle(2, 2, 12, 12, Common.ColourLight[city.Owner]);
			
			IBitmap resource;
			if (Resources.Exists("SP257"))
			{
				resource = Resources["SP257"][192, 112, 16, 16];
			}
			else
			{
				resource = new Picture(Free.Instance.City, Common.GetPalette256);
			}
			resource
				.ColourReplace(3, 0)
				.ColourReplace(5, Common.ColourDark[city.Owner]);
				
			if (city.IsInDisorder)
			{
				output.AddLayer(resource, 0, 0)
					.AddLayer(Icons.Citizen(Enums.Citizen.UnhappyMale), 5, 1);
			}
			else
			{
				output.AddLayer(resource, 0, 0)
					.DrawText($"{city.Size}", (smallFont ? 1 : 0), 5, 9, 5, TextAlign.Center);
			}

			resource?.Dispose();

			if (city.HasBuilding<CityWalls>())
			{
				output.AddLayer(Generic.Fortify, 0, 0);
			}
			
			return output;
		}
	}
}