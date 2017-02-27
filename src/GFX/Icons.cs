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
using CivOne.Interfaces;

namespace CivOne.GFX
{
	internal class Icons
	{
		private static Picture _food;
		public static Picture Food
		{
			get
			{
				if (_food == null)
				{
					_food = Resources.Instance.GetPart("SP257", 128, 32, 8, 8);
					Picture.ReplaceColours(_food, 3, 0);

					Picture temp = new Picture(_food);
					temp.FillRectangle(0, 0, 0, 1, 8);
					_food = temp;
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
					_foodLoss = Resources.Instance.GetPart("SP257", 128, 32, 8, 8);
					Picture.ReplaceColours(_foodLoss, new byte[] { 3, 15 }, new byte[] { 0, 5 });

					Picture temp = new Picture(_foodLoss);
					temp.FillRectangle(0, 0, 0, 1, 8);
					_foodLoss = temp;
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
					_shield = Resources.Instance.GetPart("SP257", 136, 32, 8, 8);
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
					_shieldLoss = Resources.Instance.GetPart("SP257", 136, 32, 8, 8);
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
					_trade = Resources.Instance.GetPart("SP257", 144, 32, 8, 8);
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
					_unhappy = Resources.Instance.GetPart("SP257", 136, 40, 8, 8);
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
					_luxuries = Resources.Instance.GetPart("SP257", 144, 40, 8, 8);
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
					_taxes = Resources.Instance.GetPart("SP257", 152, 32, 8, 8);
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
					_science = Resources.Instance.GetPart("SP257", 128, 40, 8, 8);
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
					_newspaper = Resources.Instance.GetPart("SP257", 176, 128, 32, 16);
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

		private static Picture[] _citizen = new Picture[9];
		public static Picture Citizen(Citizen citizen)
		{
			if (_citizen[(int)citizen] == null)
			{
				_citizen[(int)citizen] = Resources.Instance.GetPart("SP257", (8 * (int)citizen), 128, 8, 16);
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
				_lamp[stage] = Resources.Instance.GetPart("SP257", 128 + (8 * stage), 48, 8, 8);
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
				_governmentPortrait[governmentId, (int)advisor] = Resources.Instance.GetPart(filename, (40 * (int)advisor), 0, 40, 60);
			return _governmentPortrait[governmentId, (int)advisor];
		}

		public static Picture City(City city, bool smallFont = false)
		{
			Picture output = new Picture(16, 16);
			
			if (city.Tile.Units.Length > 0)
				output.FillRectangle(5, 0, 0, 16, 16);
			output.FillRectangle(15, 1, 1, 14, 14);
			output.FillRectangle(Common.ColourDark[city.Owner], 2, 1, 13, 13);
			output.FillRectangle(Common.ColourLight[city.Owner], 2, 2, 12, 12);
			
			Picture resource = Resources.Instance.GetPart("SP257", 192, 112, 16, 16);
			Picture.ReplaceColours(resource, 3, 0);
			Picture.ReplaceColours(resource, 5, Common.ColourDark[city.Owner]);
			output.AddLayer(resource, 0, 0);
			if (city.Size > 9)
			{
				output.DrawText($"{city.Size}", (smallFont ? 1 : 0), 5, 5, 8, 5, TextAlign.Center);
			}
			else
			{
				output.DrawText($"{city.Size}", (smallFont ? 1 : 0), 5, 5, 9, 5, TextAlign.Center);
			}

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
					_fortify = Resources.Instance.GetPart("SP257", 208, 112, 16, 16);
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
					return Resources.Instance.GetPart("SP257", 112, 32, 16, 16);
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
					return Resources.Instance.GetPart("SP257", 32, 32, 16, 16);
			}
			return new Picture(16, 16);
		}

		private static Picture _fortress;
		public static Picture Fortress
		{
			get
			{
				if (_fortress == null)
				{
					_fortress = Resources.Instance.GetPart("SP257", 224, 112, 16, 16);
					Picture.ReplaceColours(_fortress, 3, 0);
				}
				return _fortress;
			}
		}
	}
}