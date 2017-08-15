// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Linq;
using CivOne.IO;

namespace CivOne.Graphics
{
	public class Palette : BaseUnmanaged
	{
		public int Length => base.Size / 4;

		private int ToInt(int index) => ReadInt(index * 4);

		public Colour this[int index]
		{
			get
			{
				return new Colour(
					ReadByte((index * 4)),
					ReadByte((index * 4) + 1),
					ReadByte((index * 4) + 2),
					ReadByte((index * 4) + 3)
				);
			}
			set
			{
				WriteByte((index * 4), value.A);
				WriteByte((index * 4) + 1, value.R);
				WriteByte((index * 4) + 2, value.G);
				WriteByte((index * 4) + 3, value.B);
			}
		}

		public IEnumerable<Colour> Entries => Enumerable.Range(0, Length).Select(x => this[x]);

		public Palette Copy() => Palette.Copy(this);

		public void MergePalette(Palette source, int startIndex = -1, int count = -1)
		{
			if (startIndex == -1) startIndex = 0;
			if (count == -1) count = Length - startIndex;
			for (int i = startIndex; i < startIndex + count; i++)
			{
				WriteInt(i * 4, source.ToInt(i));
			}
		}

		public static Palette Copy(Palette source) => new Palette(source);

		public static implicit operator Palette(Colour[] palette)
		{
			return new Palette(palette);
		}

		private Palette(Colour[] palette) : this(palette.Length)
		{
			for (int i = 0; i < Length; i++)
				this[i] = palette[i];
		}

		private Palette(Palette source) : base(source)
		{
			for (int i = 0; i < source.Length; i++)
			{
				this[i] = source[i];
			}
		}

		public Palette(int length = 256) : base(length * 4)
		{
		}
	}
}