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

namespace CivOne
{
	public class PalaceData
	{
		private byte[] PalaceStyle = new byte[7];
		private byte[] PalaceLevel = new byte[7];
		private byte[] GardenLevel = new byte[3];

		public int PalaceLeft
		{
			get
			{
				for (int i = 0; i < 3; i++)
				{
					if (PalaceLevel[i] > 0) return i;
				}
				return 2;
			}
		}

		public int PalaceRight
		{
			get
			{
				for (int i = 6; i > 3; i--)
				{
					if (PalaceLevel[i] > 0) return i;
				}
				return 4;
			}
		}

		public PalaceStyle GetPalaceStyle(int index)
		{
			if (index < 0 || index > 6) throw new Exception("Invalid palace index");
			return (PalaceStyle)PalaceStyle[index];
		}

		public byte GetPalaceLevel(int index)
		{
			if (index < 0 || index > 6) throw new Exception("Invalid palace index");
			return PalaceLevel[index];
		}

		public byte GetGardenLevel(int index)
		{
			if (index < 0 || index > 2) throw new Exception("Invalid garden index");
			return GardenLevel[index];
		}

		public void SetPalace(int index, byte style, byte level)
		{
			if (index < 0 || index > 6)
				throw new Exception("Invalid palace index");
			if (style < 0 || style > 3)
				throw new Exception("Invalid palace style");
			if (level < 0 || level > 4)
				throw new Exception("Invalid palace level");
			
			if (level == 0 || style == 0)
			{
				PalaceStyle[index] = 0;
				PalaceLevel[index] = 0;
				return;
			}
			PalaceStyle[index] = style;
			PalaceLevel[index] = level;
		}

		public void SetGarden(int index, byte level)
		{
			if (index < 0 || index > 2) throw new Exception("Invalid garden index");
			if (level < 0 || level > 3) throw new Exception("Invalid garden level");
			
			GardenLevel[index] = level;
		}
	}
}