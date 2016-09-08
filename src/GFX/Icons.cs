// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Enums;
using CivOne.GFX;

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
	}
}