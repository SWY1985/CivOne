// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using CivOne.IO;

namespace CivOne.Graphics.Sprites
{
	internal class CachedSpriteCollection<T> : ISpriteCollection<T>, ICached
	{
		private class Sprite : ISprite
		{
			public Bytemap Bitmap { get; private set; }

			internal Sprite(Bytemap bitmap)
			{
				Bitmap = bitmap;
			}

			~Sprite()
			{
				Bitmap?.Dispose();
				Bitmap = null;
			}
		}

		private readonly Func<T, Bytemap> GetSprite;

		private readonly Dictionary<T, ISprite> _sprites = new Dictionary<T, ISprite>();

		public ISprite this[T index]
		{
			get
			{
				if (!_sprites.ContainsKey(index))
				{
					_sprites.Add(index, new Sprite(GetSprite(index)));
				}
				return _sprites[index];
			}
		}

		public void Clear()
		{
			_sprites.Clear();
		}

		public CachedSpriteCollection(Func<T, Bytemap> getSprite)
		{
			GetSprite = getSprite;
		}

		~CachedSpriteCollection()
		{
			_sprites.Clear();
		}
	}
}