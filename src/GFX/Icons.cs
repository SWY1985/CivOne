// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Linq;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.GFX
{
	internal class Icons
	{
		private static Bitmap _food;
		public static Bitmap Food
		{
			get
			{
				if (_food == null)
				{
					_food = (Bitmap)Resources.Instance.GetPart("SP257", 128, 32, 8, 8);
					Picture.ReplaceColours(_food, 3, 0);

					Picture temp = new Picture(_food);
					temp.FillRectangle(0, 0, 0, 1, 8);
					_food = temp.Image;
				}
				return _food;
			}
		}

		private static Bitmap _foodLoss;
		public static Bitmap FoodLoss
		{
			get
			{
				if (_foodLoss == null)
				{
					_foodLoss = (Bitmap)Resources.Instance.GetPart("SP257", 128, 32, 8, 8).Clone();
					Picture.ReplaceColours(_foodLoss, new byte[] { 3, 15 }, new byte[] { 0, 5 });

					Picture temp = new Picture(_foodLoss);
					temp.FillRectangle(0, 0, 0, 1, 8);
					_foodLoss = temp.Image;
				}
				return _foodLoss;
			}
		}
		
		private static Bitmap _shield;
		public static Bitmap Shield
		{
			get
			{
				if (_shield == null)
				{
					_shield = (Bitmap)Resources.Instance.GetPart("SP257", 136, 32, 8, 8);
					Picture.ReplaceColours(_shield, 3, 0);
				}
				return _shield;
			}
		}
		
		private static Bitmap _shieldLoss;
		public static Bitmap ShieldLoss
		{
			get
			{
				if (_shieldLoss == null)
				{
					_shieldLoss = (Bitmap)Resources.Instance.GetPart("SP257", 136, 32, 8, 8).Clone();
					Picture.ReplaceColours(_shieldLoss, new byte[] { 3, 15 }, new byte[] { 0, 5 });
				}
				return _shieldLoss;
			}
		}
		
		private static Bitmap _trade;
		public static Bitmap Trade
		{
			get
			{
				if (_trade == null)
				{
					_trade = (Bitmap)Resources.Instance.GetPart("SP257", 144, 32, 8, 8);
					Picture.ReplaceColours(_trade, 3, 0);
				}
				return _trade;
			}
		}
		
		private static Bitmap _luxuries;
		public static Bitmap Luxuries
		{
			get
			{
				if (_luxuries == null)
				{
					_luxuries = (Bitmap)Resources.Instance.GetPart("SP257", 144, 40, 8, 8);
					Picture.ReplaceColours(_luxuries, 3, 0);
				}
				return _luxuries;
			}
		}
		
		private static Bitmap _taxes;
		public static Bitmap Taxes
		{
			get
			{
				if (_taxes == null)
				{
					_taxes = (Bitmap)Resources.Instance.GetPart("SP257", 152, 32, 8, 8);
					Picture.ReplaceColours(_taxes, 3, 0);
				}
				return _taxes;
			}
		}
		
		private static Bitmap _science;
		public static Bitmap Science
		{
			get
			{
				if (_science == null)
				{
					_science = (Bitmap)Resources.Instance.GetPart("SP257", 128, 40, 8, 8);
					Picture.ReplaceColours(_science, 3, 0);
				}
				return _science;
			}
		}
		
		private static Bitmap _newspaper;
		public static Bitmap Newspaper
		{
			get
			{
				if (_newspaper == null)
				{
					_newspaper = (Bitmap)Resources.Instance.GetPart("SP257", 176, 128, 32, 16);
				}
				return _newspaper;
			}
		}

		private static Bitmap[] _population = new Bitmap[9];
		public static Bitmap Population(Population population)
		{
			if (_population[(int)population] == null)
			{
				_population[(int)population] = (Bitmap)Resources.Instance.GetPart("SP257", (8 * (int)population), 128, 8, 16);
			}
			return _population[(int)population];
		}

		private static Bitmap[,] _governmentPortrait = new Bitmap[7, 4];
		public static Bitmap GovernmentPortrait(Government government, Advisor advisor, bool modern)
		{
			string filename;
			int governmentId;
			switch (government)
			{
				case Government.Anarchy:
				case Government.Despotism:
					governmentId = (modern ? 1 : 0);
					filename = "GOVT0" + (modern ? "M" : "A");
					break;
				case Government.Monarchy:
					governmentId = (modern ? 3 : 2);
					filename = $"GOVT1" + (modern ? "M" : "A");
					break;
				case Government.Republic:
				case Government.Democracy:
					governmentId = (modern ? 5 : 4);
					filename = $"GOVT2" + (modern ? "M" : "A");
					break;
				case Government.Communism:
					governmentId = 6;
					filename = "GOVT3A";
					break;
				default:
					return null;
			}
			if (_governmentPortrait[governmentId, (int)advisor] == null)
				_governmentPortrait[governmentId, (int)advisor] = (Bitmap)Resources.Instance.GetPart(filename, (40 * (int)advisor), 0, 40, 60);
			return _governmentPortrait[governmentId, (int)advisor];
		}

		public static Bitmap City(City city, bool smallFont = false)
		{
			Picture output = new Picture(16, 16);
			
			if (city.Tile.Units.Length > 0)
				output.FillRectangle(5, 0, 0, 16, 16);
			output.FillRectangle(15, 1, 1, 14, 14);
			output.FillRectangle(Common.ColourDark[city.Owner], 2, 1, 13, 13);
			output.FillRectangle(Common.ColourLight[city.Owner], 2, 2, 12, 12);
			
			Bitmap resource = (Bitmap)Resources.Instance.GetPart("SP257", 192, 112, 16, 16).Clone();
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

			if (city.Buildings.Any(b => b.Id == new CityWalls().Id))
			{
				output.AddLayer(Fortify, 0, 0);
			}
			
			return output.Image;
		}

		private static Bitmap _fortify;
		public static Bitmap Fortify
		{
			get
			{
				if (_fortify == null)
				{
					_fortify = (Bitmap)Resources.Instance.GetPart("SP257", 208, 112, 16, 16);
					Picture.ReplaceColours(_fortify, 3, 0);
				}
				return _fortify;
			}
		}
	}
}