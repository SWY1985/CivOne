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
using CivOne.Governments;

namespace CivOne.Graphics
{
	internal class Icons
	{
		private static Resources Resources => Resources.Instance;
		private static Picture _food;
		public static Picture Food
		{
			get
			{
				if (_food == null)
				{
					_food = Resources["SP257"].GetPart(128, 32, 8, 8);
					Picture.ReplaceColours(_food, 3, 0);
					_food.FillRectangle(0, 0, 0, 1, 8);
				}
				return _food;
			}
		}

		private static Picture _foodLoss;
		public static Picture FoodLoss
		{
			get
			{
				if (_foodLoss == null)
				{
					_foodLoss = Resources["SP257"].GetPart(128, 32, 8, 8);
					Picture.ReplaceColours(_foodLoss, new byte[] { 3, 15 }, new byte[] { 0, 5 });
					_foodLoss.FillRectangle(0, 0, 0, 1, 8);
				}
				return _foodLoss;
			}
		}
		
		private static Picture _shield;
		public static Picture Shield
		{
			get
			{
				if (_shield == null)
				{
					_shield = Resources["SP257"].GetPart(136, 32, 8, 8);
					Picture.ReplaceColours(_shield, 3, 0);
				}
				return _shield;
			}
		}
		
		private static Picture _shieldLoss;
		public static Picture ShieldLoss
		{
			get
			{
				if (_shieldLoss == null)
				{
					_shieldLoss = Resources["SP257"].GetPart(136, 32, 8, 8);
					Picture.ReplaceColours(_shieldLoss, new byte[] { 3, 15 }, new byte[] { 0, 5 });
				}
				return _shieldLoss;
			}
		}
		
		private static Picture _trade;
		public static Picture Trade
		{
			get
			{
				if (_trade == null)
				{
					_trade = Resources["SP257"].GetPart(144, 32, 8, 8);
					Picture.ReplaceColours(_trade, 3, 0);
				}
				return _trade;
			}
		}
		
		private static Picture _unhappy;
		public static Picture Unhappy
		{
			get
			{
				if (_unhappy == null)
				{
					_unhappy = Resources["SP257"].GetPart(136, 40, 8, 8);
					Picture.ReplaceColours(_unhappy, 3, 0);
				}
				return _unhappy;
			}
		}
		
		private static Picture _luxuries;
		public static Picture Luxuries
		{
			get
			{
				if (_luxuries == null)
				{
					_luxuries = Resources["SP257"].GetPart(144, 40, 8, 8);
					Picture.ReplaceColours(_luxuries, 3, 0);
				}
				return _luxuries;
			}
		}
		
		private static Picture _taxes;
		public static Picture Taxes
		{
			get
			{
				if (_taxes == null)
				{
					_taxes = Resources["SP257"].GetPart(152, 32, 8, 8);
					Picture.ReplaceColours(_taxes, 3, 0);
				}
				return _taxes;
			}
		}
		
		private static Picture _science;
		public static Picture Science
		{
			get
			{
				if (_science == null)
				{
					_science = Resources["SP257"].GetPart(128, 40, 8, 8);
					Picture.ReplaceColours(_science, 3, 0);
				}
				return _science;
			}
		}
		
		private static Picture _newspaper;
		public static Picture Newspaper
		{
			get
			{
				if (_newspaper == null)
				{
					_newspaper = Resources["SP257"].GetPart(176, 128, 32, 16);
				}
				return _newspaper;
			}
		}

		private static Picture _sellButton;
		public static Picture SellButton
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

		private static Picture[] _helperArrow;
		public static Picture HelperArrow(Direction direction)
		{
			if (_helperArrow == null)
			{
				_helperArrow = new Picture[4];
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

		private static Picture[] _citizen = new Picture[9];
		public static Picture Citizen(Citizen citizen)
		{
			if (_citizen[(int)citizen] == null)
			{
				_citizen[(int)citizen] = Resources["SP257"].GetPart((8 * (int)citizen), 128, 8, 16);
			}
			return _citizen[(int)citizen];
		}

		private static Picture[] _lamp = new Picture[4];
		public static Picture Lamp(int stage)
		{
			if (stage < 0 || stage > 3)
				return null;
			
			if (_lamp[stage] == null)
			{
				_lamp[stage] = Resources["SP257"].GetPart(128 + (8 * stage), 48, 8, 8);
			}
			return _lamp[stage];
		}

		private static Picture[,] _governmentPortrait = new Picture[7, 4];
		public static Picture GovernmentPortrait(IGovernment government, Advisor advisor, bool modern)
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
				_governmentPortrait[governmentId, (int)advisor] = Resources[filename].GetPart((40 * (int)advisor), 0, 40, 60);
			return _governmentPortrait[governmentId, (int)advisor];
		}

		public static Picture City(City city, bool smallFont = false)
		{
			Picture output = new Picture(16, 16);
			TextSettings settings = new TextSettings()
			{
				FontId = smallFont ? 1 : 0,
				Alignment = TextAlign.Center
			};
			
			if (city.Tile.Units.Length > 0)
				output.FillRectangle(5, 0, 0, 16, 16);
			output.FillRectangle(15, 1, 1, 14, 14);
			output.FillRectangle(Common.ColourDark[city.Owner], 2, 1, 13, 13);
			output.FillRectangle(Common.ColourLight[city.Owner], 2, 2, 12, 12);
			
			Picture resource;
			if (Resources.Exists("SP257"))
			{
				resource = Resources["SP257"].GetPart(192, 112, 16, 16);
			}
			else
			{
				resource = Free.Instance.City;
			}
			Picture.ReplaceColours(resource, 3, 0);
			Picture.ReplaceColours(resource, 5, Common.ColourDark[city.Owner]);
			output.AddLayer(resource, 0, 0);
			output.DrawText($"{city.Size}", (smallFont ? 1 : 0), 5, 9, 5, TextAlign.Center);
			// output.DrawText($"{city.Size}", 9, 5);
			resource?.Dispose();

			if (city.HasBuilding<CityWalls>())
			{
				output.AddLayer(Fortify, 0, 0);
			}
			
			return output;
		}

		private static Picture _fortify;
		public static Picture Fortify
		{
			get
			{
				if (_fortify == null)
				{
					if (Resources.Exists("SP257"))
					{
						_fortify = Resources["SP257"].GetPart(208, 112, 16, 16);
					}
					else
					{
						_fortify = Free.Instance.Fortify;
					}
					Picture.ReplaceColours(_fortify, 3, 0);
				}
				return _fortify;
			}
		}

		public static Picture Cursor(MouseCursor cursor, bool builtIn = false)
		{
			switch (cursor)
			{
				case MouseCursor.Pointer:
					if (builtIn)
					{
						return new Picture(new byte[,]
						{
							{ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0 },
							{ 0, 5,15,15,15,15,15,15,15,15, 5, 0, 0, 0, 0, 0 },
							{ 0, 0, 5,15,15,15,15,15,15,15, 5, 0, 0, 0, 0, 0 },
							{ 0, 0, 0, 5,15,15,15,15,15,15, 5, 0, 0, 0, 0, 0 },
							{ 0, 0, 0, 0, 5,15,15,15,15,15,15, 5, 5, 0, 0, 0 },
							{ 0, 0, 0, 0, 0, 5,15,15,15,15,15,15,15, 5, 5, 0 },
							{ 0, 0, 0, 0, 0, 0, 5,15,15,15, 5,15,15,15,15, 5 },
							{ 0, 0, 0, 0, 0, 0, 0, 5,15,15, 5, 5, 5,15,15, 5 },
							{ 0, 0, 0, 0, 0, 0, 0, 0, 5,15, 5, 0, 0, 5, 5, 0 },
							{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 0, 0, 0, 0, 0,},
							{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0,}
						}, Common.GetPalette256);
					}
					return Resources["SP257"].GetPart(113, 33, 15, 15);
				case MouseCursor.Goto:
					if (builtIn)
					{
						return new Picture(new byte[,]
						{
							{ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0 },
							{ 0, 5,15,15,15,15,15,15,15,15, 5, 0, 0, 0, 0, 0 },
							{ 0, 0, 5,15,15,15,15,15,15,15, 5, 0, 0, 0, 0, 0 },
							{ 0, 0, 0, 5,15,15,15,15,15,15, 5, 0, 0, 0, 0, 0 },
							{ 0,15,15,15, 5,15,15,15,15,15,15, 5, 5, 0, 0, 0 },
							{15, 0, 0, 0,15, 5,15,15,15,15,15,15,15, 5, 5, 0 },
							{15, 0,15, 0,15, 0, 5,15,15,15, 5,15,15,15,15, 5 },
							{ 0, 0,15,15, 0, 0, 0, 5,15,15, 5, 5, 5,15,15, 5 },
							{ 0, 0, 0, 0, 0, 0, 0, 0, 5,15, 5, 0, 0, 5, 5, 0 },
							{ 0,15,15,15, 0, 0, 0, 0, 0, 5, 5, 0, 0, 0, 0, 0,},
							{15, 0, 0, 0,15, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0,},
							{15, 0, 0, 0,15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,},
							{ 0,15,15,15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,}
						}, Common.GetPalette256);
					}
					return Resources["SP257"].GetPart(33, 33, 15, 15);
			}
			return null;
		}

		private static Picture _fortress;
		public static Picture Fortress
		{
			get
			{
				if (_fortress == null)
				{
					_fortress = Resources["SP257"].GetPart(224, 112, 16, 16);
					Picture.ReplaceColours(_fortress, 3, 0);
				}
				return _fortress;
			}
		}
	}
}