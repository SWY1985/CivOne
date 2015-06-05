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
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseUnit : IUnit
	{
		private static Picture[] _iconCache = new Picture[28];
		public virtual Picture Icon
		{
			get;
			private set;
		}
		public string Name { get; protected set; }
		public IAdvance RequiredTech { get; protected set; }
		public IAdvance ObsoleteTech { get; protected set; }
		public UnitClass Class { get; protected set; }
		public Unit Type { get; protected set; }
		public byte Price { get; protected set; }
		public byte Attack { get; protected set; }
		public byte Defense { get; protected set; }
		public byte Move { get; protected set; }
		
		protected void SetIcon(char page, int col, int row)
		{
			if (_iconCache[(int)Type] == null)
			{
				Bitmap icon = Resources.Instance.LoadPIC(string.Format("ICONPG{0}", page), true).GetPart(col * 160, row * 62, 160, 60);
				Picture.ReplaceColours(icon, (byte)(Settings.Instance.GraphicsMode == GraphicsMode.Graphics256 ? 253 : 15), 0);
				_iconCache[(int)Type] = new Picture(icon);
			}
			Icon = _iconCache[(int)Type];
		}
		
		protected BaseUnit(byte price = 1, byte attack = 1, byte defense = 1, byte move = 1)
		{
			Price = price;
			Attack = attack;
			Defense = defense;
			Move = move;
		}
	}
}