// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.IO;

namespace CivOne.Sprites
{
	public class CachedSprite : BaseInstance, ISprite
	{
		private readonly Func<Bytemap> GetSprite;

		private Bytemap _bitmap;
		public Bytemap Bitmap
		{
			get
			{
				if (_bitmap == null)
					_bitmap = GetSprite();
				return _bitmap;
			}
		}

		public CachedSprite(Func<Bytemap> getSprite)
		{
			GetSprite = getSprite;
		}

		~CachedSprite()
		{
			_bitmap?.Dispose();
			_bitmap = null;
		}
	}
}